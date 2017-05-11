Imports System.Drawing
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Data.visualize.Network.Visualize
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors

Module Module1

    Sub Main()
        Dim json = SMRUCC.WebCloud.d3js.Network.htmlwidget.BuildData.BuildGraph("../../..\viewer.html")
        Dim colors As SolidBrush() = Designer.GetColors("Paired:c12").Select(Function(c) New SolidBrush(c)).ToArray
        Dim graph = json.CreateGraph(Function(n) colors(CInt(n.NodeType)))
        Call graph.doRandomLayout
        Call graph.doForceLayout(showProgress:=True, iterations:=50)
        Call graph.Tabular.Save("./test")
        Call graph.DrawImage(New Size(2000, 2000), scale:=4).Save("../../..\/viewer.png")
    End Sub
End Module
