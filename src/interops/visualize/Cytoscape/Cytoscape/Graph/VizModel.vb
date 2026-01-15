#Region "Microsoft.VisualBasic::7bf9857ad893d055061201085a3095f2, visualize\Cytoscape\Cytoscape\Graph\VizModel.vb"

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
    '    Code Lines: 99 (79.84%)
    ' Comment Lines: 9 (7.26%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 16 (12.90%)
    '     File Size: 5.24 KB


    '     Module VizModel
    ' 
    '         Function: createProperties, getCommonOrSharedName, ToNetworkGraph
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph.EdgeBundling
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.File

Namespace CytoscapeGraphView

    ''' <summary>
    ''' 在这个模块之中提供将<see cref="XGMMLgraph"/>转换为<see cref="NetworkGraph"/>模型的方法
    ''' 用于进行网络图的自定义渲染
    ''' </summary>
    Public Module VizModel

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Private Function createProperties(element As AttributeDictionary, propertyNames$()) As Dictionary(Of String, String)
            Return propertyNames _
                .ToDictionary(Function(key) key,
                              Function(key)
                                  Return element(key)?.Value
                              End Function)
        End Function

        <Extension>
        Private Function getCommonOrSharedName(xgmmlNode As XGMMLnode) As String
            Dim sharedName = xgmmlNode.attributes.FirstOrDefault(Function(a) a.name = "shared name")
            Dim common = xgmmlNode.attributes.FirstOrDefault(Function(a) a.name = "common")

            If sharedName Is Nothing AndAlso common Is Nothing Then
                Return xgmmlNode.label
            End If
            If sharedName Is Nothing Then
                Return common.Value
            ElseIf common Is Nothing Then
                Return sharedName.Value
            Else
                If common.Value.IsPattern("\d+") Then
                    Return sharedName.Value
                Else
                    Return common.Value
                End If
            End If
        End Function

        ''' <summary>
        ''' 请注意，这个函数只会产生最基本的网络模型数据，以及布局信息，个性化的样式调整需要在外部函数调用之中自行添加完成
        ''' </summary>
        ''' <param name="graph"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ToNetworkGraph(graph As XGMMLgraph, ParamArray propertyNames As String()) As NetworkGraph
            Dim g As New NetworkGraph
            Dim node As Node
            Dim edge As Edge
            Dim nodeIndex As New Dictionary(Of String, Node)

            For Each xgmmlNode As XGMMLnode In graph.nodes
                If xgmmlNode.label.IsPattern("\d+") Then
                    xgmmlNode.label = xgmmlNode.getCommonOrSharedName
                End If

                node = New Node With {
                    .ID = xgmmlNode.id,
                    .label = xgmmlNode.label,
                    .data = New NodeData With {
                        .label = xgmmlNode.label,
                        .origID = xgmmlNode.label,
                        .initialPostion = New FDGVector2 With {
                            .x = CSng(xgmmlNode.graphics.x),
                            .y = CSng(xgmmlNode.graphics.y)
                        },
                        .Properties = xgmmlNode.createProperties(propertyNames),
                        .size = {xgmmlNode.graphics.w, xgmmlNode.graphics.h}
                    }
                }

                Call nodeIndex.Add(xgmmlNode.label, node)
                Call g.AddNode(node, assignId:=False)
            Next

            Dim index As New GraphIndex(graph)

            For Each xgmmlEdge As XGMMLedge In graph.edges
                Dim s = index.GetNode(xgmmlEdge.source)
                Dim t = index.GetNode(xgmmlEdge.target)
                Dim u As Node = g.GetElementByID(s.label)
                Dim v As Node = g.GetElementByID(t.label)

                Dim sx = s.graphics.x
                Dim sy = s.graphics.y
                Dim tx = t.graphics.x
                Dim ty = t.graphics.y
                Dim ps As New PointF(CSng(sx), CSng(sy))
                Dim pt As New PointF(CSng(tx), CSng(ty))

                edge = New Edge With {
                    .U = u,
                    .V = v,
                    .ID = xgmmlEdge.id.ToString,
                    .data = New EdgeData With {
                        .label = xgmmlEdge.label,
                        .bends = xgmmlEdge.graphics _
                            .edgeBendHandles _
                            .Select(Function(a)
                                        Dim raw As PointF = a.pointAuto(sx, sy, tx, ty)
                                        Dim xy As WayPointVector = WayPointVector.CreateVector(ps, pt, raw)

                                        Return xy
                                    End Function) _
                            .ToArray,
                        .Properties = xgmmlEdge.createProperties(propertyNames)
                    }
                }

                Call g.AddEdge(edge)
            Next

            Return g
        End Function
    End Module
End Namespace
