#Region "Microsoft.VisualBasic::54cdc7a17c350a729938863acbc101d3, visualize\Cytoscape\Cytoscape\API\GraphExtensions.vb"

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
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.File

Namespace API

    <HideModuleName>
    Public Module GraphExtensions

        ''' <summary>
        ''' Creates the network graph model from the Cytoscape data model to generates the network layout or visualization 
        ''' </summary>
        ''' <param name="g"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CreateGraph(g As XGMMLgraph) As NetworkGraph
            Dim nodes As Network.Graph.Node() =
                LinqAPI.Exec(Of Network.Graph.Node) <= From n As XGMMLnode
                                                       In g.nodes
                                                       Select n.__node()
            Dim nodeHash As New Dictionary(Of Network.Graph.Node)(nodes)
            Dim edges As Network.Graph.Edge() =
                LinqAPI.Exec(Of Network.Graph.Edge) <= From edge As XGMMLedge
                                                       In g.edges
                                                       Select edge.__edge(nodeHash)
            Dim net As New NetworkGraph(nodes, edges)

            Return net
        End Function

        <Extension>
        Private Function __node(n As XGMMLnode) As Network.Graph.Node
            Dim data As New NodeData With {
                .color = New SolidBrush(n.graphics.FillColor),
                .size = {n.graphics.radius}
            }

            Return New Network.Graph.Node(n.id, data)
        End Function

        <Extension>
        Private Function __edge(edge As XGMMLedge, nodeHash As Dictionary(Of Network.Graph.Node)) As Network.Graph.Edge
            Dim data As New EdgeData

            Return New Network.Graph.Edge(
                CStr(edge.id),
                nodeHash(edge.source),
                nodeHash(edge.target),
                data)
        End Function
    End Module
End Namespace
