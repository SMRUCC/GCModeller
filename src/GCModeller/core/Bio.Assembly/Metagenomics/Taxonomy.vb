#Region "Microsoft.VisualBasic::1344635b015ae23f19edee630459bea1, ..\GCModeller\core\Bio.Assembly\Metagenomics\Taxonomy.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
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
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy

Namespace Metagenomics

    Public Structure Taxonomy

        Public Property scientificName As String

#Region "BIOM taxonomy k__ p__ c__ o__ f__ g__ s__"

        ''' <summary>
        ''' 1. 界
        ''' </summary>
        Public Property kingdom As String
        ''' <summary>
        ''' 2. 门
        ''' </summary>
        Public Property phylum As String
        ''' <summary>
        ''' 3A. 纲
        ''' </summary>
        Public Property [class] As String
        ''' <summary>
        ''' 4B. 目
        ''' </summary>
        Public Property order As String
        ''' <summary>
        ''' 5C. 科
        ''' </summary>
        Public Property family As String
        ''' <summary>
        ''' 6D. 属
        ''' </summary>
        Public Property genus As String
        ''' <summary>
        ''' 7E. 种
        ''' </summary>
        Public Property species As String
#End Region

        ''' <summary>
        ''' 这个函数会自动调用<see cref="FillLineageEmpty"/>函数来填充缺失掉的rank部分
        ''' 所以这个构造方法是安全的构造方法，不需要担心会因为缺少否些rank而抛出错误
        ''' </summary>
        ''' <param name="lineage"></param>
        Sub New(lineage As Dictionary(Of String, String))
            lineage = lineage.FillLineageEmpty
            kingdom = lineage(NcbiTaxonomyTree.superkingdom)
            phylum = lineage(NcbiTaxonomyTree.phylum)
            [class] = lineage(NcbiTaxonomyTree.class)
            order = lineage(NcbiTaxonomyTree.order)
            family = lineage(NcbiTaxonomyTree.family)
            genus = lineage(NcbiTaxonomyTree.genus)
            species = lineage(NcbiTaxonomyTree.species)
        End Sub

        Shared ReadOnly DescRanks$() = NcbiTaxonomyTree.stdranks.Reverse.ToArray

        Sub New(lineage$())
            Call Me.New(
                lineage:=lineage _
                    .SeqIterator _
                    .ToDictionary(Function(rank) DescRanks(rank.i),
                                  Function(rank) rank.value)
            )
        End Sub

        Public Function CreateTable() As NamedValue(Of Dictionary(Of String, String))
            Dim table As New Dictionary(Of String, String) From {
                {NcbiTaxonomyTree.class, [class]},
                {NcbiTaxonomyTree.family, family},
                {NcbiTaxonomyTree.genus, genus},
                {NcbiTaxonomyTree.order, order},
                {NcbiTaxonomyTree.phylum, phylum},
                {NcbiTaxonomyTree.species, species},
                {NcbiTaxonomyTree.superkingdom, kingdom}
            }

            Return New NamedValue(Of Dictionary(Of String, String)) With {
                .Name = scientificName,
                .Value = table
            }
        End Function

        ''' <summary>
        ''' 这个函数不会比较<see cref="scientificName"/>
        ''' </summary>
        ''' <param name="another"></param>
        ''' <returns></returns>
        Public Function CompareWith(another As Taxonomy) As Relations
            With another
                If Not kingdom = .kingdom Then
                    Return Relations.Irrelevant
                End If

                Dim rel As Relations

                rel = compare(phylum, .phylum)
                If rel <> Relations.Equals Then
                    Return rel
                End If

                rel = compare([class], .class)
                If rel <> Relations.Equals Then
                    Return rel
                End If

                rel = compare(order, .order)
                If rel <> Relations.Equals Then
                    Return rel
                End If

                rel = compare(family, .family)
                If rel <> Relations.Equals Then
                    Return rel
                End If

                rel = compare(genus, .genus)
                If rel <> Relations.Equals Then
                    Return rel
                End If

                Return compare(species, .species)
            End With
        End Function

        Private Shared Function compare(me$, another$) As Relations
            If [me].StringEmpty Then
                Return Relations.Include
            ElseIf another.StringEmpty Then
                Return Relations.IncludeBy
            ElseIf Not me$ = another Then
                Return Relations.Irrelevant
            Else
                Return Relations.Equals
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator IsTrue(t As Taxonomy) As Boolean
            Return Not IsEmpty(t)
        End Operator

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function IsEmpty(t As Taxonomy) As Boolean
            Return Not t.kingdom.StringEmpty OrElse
                Not t.order.StringEmpty OrElse
                Not t.class.StringEmpty OrElse
                Not t.family.StringEmpty OrElse
                Not t.genus.StringEmpty OrElse
                Not t.phylum.StringEmpty OrElse
                Not t.scientificName.StringEmpty OrElse
                Not t.species.StringEmpty
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator IsFalse(t As Taxonomy) As Boolean
            Return IsEmpty(t)
        End Operator

        Public Overrides Function ToString() As String
            Return scientificName
        End Function
    End Structure
End Namespace
