Imports System.Text.RegularExpressions
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language

Namespace Assembly.KEGG.WebServices.InternalWebFormParsers

    Public Class AllLinksWidget

        Public Property Links As KeyValuePair()

        ' http://www.genome.jp/dbget-bin/get_linkdb?-t+8+path:map00010

        Default Public ReadOnly Property Url(ItemKey As String) As String
            Get
                Dim LQuery As String =
                    LinqAPI.DefaultFirst(Of String) <= From lnkValue As KeyValuePair
                                                       In Links
                                                       Where String.Equals(lnkValue.Key, ItemKey)
                                                       Select lnkValue.Value
                Return LQuery
            End Get
        End Property

        Public Shared Function InternalParser(html As String) As AllLinksWidget
            Dim Links As AllLinksWidget = New AllLinksWidget
            html = Regex.Match(html, "All links.+</pre>", RegexOptions.Singleline).Value
            Dim sbuf As String() = (From m As Match In Regex.Matches(html, "<a href="".+?"">.+?</a>") Select m.Value).ToArray

            Links.Links =
                LinqAPI.Exec(Of KeyValuePair) <= From s As String
                                                 In sbuf
                                                 Let url As String = "http://www.genome.jp" & s.Get_href
                                                 Let Key As String = s.GetValue
                                                 Select New KeyValuePair With {
                                                     .Key = Regex.Replace(Key, "\(.+?\)", "").Trim,
                                                     .Value = url
                                                 }
            Return Links
        End Function

        Public Overrides Function ToString() As String
            Return String.Join("; ", (From m As KeyValuePair
                                      In Links
                                      Select ss = m.ToString).ToArray)
        End Function
    End Class
End Namespace