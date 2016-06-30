Imports System.Text

Public Module Navigation

    ''' <summary>
    ''' {display, url}
    ''' </summary>
    ''' <param name="nag">{display, url}</param>
    ''' <returns></returns>
    Public Function BreadcrumbNavigation(current As String, nag As Dictionary(Of String, String)) As String
        Dim nbr As StringBuilder = New StringBuilder(1024)

        Call nbr.AppendLine("<p>")
        Call nbr.AppendLine("<a href=""http://gcmodeller.org"">HOME</a> > ")

        For Each lv In nag
            Call nbr.AppendLine($"<a href=""{lv.Value}"">{lv.Key}</a> / ")
        Next

        Call nbr.AppendLine($"<strong>{current}</strong>")
        Call nbr.AppendLine("</p>")

        Return nbr.ToString
    End Function

    Public ReadOnly Property MEMESWTomQuery As String =
        BreadcrumbNavigation("Show Result", New Dictionary(Of String, String) From {
            {"Services", "http://services.gcmodeller.org"},
            {"MEME", "http://services.gcmodeller.org/meme/"},
            {"TomQuery", "http://services.gcmodeller.org/meme/tom-query.sw/"}})

    Public ReadOnly Property MEMETomQuery As String =
        BreadcrumbNavigation("Show Result", New Dictionary(Of String, String) From {
            {"Services", "http://services.gcmodeller.org"},
            {"MEME", "http://services.gcmodeller.org/meme/"},
            {"TomQuery", "http://services.gcmodeller.org/meme/tom-query/"}})
End Module
