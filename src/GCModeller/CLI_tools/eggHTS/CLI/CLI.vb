Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
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
    Public Function KEGG_enrichment(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim enrichments As IEnumerable(Of EnrichmentTerm) = [in].LoadCsv(Of EnrichmentTerm)
        Dim pvalue As Double = args.GetValue("/pvalue", 0.05)
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $".GO_enrichment.pvalue={pvalue}.png")
        Dim size As String = args.GetValue("/size", "2000,1600")
        Dim plot As Bitmap = enrichments.KEGGEnrichmentPlot(size.SizeParser, pvalue)

        Return plot.SaveAs(out, ImageFormats.Png).CLICode
    End Function

    Public Function KOBASSplit() As Integer

    End Function
End Module
