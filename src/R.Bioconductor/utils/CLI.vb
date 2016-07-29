Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON
Imports RDotNet.Extensions.Bioinformatics
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.gplots
Imports RDotNET.Extensions.VisualBasic.grDevices
Imports RDotNET.Extensions.VisualBasic.utils.read.table

Module CLI

    <ExportAPI("/heatmap",
               Info:="Drawing a heatmap by using a matrix.",
               Example:="",
               Usage:="/heatmap /in <dataset.csv> [/out <out.tiff> /width 4000 /height 3000 /colors <RExpression>]")>
    <ParameterInfo("/in", False,
                   Description:="A matrix dataset, and first row in this csv file needs to be the property of the object and rows are the object entity.
                   Example can be found at datasets: .../datasets/ppg2008.csv")>
    <ParameterInfo("/colors", True,
                   Description:="The color schema of your heatmap, default this parameter is null and using brewer.pal(10,""RdYlBu"") from RColorBrewer.
                   This value should be an R expression.")>
    Public Function heatmap(args As CommandLine.CommandLine) As Integer
        Dim inSet As String = args("/in")
        Dim out As String = args.GetValue("/out", inSet.TrimSuffix & ".heatmap.tiff")
        Dim outDIR As String = out.ParentPath
        Dim colors As String = args("/colors")
        Dim width As Integer = args.GetValue("/width", 4000)
        Dim height As Integer = args.GetValue("/height", 3000)
        Dim hmapAPI As heatmap2 = heatmap2.Puriney

        If Not String.IsNullOrEmpty(colors) Then
            hmapAPI.col = colors
        Else
            RServer.WriteLine(New jetColors)
            hmapAPI.col = jetColors.Call
        End If

        hmapAPI.scale = "column"

        Dim hmap As New Heatmap With {
            .dataset = New readcsv(inSet),
            .heatmap = hmapAPI,
            .image = New png(out, width, height)
        }

        Dim script As String = hmap.RScript

        Call RServer.WriteLine(script)
        Call script.SaveTo(outDIR & "/heatmap.r")
        Call heatmap2OUT.RParser(hmap.output, hmap.locusId, hmap.samples).GetJson.SaveTo(outDIR & "/heatmap.output.json")

        Return 0
    End Function

    <ExportAPI("/heatmap.partitions",
               Usage:="/heatmap.partitions /in <heatmap_out.json> [/out <outDIR>]")>
    Public Function heatmapPartitions(args As CommandLine.CommandLine) As Integer
        Dim injs As String = args("/in")
        Dim out As String = args.GetValue("/out", injs.TrimSuffix & ".Meta/")
        Dim heatmap = JsonContract.LoadJsonFile(Of heatmap2OUT)(injs)
        Dim locusTree = heatmap.GetRowDendrogram()
        Dim phenosTree = heatmap.GetColDendrogram()

        Throw New NotImplementedException
    End Function
End Module
