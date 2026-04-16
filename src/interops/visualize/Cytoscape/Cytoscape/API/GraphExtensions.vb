#Region "Microsoft.VisualBasic::72b80b3f9836df47e94b1e0a030d14ef, visualize\Cytoscape\Cytoscape\API\GraphExtensions.vb"

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

    '   Total Lines: 82
    '    Code Lines: 68 (82.93%)
    ' Comment Lines: 5 (6.10%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 9 (10.98%)
    '     File Size: 3.37 KB


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

#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
#End If

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
