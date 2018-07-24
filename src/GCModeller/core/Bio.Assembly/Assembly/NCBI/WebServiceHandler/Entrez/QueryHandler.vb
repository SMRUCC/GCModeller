#Region "Microsoft.VisualBasic::68d97136a98bf73207eae909519c069b, Bio.Assembly\Assembly\NCBI\WebServiceHandler\Entrez\QueryHandler.vb"

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

    '     Class QueryHandler
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: __startQuery, DownloadCurrentPage
    '         Class Entry
    ' 
    '             Properties: AccessionId, Description, GI
    ' 
    '             Function: __buildQuery, __getTAG, (+2 Overloads) DownloadGBK, EntryParser
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF

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

            Const SAVED_FILE As String = "[$SAVED_FILE]"
            Const ACCESSION_ID As String = "[$ACCESSION_ID]"

            ''' <summary>
            ''' The BioPerl is required for download the genbank file in this function.(本函数会尝试从NCBI服务器之上下载Genbank文件，这个方法的调用需要计算机之上安装有BioPerl)
            ''' </summary>
            ''' <param name="work">保存文件的临时文件夹</param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function DownloadGBK(work As String) As GBFF.File
                Dim SavedGBK As String = DownloadGBK(work, Me.AccessionId)
                Dim GBK As GBFF.File = Nothing

                If SavedGBK.FileExists(True) Then
                    GBK = GbkParser.Read(SavedGBK)
                Else
                    Call $"Genbank database file download for ""{Me.ToString}""; ACC:={AccessionId} is not successful.".Warning
                End If

                Return GBK
            End Function

            ''' <summary>
            ''' Generates the perl process handle for download the genbank database file which was specific by the accessionID.
            ''' (生成下载Genbank文件所需要的临时脚本文件)
            ''' </summary>
            ''' <param name="AccessionID">Gene locus_tag or genome accession id.</param>
            ''' <param name="Work">Working directory</param>
            ''' <param name="savedGBK">The downloaded GenBank file save location.</param>
            ''' <param name="TempScript">Temp Script save location.</param>
            ''' <returns></returns>
            Private Shared Function __buildQuery(AccessionID As String, Work As String, ByRef savedGBK As String, ByRef TempScript As String) As Process
                TempScript = $"{Work}/{Process.GetCurrentProcess.Id}_{Rnd()}_{AccessionID}.pl"

                Dim p As New ProcessStartInfo("perl", TempScript)
                Dim Script As New StringBuilder(My.Resources.GenBankQuery) 'Perl script template

                savedGBK = $"{Work}/{AccessionID}.gbk"
                savedGBK = FileIO.FileSystem.GetFileInfo(savedGBK).FullName.Replace("\", "/")

                Call Script.Replace(ACCESSION_ID, AccessionID)
                Call Script.Replace(SAVED_FILE, savedGBK)
                Call Script.ToString.SaveTo(TempScript, Encoding.ASCII)

                p.WindowStyle = ProcessWindowStyle.Hidden

                Return New Process() With {.StartInfo = p}
            End Function

            Public Shared Function DownloadGBK(Work As String, AccessionID As String) As String
                Dim SavedGBK As String = "", TempScript As String = ""

                Call __buildQuery(AccessionID, Work, SavedGBK, TempScript).Invoke()
                Call FileIO.FileSystem.DeleteFile(TempScript, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)

                Return SavedGBK
            End Function

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
