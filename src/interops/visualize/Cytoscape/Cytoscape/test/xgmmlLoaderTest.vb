#Region "Microsoft.VisualBasic::8a72e30bf9f33b5ae9191b752f52e930, visualize\Cytoscape\Cytoscape\test\xgmmlLoaderTest.vb"

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

    ' Module xgmmlLoaderTest
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML

Module xgmmlLoaderTest

    Sub Main()
        Dim g = XGMML.RDFXml.Load("E:\GCModeller\src\interops\visualize\Cytoscape\data\demo.xgmml")
        Dim firstEdge = g.edges(1)
        Dim bend = firstEdge.graphics.edgeBendHandles
        Dim bendPoints = New GraphIndex(g).GetEdgeBends(firstEdge)
        Dim tuple = New GraphIndex(g).GetEdgeNodes(firstEdge)

        Call Console.WriteLine(tuple.source.ToString)

        For Each bpt As PointF In bendPoints
            Call Console.WriteLine(bpt.ToString)
        Next

        Call Console.WriteLine(tuple.target.ToString)

        Pause()
    End Sub
End Module

