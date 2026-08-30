Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq

''' <summary>
''' A local static website directory scanner, it walks through the html
''' files inside the website root dir and maps the local file path as the
''' website url.
''' </summary>
Public Class StaticScanner

    ''' <summary>
    ''' the website base url, example as ``https://gcmodeller.org/``, the
    ''' local file path will be mapped as the website url based on this
    ''' base url value.
    ''' </summary>
    ''' <returns></returns>
    Public Property Host As String

    ''' <summary>
    ''' the max url entry size of the generated sitemap file
    ''' </summary>
    ''' <returns></returns>
    Public Property MaxUrls As Integer = 5000

    ''' <summary>
    ''' should the html file that is not linked by any other page be
    ''' included in the generated sitemap file?
    ''' </summary>
    ''' <returns></returns>
    Public Property IncludeOrphans As Boolean = True

    ''' <summary>
    ''' the url exclude patterns, the wildcard character ``*`` and ``?``
    ''' is supported in the pattern text.
    ''' </summary>
    ''' <returns></returns>
    Public Property ExcludePatterns As String()

    ''' <summary>
    ''' how many css files will be loaded for the theme extraction
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
    ''' print the scan progress to the console stdout?
    ''' </summary>
    ''' <returns></returns>
    Public Property Verbose As Boolean = True

    ''' <summary>
    ''' the default change frequency text of the url entry
    ''' </summary>
    ''' <returns></returns>
    Public Property ChangeFreq As String = "weekly"

    ''' <summary>
    ''' scan a local static website directory
    ''' </summary>
    ''' <param name="wwwroot">
    ''' the local directory path of the website root, example as
    ''' ``G:\gcmodeller.org-website``
    ''' </param>
    ''' <returns></returns>
    Public Function Scan(wwwroot As String) As SiteData
        If Not Directory.Exists(wwwroot) Then
            Throw New DirectoryNotFoundException($"the website directory '{wwwroot}' is not exists on your filesystem!")
        End If

        Dim root As String = Path.GetFullPath(wwwroot).TrimEnd("\"c, "/"c)
        Dim host As String = If(Me.Host, "http://localhost/").TrimEnd("/"c) & "/"
        Dim result As New SiteData With {
            .BaseUrl = host,
            .Source = "local"
        }
        Dim visited As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim queue As New Queue(Of (file As String, depth As Integer))
        Dim cssFiles As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim index As Integer = 0

        For Each entry As String In indexPages(root)
            If visited.Add(entry) Then
                Call queue.Enqueue((entry, UrlEntry.DepthOf(relativePath(entry, root))))
            End If
        Next

        Do While queue.Count > 0
            If result.Entries.Count >= MaxUrls Then
                Exit Do
            End If

            Dim current As (file As String, depth As Integer) = queue.Dequeue()
            Dim html As String = ReadText(current.file)
            Dim link As String = UrlTool.ToSiteUrl(current.file, root, host)

            index += 1
            result.VisitedPages = index

            If html Is Nothing Then
                Continue Do
            End If

            If UrlTool.IsExcluded(link, ExcludePatterns) Then
                Continue Do
            End If

            If Verbose AndAlso (index <= 20 OrElse index Mod 200 = 0) Then
                Call Console.WriteLine($"[{index}] {link}")
            End If

            Dim title As String = HtmlHelper.GetTitle(html)

            If result.SiteTitle Is Nothing Then
                result.SiteTitle = If(title, New DirectoryInfo(root).Name)
                result.ColorScheme = HtmlHelper.GetColorScheme(html)
            End If

            result.Entries.Add(New UrlEntry With {
                .Loc = link,
                .LastMod = UrlEntry.LastModOf(current.file),
                .ChangeFreq = ChangeFreq,
                .Priority = UrlEntry.PriorityOf(current.depth),
                .Depth = current.depth,
                .Title = title,
                .LocalFile = current.file
            })

            ' collect the css document for the website theme extraction
            If result.CssTexts.Count < MaxCssFiles Then
                For Each href As String In HtmlHelper.GetStylesheetLinks(html)
                    Dim cssFile As String = UrlTool.ResolveLocalPath(href, current.file, root)

                    If cssFile Is Nothing OrElse Not cssFiles.Add(cssFile) Then
                        Continue For
                    End If

                    Dim css As String = ReadText(cssFile)

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

            For Each href As String In HtmlHelper.GetLinks(html)
                Dim linkFile As String = UrlTool.ResolveLocalPath(href, current.file, root)

                If linkFile Is Nothing OrElse Not System.IO.File.Exists(linkFile) Then
                    Continue For
                End If

                If Not UrlTool.IsStaticFile(linkFile) Then
                    Continue For
                End If

                If Not visited.Add(linkFile) Then
                    Continue For
                End If

                Dim siteUrl As String = UrlTool.ToSiteUrl(linkFile, root, host)

                If UrlTool.IsExcluded(siteUrl, ExcludePatterns) Then
                    Continue For
                End If

                Call queue.Enqueue((linkFile, current.depth + 1))
            Next
        Loop

        ' the html files that are not linked by any other page of this
        ' website, these pages are still a part of the website, so that
        ' they should be included in the sitemap file by default.
        If IncludeOrphans Then
            For Each file As String In allPages(root)
                If result.Entries.Count >= MaxUrls Then
                    Exit For
                End If

                If Not visited.Add(file) Then
                    Continue For
                End If

                Dim siteUrl As String = UrlTool.ToSiteUrl(file, root, host)

                If UrlTool.IsExcluded(siteUrl, ExcludePatterns) Then
                    Continue For
                End If

                Dim depth As Integer = UrlEntry.DepthOf(relativePath(file, root))
                Dim html As String = ReadText(file)

                result.Entries.Add(New UrlEntry With {
                    .Loc = siteUrl,
                    .LastMod = UrlEntry.LastModOf(file),
                    .ChangeFreq = ChangeFreq,
                    .Priority = UrlEntry.PriorityOf(depth),
                    .Depth = depth,
                    .Title = If(html Is Nothing, Nothing, HtmlHelper.GetTitle(html)),
                    .LocalFile = file
                })
            Next
        End If

        Return result.Sort().Trim(MaxUrls)
    End Function

    Private Function relativePath(file As String, root As String) As String
        Return Path.GetFullPath(file) _
            .Replace(root, "") _
            .Replace("\"c, "/"c) _
            .Trim("/"c)
    End Function

    ''' <summary>
    ''' the start page of the website: the ``index.html`` file inside the
    ''' website root dir, or all of the html files inside the website
    ''' root dir if the index page is not exists.
    ''' </summary>
    ''' <param name="root"></param>
    ''' <returns></returns>
    Private Iterator Function indexPages(root As String) As IEnumerable(Of String)
        For Each name As String In {"index.html", "index.htm", "default.html", "default.htm", "home.html"}
            Dim pageFile As String = System.IO.Path.Combine(root, name)

            If System.IO.File.Exists(pageFile) Then
                Yield pageFile
                Return
            End If
        Next

        For Each pageFile As String In Directory.EnumerateFiles(root, "*.htm*", SearchOption.TopDirectoryOnly)
            If UrlTool.IsStaticFile(pageFile) Then
                Yield pageFile
            End If
        Next
    End Function

    ''' <summary>
    ''' enumerate all of the static html page files inside the website root
    ''' </summary>
    ''' <param name="root"></param>
    ''' <returns></returns>
    Private Iterator Function allPages(root As String) As IEnumerable(Of String)
        Dim dirs As New Queue(Of String)

        Call dirs.Enqueue(root)

        Do While dirs.Count > 0
            Dim dir As String = dirs.Dequeue
            Dim files As String()

            Try
                files = Directory.GetFiles(dir)
            Catch ex As Exception
                Continue Do
            End Try

            For Each file As String In files
                If UrlTool.IsStaticFile(file) Then
                    Yield file
                End If
            Next

            Dim subDirs As String()

            Try
                subDirs = Directory.GetDirectories(dir)
            Catch ex As Exception
                Continue Do
            End Try

            For Each subDir As String In subDirs
                Call dirs.Enqueue(subDir)
            Next
        Loop
    End Function

    Private Function ReadText(pageFile As String) As String
        Try
            Return System.IO.File.ReadAllText(pageFile)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
End Class
