Imports System.IO
Imports System.Text.RegularExpressions

''' <summary>
''' The url string normalization and the in-site url test helper
''' </summary>
Public Module UrlTool

    ''' <summary>
    ''' the url protocol prefix that is not a web page url
    ''' </summary>
    ReadOnly invalidProtocol As String() = {
        "mailto:", "javascript:", "tel:", "sms:", "data:", "about:",
        "ftp:", "file:", "callto:", "skype:", "viber:", "whatsapp:"
    }

    ''' <summary>
    ''' the file extension that is not a static html page
    ''' </summary>
    ReadOnly assetExtensions As New HashSet(Of String) From {
        ".css", ".js", ".mjs", ".ts", ".map", ".json", ".xml", ".txt",
        ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".ico", ".webp", ".avif",
        ".svg", ".woff", ".woff2", ".ttf", ".eot", ".otf",
        ".pdf", ".zip", ".gz", ".tar", ".rar", ".7z", ".exe", ".msi",
        ".mp3", ".mp4", ".webm", ".ogg", ".wav", ".mov",
        ".swf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
        ".rdata", ".rds", ".csv", ".tsv", ".db", ".sqlite"
    }

    ''' <summary>
    ''' the html page file extension
    ''' </summary>
    ReadOnly pageExtensions As New HashSet(Of String) From {
        ".html", ".htm", ".shtml", ".xhtml", ".asp", ".aspx", ".php", ".jsp"
    }

    ''' <summary>
    ''' is the given string a http or https url?
    ''' </summary>
    ''' <param name="url"></param>
    ''' <returns></returns>
    Public Function IsHttpUrl(url As String) As Boolean
        If String.IsNullOrWhiteSpace(url) Then
            Return False
        End If

        Return Regex.IsMatch(url.Trim, "^https?://", RegexOptions.IgnoreCase)
    End Function

    ''' <summary>
    ''' is the given string a local filesystem directory path?
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    Public Function IsLocalDirectory(path As String) As Boolean
        If String.IsNullOrWhiteSpace(path) Then
            Return False
        End If

        Return Directory.Exists(path)
    End Function

    ''' <summary>
    ''' get the host name of a given url, example as ``gcmodeller.org``
    ''' </summary>
    ''' <param name="url"></param>
    ''' <returns></returns>
    Public Function HostOf(url As String) As String
        If Not IsHttpUrl(url) Then
            Return Nothing
        End If

        Try
            Return New Uri(url).Host.ToLower
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' get the website root url of a given page url, example as the
    ''' ``https://gcmodeller.org/index.html`` gets the
    ''' ``https://gcmodeller.org/`` value.
    ''' </summary>
    ''' <param name="url"></param>
    ''' <returns></returns>
    Public Function RootOf(url As String) As String
        If Not IsHttpUrl(url) Then
            Return Nothing
        End If

        Try
            Dim uri As New Uri(url)
            Return $"{uri.Scheme}://{uri.Authority}/"
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' remove the anchor part of a url string
    ''' </summary>
    ''' <param name="url"></param>
    ''' <returns></returns>
    Public Function StripAnchor(url As String) As String
        If url Is Nothing Then
            Return Nothing
        End If

        Dim i As Integer = url.IndexOf("#"c)

        If i >= 0 Then
            Return url.Substring(0, i)
        End If

        Return url
    End Function

    ''' <summary>
    ''' normalize a raw href value as the absolute in-site url
    ''' </summary>
    ''' <param name="raw">
    ''' the raw href attribute value, it can be a absolute url, a relative
    ''' url or a root relative url.
    ''' </param>
    ''' <param name="baseUrl">
    ''' the absolute url of the page that contains this hyper link
    ''' </param>
    ''' <returns>
    ''' this function returns Nothing if the given raw href value is not a
    ''' valid http web page url.
    ''' </returns>
    Public Function Normalize(raw As String, baseUrl As String) As String
        If String.IsNullOrWhiteSpace(raw) OrElse Not IsHttpUrl(baseUrl) Then
            Return Nothing
        End If

        Dim href As String = raw.Trim
        Dim lower As String = href.ToLower

        For Each protocol As String In invalidProtocol
            If lower.StartsWith(protocol) Then
                Return Nothing
            End If
        Next

        If href = "#" OrElse href.StartsWith("#") Then
            Return Nothing
        End If

        ' the protocol relative url, example as //gcmodeller.org/index.html
        If href.StartsWith("//") Then
            href = New Uri(baseUrl).Scheme & ":" & href
        End If

        Dim url As Uri

        Try
            url = New Uri(New Uri(baseUrl), href)
        Catch ex As Exception
            Return Nothing
        End Try

        If url.Scheme <> "http" AndAlso url.Scheme <> "https" Then
            Return Nothing
        End If

        Dim absolute As String = StripAnchor(url.AbsoluteUri)

        If String.IsNullOrWhiteSpace(absolute) Then
            Return Nothing
        End If

        Return absolute
    End Function

    ''' <summary>
    ''' is the given url a link of the target website?
    ''' </summary>
    ''' <param name="url"></param>
    ''' <param name="baseHost">
    ''' the host name of the target website, example as ``gcmodeller.org``
    ''' </param>
    ''' <param name="allowSubDomain">
    ''' should the sub domain of the target website be treated as the
    ''' in-site url?
    ''' </param>
    ''' <returns></returns>
    Public Function IsInSite(url As String, baseHost As String, Optional allowSubDomain As Boolean = True) As Boolean
        Dim host As String = HostOf(url)

        If host Is Nothing OrElse String.IsNullOrEmpty(baseHost) Then
            Return False
        End If

        If host.Equals(baseHost, StringComparison.OrdinalIgnoreCase) Then
            Return True
        End If

        If allowSubDomain Then
            Return host.EndsWith("." & baseHost, StringComparison.OrdinalIgnoreCase)
        End If

        Return False
    End Function

    ''' <summary>
    ''' should the given url be excluded from the sitemap?
    ''' </summary>
    ''' <param name="url"></param>
    ''' <param name="patterns">
    ''' a collection of the wildcard or the regex pattern string
    ''' </param>
    ''' <returns></returns>
    Public Function IsExcluded(url As String, patterns As IEnumerable(Of String)) As Boolean
        If patterns Is Nothing Then
            Return False
        End If

        For Each pattern As String In patterns
            If String.IsNullOrWhiteSpace(pattern) Then
                Continue For
            End If

            If isMatch(url, pattern.Trim) Then
                Return True
            End If
        Next

        Return False
    End Function

    Private Function isMatch(url As String, pattern As String) As Boolean
        ' a plain text pattern will be tested as the substring match
        If Not pattern.IndexOfAny({"*"c, "?"c, "^"c, "$"c, "["c, "("c}) > -1 Then
            Return url.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) > -1
        End If

        ' translate the wildcard pattern as the regex pattern
        Dim regexPattern As String

        If pattern.IndexOfAny({"^"c, "$"c, "["c, "("c}) > -1 Then
            regexPattern = pattern
        Else
            regexPattern = "^" & Regex.Escape(pattern).Replace("\*", ".*").Replace("\?", ".") & "$"
        End If

        Try
            Return Regex.IsMatch(url, regexPattern, RegexOptions.IgnoreCase)
        Catch ex As Exception
            Return url.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) > -1
        End Try
    End Function

    ''' <summary>
    ''' is the given url a static html page url?
    ''' </summary>
    ''' <param name="url"></param>
    ''' <returns></returns>
    Public Function IsStaticPage(url As String) As Boolean
        If String.IsNullOrWhiteSpace(url) Then
            Return False
        End If

        Dim pagePath As String = urlPath(url)
        Dim extension As String = System.IO.Path.GetExtension(pagePath).ToLower

        If extension.Length > 0 Then
            If assetExtensions.Contains(extension) Then
                Return False
            End If

            Return pageExtensions.Contains(extension)
        End If

        ' no file extension, treat it as a directory index page
        Return True
    End Function

    ''' <summary>
    ''' get the url path part without the query and the anchor string
    ''' </summary>
    ''' <param name="url"></param>
    ''' <returns></returns>
    Public Function urlPath(url As String) As String
        Dim path As String = StripAnchor(url)

        If path Is Nothing Then
            Return Nothing
        End If

        Dim i As Integer = path.IndexOf("?"c)

        If i >= 0 Then
            path = path.Substring(0, i)
        End If

        If IsHttpUrl(path) Then
            Try
                path = New Uri(path).AbsolutePath
            Catch ex As Exception
            End Try
        End If

        Return path
    End Function

    ''' <summary>
    ''' map a local html file path as the website url
    ''' </summary>
    ''' <param name="file">the absolute local file path of a html file</param>
    ''' <param name="wwwroot">the absolute local path of the website root dir</param>
    ''' <param name="host">the website base url, example as ``https://gcmodeller.org/``</param>
    ''' <returns></returns>
    Public Function ToSiteUrl(file As String, wwwroot As String, host As String) As String
        Dim relative As String = Path.GetFullPath(file) _
            .Replace(Path.GetFullPath(wwwroot), "") _
            .Replace("\"c, "/"c) _
            .Trim("/"c)

        Return host.Trim("/"c) & "/" & relative
    End Function

    ''' <summary>
    ''' resolve a relative href value as the local filesystem path
    ''' </summary>
    ''' <param name="href">the raw href attribute value</param>
    ''' <param name="pageFile">
    ''' the local html file that contains this hyper link
    ''' </param>
    ''' <param name="wwwroot">the absolute local path of the website root dir</param>
    ''' <returns>
    ''' this function returns Nothing if the given href value is not
    ''' point to a local file inside the website root dir.
    ''' </returns>
    Public Function ResolveLocalPath(href As String, pageFile As String, wwwroot As String) As String
        If String.IsNullOrWhiteSpace(href) Then
            Return Nothing
        End If

        Dim raw As String = href.Trim
        Dim lower As String = raw.ToLower

        For Each protocol As String In invalidProtocol
            If lower.StartsWith(protocol) Then
                Return Nothing
            End If
        Next

        If IsHttpUrl(raw) OrElse raw.StartsWith("//") Then
            Return Nothing
        End If

        If raw.StartsWith("#") Then
            Return Nothing
        End If

        ' strip the query and the anchor part
        raw = StripAnchor(raw)

        Dim i As Integer = If(raw Is Nothing, -1, raw.IndexOf("?"c))

        If i >= 0 Then
            raw = raw.Substring(0, i)
        End If

        If String.IsNullOrWhiteSpace(raw) Then
            Return Nothing
        End If

        Dim dir As String = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(pageFile))
        Dim fullPath As String

        If raw.StartsWith("/") Then
            fullPath = System.IO.Path.Combine(System.IO.Path.GetFullPath(wwwroot), raw.Trim("/"c).Replace("/"c, "\"c))
        Else
            fullPath = System.IO.Path.Combine(dir, raw.Replace("/"c, "\"c))
        End If

        Try
            fullPath = System.IO.Path.GetFullPath(fullPath)
        Catch ex As Exception
            Return Nothing
        End Try

        Dim root As String = System.IO.Path.GetFullPath(wwwroot).TrimEnd("\"c) & "\"

        If Not fullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase) Then
            Return Nothing
        End If

        Return fullPath
    End Function

    ''' <summary>
    ''' is the given local file a static html page file?
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Public Function IsStaticFile(file As String) As Boolean
        Dim extension As String = Path.GetExtension(file).ToLower

        If extension.Length = 0 Then
            Return False
        End If

        If assetExtensions.Contains(extension) Then
            Return False
        End If

        Return pageExtensions.Contains(extension)
    End Function
End Module
