Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports r = System.Text.RegularExpressions.Regex

Namespace Text.Parser.HtmlParser

    Public Module TagAttributeParser

        Const attributePattern$ = "%s\s*=\s*([""].+?[""])|(['].+?['])"

        <Extension>
        Public Function GetAttrValue(html$, attr$) As String
            If String.IsNullOrEmpty(html) Then
                Return ""
            Else
                attr = attributePattern.Replace("%s", attr)
                html = html.Match(attr, RegexICSng)
            End If

            If String.IsNullOrEmpty(html) Then
                Return ""
            Else
                Return html.GetTagValue("=", trim:=True) _
                    .Value _
                    .GetStackValue("""", """") _
                    .GetStackValue("'", "'")
            End If
        End Function

        ''' <summary>
        ''' Gets the link text in the html fragement text.
        ''' </summary>
        ''' <param name="html">A string that contains the url string pattern like: href="url_text"</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <ExportAPI("Html.Href")>
        <Extension> Public Function href(<Parameter("HTML", "A string that contains the url string pattern like: href=""url_text""")> html$) As String
            Return html.GetAttrValue("href")
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function [class](tag As String) As String
            Return tag.GetAttrValue("class")
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
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function src(img As String) As String
            Return img.GetAttrValue("src")
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