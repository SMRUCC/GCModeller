Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Text.HtmlParser

Namespace Assembly.KEGG.DBGET.LinkDB

    Public Module GenericParser

        Const regexpLine$ = "<a href="".+?"">.+?</a>.+?$"

        <Extension>
        Public Iterator Function LinkDbEntries(url As String) As IEnumerable(Of KeyValuePair)
            Dim html As String = Strings.Split(url.GET, Modules.SEPERATOR).Last
            Dim links$() = Regex _
                .Matches(html, regexpLine, RegexOptions.Multiline + RegexOptions.IgnoreCase) _
                .ToArray

            For Each line As String In links.Take(links.Length - 1)
                Dim entry As String = Regex.Match(line, ">.+?</a>").Value.GetValue
                Dim description As String = Strings.Split(line, "</a>").Last.Trim
                Dim out As New KeyValuePair With {
                    .Key = entry,
                    .Value = description
                }

                Yield out
            Next
        End Function
    End Module
End Namespace