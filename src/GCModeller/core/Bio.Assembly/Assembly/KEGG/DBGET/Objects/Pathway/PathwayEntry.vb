Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.HtmlParser

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Class PathwayEntry

        Public Property Entry As String
        Public Property Name As String
        Public Property Description As String
        Public Property [Object] As String
        Public Property Legend As String
        Public Property Url As String

        Public Const ENTRY_ITEM As String = "<a target="".+?"" href=.+?</tr>"

        Public Overrides Function ToString() As String
            Return String.Format("{0}:  {1}", Entry, Description)
        End Function

        Public Shared Function TryParseWebPage(url As String) As PathwayEntry()
            Dim html As String = url.GET
            Dim sbuf As String() = Regex.Matches(html, ENTRY_ITEM, RegexICSng).ToArray
            Dim result As PathwayEntry() = sbuf.ToArray(AddressOf __parserEntry)

            Return result
        End Function

        Private Shared Function __parserEntry(s As String) As PathwayEntry
            Dim entry As New PathwayEntry
            Dim sbuf As String() = Strings.Split(s, vbLf)
            entry.Entry = sbuf.First.GetValue
            entry.Url = sbuf.First.href
            sbuf = sbuf.Skip(3).ToArray

            Dim i As int = Scan0
            entry.Name = sbuf(++i).GetValue
            entry.Description = sbuf(++i).GetValue
            entry.Object = sbuf(++i).GetValue
            entry.Legend = sbuf(++i).GetValue

            Return entry
        End Function
    End Class
End Namespace