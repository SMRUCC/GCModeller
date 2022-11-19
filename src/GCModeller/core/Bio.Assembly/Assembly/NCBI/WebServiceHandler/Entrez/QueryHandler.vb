#Region "Microsoft.VisualBasic::a1b44064979c768b219f041ce783e074, GCModeller\core\Bio.Assembly\Assembly\NCBI\WebServiceHandler\Entrez\QueryHandler.vb"

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

    '   Total Lines: 78
    '    Code Lines: 50
    ' Comment Lines: 14
    '   Blank Lines: 14
    '     File Size: 3.67 KB


    '     Class QueryHandler
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: __startQuery, DownloadCurrentPage
    '         Class Entry
    ' 
    '             Properties: AccessionId, Description, GI
    ' 
    '             Function: __getTAG, EntryParser
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Namespace Assembly.NCBI.Entrez

    ''' <summary>
    ''' http://www.ncbi.nlm.nih.gov/nuccore/?term=
    ''' </summary>
    ''' <remarks></remarks>
    Public Class QueryHandler

        Public Const URL As String = "http://www.ncbi.nlm.nih.gov/nuccore/?term="

        ''' <summary>
        ''' 当前页面文件的URL
        ''' </summary>
        ''' <remarks></remarks>
        Dim _currentURL As String

        Sub New(keyword As String)
            _currentURL = URL & keyword.Replace(" ", "%20")
        End Sub

        Public Function DownloadCurrentPage() As Entry()
            Dim returnedEntries As Entry() = __startQuery(_currentURL)
            Return returnedEntries
        End Function

        Const REGEX_ENTRY As String = "<input name=""EntrezSystem2.PEntrez.Nuccore.Sequence_ResultsPanel.Sequence_RVDocSum.uid"" .+?</p></div></div></div>"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="url">每执行一次查询操作，假若当前页还有下一页的话，参数值会被更新</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function __startQuery(ByRef url As String) As Entry()
            Dim PageContent As String = url.GET
            PageContent = Strings.Split(PageContent, "<div class=""title_and_pager"">", Compare:=CompareMethod.Text).Last
            PageContent = Strings.Split(PageContent, "<div class=""title_and_pager bottom"">", Compare:=CompareMethod.Text).First
            Dim Entries As String() = (From m As Match In Regex.Matches(PageContent, REGEX_ENTRY, RegexOptions.Singleline) Select m.Value).ToArray
            Dim Chunkbuffer As Entry() = (From s As String In Entries Select Entry.EntryParser(s)).ToArray
            Return Chunkbuffer
        End Function

        Public Class Entry : Inherits Entrez.ComponentModels.I_QueryEntry

            Public Property Description As String
            Public Property AccessionId As String
            Public Property GI As String

            Public Shared Function EntryParser(str As String) As Entry
                Dim EntryObj As Entry = New Entry
                Dim ChunkBuffer As String() = (From m As Match In Regex.Matches(str, "<div.+?</div>", RegexOptions.Singleline) Select m.Value).ToArray
                Dim Work As String = ChunkBuffer.First

                EntryObj.Title = Regex.Match(Work, "<p class=""title""><a href="".+?"" ref="".+?"">.+?</a></p>").Value
                EntryObj.Description = __getTAG(Regex.Match(Work, "<p class=""desc"">.+?</p>").Value)
                EntryObj.URL = "http://www.ncbi.nlm.nih.gov" & Regex.Match(EntryObj.Title, "<a href="".+?""").Value.href
                EntryObj.Title = __getTAG(Regex.Match(EntryObj.Title, "ref="".+?"">.+?</a").Value)
                Work = ChunkBuffer.Last
                EntryObj.AccessionId = Regex.Match(Work, "<dt>Accession:</dt>.+?<dd>.+?</dd>").Value
                EntryObj.GI = Regex.Match(Work, "<dt>GI:</dt>.+?<dd>.+?</dd>").Value
                EntryObj.GI = __getTAG(Regex.Match(EntryObj.GI, "<dd>.+?</dd>").Value)
                EntryObj.AccessionId = __getTAG(Regex.Match(EntryObj.AccessionId, "<dd>.+?</dd>").Value)

                Return EntryObj
            End Function

            Private Shared Function __getTAG(s As String) As String
                s = s.Replace("</b>", "").Replace("<b>", "")
                s = Regex.Match(s, ">.+?<").Value
                s = Mid(s, 2, Len(s) - 2)
                Return s
            End Function
        End Class
    End Class
End Namespace
