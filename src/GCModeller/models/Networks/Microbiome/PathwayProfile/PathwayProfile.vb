#Region "Microsoft.VisualBasic::bbe4cdb90a04c675d1a3de77e47ad01b, GCModeller\models\Networks\Microbiome\PathwayProfile\PathwayProfile.vb"

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

    '   Total Lines: 123
    '    Code Lines: 86
    ' Comment Lines: 22
    '   Blank Lines: 15
    '     File Size: 5.00 KB


    '     Module PathwayProfiler
    ' 
    '         Function: EnrichmentTestInternal, (+2 Overloads) PathwayProfiles, ProfileEnrichment
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Metagenomics

Namespace PathwayProfile

    ''' <summary>
    ''' The microbiome KEGG pathway profile.
    ''' </summary>
    Public Module PathwayProfiler

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
        Private Function EnrichmentTestInternal(profiles As Profile(), mapID$) As (profile#, pvalue#)
            Dim vector#() = profiles _
                .Where(Function(tax) tax.Profile.ContainsKey(mapID)) _
                .Where(Function(tax) tax.Profile(mapID) > 0R) _
                .Select(Function(tax) tax.Profile(mapID) * tax.pct) _
                .ToArray

            If vector.Length < 3 Then
                vector = vector.Join(Repeats(0.0, 3 - vector.Length))
            End If

            Dim ZERO#() = Repeats(0R, vector.Length)
            Dim profile# = vector.Sum
            ' student t test
            Dim pvalue#
            Dim x0 = vector.FirstOrDefault
            Dim means = vector.Average / 10

            For i As Integer = 0 To vector.Length - 1
                If vector(i) = 0R Then
                    vector(i) = means
                End If
            Next

            If vector.All(Function(x) x = x0) Then
                If x0 = 0R Then
                    pvalue = 1
                Else
                    vector(Scan0) += 0.0000001
                    pvalue = t.Test(vector, ZERO, varEqual:=True).Pvalue
                End If
            Else
                ' 可能有很多零
                pvalue = t.Test(vector, ZERO, varEqual:=True).Pvalue
            End If

            Return (profile, pvalue)
        End Function

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
    End Module
End Namespace
