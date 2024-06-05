﻿#Region "Microsoft.VisualBasic::4787860a519fa1c614729b4bf813568d, models\Networks\STRING\FunctionalNetwork\GraphModel.vb"

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

    '   Total Lines: 47
    '    Code Lines: 41 (87.23%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (12.77%)
    '     File Size: 1.84 KB


    ' Module GraphModel
    ' 
    '     Function: CreateGraph
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
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
                .data = New EdgeData,
                .weight = edge.combined_score
            }
        Next

        Dim g As New NetworkGraph(gNodes.Values, gEdges)

        Call g.ComputeNodeDegrees
        Call g.UsingDegreeAsRadius

        Return g
    End Function
End Module
