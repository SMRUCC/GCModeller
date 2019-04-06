Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports r = System.Text.RegularExpressions.Regex

Namespace Text.Parser.HtmlParser

    Public Module TagAttributeParser

        Const hrefPattern$ = "href\s*=\s*[""'].+?[""']"

        ''' <summary>
        ''' Gets the link text in the html fragement text.
        ''' </summary>
        ''' <param name="html">A string that contains the url string pattern like: href="url_text"</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Html.Href")>
        <Extension> Public Function href(<Parameter("HTML", "A string that contains the url string pattern like: href=""url_text""")> html$) As String
            If String.IsNullOrEmpty(html) Then
                Return ""
            End If

            Dim url$ = r _
                .Match(html, hrefPattern, RegexOptions.IgnoreCase) _
                .Value

            If String.IsNullOrEmpty(url) Then
                Return ""
            Else
                Return url.GetTagValue("=", trim:=True).Value.GetStackValue("""", """").GetStackValue("'", "'")
            End If
        End Function

        <Extension>
        Public Function [class](tag As String) As String
            If String.IsNullOrEmpty(tag) Then
                Return ""
            End If

            Dim className = r.Match(tag, "class\s*[=]\s*[""'].+?['""]").Value

            If String.IsNullOrEmpty(className) Then
                Return ""
            Else
                Return className.GetTagValue("=", trim:=True).Value.GetStackValue("""", """").GetStackValue("'", "'")
            End If
        End Function


#Region "Parsing image source url from the img html tag."

        Public Const imgHtmlTagPattern As String = "<img.+?src=.+?>"

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetImageLinks(html As String) As String()
            Dim list$() = r _
                .Matches(html, imgHtmlTagPattern, RegexICSng) _
                .EachValue(Function(img) img.src) _
                .ToArray

            Return list
        End Function

        ''' <summary>
        ''' Parsing image source url from the img html tag.
        ''' </summary>
        ''' <param name="img"></param>
        ''' <returns></returns>
        <Extension>
        Public Function src(img As String) As String
            If String.IsNullOrEmpty(img) Then
                Return ""
            Else
                img = r.Match(img, "src\s*[=]\s*"".+?""", RegexOptions.IgnoreCase).Value
            End If

            If String.IsNullOrEmpty(img) Then
                Return ""
            Else
                img = img.GetTagValue("=", trim:=True).Value.GetStackValue("""", """")
                Return img
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function src(img As (tag$, attrs As NamedValue(Of String)())) As String
            Return img.attrs.GetByKey("src", True).Value
        End Function

        <Extension>
        Public Function img(html$) As (tag$, attrs As NamedValue(Of String)())
            Return ("img", r.Match(html, imgHtmlTagPattern, RegexICSng).Value.TagAttributes.ToArray)
        End Function
#End Region
    End Module
End Namespace