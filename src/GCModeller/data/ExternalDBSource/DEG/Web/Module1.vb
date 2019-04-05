Imports System.Runtime.CompilerServices
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

    <Extension>
    Private Function parseDEGList(html As String) As IEnumerable(Of EssentialGene)
        Dim table$ = html.GetTablesHTML.First
        Dim rows = table.GetRowsHTML

        For Each row As String In rows
            Dim columns = row.GetColumnsHTML


        Next
    End Function
End Module

Public Class EssentialGene

End Class

Public Class Genome
    Public Property ID As String
    Public Property Organism As String
    Public Property numOfDEG As Integer
    Public Property Conditions As String
    Public Property Reference As String

End Class
