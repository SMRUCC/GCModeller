﻿#Region "Microsoft.VisualBasic::1da4ca4fadbec2a8d255737be3941103, Bio.Assembly\Metagenomics\Taxonomy.vb"

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

    '     Class Taxonomy
    ' 
    '         Properties: [class], family, genus, kingdom, order
    '                     phylum, scientificName, species
    ' 
    '         Constructor: (+4 Overloads) Sub New
    '         Function: compare, CompareWith, CreateTable, IsEmpty, ToArray
    '                   ToString
    '         Operators: (+2 Overloads) IsFalse, (+2 Overloads) IsTrue
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy

Namespace Metagenomics

    ''' <summary>
    ''' 主要是用来保存csv以及Xml文件使用
    ''' </summary>
    ''' 
    <XmlType("taxonomy", [Namespace]:=SMRUCC.genomics.LICENSE.GCModeller)>
    Public Class Taxonomy

        <XmlAttribute> Public Property scientificName As String

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
        Sub New(lineage As Dictionary(Of String, String), Optional empty$ = "")
            lineage = lineage.FillLineageEmpty(empty)
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
                    .Take(7) _
                    .SeqIterator _
                    .ToDictionary(Function(rank) DescRanks(rank.i),
                                  Function(rank) rank.value)
            )
        End Sub

        Sub New(copy As Taxonomy)
            With copy
                Me.scientificName = .scientificName

                Me.class = .class
                Me.family = .family
                Me.genus = .genus
                Me.kingdom = .kingdom
                Me.order = .order
                Me.phylum = phylum
                Me.species = .species
            End With
        End Sub

        ''' <summary>
        ''' 需要在这里使用无参的构造函数来提供按照属性赋值的初始化形式
        ''' </summary>
        Sub New()
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
        ''' Convert current <see cref="Taxonomy"/> object as a string array.(返回来的元素值是按照<see cref="TaxonomyRanks"/>从大到小排列的)
        ''' </summary>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ToArray() As String()
            Return {kingdom, phylum, [class], order, family, genus, species}
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

                ' 2017-12-18
                '
                ' 在这里需要注意双名法可能会带来BUG
                ' 例如脆弱拟杆菌可能会被注释为：
                '
                ' k__Bacteria;p__Bacteroidetes;c__Bacteroidia;o__Bacteroidales;f__Bacteroidaceae;g__Bacteroides;s__Bacteroides fragilis（双名法全写形式）
                ' k__Bacteria;p__Bacteroidetes;c__Bacteroidia;o__Bacteroidales;f__Bacteroidaceae;g__Bacteroides;s__fragilis（双名法简写形式）
                '
                ' 假若直接对二者的species name进行比较的话，肯定会因为字符串不相等而返回Irrelevant不相关的结果
                ' 但是因为species name是使用双名法来进行命名的，所以将简写形式使用genus属名称补全就可以相等了

                ' 先进行直接比较
                rel = compare(species, .species)

                If rel = Relations.Irrelevant Then
                    ' 可能是双名法带来的问题，尝试判断一下，如果有任何一个只有一个token，
                    ' 则尝试将其添加上genus名称再做比较
                    Dim s1 = species, s2 = .species

                    ' 因为经过前面的比较genus是一样的，所以在这里可以
                    ' 直接使用当前的这个taxonomy对象的genus值

                    If s1.IndexOf(" "c) = -1 Then
                        ' 可能是s1为简写形式
                        s1 = genus & " " & s1
                    End If
                    If s2.IndexOf(" "c) = -1 Then
                        ' 可能是s2为简写形式
                        s2 = genus & " " & s2
                    End If

                    ' 再做一次比较
                    Return compare(s1, s2)
                Else
                    Return rel
                End If
            End With
        End Function

        Private Shared Function compare(me$, another$) As Relations
            If [me].TaxonomyRankEmpty Then
                Return Relations.Include
            ElseIf another.TaxonomyRankEmpty Then
                Return Relations.IncludeBy
            ElseIf Not me$.TextEquals(another) Then
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
            If t Is Nothing Then
                Return True
            Else
                Return Not t.kingdom.TaxonomyRankEmpty OrElse
                    Not t.order.TaxonomyRankEmpty OrElse
                    Not t.class.TaxonomyRankEmpty OrElse
                    Not t.family.TaxonomyRankEmpty OrElse
                    Not t.genus.TaxonomyRankEmpty OrElse
                    Not t.phylum.TaxonomyRankEmpty OrElse
                    Not t.scientificName.TaxonomyRankEmpty OrElse
                    Not t.species.TaxonomyRankEmpty
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Operator IsFalse(t As Taxonomy) As Boolean
            Return IsEmpty(t)
        End Operator

        Public Overrides Function ToString() As String
            Dim tax As New List(Of String)
            Dim i As VBInteger = Scan0

            tax += BIOMPrefixAlt(++i) & Me.kingdom
            tax += BIOMPrefixAlt(++i) & Me.phylum
            tax += BIOMPrefixAlt(++i) & Me.class
            tax += BIOMPrefixAlt(++i) & Me.order
            tax += BIOMPrefixAlt(++i) & Me.family
            tax += BIOMPrefixAlt(++i) & Me.genus
            tax += BIOMPrefixAlt(++i) & Me.species

            Return tax.JoinBy(";")
        End Function
    End Class
End Namespace
