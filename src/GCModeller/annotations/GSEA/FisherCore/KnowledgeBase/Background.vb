#Region "Microsoft.VisualBasic::b1e784aa1d07cc2365b0e4a2b3d61566, annotations\GSEA\FisherCore\KnowledgeBase\Background.vb"

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
''' 假设基因组是有许多个功能聚类的集合构成的
''' </summary>
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

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetClusterTable() As Dictionary(Of String, Cluster)
        Return clusters.ToDictionary(Function(c) c.ID)
    End Function

    Public Overrides Function ToString() As String
        Return name
    End Function
End Class
