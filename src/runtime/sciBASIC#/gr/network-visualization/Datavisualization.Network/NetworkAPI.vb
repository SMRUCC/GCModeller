﻿#Region "Microsoft.VisualBasic::15ca561948589b43e030e54a06057fc7, gr\network-visualization\Datavisualization.Network\NetworkAPI.vb"

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

    ' Module NetworkAPI
    ' 
    '     Function: EndPoints, FromCorrelations, GetConnections, GetNetworkNodes, GetNextConnects
    '               GetNHetworkEdges, ReadnetWork, SaveNetwork, Trim, WriteNetwork
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Math
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports ______NETWORK__ =
    Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic.Network(Of
    Microsoft.VisualBasic.Data.visualize.Network.FileStream.Node,
    Microsoft.VisualBasic.Data.visualize.Network.FileStream.NetworkEdge)

<Package("DataVisualization.Network", Publisher:="xie.guigang@gmail.com")>
Public Module NetworkAPI

    <Extension>
    Public Function EndPoints(network As Graph.NetworkGraph) As (input As Graph.Node(), output As Graph.Node())
        Return New NetworkGraph(Of Graph.Node, Graph.Edge)(network.nodes, network.edges).EndPoints
    End Function

    <ExportAPI("Read.Network")>
    Public Function ReadnetWork(file As String) As FileStream.NetworkEdge()
        Return file.LoadCsv(Of FileStream.NetworkEdge)(False).ToArray
    End Function

    <ExportAPI("Get.NetworkEdges")>
    Public Function GetNHetworkEdges(Network As ______NETWORK__) As FileStream.NetworkEdge()
        Return Network.Edges
    End Function

    <ExportAPI("Get.NetworkNodes")>
    Public Function GetNetworkNodes(Network As ______NETWORK__) As FileStream.Node()
        Return Network.Nodes
    End Function

    <ExportAPI("Save")>
    Public Function SaveNetwork(network As ______NETWORK__, <Parameter("DIR.Export")> EXPORT As String) As Boolean
        Return network.Save(EXPORT, Encodings.UTF8)
    End Function

    <ExportAPI("Write.Network")>
    Public Function WriteNetwork(Network As FileStream.NetworkEdge(), <Parameter("Path.Save")> SaveTo As String) As Boolean
        Return Network.SaveTo(SaveTo, False)
    End Function

    ''' <summary>
    ''' 这个查找函数是忽略掉了方向了的
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="node"></param>
    ''' <returns></returns>
    <Extension, ExportAPI("GetConnections")>
    Public Function GetConnections(source As IEnumerable(Of FileStream.NetworkEdge), node As String) As FileStream.NetworkEdge()
        Dim LQuery = LinqAPI.Exec(Of FileStream.NetworkEdge) <=
 _
            From x As FileStream.NetworkEdge
            In source.AsParallel
            Where Not String.IsNullOrEmpty(x.GetConnectedNode(node))
            Select x

        Return LQuery
    End Function

    ''' <summary>
    ''' 查找To关系的节点边
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="from"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("Get.Connects.Next")>
    <Extension>
    Public Function GetNextConnects(source As IEnumerable(Of FileStream.NetworkEdge), from As String) As FileStream.NetworkEdge()
        Dim LQuery = LinqAPI.Exec(Of FileStream.NetworkEdge) <=
 _
            From x As FileStream.NetworkEdge
            In source.AsParallel
            Where from.TextEquals(x.FromNode)
            Select x

        Return LQuery
    End Function

    <Extension>
    Public Function Trim(network As FileStream.NetworkTables, Optional doNothing As Boolean = False) As FileStream.NetworkTables
        If Not doNothing Then
            Call network.RemoveSelfLoop()
            Call network.RemoveDuplicated()
        End If

        Return network
    End Function

    ''' <summary>
    ''' 变量的属性里面必须是包含有相关度的
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="cut"><see cref="Abs(Double)"/></param>
    ''' <param name="trim">Removes the duplicated edges and self loops?</param>
    ''' <returns></returns>
    <Extension>
    Public Function FromCorrelations(data As IEnumerable(Of DataSet),
                                     Optional nodeTypes As Dictionary(Of String, String) = Nothing,
                                     Optional interacts As Dictionary(Of String, String) = Nothing,
                                     Optional cut# = 0R,
                                     Optional trim As Boolean = False) As FileStream.NetworkTables

        Dim array As DataSet() = data.ToArray

        If nodeTypes Is Nothing Then
            nodeTypes = New Dictionary(Of String, String)
        End If
        If interacts Is Nothing Then
            interacts = New Dictionary(Of String, String)
        End If

        VBDebugger.Mute = True

        Dim nodes As FileStream.Node() = LinqAPI.Exec(Of FileStream.Node) _
 _
            () <= From v As DataSet
                  In array
                  Let type As String = nodeTypes.TryGetValue(v.ID, [default]:="variable")
                  Select New FileStream.Node With {
                      .ID = v.ID,
                      .NodeType = type,
                      .Properties = v _
                          .Properties _
                          .ToDictionary(Function(k) k.Key,
                                        Function(k)
                                            Return CStr(k.Value)
                                        End Function)
                  }

        Dim edges As New List(Of FileStream.NetworkEdge)
        Dim interact$
        Dim c#

        For Each var As DataSet In array
            For Each k$ In var.Properties.Keys
                c# = var.Properties(k$)

                If Abs(c) < cut Then
                    Continue For
                End If

                interact = interacts.TryGetValue(
                    $"{var.ID} --> {k}",
                    [default]:="correlates")
                edges += New FileStream.NetworkEdge With {
                    .FromNode = var.ID,
                    .ToNode = k,
                    .value = c,
                    .Interaction = interact,
                    .Properties = New Dictionary(Of String, String) From {
                        {"type", If(c# > 0, "positive", "negative")},
                        {"abs", Abs(c#)}
                    }
                }
            Next
        Next

        VBDebugger.Mute = False

        Dim out As New FileStream.NetworkTables With {
            .Edges = edges,
            .Nodes = nodes
        }

        If trim Then
            Return out.Trim
        Else
            Return out
        End If
    End Function
End Module
