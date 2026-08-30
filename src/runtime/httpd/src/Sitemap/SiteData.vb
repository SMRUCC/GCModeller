''' <summary>
''' The website data model that is produced by the <see cref="WebCrawler"/>
''' or the <see cref="StaticScanner"/>, it contains the url entries of the
''' website and the css document text for the theme extraction.
''' </summary>
Public Class SiteData

    ''' <summary>
    ''' all of the in-site url entries of the target website
    ''' </summary>
    Public Property Entries As New List(Of UrlEntry)
    ''' <summary>
    ''' the css document text collection for the website theme extraction
    ''' </summary>
    Public Property CssTexts As New List(Of String)
    ''' <summary>
    ''' the ``&lt;title>`` text of the website index page
    ''' </summary>
    Public Property SiteTitle As String
    ''' <summary>
    ''' the website root url, example as ``https://gcmodeller.org/``
    ''' </summary>
    ''' <returns></returns>
    Public Property BaseUrl As String
    ''' <summary>
    ''' the source of this website data: ``http`` or ``local``
    ''' </summary>
    ''' <returns></returns>
    Public Property Source As String
    ''' <summary>
    ''' how many pages have been visited
    ''' </summary>
    ''' <returns></returns>
    Public Property VisitedPages As Integer
    ''' <summary>
    ''' the ``color-scheme`` meta value of the index page
    ''' </summary>
    ''' <returns></returns>
    Public Property ColorScheme As String
    ''' <summary>
    ''' how many in-site pages that owns a hyper link point to a given url,
    ''' this value is counted by the crawler or the scanner when it walks
    ''' through the hyper links of the website pages.
    ''' </summary>
    ''' <returns></returns>
    Public Property InLinks As New Dictionary(Of String, Integer)

    ''' <summary>
    ''' get the in-site link reference count of a given url
    ''' </summary>
    ''' <param name="loc"></param>
    ''' <returns>
    ''' this function returns zero when the given url have not been
    ''' referenced by any page of the website.
    ''' </returns>
    Public Function InLinkCountOf(loc As String) As Integer
        If String.IsNullOrWhiteSpace(loc) OrElse InLinks Is Nothing Then
            Return 0
        End If

        Dim count As Integer = 0

        If InLinks.TryGetValue(loc.Trim, count) Then
            Return count
        End If

        Return 0
    End Function

    ''' <summary>
    ''' count a in-site link reference of a given url
    ''' </summary>
    ''' <param name="loc"></param>
    Public Sub AddInLink(loc As String)
        If String.IsNullOrWhiteSpace(loc) Then
            Return
        End If

        loc = loc.Trim

        If InLinks.ContainsKey(loc) Then
            InLinks(loc) += 1
        Else
            InLinks(loc) = 1
        End If
    End Sub

    ''' <summary>
    ''' sort the url entries by the crawl depth level and then by the url
    ''' location string.
    ''' </summary>
    ''' <returns></returns>
    Public Function Sort() As SiteData
        Entries = Entries _
            .OrderBy(Function(e) e.Depth) _
            .ThenBy(Function(e) e.Loc, StringComparer.OrdinalIgnoreCase) _
            .ToList

        Return Me
    End Function

    ''' <summary>
    ''' remove the duplicated url entry by the <see cref="UrlEntry.Loc"/>
    ''' value, and then limit the total url entry size.
    ''' </summary>
    ''' <param name="maxUrls"></param>
    ''' <returns></returns>
    Public Function Trim(maxUrls As Integer) As SiteData
        Dim unique As New List(Of UrlEntry)
        Dim seen As New HashSet(Of String)

        For Each entry As UrlEntry In Entries
            If String.IsNullOrWhiteSpace(entry.Loc) Then
                Continue For
            End If

            If seen.Add(entry.Loc) Then
                unique.Add(entry)
            End If
        Next

        Entries = unique

        If maxUrls > 0 AndAlso Entries.Count > maxUrls Then
            Entries = Entries.Take(maxUrls).ToList
        End If

        Return Me
    End Function

    ''' <summary>
    ''' sort the url entries by the page priority value in descending order,
    ''' and then by the url location string.
    ''' </summary>
    ''' <returns></returns>
    Public Function SortByPriority() As SiteData
        Entries = Entries _
            .OrderByDescending(Function(e) e.Priority) _
            .ThenBy(Function(e) e.Loc, StringComparer.OrdinalIgnoreCase) _
            .ToList

        Return Me
    End Function

    Public Overrides Function ToString() As String
        Return $"[{Source}] {Entries.Count} urls, {CssTexts.Count} css documents"
    End Function
End Class
