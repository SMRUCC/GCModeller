﻿#Region "Microsoft.VisualBasic::afae0809fc828d2bab39bc68c0e82d69, models\Networks\Microbiome\PathwayProfile.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Module PathwayProfile
    ' 
    '     Function: (+2 Overloads) CreateProfile, EnrichmentTestInternal, MicrobiomePathwayNetwork, PathwayProfile, (+3 Overloads) PathwayProfiles
    '               ProfileEnrichment
    '     Class Profile
    ' 
    '         Properties: pct, Profile, RankGroup, Taxonomy
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '     Class EnrichmentProfiles
    ' 
    '         Properties: pathway, profile, pvalue, RankGroup
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports RDotNET.Extensions.VisualBasic.API
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Metagenomics
Imports SMRUCC.genomics.Model.Network.KEGG
Imports Numeric = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

''' <summary>
''' The microbiome KEGG pathway profile.
''' </summary>
Public Module PathwayProfile

    ''' <summary>
    ''' 返回每一个代谢途径的计数
    ''' </summary>
    ''' <param name="metagenome"></param>
    ''' <param name="maps"></param>
    ''' <param name="coverage#"></param>
    ''' <returns></returns>
    <Extension>
    Public Function PathwayProfile(metagenome As IEnumerable(Of TaxonomyRef), maps As MapRepository, Optional coverage# = 0.3) As Dictionary(Of String, Double)
        Dim profile As New Dictionary(Of String, Counter)

        For Each genome As TaxonomyRef In metagenome
            Dim KOlist$() = genome.KOTerms

            ' query KEGG map这里是主要的限速步骤
            Dim pathways = maps _
                .QueryMapsByMembers(KOlist) _
                .Where(Function(map)
                           With map.KOIndex
                               Return .Intersect(collection:=KOlist).Count / .Count >= coverage
                           End With
                       End Function) _
                .ToArray

            For Each map As MapIndex In pathways
                If Not profile.ContainsKey(map.MapID) Then
                    Call profile.Add(map.MapID, New Counter)
                End If

                Call profile(map.MapID).Hit()
            Next
        Next

        Return profile.AsNumeric
    End Function

    ''' <summary>
    ''' 将给定的物种分类下的所有的物种基因组的KEGG代谢途径覆盖度信息提取出来
    ''' </summary>
    ''' <param name="taxonomy"></param>
    ''' <param name="uniprot"></param>
    ''' <param name="ref"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function PathwayProfiles(taxonomy As Taxonomy, uniprot As TaxonomyRepository, ref As MapRepository) As Dictionary(Of String, Double)
        Return uniprot.PopulateModels({taxonomy}, distinct:=True).PathwayProfile(ref)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="taxonomyGroup">物种分类信息以及对应的OTU相对丰度数量</param>
    ''' <param name="uniprot"></param>
    ''' <param name="ref"></param>
    ''' <param name="rank"></param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateProfile(taxonomyGroup As (taxonomy As Taxonomy, counts#)(),
                                  uniprot As TaxonomyRepository,
                                  ref As MapRepository,
                                  Optional rank As TaxonomyRanks = TaxonomyRanks.Genus) As Dictionary(Of String, Profile())

        Dim ALL = taxonomyGroup.Select(Function(tax) tax.counts).Sum
        Dim profiles = taxonomyGroup _
            .AsParallel _
            .Select(Function(tax)
                        Dim taxonomy As Taxonomy = tax.taxonomy
                        Dim profile = taxonomy.PathwayProfiles(uniprot, ref)

                        ' 因为可能是gast.taxonomy，所以在这里需要使用new来进行复制
                        ' 否则后面的json/XML序列化会出错
                        Return New Profile(
                            tax:=New Taxonomy(taxonomy),
                            profile:=profile,
                            pct:=tax.counts / ALL
                        ) With {
                            .RankGroup = taxonomy.TaxonomyRankString(rank)
                        }
                    End Function) _
            .ToArray

        ' 下面按照rank进行数据分组
        Dim profileGroup = profiles _
            .GroupBy(Function(tax) tax.RankGroup) _
            .ToDictionary(Function(g) g.Key,
                          Function(profile) profile.ToArray)

        Return profileGroup
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="gast">其实只需要Taxonomy和对应的丰度数量信息即可</param>
    ''' <param name="uniprot"></param>
    ''' <param name="ref"></param>
    ''' <param name="rank"></param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateProfile(gast As IEnumerable(Of gast.gastOUT),
                                  uniprot As TaxonomyRepository,
                                  ref As MapRepository,
                                  Optional rank As TaxonomyRanks = TaxonomyRanks.Genus) As Dictionary(Of String, Profile())
        Return gast _
            .Select(Function(tax)
                        Dim name$ = tax.taxonomy
                        Dim taxonomy As New Taxonomy(BIOMTaxonomy.TaxonomyParser(name))

                        Return (taxonomy, CDbl(tax.counts))
                    End Function) _
            .ToArray _
            .CreateProfile(uniprot, ref, rank)
    End Function

    Public Class Profile

        ''' <summary>
        ''' 物种分类信息
        ''' </summary>
        ''' <returns></returns>
        Public Property Taxonomy As Taxonomy
        ''' <summary>
        ''' 该分类下的所有的具有覆盖度结果的KEGG编号的列表和相对应的覆盖度值
        ''' </summary>
        ''' <returns></returns>
        Public Property Profile As Dictionary(Of String, Double)
        ''' <summary>
        ''' 这个物种的相对百分比含量
        ''' </summary>
        ''' <returns></returns>
        Public Property pct As Double
        Public Property RankGroup As String

        Sub New()
        End Sub

        Sub New(tax As Taxonomy, profile As Dictionary(Of String, Double), pct#)
            Me.Taxonomy = tax
            Me.Profile = profile
            Me.pct = pct
        End Sub
    End Class

    ''' <summary>
    ''' 进行显著性检验
    ''' </summary>
    ''' <param name="profileData"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ProfileEnrichment(profileData As IEnumerable(Of Profile)) As Dictionary(Of String, (profile#, pvalue#))
        ' 转换为每一个mapID对应的pathway按照taxonomy排列的向量
        Dim profiles = profileData.ToArray
        Dim profileTable = profiles _
            .Select(Function(tax) tax.Profile.Keys) _
            .IteratesALL _
            .Distinct _
            .OrderBy(Function(id) id) _
            .ToDictionary(Function(mapID) mapID,
                          Function(mapID)
                              Return profiles.EnrichmentTestInternal(mapID)
                          End Function)

        Return profileTable
    End Function

    ''' <summary>
    ''' 是按照每一个物种的百分比来和等长的零向量作比较的
    ''' </summary>
    ''' <param name="profiles"></param>
    ''' <param name="mapID$"></param>
    ''' <returns></returns>
    <Extension>
    Private Function EnrichmentTestInternal(profiles As IEnumerable(Of Profile), mapID$) As (profile#, pvalue#)
        Dim vector#() = profiles _
            .Where(Function(tax) tax.Profile.ContainsKey(mapID)) _
            .Where(Function(tax) tax.Profile(mapID) > 0R) _
            .Select(Function(tax) tax.Profile(mapID) * tax.pct) _
            .ToArray
        Dim ZERO#() = Repeats(0R, vector.Length)

        Dim profile# = vector.Sum
        ' student t test
        Dim pvalue#
        Dim x0 = vector.FirstOrDefault

        If vector.Length < 3 Then
            pvalue = 1
        ElseIf vector.All(Function(x) x = x0) Then
            If x0 = 0R Then
                pvalue = 1
            Else
                pvalue = 0
            End If
        Else
            ' 可能有很多零
            pvalue = stats.Ttest(vector, ZERO, varEqual:=True).pvalue
        End If

        Return (profile, pvalue)
    End Function

    Public Class EnrichmentProfiles
        Public Property RankGroup As String
        Public Property pathway As String
        Public Property profile As Double
        Public Property pvalue As Double
    End Class

    <Extension>
    Public Function PathwayProfiles(gast As IEnumerable(Of gast.gastOUT),
                                    uniprot As TaxonomyRepository,
                                    ref As MapRepository,
                                    Optional rank As TaxonomyRanks = TaxonomyRanks.Genus) As EnrichmentProfiles()

        Dim profiles = gast.CreateProfile(uniprot, ref, rank)
        Dim profileTable = profiles.PathwayProfiles

        Return profileTable
    End Function

    ''' <summary>
    ''' 将会通过这个函数计算出富集结果的显著性程度
    ''' </summary>
    ''' <param name="profiles"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function PathwayProfiles(profiles As Dictionary(Of String, Profile())) As EnrichmentProfiles()
        Return profiles _
            .Select(Function(group)
                        Dim tax = group.Key ' 物种的分类字符串
                        Dim enrichment = group.Value.ProfileEnrichment

                        Return enrichment _
                            .Select(Function(pathway)
                                        Return New EnrichmentProfiles With {
                                            .pathway = pathway.Key,
                                            .profile = pathway.Value.profile,
                                            .pvalue = pathway.Value.pvalue,
                                            .RankGroup = tax
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL _
            .ToArray
    End Function

    <Extension>
    Public Function MicrobiomePathwayNetwork(profiles As EnrichmentProfiles(), KEGG As MapRepository, Optional cutoff# = 0.05) As NetworkGraph
        Dim idlist = profiles _
            .Where(Function(map) map.pvalue <= cutoff) _
            .Select(Function(map) map.pathway) _
            .ToArray
        Dim mapGroup = profiles _
            .GroupBy(Function(map) map.pathway) _
            .ToDictionary(Function(g) g.Key,
                          Function(mapProfiles) mapProfiles.ToArray)
        Dim maps = idlist _
            .Select(Function(mapID) KEGG.GetByKey(mapID).Map) _
            .ToArray
        Dim network As NetworkGraph = maps.BuildNetwork(
            Sub(mapNode)
                With mapGroup(mapNode.Label).Shadows
                    mapNode.Data!pvalue = (-Numeric.Log10(!pvalue)).Average
                    mapNode.Data!profile = !profile.Average
                End With
            End Sub)

        Return network
    End Function
End Module
