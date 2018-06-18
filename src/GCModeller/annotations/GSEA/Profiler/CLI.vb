Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.GeneOntology.OBO

<CLI>
Public Module CLI

    <ExportAPI("/KO.clusters")>
    <Usage("/KO.clusters /uniprot <uniprot.XML> /maps <kegg_maps.XML/directory> [/out <clusters.XML>]")>
    Public Function CreateKOCluster(args As CommandLine) As Integer
        Dim uniprot$ = args <= "/uniprot"
        Dim maps$ = args <= "/maps"
        Dim out$ = args("/out") Or $"{uniprot.TrimSuffix}_KO.XML"
        Dim kegg = (ls - l - r - "*.Xml" <= maps).Select(AddressOf LoadXml(Of Map))
        Dim entries = UniProtXML.EnumerateEntries(uniprot)
        Dim model As Genome = GSEA.ImportsUniProt(
            entries,
            getTerm:=GSEA.UniProtGetKOTerms,
            define:=GSEA.KEGGClusters(kegg)
        )

        Return model.GetXml.SaveTo(out).CLICode
    End Function

    <ExportAPI("/GO.clusters")>
    <Usage("/GO.clusters /uniprot <uniprot.XML> /go <go.obo> [/out <clusters.XML>]")>
    Public Function CreateGOClusters(args As CommandLine) As Integer
        Dim uniprot$ = args <= "/uniprot"
        Dim obo$ = args <= "/go"
        Dim out$ = args("/out") Or $"{uniprot.TrimSuffix}_GO.XML"
        Dim go = GSEA.Imports.GOClusters(GO_OBO.Open(obo))
        Dim entries = UniProtXML.EnumerateEntries(uniprot)
        Dim model As Genome = GSEA.Imports.ImportsUniProt(
            entries,
            getTerm:=GSEA.UniProtGetGOTerms,
            define:=go
        )

        Return model.GetXml.SaveTo(out).CLICode
    End Function

    <ExportAPI("/GSEA")>
    <Usage("/GSEA /background <clusters.XML> /geneSet <geneSet.txt> /uniprot <uniprot.XML> [/out <out.csv>]")>
    Public Function EnrichmentTest(args As CommandLine) As Integer
        Dim backgroundXML$ = args("/background")
        Dim background = backgroundXML.LoadXml(Of Genome)
        Dim list$ = args("/geneset")
        Dim geneSet$() = list _
            .IterateAllLines _
            .Select(Function(l)
                        Return Strings.Trim(l).Split.First
                    End Function) _
            .ToArray
        Dim out$ = args("/out") Or $"{list.TrimSuffix}_{backgroundXML.BaseName}_enrichment.csv"

        ' 先推测一下是否是uniprot编号？
        ' 如果不是，则会需要进行编号转换操作
        Dim convertor As New IDConvertor(UniProtXML.EnumerateEntries(args <= "/uniprot"))
        Dim type As IDTypes = convertor.GetType(geneSet)

        If type = IDTypes.NA Then
            Throw New NotSupportedException(geneSet.GetJson)
        End If

        Dim converts As NamedVector(Of String)() = convertor _
            .Converts(geneSet, type) _
            .ToArray
        Dim convertGeneSet$() = converts _
            .Select(Function(c) c.vector) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim result As EnrichmentResult() = background _
            .Enrichment(convertGeneSet) _
            .FDRCorrection _
            .ToArray

        Call converts _
            .Select(Function(c)
                        Return $"{c.name}{ASCII.TAB}{c.vector.JoinBy(",")}"
                    End Function) _
            .FlushAllLines(out.TrimSuffix & ".converts.txt")

        Return result.SaveTo(out).CLICode
    End Function
End Module
