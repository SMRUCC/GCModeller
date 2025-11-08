Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI.PubMed

''' <summary>
''' parse the pubmed database file in plaintext format
''' </summary>
Public Module PlainTextParser

    Public Iterator Function LoadArticles(file As String) As IEnumerable(Of PubmedArticle)
        Dim blocks As String()() = file _
            .IterateAllLines _
            .Split(Function(line) line.StringEmpty) _
            .Where(Function(b)
                       Return Not (b.IsNullOrEmpty OrElse
                           b.All(Function(si) si.StringEmpty(, True)))
                   End Function) _
            .ToArray

        For Each block As String() In blocks
            Yield ParseArticle(block)
        Next
    End Function

    Private Function ParseArticle(lines As String()) As PubmedArticle

    End Function
End Module
