Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.Uniprot.Web
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.SequenceModel.FASTA

Module CLI

    ''' <summary>
    ''' go enrichment 绘图
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Go.enrichment.plot", Usage:="/Go.enrichment.plot /in <enrichmentTerm.csv> [/pvalue <0.05> /size <2000,1600> /go <go.obo> /out <out.png>]")>
    <Group(CLIGroups.Enrichment_CLI)>
    Public Function GO_enrichment(args As CommandLine) As Integer
        Dim goDB As String = args.GetValue("/go", GCModeller.FileSystem.GO & "/go.obo")
        Dim terms = GO_OBO.Open(goDB).ToDictionary(Function(x) x.id)
        Dim [in] As String = args("/in")
        Dim enrichments As IEnumerable(Of EnrichmentTerm) = [in].LoadCsv(Of EnrichmentTerm)
        Dim pvalue As Double = args.GetValue("/pvalue", 0.05)
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $".GO_enrichment.pvalue={pvalue}.png")
        Dim size As String = args.GetValue("/size", "2000,1600")
        Dim plot As Bitmap = enrichments.EnrichmentPlot(terms, pvalue, size.SizeParser)

        Return plot.SaveAs(out, ImageFormats.Png).CLICode
    End Function

    <ExportAPI("/KEGG.enrichment.plot", Usage:="/KEGG.enrichment.plot /in <enrichmentTerm.csv> [/pvalue <0.05> /size <2000,1600> /out <out.png>]")>
    <Group(CLIGroups.Enrichment_CLI)>
    Public Function KEGG_enrichment(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim enrichments As IEnumerable(Of EnrichmentTerm) = [in].LoadCsv(Of EnrichmentTerm)
        Dim pvalue As Double = args.GetValue("/pvalue", 0.05)
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $".GO_enrichment.pvalue={pvalue}.png")
        Dim size As String = args.GetValue("/size", "2000,1600")
        Dim plot As Bitmap = enrichments.KEGGEnrichmentPlot(size.SizeParser, pvalue)

        Return plot.SaveAs(out, ImageFormats.Png).CLICode
    End Function

    ''' <summary>
    ''' 4. 导出KOBAS结果
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/KOBAS.split", Usage:="/KOBAS.split /in <kobas.out_run.txt> [/out <DIR>]")>
    <Group(CLIGroups.Enrichment_CLI)>
    Public Function KOBASSplit(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix)

        Call KOBAS.SplitData([in], out)

        Return 0
    End Function

    ''' <summary>
    ''' Sample文件之中必须有一列ORF列，有一列为uniprot编号为主键
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/KOBAS.add.ORF", Usage:="/KOBAS.add.ORF /in <table.csv> /sample <sample.csv> [/out <out.csv>]")>
    <Group(CLIGroups.Enrichment_CLI)>
    Public Function KOBASaddORFsource(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim sample As String = args("/sample")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-ORF.csv")
        Dim table As List(Of EnrichmentTerm) = [in].LoadCsv(Of EnrichmentTerm)
        Dim sampleData As Dictionary(Of String, String()) =
            EntityObject _
            .LoadDataSet(sample).GroupBy(Function(p) p.ID) _
            .ToDictionary(Function(u) u.Key,
                          Function(g) g.Select(Function(u) u("ORF")).Distinct.ToArray)

        For Each term As EnrichmentTerm In table
            Dim uniprotIDs = term.Input.Split("|"c)
            Dim ORFs As String() = sampleData _
                .Selects(uniprotIDs, True) _
                .IteratesALL _
                .ToArray

            term.ORF = ORFs
        Next

        Return table.SaveTo(out).CLICode
    End Function
End Module
