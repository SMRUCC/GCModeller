Imports System.Drawing
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors

Module Module1

    Sub Main()
        Dim json = SMRUCC.WebCloud.d3js.Network.htmlwidget.BuildData.BuildGraph("../../..\viewer.html")
        Dim colors As SolidBrush() = Designer.GetColors("d3.scale.category10()") _
            .Select(Function(c) New SolidBrush(c)) _
            .ToArray
        Dim graph = json.CreateGraph(Function(n) colors(CInt(n.NodeType)))

        NetworkVisualizer.DefaultEdgeColor = Color.DimGray

        Call graph.doRandomLayout
        Call graph.doForceLayout(showProgress:=True, Repulsion:=1800, Stiffness:=50, Damping:=0.45, iterations:=1500)
        Call graph.Tabular.Save("./test")
        Call graph.DrawImage(
            New Size(1200, 1200),
            scale:=3.5,
            labelColorAsNodeColor:=True).Save("../../..\/viewer.png")
    End Sub
End Module
