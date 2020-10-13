#Region "Microsoft.VisualBasic::fcf4a5712a71dca4576e715272ef9dd5, core\Bio.Assembly\Assembly\KEGG\Web\WebRequest.vb"

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

    '     Module WebRequest
    ' 
    '         Function: GetText
    '         Delegate Function
    ' 
    '             Function: __downloadDirect, BatchQuery, doParseQueryEntry, Download16S_rRNA, Downloads
    '                       DownloadsBatch, DownloadSequence, downloadWithCache, (+2 Overloads) FetchNt, (+2 Overloads) FetchSeq
    '                       GetPageContent, GetQueryEntry, GetSpCode, (+2 Overloads) HandleQuery, LoadList
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports r = System.Text.RegularExpressions.Regex

Namespace Assembly.KEGG.WebServices

    ''' <summary>
    ''' KEGG web query request handler.(KEGG数据库web查询处理模块)
    ''' 
    ''' ``KEGG``是一个代谢图，收录基因和基因组的数据库，数据库可以分为 3 大部分，基因数据库，化学分子物质数据库，以及基于基因和化学分子物质相互关系而建立起来的代谢路径数据库，
    ''' 在KEGG数据库中，有一个“专有名词”KO（KEGG Orthology），它是蛋白质（酶）的一个分类体系，序列高度相似，并且在同一条通路上有相似功能的蛋白质被归为一组，然后打上KO
    ''' （或K标签，KEGG orthology (ko)代表的是某个代谢途径，k代表的是某个酶，c代表的是某个化合物，M代表的是某个模块，后面都会跟着编号。图中的正方形代表酶，圆形代表代谢物，
    ''' ``5.4.4.4``代表的是EC编号。
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <Package("KEGG.DBGET",
                      Url:="http://www.genome.jp/dbget/",
                      Description:="KEGG web query request handler for DBGET system.<br />
<pre>DBGET is an integrated database retrieval system for major biological databases, which are classified into five categories:
1. KEGG databases in DBGET	
2. Other DBGET databases	
3. Searchable databases 
4. Link-only databases 
5. PubMed database</pre>",
                      Publisher:="xie.guigang@gmail.com")>
    Public Module WebRequest

        ''' <summary>
        ''' 得到表格之中的某一项的文本值
        ''' </summary>
        ''' <param name="form"></param>
        ''' <param name="key$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GetText(form As WebForm, key$) As String
            Return form(key) _
                .FirstOrDefault _
                .StripHTMLTags _
                .StripBlank _
                .GetTagValue(vbLf) _
                .Value
        End Function

        ''' <summary>
        ''' The unify interface for gets the data from KEGG database.(从KEGG数据库获取序列数据以及从本底的数据库之中获取序列数据的统一接口)
        ''' </summary>
        ''' <param name="speciesId">The KEGG brief code of the species genome.(物种简号)</param>
        ''' <param name="LocusID">The GeneID.(基因号)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Delegate Function GetFastaSequenceMethod(speciesId As String, LocusID As String) As FASTA.FastaSeq

        ''' <summary>
        ''' 好像因为没有窗体所以这段代码不能够正常的工作
        ''' </summary>
        ''' <param name="url"></param>
        ''' <returns></returns>
        Private Function GetPageContent(url As String) As String
            Dim browser As New WebBrowser
            Dim LoadComplete As Boolean = False

            If String.IsNullOrEmpty(url) Then Return ""
            If url.Equals("about:blank") Then Return ""
            If Not url.StartsWith("http://") AndAlso Not url.StartsWith("https://") Then
                url = "http://" & url
            End If

            browser.ScriptErrorsSuppressed = True

            Call browser.Navigate(New Uri(url))

            Do While (browser.ReadyState <> WebBrowserReadyState.Complete)
                Call Microsoft.VisualBasic.Parallel.DoEvents()
            Loop

            Dim pageContent As String = browser.DocumentText
            Return pageContent
        End Function

        <ExportAPI("EntryList.Load")>
        Public Function LoadList(url As String) As ListEntry()
            Dim html$ = GetPageContent(url)
            Dim TempChunk As String() = (From m As Match
                                         In r.Matches(html, "^<a href="".+?"">.+?</a>.+?$", RegexOptions.Multiline)
                                         Select m.Value).ToArray
            Dim LQuery = (From s As String
                          In TempChunk
                          Select ListEntry.InternalParser(s)).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' Download a protein sequence data from the KEGG database.
        ''' (从KEGG数据库之中下载一条蛋白质分子序列)
        ''' </summary>
        ''' <param name="specieId">KEGG species id.(KEGG物种编号)</param>
        ''' <param name="accessionId">NCBI gene locus tag.(NCBI基因编号)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        Public Function FetchSeq(<Parameter("sp.Id", "KEGG species id.")> specieId As String,
                                 <Parameter("locusId", "NCBI gene locus tag.")> accessionId As String) As FastaSeq
            Static prot As New FetchSequence(isNucl:=False, cache:="./.kegg/dbget/fetchSeq/prot/")
            Return prot.Query(Of FastaSeq)(New QueryEntry With {.locusID = accessionId, .speciesID = specieId})
        End Function

        ''' <summary>
        ''' Fetch the nucleotide sequence fasta data from the kegg database.
        ''' </summary>
        ''' <param name="specieId"></param>
        ''' <param name="accessionId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        Public Function FetchNt(specieId As String, accessionId As String) As FASTA.FastaSeq
            Static nucl As New FetchSequence(isNucl:=True, cache:="./.kegg/dbget/fetchSeq/nucl/")
            Return nucl.Query(Of FastaSeq)(New QueryEntry With {.locusID = accessionId, .speciesID = specieId})
        End Function

        ''' <summary>
        ''' Fetch the nucleotide sequence fasta data from the kegg database.
        ''' </summary>
        ''' <param name="entry"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        Public Function FetchNt(entry As QueryEntry) As FastaSeq
            Return FetchNt(entry.speciesID, entry.locusID)
        End Function

        ''' <summary>
        ''' Download a protein sequence data from the KEGG database.(从KEGG数据库之中下载一条蛋白质分子序列)
        ''' </summary>
        ''' <param name="entry">KEGG sequence query entry.(KEGG数据库的分子序列查询入口点)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        Public Function FetchSeq(entry As QueryEntry) As FastaSeq
            If entry Is Nothing Then
                Return Nothing
            Else
                Return FetchSeq(entry.speciesID, entry.locusID)
            End If
        End Function

        Const KEGG_DBGET_WWW_QUERY As String = "http://www.genome.jp/dbget-bin/www_bfind_sub?mode=bfind&max_hit=1000&locale=en&serv=gn&dbkey=genes&keywords={0}&page="

        'Const QUERY_RESULT_LINK_ITEM As String = "<a href=""/dbget-bin/www_bget?[^:]+[:][^:]+"">[^:]+[:][^:]+</a><br><div style=""margin-left:2em"">[^<^>]+</div>"
        Public Const QUERY_RESULT_LINK_ITEM As String = "<a href=""/dbget-bin/www_bget\?.+?</div>"
        Public Const QUERY_RESULT_LINK1 As String = ">[^:]+[:][^:]+<"
        Public Const QUERY_RESULT_LINK2 As String = ">[^<^>]+<"

        ''' <summary>
        ''' Get an entry list from a keyword throught the KEGG database web request.``{(speciesId:AccessionId), entry_description}``
        ''' </summary>
        ''' <param name="keyword"></param>
        ''' <returns>如果没有任何结果则返回一个空列表</returns>
        ''' <remarks></remarks>
        ''' 
        Public Function HandleQuery(keyword As String) As QueryEntry()
            Return HandleQuery(keyword, 1)
        End Function

        ''' <summary>
        ''' Get an entry list from a keyword throught the KEGG database web request.``{(speciesId:AccessionId), entry_description}``
        ''' </summary>
        ''' <param name="keyword"></param>
        ''' <returns>如果没有任何结果则返回一个空列表</returns>
        ''' <remarks></remarks>
        ''' 
        Public Function HandleQuery(keyword As String, page As Integer) As QueryEntry()
            Return String.Format(KEGG_DBGET_WWW_QUERY & page, keyword) _
                .GET _
                .doParseQueryEntry
        End Function

        <Extension>
        Friend Iterator Function doParseQueryEntry(html As String) As IEnumerable(Of QueryEntry)
            Dim matches As MatchCollection = r.Matches(html, QUERY_RESULT_LINK_ITEM)

            If matches.Count = 0 Then
                Return
            End If

            Dim match$
            Dim key$
            Dim value$

            For i As Integer = 0 To matches.Count - 1
                match = matches(i).Value
                key = Regex.Match(match, QUERY_RESULT_LINK1).Value

                value = Mid(match, Len(key) + 30)
                value = Regex.Match(value, QUERY_RESULT_LINK2).Value
                value = Mid(value, 3, Len(value) - 3)

                key = Mid(key, 2, Len(key) - 10)

                Yield New QueryEntry(key, description:=value)
            Next
        End Function

        ''' <summary>
        ''' Batch query protein sequence fasta data from the KEGG server.
        ''' </summary>
        ''' <param name="keyword"></param>
        ''' <param name="limit">大批量的数据查询会不会被KEGG封IP？，可以使用本参数来控制数据的返回量</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        Public Function BatchQuery(keyword$, Optional limit As Integer = 30) As FastaFile
            Dim list As QueryEntry() = HandleQuery(keyword)

            Call $"KEGG DBGET Service return {list.Length} records...".__DEBUG_ECHO

            If limit > list.Length Then
                limit = list.Length
            End If

            ' 使用DBGET服务执行对KEGG数据库服务器的数据查询
            Dim LQuery = (From entry As QueryEntry
                          In list.Take(limit)
                          Select FetchSeq(entry)).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' Download fasta sequence data from KEGG database, this function will automatically handles the species brief code.
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        Public Function DownloadSequence(id As String) As FASTA.FastaSeq
            Dim entry As QueryEntry = GetQueryEntry(id)
            Dim fa = WebRequest.FetchSeq(entry)

            If fa Is Nothing Then
                Call $"[KEGG_DATA_NOT_FOUND] [{Scripting.ToString(entry)}] KEGG not sure the object is a protein.".__DEBUG_ECHO
            End If

            Return fa
        End Function

        ''' <summary>
        ''' Handle query for a gene locus from KEGG
        ''' </summary>
        ''' <param name="locus_id"></param>
        ''' <returns></returns>
        <ExportAPI("QueryEntry.GET")>
        Public Function GetQueryEntry(locus_id As String) As QueryEntry
            Dim list As QueryEntry() = WebRequest.HandleQuery(locus_id)

            If list.IsNullOrEmpty Then
                Call $"[KEGG_ENTRY_NOT_FOUND] [Query_LocusTAG={locus_id}]".__DEBUG_ECHO
                Return Nothing
            End If

            Dim LQuery = From queryEntry As QueryEntry
                                       In list
                         Where String.Equals(locus_id, queryEntry.locusID, StringComparison.OrdinalIgnoreCase)
                         Select queryEntry
            Dim entry As QueryEntry = LQuery.FirstOrDefault

            If entry Is Nothing Then
                Call $"[KEGG_ENTRY_NOT_FOUND] [Query_LocusTAG={locus_id}]".__DEBUG_ECHO
            End If

            Return entry
        End Function

        <ExportAPI("Fasta.Download")>
        Public Function Downloads(DIR As String, sId As String) As FastaSeq
            Dim path As String = $"{DIR}/Downloaded/{sId}.fasta"

            If path.FileExists Then
                Return FastaSeq.Load(path)
            Else
                Try
                    Return downloadWithCache(path, sId)
                Catch ex As Exception
                    Call App.LogException(ex)
                    Return Nothing
                End Try
            End If
        End Function

        Private Function downloadWithCache(fa As String, sId As String) As FastaSeq
            Dim fasta As FastaSeq = WebRequest.DownloadSequence(sId)

            If Not fasta Is Nothing Then
                Call fasta.SaveTo(fa)
            End If

            Return fasta
        End Function

        ''' <summary>
        ''' 同一个基因组内的蛋白质的序列下载推荐使用这个方法来完成，这个方法KEGG服务器的负担会比较轻
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Downloads.Batch")>
        <Description("It is recommended using this method for the batch downloaded of the protein sequence from the KEGG server when the protein is in the same genome.")>
        Public Function DownloadsBatch(DIR$, list As IEnumerable(Of String)) As FastaFile
            Dim LQuery = (From sId As String
                          In list
                          Let Entry As QueryEntry = GetQueryEntry(sId)
                          Where Not Entry Is Nothing
                          Select Entry).FirstOrDefault

            If LQuery Is Nothing Then ' 找不到记录
                Call $"Could not found any record from KEGG database for {list.Take(5).ToArray.JoinBy("; ")}!!!".__DEBUG_ECHO
                Return Nothing
            End If

            Dim sp As String = LQuery.speciesID
            Dim batchDownloads = (From sId As String In list Select __downloadDirect(DIR, sId, sp)).ToArray ' invoke the batch downloads task

            Return New FastaFile(batchDownloads)
        End Function

        Private Function __downloadDirect(DIR As String, sId As String, sp As String) As FASTA.FastaSeq
            Dim path As String = $"{DIR}/Downloaded/{sId}.fasta"

            If path.FileExists Then
                Return FastaSeq.Load(path)
            Else
                Try
                    Dim entry As New QueryEntry With {
                        .speciesID = sp,
                        .locusID = sId
                    }
                    Dim fa As FastaSeq = WebRequest.FetchSeq(entry)
                    If Not fa Is Nothing Then
                        Call fa.SaveTo(path)
                    End If
                    Return fa
                Catch ex As Exception
                    Call App.LogException(ex)
                    Return Nothing
                End Try
            End If
        End Function

        ''' <summary>
        ''' 都定义在这个地方了。。。。
        ''' </summary>
        Const _16S_rRNA As String = "http://www.genome.jp/dbget-bin/www_bget?ko:K01977"

        ''' <summary>
        ''' http://www.genome.jp/dbget-bin/www_bget?ko:K01977
        ''' 
        ''' 这个函数先下载单独的16sRNA序列，然后再合并为同一个大文件返回
        ''' </summary>
        ''' <param name="outDIR"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Download.16S_rRNA")>
        Public Function Download16S_rRNA(outDIR As String) As FastaFile
            Dim ortholog = DBGET.bGetObject.SSDB.API.QueryURL(_16S_rRNA)
            Dim out As New List(Of FastaSeq)

            For Each gene As QueryEntry In ortholog.genes
                Dim fa As FastaSeq = KEGG.WebServices.FetchNt(gene.speciesID, gene.locusID)

                If Not fa Is Nothing Then
                    Dim path As String = $"{outDIR}/{gene.speciesID}_{gene.locusID}.fasta"
                    Call fa.SaveTo(path)
                    Call out.Add(fa)
                Else
                    Call $"{gene.speciesID}:{gene.locusID} Download failure!".__DEBUG_ECHO
                End If
            Next

            Return New FastaFile(out)
        End Function

        ''' <summary>
        ''' 从KEGG服务器上面得到基因组的摘要代码
        ''' </summary>
        ''' <param name="locusId"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("SpCode")>
        Public Function GetSpCode(locusId As String) As String
            Dim query = HandleQuery(keyword:=locusId)
            If query.IsNullOrEmpty Then
                Return ""
            End If

            Dim LQuery = (From x As QueryEntry In query
                          Where String.Equals(locusId, x.locusID, StringComparison.OrdinalIgnoreCase)
                          Select x).FirstOrDefault

            If LQuery Is Nothing Then
                Return ""
            Else
                Return LQuery.speciesID
            End If
        End Function
    End Module
End Namespace
