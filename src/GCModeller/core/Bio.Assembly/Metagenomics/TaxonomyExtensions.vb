#Region "Microsoft.VisualBasic::23d49fefbdffecfb953ff28a35ad8cce, ..\GCModeller\core\Bio.Assembly\Metagenomics\TaxonomyExtensions.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges

Namespace Metagenomics

    Public Interface ITaxonomyLineage
        Property Taxonomy As String
    End Interface

    Public Module TaxonomyExtensions

        Public Delegate Function TaxonomyProjector(Of T)(obj As T) As Taxonomy

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function TaxonomyLineage(tax As ITaxonomyLineage) As Taxonomy
            Return New Taxonomy(BIOMTaxonomy.TaxonomyParser(tax.Taxonomy))
        End Function

        ''' <summary>
        ''' 判断当前的这个<paramref name="rank"/>字符串所代表的物种分类名称是否是空的？？
        ''' </summary>
        ''' <param name="rank"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function TaxonomyRankEmpty(rank As String) As Boolean
            Return rank.StringEmpty OrElse
                rank.TextEquals("NA") OrElse
                rank.TextEquals("unclassified") OrElse
                rank.TextEquals("unknown") OrElse
                rank.TextEquals("Unassigned")
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <typeparam name="TOut"></typeparam>
        ''' <param name="seq"></param>
        ''' <param name="ref">如果目标<paramref name="getTaxonomy"/>的分类结果是
        ''' 位于当前的这个参考<paramref name="ref"/>范围内的，则会被筛选出来
        ''' </param>
        ''' <param name="getTaxonomy"></param>
        ''' <param name="getValue"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Iterator Function SelectByTaxonomyRange(Of T, TOut)(seq As IEnumerable(Of T),
                                                                   ref As Taxonomy,
                                                                   getTaxonomy As TaxonomyProjector(Of T),
                                                                   getValue As Projector(Of T, TOut)) As IEnumerable(Of TOut)
            Dim pop As TOut

            For Each o As T In seq
                Dim tax As Taxonomy = getTaxonomy(obj:=o)
                Dim rel As Relations = ref.CompareWith(tax)

                If rel = Relations.Equals OrElse rel = Relations.Include Then
                    pop = getValue([in]:=o)
                    Yield pop
                End If
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function SelectByTaxonomyRange(relativeAbundance As IEnumerable(Of KeyValuePair(Of Taxonomy, Double)), ref As Taxonomy) As IEnumerable(Of Double)
            Return relativeAbundance.SelectByTaxonomyRange(ref, Function(tax) tax.Key, Function(tax) tax.Value)
        End Function

        Public Function ParseRank(value As String) As TaxonomyRanks
            Select Case LCase(value)
                Case "kingdom", "k"
                    Return TaxonomyRanks.Kingdom
                Case "phylum", "p"
                    Return TaxonomyRanks.Phylum
                Case "class", "c"
                    Return TaxonomyRanks.Class
                Case "order", "o"
                    Return TaxonomyRanks.Order
                Case "family", "f"
                    Return TaxonomyRanks.Family
                Case "genus", "g"
                    Return TaxonomyRanks.Genus
                Case "species", "s"
                    Return TaxonomyRanks.Species
                Case "strain"
                    Return TaxonomyRanks.Strain
                Case Else
                    Return TaxonomyRanks.NA
            End Select
        End Function
    End Module
End Namespace
