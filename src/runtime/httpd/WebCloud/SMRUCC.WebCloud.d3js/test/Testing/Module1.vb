Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts

Module Module1

    Sub Main()
        Dim json = SMRUCC.WebCloud.d3js.Network.htmlwidget.BuildData.BuildGraph("D:\GCModeller\src\runtime\httpd\WebCloud\SMRUCC.WebCloud.d3js\test\viewer.html")
        Dim graph = json.CreateGraph
        Call graph.doForceLayout
        Call graph.Tabular.Save("./test")
    End Sub
End Module
