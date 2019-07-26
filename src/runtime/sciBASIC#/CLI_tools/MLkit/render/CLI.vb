Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging.Driver

Module CLI

    <ExportAPI("/network")>
    <Usage("/network /model <network_tables.directory> [/size <default=5000,3000> /out <image.png/svg>]")>
    Public Function VisualizeNetwork(args As CommandLine) As Integer
        Dim in$ = args("/model")
        Dim size$ = args("/size") Or "5000,3000"
        Dim out$ = args("/out") Or $"{[in].TrimDIR}/image.png"
        Dim model = NetworkTables.Load(DIR:=[in]).AnalysisDegrees
        Dim graph As NetworkGraph = model.CreateGraph
        Dim image As GraphicsData = graph.DrawImage(canvasSize:=size)

        Return image.Save(out).CLICode
    End Function
End Module
