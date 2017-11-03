#Region "Microsoft.VisualBasic::bfca67d5c500498491d740f3f4e5ff22, ..\core\Bio.Assembly\Assembly\KEGG\DBGET\LinkDB\Pathways.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Net
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Terminal
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Namespace Assembly.KEGG.DBGET.LinkDB

    ''' <summary>
    ''' LinkDB Search for KEGG pathways
    ''' </summary>
    Public Module Pathways

        Const KEGGPathwayLinkDB_URL As String = "http://www.genome.jp/dbget-bin/get_linkdb?-t+pathway+genome:{0}"

        Public Function URLProvider(sp As String) As String
            Dim url As String = String.Format(KEGGPathwayLinkDB_URL, sp)
            Return url
        End Function

        Const LinkItem As String = "<a href="".+?"">.+?</a>.+?$"

        Public Iterator Function AllEntries(sp As String) As IEnumerable(Of ListEntry)
            Dim html As String = Strings.Split(URLProvider(sp).GET, Modules.SEPERATOR).Last
            Dim Entries As String() =
                Regex.Matches(html, LinkItem, RegexICMul).ToArray

            For Each entry As String In Entries.Take(Entries.Length - 1)
                Dim key As String = Regex.Match(entry, ">.+?</a>").Value
                key = Mid(key, 2, Len(key) - 5)
                Dim Description As String = Strings.Split(entry, "</a>").Last.Trim
                Dim url As String = "http://www.genome.jp" & entry.href

                Yield New ListEntry With {
                    .EntryID = key,
                    .Description = Description,
                    .Url = url
                }
            Next
        End Function

        ''' <summary>
        ''' 下载某一个物种所注释的代谢途径的数据
        ''' </summary>
        ''' <param name="sp"></param>
        ''' <param name="EXPORT"></param>
        ''' <returns></returns>
        Public Function Downloads(sp$,
                                  Optional EXPORT$ = "./LinkDB-Pathways/",
                                  Optional forceUpdate As Boolean = False) As String()

            Dim entries As New List(Of ListEntry)
            Dim briefHash As Dictionary(Of String, BriteHEntry.Pathway) =
                BriteHEntry.Pathway.LoadDictionary
            Dim Downloader As New WebClient()
            Dim Progress As New ProgressBar("KEGG LinkDB Downloads KEGG Pathways....", 1, CLS:=True)
            Dim failures As New List(Of String)

            ' VBDebugger.Mute = True

            Dim all As ListEntry() = AllEntries(sp).ToArray
            Dim i As int = 1

            For Each entry As ListEntry In all
                Dim ImageUrl = String.Format("http://www.genome.jp/kegg/pathway/{0}/{1}.png", sp, entry.EntryID)
                Dim pathwayPage = "http://www.genome.jp/dbget-bin/www_bget?pathway+" & entry.EntryID
                Dim path As String = EXPORT & "/webpages/" & entry.EntryID & ".html"
                Dim img As String = EXPORT & $"/{entry.EntryID}.png"
                Dim bCode As String = Regex.Match(entry.EntryID, "\d+").Value
                Dim xml$ = BriteHEntry.Pathway.CombineDIR(briefHash(bCode), EXPORT) & $"/{entry.EntryID}.Xml"

                If xml.FileLength > 0 AndAlso img.FileLength > 0 Then
                    If Not forceUpdate Then
                        GoTo EXIT_LOOP
                    End If
                End If

                Call pathwayPage.GET.SaveTo(path)
                Call Downloader.DownloadFile(ImageUrl, img)

                Dim data As Pathway = Pathway.DownloadPage(path)

                If data Is Nothing Then
                    failures += entry.EntryID
                Else
                    entries += entry
                    data.Genes = KEGGgenes.Download($"http://www.genome.jp/dbget-bin/get_linkdb?-t+genes+path:{entry.EntryID}").ToArray

                    Call data.SaveAsXml(xml)
                End If

                Call Thread.Sleep(1000)
EXIT_LOOP:      Call Progress.SetProgress(++i / all.Length * 100, entry.GetJson)
            Next

            ' VBDebugger.Mute = False

            Call entries.GetJson.SaveTo(EXPORT & $"/{sp}.json")

            Return failures
        End Function
    End Module
End Namespace
