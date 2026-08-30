Imports System.Threading

''' <summary>
''' A breadth first http web crawler for the static web pages of the
''' target website.
''' </summary>
Public Class WebCrawler

    ''' <summary>
    ''' the http request user agent string
    ''' </summary>
    ''' <returns></returns>
    Public Property UserAgent As String = "Mozilla/5.0 (compatible; GCModeller-Sitemap/1.0)"

    ''' <summary>
    ''' the thread sleep time in time unit ``seconds`` before fetch the
    ''' next url page, a small sleep time interval will make the crawler
    ''' more friendly to the target website server.
    ''' </summary>
    ''' <returns></returns>
    Public Property SleepSeconds As Double = 0.5

    ''' <summary>
    ''' the max crawl depth level, the index page is level zero
    ''' </summary>
    ''' <returns></returns>
    Public Property MaxDepth As Integer = 5

    ''' <summary>
    ''' the max url entry size of the generated sitemap file
    ''' </summary>
    ''' <returns></returns>
    Public Property MaxUrls As Integer = 5000

    ''' <summary>
    ''' should the sub domain of the target website be treated as the
    ''' in-site url?
    ''' </summary>
    ''' <returns></returns>
    Public Property AllowSubDomain As Boolean = True

    ''' <summary>
    ''' the url exclude patterns, the wildcard character ``*`` and ``?``
    ''' is supported in the pattern text.
    ''' </summary>
    ''' <returns></returns>
    Public Property ExcludePatterns As String()

    ''' <summary>
    ''' how many css files will be downloaded for the theme extraction
    ''' </summary>
    ''' <returns></returns>
    Public Property MaxCssFiles As Integer = 6

    ''' <summary>
    ''' the max size in bytes of a single css file, a too large css file
    ''' will be skipped.
    ''' </summary>
    ''' <returns></returns>
    Public Property MaxCssSize As Integer = 512 * 1024

    ''' <summary>
    ''' print the crawl progress to the console stdout?
    ''' </summary>
    ''' <returns></returns>
    Public Property Verbose As Boolean = True

    ''' <summary>
    ''' the default change frequency text of the url entry
    ''' </summary>
    ''' <returns></returns>
    Public Property ChangeFreq As String = "weekly"

    ''' <summary>
    ''' calculate the md5 fingerprint of the page from the raw html document
    ''' text instead of the normalized html document text?
    ''' </summary>
    ''' <returns></returns>
    Public Property RawMd5 As Boolean = False

    ''' <summary>
    ''' crawl the target website from a given index page url
    ''' </summary>
    ''' <param name="startUrl">
    ''' the index page url of the target website, example as
    ''' ``https://gcmodeller.org/index.html``
    ''' </param>
    ''' <returns></returns>
    Public Function Crawl(startUrl As String) As SiteData
        Dim base As String = UrlTool.Normalize(startUrl, startUrl)

        If base Is Nothing Then
            Throw New ArgumentException($"the given string '{startUrl}' is not a valid http url!")
        End If

        Dim host As String = UrlTool.HostOf(base)
        Dim result As New SiteData With {
            .BaseUrl = UrlTool.RootOf(base),
            .Source = "http"
        }
        Dim visited As New HashSet(Of String)
        Dim queue As New Queue(Of (url As String, depth As Integer))
        Dim cssUrls As New HashSet(Of String)
        Dim index As Integer = 0

        Call queue.Enqueue((base, 0))
        Call visited.Add(base)

        Do While queue.Count > 0
            If result.Entries.Count >= MaxUrls Then
                Exit Do
            End If

            Dim current As (url As String, depth As Integer) = queue.Dequeue()
            Dim html As String = Download(current.url)

            index += 1
            result.VisitedPages = index

            If html Is Nothing Then
                Call warn($"[{index}] skip {current.url}, the http request is failed.")
                Continue Do
            End If

            If Verbose Then
                Call Console.WriteLine($"[{index}] {current.url}")
            End If

            Dim title As String = HtmlHelper.GetTitle(html)

            If result.SiteTitle Is Nothing Then
                result.SiteTitle = If(title, UrlTool.HostOf(base))
                result.ColorScheme = HtmlHelper.GetColorScheme(html)
            End If

            result.Entries.Add(New UrlEntry With {
                .Loc = current.url,
                .LastMod = UrlEntry.LastModOf(DateTime.Now),
                .ChangeFreq = ChangeFreq,
                .Priority = UrlEntry.PriorityOf(current.depth),
                .Depth = current.depth,
                .Title = title,
                .ContentMd5 = ContentHash.Compute(html, RawMd5),
                .ContentSize = If(html Is Nothing, 0, html.Length)
            })

            ' collect the css document for the website theme extraction
            If result.CssTexts.Count < MaxCssFiles Then
                For Each href As String In HtmlHelper.GetStylesheetLinks(html)
                    Dim cssUrl As String = UrlTool.Normalize(href, current.url)

                    If cssUrl Is Nothing OrElse Not cssUrls.Add(cssUrl) Then
                        Continue For
                    End If

                    Dim css As String = Download(cssUrl)

                    If Not String.IsNullOrWhiteSpace(css) AndAlso css.Length <= MaxCssSize Then
                        result.CssTexts.Add(css)
                    End If

                    If result.CssTexts.Count >= MaxCssFiles Then
                        Exit For
                    End If
                Next

                For Each style As String In HtmlHelper.GetInlineStyles(html)
                    If result.CssTexts.Count >= MaxCssFiles Then
                        Exit For
                    End If

                    result.CssTexts.Add(style)
                Next
            End If

            If current.depth >= MaxDepth Then
                Continue Do
            End If

            For Each href As String In HtmlHelper.GetLinks(html)
                Dim url As String = UrlTool.Normalize(href, current.url)

                If url Is Nothing Then
                    Continue For
                End If

                If Not UrlTool.IsInSite(url, host, AllowSubDomain) Then
                    Continue For
                End If

                If Not UrlTool.IsStaticPage(url) Then
                    Continue For
                End If

                If UrlTool.IsExcluded(url, ExcludePatterns) Then
                    Continue For
                End If

                ' count the in-site link reference of this url, the same url
                ' that is referenced by multiple pages should be counted
                ' multiple times.
                Call result.AddInLink(url)

                If Not visited.Add(url) Then
                    Continue For
                End If

                Call queue.Enqueue((url, current.depth + 1))
            Next
        Loop

        Return result.Sort().Trim(MaxUrls)
    End Function

    ''' <summary>
    ''' download the text content of a given url
    ''' </summary>
    ''' <param name="url"></param>
    ''' <returns>
    ''' this function returns Nothing if the http request is failed.
    ''' </returns>
    Public Function Download(url As String) As String
        If SleepSeconds > 0 Then
            Call Thread.Sleep(CInt(SleepSeconds * 1000))
        End If

        Try
            Dim text As String = url.GetRequest(userAgent:=UserAgent)

            If String.IsNullOrWhiteSpace(text) Then
                Return Nothing
            End If

            Return text
        Catch ex As Exception
            Call warn($"http request error: {url} -> {ex.Message}")
            Return Nothing
        End Try
    End Function

    Private Sub warn(message As String)
        Console.ForegroundColor = ConsoleColor.Yellow
        Console.WriteLine($"[warn] {message}")
        Console.ResetColor()
    End Sub
End Class
