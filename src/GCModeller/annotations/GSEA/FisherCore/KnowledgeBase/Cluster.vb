﻿#Region "Microsoft.VisualBasic::075b1b6698f781035645ab230da3cd2c, annotations\GSEA\FisherCore\KnowledgeBase\Cluster.vb"

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

    '   Total Lines: 124
    '    Code Lines: 79 (63.71%)
    ' Comment Lines: 31 (25.00%)
    '    - Xml Docs: 93.55%
    ' 
    '   Blank Lines: 14 (11.29%)
    '     File Size: 4.50 KB


    ' Class Cluster
    ' 
    '     Properties: [class], category, description, ID, memberIds
    '                 members, names
    ' 
    '     Function: getCollection, GetMemberById, getSize, Intersect, ToString
    ' 
    '     Sub: loadIndex
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models

''' <summary>
''' A collection of the functional related <see cref="BackgroundGene"/>.
''' </summary>
''' <remarks>
''' (主要是KEGG代谢途径，也可以是其他的具有生物学意义的聚类结果)
''' </remarks>
Public Class Cluster : Inherits ListOf(Of BackgroundGene)
    Implements INamedValue

    ''' <summary>
    ''' The cluster id.
    ''' </summary>
    ''' <remarks>
    ''' (代谢途径的编号或者其他的标识符)
    ''' </remarks>
    ''' <returns></returns>
    <XmlAttribute>
    Public Property ID As String Implements IKeyedEntity(Of String).Key
    ''' <summary>
    ''' The common name of current term <see cref="ID"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property names As String

    ''' <summary>
    ''' A brief description on term function.
    ''' </summary>
    ''' <returns></returns>
    <XmlElement>
    Public Property description As String

    <XmlAttribute> Public Property category As String
    <XmlAttribute> Public Property [class] As String

    ''' <summary>
    ''' 当前的这个聚类之中的基因列表
    ''' </summary>
    ''' <returns></returns>
    Public Property members As BackgroundGene()

    Dim index As Index(Of String)

    ''' <summary>
    ''' get the collection of all member theirs <see cref="BackgroundGene.accessionID"/>
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property memberIds As String()
        Get
            Return members _
                .Select(Function(gene) gene.accessionID) _
                .ToArray
        End Get
    End Property

    Public Function GetMemberById(id As String) As BackgroundGene
        Return members _
            .Where(Function(g)
                       Dim test1 = g.accessionID.TextEquals(id, null_equals:=False, empty_equals:=False)
                       If test1 Then Return True
                       test1 = g.locus_tag IsNot Nothing AndAlso g.locus_tag.name.TextEquals(id, null_equals:=False, empty_equals:=False)
                       Return test1
                   End Function) _
            .FirstOrDefault
    End Function

    Private Sub loadIndex(is_locus_tag As Boolean)
        If Not is_locus_tag Then
            ' create id index from all id dataset
            index = members _
                .Select(Function(name) name.EnumerateAllIds) _
                .IteratesALL _
                .Where(Function(str) Not str.StringEmpty) _
                .Distinct _
                .ToArray
        Else
            ' create id index just from the locus tag data
            index = members _
                .Select(Function(name)
                            If name.locus_tag Is Nothing OrElse name.locus_tag.name.StringEmpty Then
                                Return ""
                            Else
                                Return Strings _
                                    .Trim(name.locus_tag.name) _
                                    .Split(":"c) _
                                    .Last
                            End If
                        End Function) _
                .Where(Function(str) Not str.StringEmpty) _
                .Distinct _
                .ToArray
        End If
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Intersect(list As IEnumerable(Of String), Optional isLocustag As Boolean = False) As IEnumerable(Of String)
        If index Is Nothing Then
            Call loadIndex(isLocustag)
        End If

        Return index.Intersect(collection:=list)
    End Function

    Public Overrides Function ToString() As String
        Return $"Dim {ID} = '{names}' {{{size}}}"
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Protected Overrides Function getSize() As Integer
        Return members.Length
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Protected Overrides Function getCollection() As IEnumerable(Of BackgroundGene)
        Return members
    End Function
End Class
