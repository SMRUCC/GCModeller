﻿#Region "Microsoft.VisualBasic::5862e7cc8e4180fdf8c059ab32f969e1, GSEA\GSEA\KnowledgeBase\Cluster.vb"

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

    ' Class Cluster
    ' 
    '     Properties: description, ID, members, names
    ' 
    '     Function: getCollection, getSize, Intersect, ToString
    ' 
    ' Class BackgroundGene
    ' 
    '     Properties: locus_tag, name, term_id
    ' 
    '     Function: ToString
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
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

''' <summary>
''' 主要是KEGG代谢途径，也可以是其他的具有生物学意义的聚类结果
''' </summary>
Public Class Cluster : Inherits ListOf(Of BackgroundGene)
    Implements INamedValue

    ''' <summary>
    ''' The cluster id.(代谢途径的编号或者其他的标识符)
    ''' </summary>
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

    ''' <summary>
    ''' 当前的这个聚类之中的基因列表
    ''' </summary>
    ''' <returns></returns>
    Public Property members As BackgroundGene()

    Dim index As Index(Of String)

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Intersect(list As IEnumerable(Of String), Optional isLocustag As Boolean = False) As IEnumerable(Of String)
        If index Is Nothing Then
            If Not isLocustag Then
                index = members _
                    .Select(Function(name) name.AsEnumerable) _
                    .IteratesALL _
                    .Distinct _
                    .ToArray
            Else
                index = members _
                    .Select(Function(name) name.locus_tag.name.Split(":"c).Last) _
                    .Distinct _
                    .ToArray
            End If
        End If

        Return index.Intersect(collection:=list)
    End Function

    Public Overrides Function ToString() As String
        Return $"Dim {ID} = '{names}'"
    End Function

    Protected Overrides Function getSize() As Integer
        Return members.Length
    End Function

    Protected Overrides Function getCollection() As IEnumerable(Of BackgroundGene)
        Return members
    End Function
End Class

<XmlType("gene")>
Public Class BackgroundGene : Inherits Synonym

    ''' <summary>
    ''' The gene name
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute>
    Public Property name As String

    <XmlElement>
    Public Property term_id As String()
    Public Property locus_tag As NamedValue

    Public Overrides Function ToString() As String
        Return $"{MyBase.ToString}  [{locus_tag.text}]"
    End Function

End Class
