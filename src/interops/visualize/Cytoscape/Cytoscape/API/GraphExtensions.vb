#Region "Microsoft.VisualBasic::f557146b93e4644f68d48cdb39029a90, visualize\Cytoscape\Cytoscape\API\GraphExtensions.vb"

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

    '     Module GraphExtensions
    ' 
    '         Function: __edge, __node, CreateGraph
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML

Namespace API

    <HideModuleName>
    Public Module GraphExtensions

        ''' <summary>
        ''' Creates the network graph model from the Cytoscape data model to generates the network layout or visualization 
        ''' </summary>
        ''' <param name="g"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CreateGraph(g As Graph) As NetworkGraph
            Dim nodes As Network.Graph.Node() =
                LinqAPI.Exec(Of Network.Graph.Node) <= From n As XGMML.Node
                                                       In g.Nodes
                                                       Select n.__node()
            Dim nodeHash As New Dictionary(Of Network.Graph.Node)(nodes)
            Dim edges As Network.Graph.Edge() =
                LinqAPI.Exec(Of Network.Graph.Edge) <= From edge As XGMML.Edge
                                                       In g.Edges
                                                       Select edge.__edge(nodeHash)
            Dim net As New NetworkGraph(nodes, edges)

            Return net
        End Function

        <Extension>
        Private Function __node(n As XGMML.Node) As Network.Graph.Node
            Dim data As New NodeData With {
                .Color = New SolidBrush(n.Graphics.FillColor),
                .radius = n.Graphics.radius
            }

            Return New Network.Graph.Node(n.id, data)
        End Function

        <Extension>
        Private Function __edge(edge As XGMML.Edge, nodeHash As Dictionary(Of Network.Graph.Node)) As Network.Graph.Edge
            Dim data As New EdgeData

            Return New Network.Graph.Edge(
                CStr(edge.Id),
                nodeHash(edge.source),
                nodeHash(edge.target),
                data)
        End Function
    End Module
End Namespace
