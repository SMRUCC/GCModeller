Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.Data

Public Module WebParser

    Const dataAPI As String = "http://origin.tubic.org/deg/public/index.php/browse/bacteria"

    Public Sub ParserWorkflow(save As String)
        Dim genomes = GetGenomeList().ToArray
        Dim web As New WebQuery(Of Genome)(Function(genome) sprintf(listAPI, genome.ID, genome.ID, 1), Function(g) g.ID, Function(s, type) s, $"{save}/.essentialgenes")

        For Each genome As Genome In genomes
            Dim html$ = web.Query(Of String)({genome}, "*.html").First
            Dim saveXml$ = $"{save}/{genome.Organism.NormalizePathString}.Xml"

            genome.EssentialGenes = genome.ParseDEGList(html, $"{save}/{genome.Organism.NormalizePathString}").ToArray
            genome.GetXml.SaveTo(saveXml)
        Next
    End Sub

    Public Iterator Function GetGenomeList() As IEnumerable(Of Genome)
        Dim table = dataAPI.GET.GetTablesHTML.FirstOrDefault

        If table.StringEmpty Then
            Return
        End If

        Dim rows = table.GetRowsHTML

        For Each row As String In rows.Skip(1)
            Dim columns = row.GetColumnsHTML

            Yield New Genome With {
                .ID = columns(2).href.BaseName,
                .Organism = columns(1).StripHTMLTags,
                .numOfDEG = columns(2).StripHTMLTags,
                .Conditions = columns(3),
                .Reference = columns(4)
            }
        Next
    End Function

    Const listAPI$ = "http://origin.tubic.org/deg/public/index.php/query/bacteria/degac/%s.html?lineage=bacteria&field=degac&term=%s&page=%s"

    <Extension>
    Public Function ParseDEGList(genome As Genome) As IEnumerable(Of EssentialGene)
        Return genome.ParseDEGList(sprintf(listAPI, genome.ID, genome.ID, 1).GET)
    End Function

    Private Function getTotalPages(html As String) As Integer
        Dim alist = html.Matches("<a.+?</a>").ToArray
        Dim pagelink = alist.Where(Function(a) a.class = "page-link").ToArray
        Dim spans = pagelink.Last.Matches("<span.+?</span>").ToArray
        Dim textPrimary = spans.Where(Function(s) s.class = "text-primary").ToArray

        Return textPrimary.Last.StripHTMLTags
    End Function

    <Extension>
    Private Iterator Function ParseDEGList(genome As Genome, html As String, Optional cache$ = "./") As IEnumerable(Of EssentialGene)
        Dim allPages As Integer = getTotalPages(html)
        Dim web As New WebQuery(Of Integer)(Function(page) sprintf(listAPI, genome.ID, genome.ID, page), Function(page) CStr(page), Function(s, type) s, cache:=cache)
        Dim detailsCache$ = $"{cache}/details/"

        For Each gene As EssentialGene In html.parseDEGList(detailsCache)
            Yield gene
        Next

        For Each geneList In web.Query(Of String)(Enumerable.Range(2, allPages), "*.html")
            For Each gene In geneList.parseDEGList(detailsCache)
                Yield gene
            Next
        Next
    End Function

    Const detailsAPI As String = "http://origin.tubic.org/deg/public/index.php/information/bacteria/%s.html"

    <Extension>
    Private Iterator Function parseDEGList(html As String, cache$) As IEnumerable(Of EssentialGene)
        Dim table$ = html.GetTablesHTML.First
        Dim rows = table.GetRowsHTML
        Dim web As New WebQuery(Of EssentialGene)(Function(g) sprintf(detailsAPI, g.ID), Function(g) g.ID, Function(s, type) s, cache)
        Dim parseList = Iterator Function() As IEnumerable(Of EssentialGene)
                            For Each row As String In rows.Skip(1)
                                Dim columns = row.GetColumnsHTML

                                Yield New EssentialGene With {
                                    .ID = columns(1).StripHTMLTags,
                                    .Name = columns(2),
                                    .FunctionDescrib = columns(3),
                                    .Organism = columns(4)
                                }
                            Next
                        End Function

        For Each gene As EssentialGene In parseList()
            Dim details As String = web.Query(Of String)({gene}, "*.html").First

            Yield gene.fillDetails(details)
        Next
    End Function

    <Extension>
    Private Function fillDetails(gene As EssentialGene, html$) As EssentialGene
        Dim table = html.GetTablesHTML.First
        Dim rows = table.GetRowsHTML
        Dim details = rows.Skip(1).Select(Function(r) r.GetColumnsHTML).ToDictionary(Function(c) c.First.Replace(" ", "_"), Function(c) c.Last)

        With details
            gene.RefSeq = !Gene_ref
            gene.UniProt = !Uniprot_accession
            gene.COG = !COG
            gene.GO = !GO_annotation.Split
            gene.FuncClass = !Function_Class
            gene.Condition = !Condition
            gene.Reference = !Reference
            gene.Nt = !Nucleotide_sequence
            gene.Aa = !Amino_acid_sequence
        End With

        Return gene
    End Function
End Module

Public Class EssentialGene
    Public Property ID As String
    Public Property Name As String
    Public Property FunctionDescrib As String
    Public Property Organism As String
    Public Property geneRef As String
    Public Property RefSeq As String
    Public Property UniProt As String
    Public Property COG As String
    Public Property GO As String()
    Public Property FuncClass As String
    Public Property Reference As String
    Public Property Condition As String
    Public Property Nt As String
    Public Property Aa As String

End Class

Public Class Genome : Inherits XmlDataModel
    Implements Enumeration(Of EssentialGene)

    Public Property ID As String
    Public Property Organism As String
    Public Property numOfDEG As Integer
    Public Property Conditions As String
    Public Property Reference As String

    Public Property EssentialGenes As EssentialGene()

    Public Iterator Function GenericEnumerator() As IEnumerator(Of EssentialGene) Implements Enumeration(Of EssentialGene).GenericEnumerator
        For Each gene As EssentialGene In EssentialGenes
            Yield gene
        Next
    End Function

    Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of EssentialGene).GetEnumerator
        Yield GenericEnumerator()
    End Function
End Class
