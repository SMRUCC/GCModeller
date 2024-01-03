﻿#Region "Microsoft.VisualBasic::1d9702ede31d9654118d3391e0b07883, GCModeller\annotations\GSEA\FisherCore\KnowledgeBase\Background.vb"

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

    '   Total Lines: 57
    '    Code Lines: 39
    ' Comment Lines: 11
    '   Blank Lines: 7
    '     File Size: 1.95 KB


    ' Class Background
    ' 
    '     Properties: build, clusters, comments, id, name
    '                 size
    ' 
    '     Function: GetClusterTable, SubsetOf, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

''' <summary>
''' a collection of the functional related gene consist of this background model.
''' (假设基因组是有许多个功能聚类的集合构成的)
''' </summary>
''' <remarks>
''' the functional related geneset is modelling as the <see cref="clusters"/> at here.
''' </remarks>
<XmlRoot("background", [Namespace]:="http://gcmodeller.org/GSEA/background.xml")>
Public Class Background : Inherits XmlDataModel
    Implements INamedValue

    Public Property name As String Implements IKeyedEntity(Of String).Key
    ''' <summary>
    ''' A brief unique id code. Such as kegg organism code or ncbi taxonomy id.
    ''' </summary>
    ''' <returns></returns>
    Public Property id As String
    Public Property comments As String
    Public Property build As Date = Now
    ''' <summary>
    ''' The number of genes in this background genome.
    ''' </summary>
    ''' <returns></returns>
    Public Property size As Integer

    ''' <summary>
    ''' a collection of the functional related genesets.
    ''' </summary>
    ''' <returns></returns>
    <XmlElement>
    Public Property clusters As Cluster()

    Default Public ReadOnly Property Item(id As String) As Cluster
        Get
            Return clusters _
                .Where(Function(c) c.ID = id) _
                .FirstOrDefault
        End Get
    End Property

    Public Function SubsetOf(predicate As Func(Of Cluster, Boolean)) As Background
        Return New Background With {
            .build = build,
            .clusters = clusters.Where(predicate).ToArray,
            .comments = comments,
            .name = name,
            .size = size
        }
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="geneId"></param>
    ''' <returns>
    ''' returns nothing if target gene is not found
    ''' </returns>
    Public Function GetBackgroundGene(geneId As String) As BackgroundGene
        For Each cluster As Cluster In clusters
            Dim gene As BackgroundGene = cluster.GetMemberById(geneId)

            If Not gene Is Nothing Then
                Return gene
            End If
        Next

        Return Nothing
    End Function

    ''' <summary>
    ''' Make a dictionary index of the <see cref="clusters"/>, via the <see cref="Cluster.ID"/> as key.
    ''' </summary>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetClusterTable() As Dictionary(Of String, Cluster)
        Return clusters.ToDictionary(Function(c) c.ID)
    End Function

    Public Overrides Function ToString() As String
        Return name
    End Function
End Class
