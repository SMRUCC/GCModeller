Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
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
        Dim terms As Dictionary(Of String, String()) = LoadTerms(lines) _
            .GroupBy(Function(a) a.Name) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a.Values
                          End Function)
        Dim article As New Article With {
            .Abstract = New Abstract(terms.TryGetValue("AB")),
            .ArticleTitle = terms.TryGetValue("TI").JoinBy("; "),
            .Journal = New Journal(terms.TryGetValue("JT").JoinBy("; ")),
            .AuthorList = New AuthorList With {
                .Authors = terms.TryGetValue("AU") _
                    .SafeQuery _
                    .Select(Function(name) New Author With {.Initials = name}) _
                    .ToArray
            }
        }
        Dim cite As New MedlineCitation With {
            .Owner = terms.TryGetValue("OWN").JoinBy("; "),
            .Status = terms.TryGetValue("STAT").JoinBy("; "),
            .PMID = New PMID(terms.TryGetValue("PMID").DefaultFirst),
            .Article = article,
            .KeywordList = New KeywordList With {
                .Keywords = terms _
                    .TryGetValue("OT") _
                    .SafeQuery _
                    .Select(Function(key) New Keyword(key)) _
                    .ToArray
            },
            .MeshHeadingList = terms.TryGetValue("MH") _
                .SafeQuery _
                .Select(Function(key) New MeshHeading(key)) _
                .ToArray
        }
        Dim metadata As New PubmedData

        Return New PubmedArticle With {
            .MedlineCitation = cite,
            .PubmedData = metadata
        }
    End Function

    Private Iterator Function LoadTerms(lines As String()) As IEnumerable(Of NamedValue(Of String))
        Dim data As New List(Of NamedValue(Of String))
        Dim temp As New List(Of String)
        Dim term As String = Nothing

        For Each line As String In lines
            If Not line.StartsWith("    ") Then
                ' start with new term
                If temp.Any Then
                    Yield New NamedValue(Of String)(term, temp.JoinBy(" "))
                    Call temp.Clear()
                End If

                term = line.Substring(0, 4)
                temp.Add(line.Substring(5).Trim)
            Else
                temp.Add(line.Trim)
            End If
        Next

        If temp.Any Then
            Yield New NamedValue(Of String)(term, temp.JoinBy(" "))
        End If
    End Function
End Module
