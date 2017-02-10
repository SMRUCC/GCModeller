Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.Analysis.Microarray

Partial Module CLI

    <ExportAPI("/DEP.uniprot.list",
               Usage:="/DEP.uniprot.list /DEP <log2-test.DEP.csv> /sample <sample.csv> [/out <out.txt>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function DEPUniprotIDlist(args As CommandLine) As Integer
        Dim DEP As String = args("/DEP")
        Dim sample As String = args("/sample")
        Dim out As String = args.GetValue("/out", DEP.TrimSuffix & "-uniprot.ID.list.txt")
        Dim DEP_data As IEnumerable(Of DEP) = EntityObject _
            .LoadDataSet(Of DEP)(path:=DEP) _
            .Where(Function(d) d.isDEP) _
            .ToArray
        Dim sampleData As Dictionary(Of String, String()) =
            sample _
            .LoadCsv(Of UniprotAnnotations) _
            .GroupBy(Function(p) p.ORF) _
            .ToDictionary(Function(p) p.Key,
                          Function(g) g.ToArray(
                          Function(p) p.ID))
        Dim list$() = DEP_data _
            .Select(Function(d) sampleData(d.ID)) _
            .IteratesALL _
            .Distinct _
            .ToArray

        Return list.SaveTo(out).CLICode
    End Function

    <ExportAPI("/DEP.venn",
               Info:="Generate the VennDiagram plot data and the venn plot tiff.",
               Usage:="/DEP.venn /data <Directory> [/FC.tag <FC.avg> /title <VennDiagram title> /pvalue <p.value> /out <out.DIR>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function VennData(args As CommandLine) As Integer
        Dim DIR$ = args("/data")
        Dim FCtag$ = args.GetValue("/FC.tag", "FC.avg")
        Dim pvalue$ = args.GetValue("/pvalue", "p.value")
        Dim out As String = args.GetValue("/out", DIR.TrimDIR & ".venn/")
        Dim dataOUT = out & "/DEP.venn.csv"
        Dim title$ = args.GetValue("/title", "VennDiagram title")

        Call DEGDesigner _
            .MergeMatrix(DIR, "*.csv", 1.5, 0.05, FCtag, 1 / 1.5, pvalue) _
            .SaveDataSet(dataOUT)
        Call Apps.VennDiagram.Draw(dataOUT, title, out:=out & "/venn.tiff")

        Return 0
    End Function

    <ExportAPI("/DEP.heatmap",
               Info:="Generates the heatmap plot input data.",
               Usage:="/DEP.heatmap /data <Directory> [/FC.tag <FC.avg> /pvalue <p.value> /out <out.csv>]")>
    <Group(CLIGroups.DEP_CLI)>
    Public Function Heatmap(args As CommandLine) As Integer
        Dim DIR$ = args("/data")
        Dim FCtag$ = args.GetValue("/FC.tag", "FC.avg")
        Dim pvalue$ = args.GetValue("/pvalue", "p.value")
        Dim out As String = args.GetValue("/out", DIR.TrimDIR & ".heatmap/")
        Dim dataOUT = out & "/DEP.heatmap.csv"

        Return DEGDesigner _
            .MergeMatrix(DIR, "*.csv", 1.5, 0.05, FCtag, 1 / 1.5, pvalue) _
            .SaveDataSet(dataOUT, blank:=1)
    End Function
End Module