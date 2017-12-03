Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Partial Module CLI

    ''' <summary>
    ''' 将perseus软件的输出转换为csv文档并且导出uniprot编号以方便进行注释
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Perseus.Table")>
    <Usage("/Perseus.Table /in <proteinGroups.txt> [/out <out.csv>]")>
    <Group(CLIGroups.Samples_CLI)>
    Public Function PerseusTable(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".csv")
        Dim data As Perseus() = [in].LoadTsv(Of Perseus)
        Dim idlist As String() = data _
            .Select(Function(prot) prot.ProteinIDs) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim uniprotIDs$() = idlist _
            .Select(Function(s) s.Split("|"c, ":"c)(1)) _
            .Distinct _
            .ToArray

        Call idlist.SaveTo(out.TrimSuffix & ".proteinIDs.txt")
        Call uniprotIDs.SaveTo(out.TrimSuffix & ".uniprotIDs.txt")

        Return data.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Perseus.Stat")>
    <Usage("/Perseus.Stat /in <proteinGroups.txt> [/out <out.csv>]")>
    <Group(CLIGroups.Samples_CLI)>
    Public Function PerseusStatics(args As CommandLine) As Integer
        Dim in$ = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".perseus.Stat.csv")
        Dim data As Perseus() = [in].LoadTsv(Of Perseus)
        Dim csv As New IO.File

        Call csv.AppendLine({"MS/MS", CStr(Perseus.TotalMSDivideMS(data))})
        Call csv.AppendLine({"Peptides", CStr(Perseus.TotalPeptides(data))})
        Call csv.AppendLine({"ProteinGroups", CStr(data.Length)})

        Return csv.Save(out, Encodings.ASCII).CLICode
    End Function

    <ExportAPI("/Perseus.MajorityProteinIDs")>
    <Usage("/Perseus.MajorityProteinIDs /in <table.csv> [/out <out.txt>]")>
    <Description("Export the uniprot ID list from ``Majority Protein IDs`` row and generates a text file for batch search of the uniprot database.")>
    Public Function MajorityProteinIDs(args As CommandLine) As Integer
        With args <= "/in"
            Dim out$ = (args <= "/out") Or (.TrimSuffix & "-uniprotID.txt").AsDefault
            Dim table As Perseus() = .LoadCsv(Of Perseus)
            Dim major$() = table _
                .Select(Function(protein)
                            Return protein.Majority_proteinIDs
                        End Function) _
                .IteratesALL _
                .Distinct _
                .ToArray

            Return major _
                .FlushAllLines(out) _
                .CLICode
        End With
    End Function

    <ExportAPI("/labelFree.t.test")>
    <Usage("/labelFree.t.test /in <matrix.csv> /sampleInfo <sampleInfo.csv> /design <analysis_designer.csv> [/level <default=1.5> /p.value <default=0.05> /FDR <default=0.05> /out <out.csv>]")>
    Public Function labelFreeTtest(args As CommandLine) As Integer
        Dim data As DataSet() = DataSet.LoadDataSet(args <= "/in").ToArray
        Dim level# = args.GetValue("/level", 1.5)
        Dim pvalue# = args.GetValue("/p.value", 0.05)
        Dim FDR# = args.GetValue("/FDR", 0.05)
        Dim out$ = args.GetValue("/out", (args <= "/in").TrimSuffix & ".log2FC.t.test.csv")
        Dim sampleInfo As SampleInfo() = (args <= "/sampleInfo").LoadCsv(Of SampleInfo)
        Dim designer As AnalysisDesigner = (args <= "/design").LoadCsv(Of AnalysisDesigner).First
        Dim DEPs As DEP_iTraq() = data.logFCtest(designer, sampleInfo, level, pvalue, FDR)

        Return DEPs _
            .Where(Function(x) x.log2FC <> 0R) _
            .ToArray _
            .SaveDataSet(out) _
            .CLICode
    End Function
End Module