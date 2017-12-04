#Region "Microsoft.VisualBasic::38ae0320ad1c5785aca0113dec9843d8, ..\GCModeller\models\Networks\STRING\FunctionalNetwork\GraphModel.vb"

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
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Data.STRING

Public Module GraphModel

    <Extension>
    Public Function CreateGraph(edges As IEnumerable(Of InteractExports), nodes As IEnumerable(Of Coordinates), Optional factor# = 1500) As NetworkGraph
        Dim g As New NetworkGraph
        Dim gEdges As New List(Of Edge)
        Dim gNodes = nodes _
            .Select(Function(n)
                        Return New Graph.Node With {
                            .ID = n.node,
                            .Data = New NodeData With {
                                .label = n.node,
                                .Color = n.color.GetBrush,
                                .origID = n.node,
                                .initialPostion = New FDGVector2(n.x_position * factor, n.y_position * factor)
                            }
                        }
                    End Function) _
            .ToDictionary

        For Each edge As InteractExports In edges
            gEdges += New Edge With {
                .U = gNodes(edge.node1),
                .V = gNodes(edge.node2),
                .Data = New EdgeData With {
                    .weight = edge.combined_score
                }
            }
        Next

        g.nodes = gNodes.Values.AsList
        g.edges = gEdges

        Call g.ComputeNodeDegrees
        Call g.UsingDegreeAsRadius

        Return g
    End Function
End Module

