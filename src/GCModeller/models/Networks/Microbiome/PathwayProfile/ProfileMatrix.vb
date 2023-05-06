#Region "Microsoft.VisualBasic::bb340459886a2e4c644a26e097a07098, GCModeller\models\Networks\Microbiome\PathwayProfile\ProfileMatrix.vb"

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


' Code Statistics:

'   Total Lines: 131
'    Code Lines: 80
' Comment Lines: 34
'   Blank Lines: 17
'     File Size: 5.72 KB


'     Module ProfileMatrix
' 
'         Function: (+2 Overloads) CreateProfile, PathwayProfile, PathwayProfiles
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Metagenomics

Namespace PathwayProfile

    Public Module ProfileMatrix

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
                    If Not profile.ContainsKey(map.EntryId) Then
                        Call profile.Add(map.EntryId, New Counter)
                    End If

                    Call profile(map.EntryId).Hit()
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
                .Select(Function(tax)
                            Dim taxonomy As Taxonomy = tax.taxonomy
                            Dim profile = taxonomy.PathwayProfiles(uniprot, ref)

                            Call taxonomy.ToString(BIOMstyle:=True).__DEBUG_ECHO

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
                              Function(profile)
                                  Return profile.ToArray
                              End Function)

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
    End Module
End Namespace
