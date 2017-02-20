Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Visualize

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

    <ExportAPI("/DEP.uniprot.list2",
               Usage:="/DEP.uniprot.list2 /in <log2.test.csv> [/DEP.Flag <is.DEP?> /uniprot.Flag <uniprot> /species <scientifcName> /uniprot <uniprotXML> /out <out.txt>]")>
    Public Function DEPUniprotIDs2(args As CommandLine) As Integer
        Dim [in] = args("/in")
        Dim DEPFlag As String = args.GetValue("/DEP.flag", "is.DEP?")
        Dim uniprot As String = args.GetValue("/uniprot.Flag", "uniprot")
        Dim data = EntityObject.LoadDataSet([in])
        Dim DEPs = data.Where(Function(prot) prot(DEPFlag).getBoolean).ToArray
        Dim uniprotIDs$() = DEPs _
            .Select(Function(prot) prot(uniprot).Split(";"c)) _
            .Unlist _
            .Distinct _
            .Select(AddressOf Trim) _
            .ToArray
        Dim sciName$ = args("/species")
        Dim out As String = args.GetValue(
            "/out",
            [in].TrimSuffix & $"DEPs={DEPs.Length}.uniprotIDs.txt")

        uniprot$ = args("/uniprot")

        If Not sciName.StringEmpty AndAlso uniprot.FileExists(True) Then
            ' 将结果过滤为指定的物种的编号
            Dim table As Dictionary(Of entry) = UniprotXML.LoadDictionary(uniprot)
            uniprotIDs = uniprotIDs _
                .Where(Function(ID) table.ContainsKey(ID) AndAlso
                                    table(ID).organism.scientificName = sciName) _
                .ToArray
        End If

        Return uniprotIDs.SaveTo(out).CLICode
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

    <ExportAPI("/DEP.logFC.hist", Usage:="/DEP.logFC.hist /in <log2test.csv> [/tag <logFC> /size <1600,1200> /out <out.png>]")>
    Public Function logFCHistogram(args As CommandLine) As Integer
        Dim [in] = args("/in")
        Dim tag As String = args.GetValue("/tag", "logFC")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".logFCHistogram.png")
        Dim data = EntityObject.LoadDataSet([in])

        Return data _
            .logFCHistogram(tag,
                            size:=args.GetValue("/size", New Size(1600, 1200)),
                            [step]:=0.5) _
            .SaveAs(out) _
            .CLICode
    End Function

    <ExportAPI("/DEP.logFC.Volcano", Usage:="/DEP.logFC.Volcano /in <DEP.qlfTable.csv> [/size <1600,1200> /out <plot.csv>]")>
    Public Function logFCVolcano(args As CommandLine) As Integer
        Dim in$ = args("/in")
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".DEP.vocano.plot.png")
        Dim sample = EntityObject.LoadDataSet([in])
        Dim size As Size = args.GetValue("/size", New Size(1600, 1200))

        Return Volcano.PlotDEGs(sample, pvalue:="PValue",
                                displayLabel:=LabelTypes.DEG,
                                size:=size) _
            .SaveAs(out) _
            .CLICode
    End Function
End Module