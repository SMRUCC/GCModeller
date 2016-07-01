Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.GCModeller.AnalysisTools.ModelSolvers.FBA.Models.rFBA
Imports RDotNET.Extensions.Bioinformatics
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.gplots
Imports RDotNET.Extensions.VisualBasic.grDevices
Imports RDotNET.Extensions.VisualBasic.utils.read.table

Partial Module CLI

    <ExportAPI("/heatmap",
               Info:="Draw heatmap from the correlations between the genes and the metabolism flux.",
               Usage:="/heatmap /x <matrix.csv> [/out <out.tiff> /name <Name> /width <8000> /height <6000>]")>
    Public Function Heatmap(args As CommandLine.CommandLine) As Integer
        Dim inX As String = args("/x")
        Dim out As String = args.GetValue("/out", inX.TrimFileExt & ".tiff")
        Dim outDIR As String = out.ParentPath
        Dim nameMap As String = args.GetValue("/name", "Name")
        Dim script As New Heatmap With {
            .dataset = New readcsv(inX),
            .rowNameMaps = nameMap,
            .image = New tiff(out, args.GetValue("/width", 8000), args.GetValue("/height", 6000)),
            .heatmap = heatmap2.Puriney
        }

        Dim scriptText As String = script.RScript
        Dim STD As String() = RServer.WriteLine(scriptText)
        Dim result As heatmap2OUT = heatmap2OUT.RParser(script.output, script.locusId, script.samples)

        Call scriptText.SaveTo(outDIR & "/heatmap.r")
        Call result.GetJson.SaveTo(outDIR & "/heatmap.result.json")
        Return STD.FlushAllLines(outDIR & "/heatmap.STD.txt")
    End Function

    <ExportAPI("/heatmap.scale", Usage:="/heatmap.scale /x <matrix.csv> [/factor 30 /out <out.csv>]")>
    Public Function ScaleHeatmap(args As CommandLine.CommandLine) As Integer
        Dim inX As String = args("/x")
        Dim factor As Double = args.GetValue("/factor", 30)
        Dim out As String = args.GetValue("/out", inX.TrimFileExt & "-" & factor & "__scales.csv")
        Dim MAT As File = File.Load(inX)
        Dim title As RowObject = MAT.First()
        title(Scan0) = NameOf(RPKMStat.Locus)
        Dim data = MAT.AsDataSource(Of RPKMStat)
        data = (From x As RPKMStat In data
                Select New RPKMStat With {
                    .Locus = x.Locus,
                    .Properties = x.Properties _
                        .ToDictionary(Function(key) key.Key,
                                      Function(v) v.Value * factor)}).ToArray
        Return data.SaveTo(out).CLICode
    End Function
End Module
