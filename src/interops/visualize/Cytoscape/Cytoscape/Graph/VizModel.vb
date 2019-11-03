#Region "Microsoft.VisualBasic::841ecbe5fc67aee052137fca4b7044f3, visualize\Cytoscape\Cytoscape\Graph\Visualization\GraphDrawing.vb"

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

'     Module GraphDrawing
' 
'         Function: __calculation, getRectange, getSize, (+2 Overloads) InvokeDrawing
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.File

Namespace CytoscapeGraphView

    ''' <summary>
    ''' 在这个模块之中提供将<see cref="XGMMLgraph"/>转换为<see cref="NetworkGraph"/>模型的方法
    ''' 用于进行网络图的自定义渲染
    ''' </summary>
    Public Module VizModel

        <Extension>
        Public Function ToNetworkGraph(graph As XGMMLgraph) As NetworkGraph
            Dim g As New NetworkGraph
            Dim node As Node
            Dim edge As Edge
            Dim nodeIndex As New Dictionary(Of String, Node)

            For Each xgmmlNode As XGMMLnode In graph.nodes
                node = New Node With {
                    .ID = xgmmlNode.id,
                    .label = xgmmlNode.label,
                    .data = New NodeData With {
                        .label = xgmmlNode.label,
                        .origID = xgmmlNode.label
                    }
                }

                Call nodeIndex.Add(node.label, node)
                Call g.AddNode(node)
            Next

            Dim index As New GraphIndex(graph)

            For Each xgmmlEdge As XGMMLedge In graph.edges
                Dim s As Node = g.GetNode(index.GetNode(xgmmlEdge.source).label)
                Dim t As Node = g.GetNode(index.GetNode(xgmmlEdge.target).label)

                edge = New Edge With {
                    .U = s,
                    .V = t,
                    .ID = xgmmlEdge.id,
                    .data = New EdgeData With {
                        .label = xgmmlEdge.label
                    }
                }

                Call g.AddEdge(edge)
            Next

            Return g
        End Function
    End Module
End Namespace
