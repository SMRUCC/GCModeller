#Region "Microsoft.VisualBasic::12fb432eaf9cebe6647a9d5443273ce4, modules\Knowledge_base\Knowledge_base\Wikipedia\BingSearch.vb"

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

    '   Total Lines: 57
    '    Code Lines: 39
    ' Comment Lines: 6
    '   Blank Lines: 12
    '     File Size: 1.98 KB


    ' Module BingSearch
    ' 
    '     Function: CreateQueryURL, CreateRefer, GetWikiMarkdown, GetWikiPageText, WikiBingSearch
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.KnowledgeBase.Web.Bing
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Public Module BingSearch

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function CreateRefer(refer As String) As Dictionary(Of String, String)
        Return New Dictionary(Of String, String) From {{NameOf(refer), refer}}
    End Function

    ''' <summary>
    ''' Bing search for wiki english
    ''' </summary>
    ''' <param name="term"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function CreateQueryURL(term As String) As String
        Return $"https://cn.bing.com/search?q={term.UrlEncode}+site%3A+en.wikipedia.org&qs=n&form=QBRE&sp=-1&pq={term.UrlEncode}+site%3A+en.wikipedia.org&sc=0-42&sk=&cvid=939096C192AD4B758FD3CBA281CBD413"
    End Function

    Public Function GetWikiMarkdown(term As String) As String
        Dim url$ = $"https://en.wikipedia.org/w/index.php?title={term}&action=edit"
        Dim html$ = url.GET()

        Throw New NotImplementedException
    End Function

    Public Function GetWikiPageText(term As String) As String
        Dim url$ = $"https://en.wikipedia.org/wiki/{term}"
        Dim html = url.GET
        Dim text$ = html.StripHTMLTags(stripBlank:=True)

        Return text
    End Function

    Public Iterator Function WikiBingSearch(term As String, Optional limits% = 100) As IEnumerable(Of WebResult)
        Dim url$ = CreateQueryURL(term)
        Dim list = SearchEngineProvider.DownloadResult(url)
        Dim n% = 0

        Do While n < limits

            For Each entry In list.CurrentPage
                Yield entry
            Next

            n += list.CurrentPage.Length

            If list.HaveNext Then
                list = list.NextPage
            End If
        Loop
    End Function
End Module
