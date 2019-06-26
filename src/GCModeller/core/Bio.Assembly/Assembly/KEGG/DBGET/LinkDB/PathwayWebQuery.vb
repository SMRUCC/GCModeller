Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.ComponentModel
Imports r = System.Text.RegularExpressions.Regex

Namespace Assembly.KEGG.DBGET.LinkDB

    Public Class PathwayEntryQuery : Inherits WebQuery(Of String)

        Const KEGGPathwayLinkDB_URL As String = "http://www.genome.jp/dbget-bin/get_linkdb?-t+pathway+genome:{0}"

        Public Sub New(<CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False)

            Call MyBase.New(url:=AddressOf URLProvider,
                            contextGuid:=Function(code) code,
                            parser:=AddressOf AllEntries,
                            prefix:=Function(code) CStr(code.First),
                            cache:=cache,
                            interval:=interval,
                            offline:=offline
                       )
        End Sub

        Public Shared Function URLProvider(sp As String) As String
            Dim url As String = String.Format(KEGGPathwayLinkDB_URL, sp)
            Return url
        End Function

        Const LinkItem As String = "<a href="".+?"">.+?</a>.+?$"

        Private Shared Function AllEntries(html$, null As Type) As ListEntry()
            Dim entries As String()

            html = Strings.Split(html, Modules.SEPERATOR).Last
            entries = r.Matches(html, LinkItem, RegexICMul).ToArray

            Return ParseEntries(entries.Take(entries.Length - 1)).ToArray
        End Function

        Private Shared Iterator Function ParseEntries(entries As IEnumerable(Of String)) As IEnumerable(Of ListEntry)
            Dim key As String
            Dim description As String
            Dim url As String

            For Each entry As String In entries
                key = r.Match(entry, ">.+?</a>").Value
                key = Mid(key, 2, Len(key) - 5)
                description = Strings.Split(entry, "</a>").Last.Trim
                url = "http://www.genome.jp" & entry.href

                Yield New ListEntry With {
                    .EntryID = key,
                    .Description = description,
                    .Url = url
                }
            Next
        End Function
    End Class

    Public Class PathwayMapDownloader : Inherits WebQuery(Of ListEntry)

        Public Sub New(<CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False)

            Call MyBase.New(url:=AddressOf pathwayPage,
                            contextGuid:=Function(entry) entry.EntryID,
                            parser:=AddressOf pageParser,
                            prefix:=Function(entry) entry.Split(":"c).First,
                            cache:=cache,
                            interval:=interval,
                            offline:=offline
                        )
        End Sub

        Private Shared Function pathwayPage(entry As ListEntry) As String
            Return "http://www.genome.jp/dbget-bin/www_bget?pathway+" & entry.EntryID
        End Function

        Private Shared Function pageParser(html$, null As Type) As Object
            Return Pathway.DownloadPage(html)
        End Function
    End Class
End Namespace