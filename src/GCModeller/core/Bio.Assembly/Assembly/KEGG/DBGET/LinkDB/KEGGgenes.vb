Imports System.Text.RegularExpressions
Imports LANS.SystemsBiology.Assembly.KEGG.DBGET.LinkDB
Imports LANS.SystemsBiology.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel

Namespace Assembly.KEGG.DBGET.LinkDB

    ''' <summary>
    ''' http://www.genome.jp/dbget-bin/www_bget?
    ''' </summary>
    Public Module KEGGgenes

        Public Const URL_MODULE_GENES As String = "http://www.genome.jp/dbget-bin/get_linkdb?-t+genes+md:{0}"
        Public Const URL_PATHWAY_GENES As String = "http://www.genome.jp/dbget-bin/get_linkdb?-t+genes+path:{0}"

        Public Iterator Function Download(url As String) As IEnumerable(Of KeyValuePair)
            Dim html As String = Strings.Split(url.GET, Modules.SEPERATOR).Last
            Dim Entries = Regex.Matches(html, "<a href="".+?"">.+?</a>.+?$", RegexOptions.Multiline + RegexOptions.IgnoreCase).ToArray

            For Each item As String In Entries.Take(Entries.Length - 1)
                Dim Entry As String = Regex.Match(item, ">.+?</a>").Value
                Entry = Mid(Entry, 2, Len(Entry) - 5)

                Dim Description As String = Strings.Split(item, "</a>").Last.Trim
                Dim gene As New KeyValuePair With {
                    .Key = Entry,
                    .Value = Description
                }

                Yield gene
            Next
        End Function
    End Module
End Namespace