Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language

Public Class HtmlPage : Inherits ClassObject

    Public Property Url As String
    Public Property Title As String
    Public Property html As String

    Public Function BuildPage(template As String) As String
        Dim sb As New StringBuilder(template)

        Call sb.Replace("{title}", Title)
        Call sb.Replace("{HTML}", html)

        Return sb.ToString
    End Function

    Public Shared Function LoadPage(path As String, wwwroot As String) As HtmlPage
        Dim content As String = path.ReadAllText
        Dim url As String = RelativePath(wwwroot, path)
        Dim html As HtmlPage = HtmlPage.FromStream(content, url)
        Return html
    End Function

    Public Shared Function FromStream(content As String, Optional url As String = "#") As HtmlPage
        Dim head As String = Regex.Match(content, "---.+?---", RegexOptions.Singleline).Value
        Dim title As String = Regex.Match(head, "title:.+?$", RegexICMul).Value

        title = title.GetTagValue(":").x.Trim
        content = Mid(content, head.Length + 1).Trim

        Return New HtmlPage With {
            .html = content,
            .Title = title,
            .Url = url
        }
    End Function
End Class
