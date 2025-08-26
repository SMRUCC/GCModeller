#Region "Microsoft.VisualBasic::89765c198fcf133adb5dadc8f4d77989, visualize\Cytoscape\Cytoscape\Graph\Xgmml\GraphIndex.vb"

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

    '   Total Lines: 80
    '    Code Lines: 60 (75.00%)
    ' Comment Lines: 5 (6.25%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 15 (18.75%)
    '     File Size: 2.85 KB


    '     Class GraphIndex
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: DeleteDuplication, ExistEdge, GetEdgeBends, GetEdgeNodes, (+2 Overloads) GetNode
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph.EdgeBundling
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.File

Namespace CytoscapeGraphView.XGMML

    Public Class GraphIndex

        Dim graph As XGMMLgraph
        Dim nodeTable As Dictionary(Of String, XGMMLnode)

        Sub New(g As XGMMLgraph)
            graph = g
            nodeTable = g.nodes.ToDictionary(Function(n) n.label)
        End Sub

        Public Function GetEdgeNodes(edge As XGMMLedge) As (source As XGMMLnode, target As XGMMLnode)
            Dim s = GetNode(edge.source)
            Dim t = GetNode(edge.target)

            Return (s, t)
        End Function

        Public Function GetEdgeBends(edge As XGMMLedge) As PointF()
            Dim [handles] As Handle() = edge.graphics.edgeBendHandles
            Dim s = GetNode(edge.source)
            Dim t = GetNode(edge.target)
            Dim sx = s.graphics.x
            Dim sy = s.graphics.y
            Dim tx = t.graphics.x
            Dim ty = t.graphics.y
            Dim bends As PointF() = [handles] _
                .Select(Function(b)
                            If b.isDirectPoint Then
                                Return b.originalLocation
                            Else
                                Return b.convert(sx, sy, tx, ty)
                            End If
                        End Function) _
                .ToArray

            Return bends
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="label">Synonym</param>
        ''' <returns></returns>
        Public Function GetNode(label As String) As XGMMLnode
            Return nodeTable.TryGetValue(label)
        End Function

        Public Function GetNode(ID As Long) As XGMMLnode
            Return LinqAPI.DefaultFirst(Of XGMMLnode) _
 _
                () <= From node As XGMMLnode
                      In graph.nodes
                      Where node.id = ID
                      Select node

        End Function

        Public Function DeleteDuplication() As XGMMLgraph
            Dim sw As Stopwatch = Stopwatch.StartNew
            Dim edges = graph.edges

            Call $"{NameOf(edges)}:={edges.Length} in the network model...".debug
            graph.edges = Distinct(graph.edges)
            Call $"{NameOf(edges)}:={edges.Length} left after remove duplicates in {sw.ElapsedMilliseconds}ms....".debug

            Return graph
        End Function

        Public Function ExistEdge(Edge As XGMMLedge) As Boolean
            Return Not (GetNode(Edge.source) Is Nothing OrElse GetNode(Edge.target) Is Nothing)
        End Function
    End Class
End Namespace
