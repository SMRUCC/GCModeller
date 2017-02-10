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

    <ExportAPI("/DEP.venn", Usage:="/DEP.venn /data <Directory> [/FC.tag <FC.avg> /pvalue <p.value> /out <out.DIR>]")>
    Public Function VennData(args As CommandLine) As Integer
        Dim DIR$ = args("/data")
        Dim FCtag$ = args.GetValue("/FC.tag", "FC.avg")
        Dim pvalue$ = args.GetValue("/pvalue", "p.value")
        Dim out As String = args.GetValue("/out", DIR.TrimDIR & ".venn/")

        Call DEGDesigner _
            .MergeMatrix(DIR, "*.csv", 1.5, 0.05, FCtag, 1 / 1.5, pvalue) _
            .SaveDataSet(out & "/DEP.venn.csv")


        Return 0
    End Function
End Module