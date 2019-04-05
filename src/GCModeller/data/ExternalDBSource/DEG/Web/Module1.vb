Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Module Module1

    Const dataAPI As String = "http://origin.tubic.org/deg/public/index.php/browse/bacteria"

    Public Iterator Function GetGenomeList() As IEnumerable(Of Genome)
        Dim table = dataAPI.GET.GetTablesHTML.FirstOrDefault

        If table.StringEmpty Then
            Return
        End If

        Dim rows = table.GetRowsHTML

        For Each row As String In rows
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
    Public Iterator Function ParseDEGList(genome As Genome) As IEnumerable(Of EssentialGene)
        Dim html = sprintf(listAPI, genome.ID, genome.ID, 1).GET
        Dim allPages As Integer = html.Matches("<a>.+?</a>").ToArray.FirstOrDefault(Function(a) a.class = "page-link") _
            .Matches("<span>.+?</span>").ToArray.Where(Function(s) s.class = "text-primary").Last.StripHTMLTags

        For i As Integer = 1 To allPages
            Dim url = sprintf(listAPI, genome.ID, genome.ID, i)

            For Each gene In url.GET.parseDEGList
                Yield gene
            Next
        Next
    End Function

    Const detailsAPI As String = "http://origin.tubic.org/deg/public/index.php/information/bacteria/%s.html"

    <Extension>
    Private Iterator Function parseDEGList(html As String) As IEnumerable(Of EssentialGene)
        Dim table$ = html.GetTablesHTML.First
        Dim rows = table.GetRowsHTML

        Call Thread.Sleep(1000)

        For Each row As String In rows
            Dim columns = row.GetColumnsHTML

            Yield New EssentialGene With {
                .ID = columns(1).StripHTMLTags,
                .Name = columns(2),
                .FunctionDescrib = columns(3),
                .Organism = columns(4)
            }.fillDetails
        Next
    End Function

    <Extension>
    Private Function fillDetails(gene As EssentialGene) As EssentialGene
        Dim html = sprintf(detailsAPI, gene.ID).GET
        Dim table = html.GetTablesHTML.First
        Dim rows = table.GetRowsHTML
        Dim details = rows.Select(Function(r) r.GetColumnsHTML).ToDictionary(Function(c) c.First.Replace(" ", "_"), Function(c) c.Last)

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

Public Class Genome
    Public Property ID As String
    Public Property Organism As String
    Public Property numOfDEG As Integer
    Public Property Conditions As String
    Public Property Reference As String

End Class
