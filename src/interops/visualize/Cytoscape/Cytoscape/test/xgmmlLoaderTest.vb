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
