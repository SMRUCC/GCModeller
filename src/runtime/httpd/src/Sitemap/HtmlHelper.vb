Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.MIME.Html.Document

''' <summary>
''' The html document content helper for extracting the hyper links, the
''' css style reference and the page title from a html document.
''' </summary>
Public Module HtmlHelper

    ReadOnly anchorHref As New Regex(
        "<a\s[^>]*?href\s*=\s*(?:""([^""]*)""|'([^']*)'|([^\s>]+))",
        RegexOptions.IgnoreCase Or RegexOptions.Singleline)

    ReadOnly areaHref As New Regex(
        "<(?:area|link)\s[^>]*?href\s*=\s*(?:""([^""]*)""|'([^']*)'|([^\s>]+))",
        RegexOptions.IgnoreCase Or RegexOptions.Singleline)

    ReadOnly tagAttribute As New Regex(
        "(?<name>[\w\-:]+)\s*=\s*(?:""(?<value>[^""]*)""|'(?<value>[^']*)'|(?<value>[^\s>]+))",
        RegexOptions.IgnoreCase Or RegexOptions.Singleline)

    ReadOnly styleBlock As New Regex(
        "<style[^>]*>(?<css>.*?)</style>",
        RegexOptions.IgnoreCase Or RegexOptions.Singleline)

    ReadOnly titleBlock As New Regex(
        "<title[^>]*>(?<title>.*?)</title>",
        RegexOptions.IgnoreCase Or RegexOptions.Singleline)

    ReadOnly metaColorScheme As New Regex(
        "<meta[^>]*name\s*=\s*[""']color-scheme[""'][^>]*content\s*=\s*[""'](?<value>[^""]*)[""'][^>]*>",
        RegexOptions.IgnoreCase Or RegexOptions.Singleline)

    ReadOnly htmlTags As New Regex("<[^>]+>", RegexOptions.Singleline)

    ''' <summary>
    ''' extract all of the hyper link url from a html document text
    ''' </summary>
    ''' <param name="html"></param>
    ''' <returns>
    ''' this function returns the raw href attribute value, the returned
    ''' value should be normalized by the <see cref="UrlTool.Normalize"/>
    ''' function at last.
    ''' </returns>
    Public Iterator Function GetLinks(html As String) As IEnumerable(Of String)
        If String.IsNullOrEmpty(html) Then
            Return
        End If

        Dim links As New List(Of String)

        ' the primary extractor: parse the html document as a DOM tree
        Try
            Dim document As HtmlDocument = HtmlDocument.LoadDocument(ensureTextStream(html), strip:=False)

            For Each anchor As HtmlElement In document.getElementsByTagName("a")
                Dim href As ValueAttribute = anchor("href")

                If Not String.IsNullOrEmpty(href.Value) Then
                    links.Add(href.Value)
                End If
            Next
        Catch ex As Exception
            ' the html document parser is failed, use the regex fallback
        End Try

        ' the regex fallback extractor
        If links.Count = 0 Then
            For Each m As Match In anchorHref.Matches(html)
                links.Add(matchValue(m))
            Next
        End If

        For Each link As String In links _
            .Where(Function(s) Not String.IsNullOrWhiteSpace(s)) _
            .Select(Function(s) s.Trim) _
            .Distinct

            Yield link
        Next
    End Function

    ''' <summary>
    ''' extract the css file reference url from the ``&lt;link rel="stylesheet">``
    ''' tags of a html document.
    ''' </summary>
    ''' <param name="html"></param>
    ''' <returns></returns>
    Public Iterator Function GetStylesheetLinks(html As String) As IEnumerable(Of String)
        If String.IsNullOrEmpty(html) Then
            Return
        End If

        For Each m As Match In areaHref.Matches(html)
            Dim tag As String = m.Value

            ' only the <link> tag that owns a stylesheet rel attribute
            ' value is a css file reference
            If Not Regex.IsMatch(tag, "<link", RegexOptions.IgnoreCase) Then
                Continue For
            End If

            If Not Regex.IsMatch(tag, "rel\s*=\s*[""']?[^""'>]*stylesheet", RegexOptions.IgnoreCase) Then
                Continue For
            End If

            Dim href As String = matchValue(m)

            If Not String.IsNullOrWhiteSpace(href) Then
                Yield href.Trim
            End If
        Next
    End Function

    ''' <summary>
    ''' extract the inline css style text from the ``&lt;style>`` blocks
    ''' of a html document.
    ''' </summary>
    ''' <param name="html"></param>
    ''' <returns></returns>
    Public Iterator Function GetInlineStyles(html As String) As IEnumerable(Of String)
        If String.IsNullOrEmpty(html) Then
            Return
        End If

        For Each m As Match In styleBlock.Matches(html)
            Dim css As String = m.Groups("css").Value

            If Not String.IsNullOrWhiteSpace(css) Then
                Yield css
            End If
        Next
    End Function

    ''' <summary>
    ''' get the ``&lt;title>`` text of a html document
    ''' </summary>
    ''' <param name="html"></param>
    ''' <returns></returns>
    Public Function GetTitle(html As String) As String
        If String.IsNullOrEmpty(html) Then
            Return Nothing
        End If

        Dim m As Match = titleBlock.Match(html)

        If Not m.Success Then
            Return Nothing
        End If

        Dim title As String = htmlTags.Replace(m.Groups("title").Value, " ").Trim

        title = Regex.Replace(title, "\s+", " ")
        title = title _
            .Replace("&amp;", "&") _
            .Replace("&lt;", "<") _
            .Replace("&gt;", ">") _
            .Replace("&quot;", """") _
            .Replace("&#39;", "'") _
            .Replace("&nbsp;", " ") _
            .Trim

        If title.Length = 0 Then
            Return Nothing
        End If

        Return title
    End Function

    ''' <summary>
    ''' get the ``color-scheme`` meta value of a html document, this value
    ''' is a strong hint of the dark or light theme mode
    ''' </summary>
    ''' <param name="html"></param>
    ''' <returns></returns>
    Public Function GetColorScheme(html As String) As String
        If String.IsNullOrEmpty(html) Then
            Return Nothing
        End If

        Dim m As Match = metaColorScheme.Match(html)

        If Not m.Success Then
            Return Nothing
        End If

        Return m.Groups("value").Value.Trim.ToLower
    End Function

    Private Function matchValue(m As Match) As String
        If m.Groups(1).Success Then
            Return m.Groups(1).Value
        ElseIf m.Groups(2).Success Then
            Return m.Groups(2).Value
        ElseIf m.Groups(3).Success Then
            Return m.Groups(3).Value
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' the <see cref="HtmlDocument.LoadDocument"/> function treats the
    ''' input text as an url or a file path when the given text does not
    ''' contains any newline character, so that we needs to make sure that
    ''' the input html text always contains the newline character.
    ''' </summary>
    ''' <param name="html"></param>
    ''' <returns></returns>
    Private Function ensureTextStream(html As String) As String
        If html.IndexOf(vbLf) < 0 Then
            Return html.Replace(">", ">" & vbLf)
        End If

        Return html
    End Function
End Module
