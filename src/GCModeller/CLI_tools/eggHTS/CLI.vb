Imports System.Drawing
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Analysis.Microarray.DAVID
Imports SMRUCC.genomics.Assembly.Uniprot.Web
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.GeneOntology.GoStat
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.CommandLine
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports Microsoft.VisualBasic.Scripting
Imports System.IO
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly
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

    <ExportAPI("/protein.EXPORT",
               Usage:="/protein.EXPORT /in <uniprot.xml> [/sp <name> /exclude /out <out.fasta>]")>
    <Argument("/sp", True, CLITypes.String,
              AcceptTypes:={GetType(String)},
              Description:="The organism scientific name.")>
    Public Function proteinEXPORT(args As CommandLine) As Integer
        Dim [in] As String = args <= "/in"
        Dim sp As String = args <= "/sp"
        Dim exclude As Boolean = args.GetBoolean("/exclude")
        Dim out As String = args.GetValue(
            "/out",
            [in].TrimSuffix & $"{If(sp.IsBlank, "", If(exclude, "-exclude", "") & "-" & sp.NormalizePathString)}.fasta")
        Dim uniprotXML As UniprotXML = UniprotXML.Load([in])

        Using writer As StreamWriter = out.OpenWriter(Encodings.ASCII)
            Dim source As IEnumerable(Of Uniprot.XML.entry) = uniprotXML.entries

            If Not String.IsNullOrEmpty(sp) Then
                If exclude Then
                    source = source _
                        .Where(Function(gene) Not gene.organism.scientificName = sp) _
                        .ToArray
                Else
                    source = source _
                        .Where(Function(gene) gene.organism.scientificName = sp) _
                        .ToArray
                End If
            End If

            For Each prot As Uniprot.XML.entry In uniprotXML.entries
                Dim fa As New FastaToken With {
                    .SequenceData = prot.sequence.sequence.lTokens.JoinBy(""),
                    .Attributes = {prot.gene.ORF}
                }
                Call writer.WriteLine(fa.GenerateDocument(-1))
            Next
        End Using

        Return 0
    End Function
End Module
