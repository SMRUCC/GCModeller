#Region "Microsoft.VisualBasic::cb48b32710cdff8001caf95da0b65185, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\LinkDB\PathwayWebQuery.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 90
    '    Code Lines: 72
    ' Comment Lines: 0
    '   Blank Lines: 18
    '     File Size: 3.64 KB


    '     Class PathwayEntryQuery
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: AllEntries, ParseEntries, URLProvider
    ' 
    '     Class PathwayMapDownloader
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: pageParser, pathwayPage
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
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
                    .entryId = key,
                    .description = description,
                    .url = url
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
                            contextGuid:=Function(entry) entry.entryId,
                            parser:=AddressOf pageParser,
                            prefix:=Function(entry) entry.Split(":"c).First,
                            cache:=cache,
                            interval:=interval,
                            offline:=offline
                        )
        End Sub

        Private Shared Function pathwayPage(entry As ListEntry) As String
            Return "http://www.genome.jp/dbget-bin/www_bget?pathway+" & entry.entryId
        End Function

        Private Shared Function pageParser(html$, null As Type) As Object
            Return Pathway.DownloadPage(html)
        End Function
    End Class
End Namespace
