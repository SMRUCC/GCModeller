#Region "Microsoft.VisualBasic::f71ec9b2bfb0eb374060104ec25c033b, modules\Knowledge_base\test\Program.vb"

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

    '   Total Lines: 131
    '    Code Lines: 86 (65.65%)
    ' Comment Lines: 23 (17.56%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 22 (16.79%)
    '     File Size: 6.20 KB


    ' Module Program
    ' 
    '     Function: Main
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
'  Program.vb - PubMedEUtilities 使用示例
'  ---------------------------------------------------------------------------
'  演示如何调用 PubMedEUtilities 模块检索 PubMed 文献并下载 PMC 全文。
'  适用于 VB.NET 控制台应用程序（.NET Core 3.1+ / .NET 5+）。
' ============================================================================

Imports System.IO
Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI.PubMedFetcher

Module Program

    Function Main(args As String()) As Integer
        ' Main 必须返回 Integer；异步逻辑通过 .GetAwaiter().GetResult() 同步等待
        MainAsync(args).GetAwaiter().GetResult()
        Return 0
    End Function

    Async Function MainAsync(args As String()) As Task

        ' -------------------------------------------------------------------
        ' 1. 配置客户端（强烈建议填写 email；如有 API Key 可显著提升速率）
        ' -------------------------------------------------------------------
        PubMedEUtilities.Configure(
            email:="your_email@example.com",
            apiKey:="",                       ' 可选：在 https://www.ncbi.nlm.nih.gov/account/ 申请
            toolName:="MyPubMedTool")

        ' 输出目录
        Dim outDir As String = Path.Combine(Environment.CurrentDirectory, "pmc_output")
        Directory.CreateDirectory(outDir)

        ' -------------------------------------------------------------------
        ' 示例 A：按关键词检索，批量获取题录 + 摘要 + PMC 全文
        ' -------------------------------------------------------------------
        Console.WriteLine(New String("="c, 70))
        Console.WriteLine("示例 A：关键词检索")
        Console.WriteLine(New String("="c, 70))

        Dim opts As New SearchOptions With {
            .Term = "CRISPR gene editing[Title/Abstract] AND 2023[dp]",
            .RetMax = 5,
            .Sort = "pub_date",
            .FetchPmcFullText = True,
            .PreferredPmcFormat = "html",     ' 优先 HTML；失败自动回退纯文本
            .OutputDirectory = outDir
        }

        Dim articles = Await PubMedEUtilities.SearchAndFetchAsync(opts)

        Console.WriteLine($"共检索到 {articles.Count} 篇文献：")
        Console.WriteLine()
        For Each art In articles
            Console.WriteLine($"• PMID: {art.Pmid}")
            Console.WriteLine($"  标题: {art.Title}")
            Console.WriteLine($"  作者: {String.Join(", ", art.Authors)}")
            Console.WriteLine($"  期刊: {art.JournalAbbrev} {art.PubDate};{art.Volume}({art.Issue}):{art.Pages}")
            Console.WriteLine($"  DOI : {art.Doi}")
            Console.WriteLine($"  PMC : {If(String.IsNullOrEmpty(art.PmcId), "无", "PMC" & art.PmcId)}")
            Console.WriteLine($"  全文: {If(art.HasPmcFullText, $"已获取 ({art.PmcFullTextFormat}, {art.PmcFullText.Length} 字符)", "未获取")}")
            If art.Abstract.Length > 0 Then
                Dim preview As String = If(art.Abstract.Length > 200,
                                           art.Abstract.Substring(0, 200) & "...",
                                           art.Abstract)
                Console.WriteLine($"  摘要: {preview}")
            End If
            For Each msg In art.Messages
                Console.WriteLine($"  [!] {msg}")
            Next
            Console.WriteLine()
        Next


        ' -------------------------------------------------------------------
        ' 示例 B：按单个 PMID 获取（已知 PMID 的情况）
        ' -------------------------------------------------------------------
        Console.WriteLine(New String("="c, 70))
        Console.WriteLine("示例 B：按 PMID 获取")
        Console.WriteLine(New String("="c, 70))

        Dim pmid As String = "36652858"   ' 替换为实际 PMID
        Try
            Dim [single] As PubMedArticle = Await PubMedEUtilities.FetchByPmidAsync(
                pmid,
                fetchPmcFullText:=True,
                preferredPmcFormat:="text",   ' 这次优先纯文本
                outputDirectory:=outDir)

            If [single] IsNot Nothing Then
                Console.WriteLine($"PMID {[single].Pmid}: {[single].Title}")
                Console.WriteLine($"引用: {[single].Citation}")
                Console.WriteLine($"PMC 全文格式: {[single].PmcFullTextFormat}, 长度: {[single].PmcFullText.Length}")
            Else
                Console.WriteLine($"未找到 PMID={pmid}")
            End If
        Catch ex As Exception
            Console.WriteLine($"错误: {ex.Message}")
        End Try


        ' -------------------------------------------------------------------
        ' 示例 C：仅检索 PMID 列表（不下载全文，速度最快）
        ' -------------------------------------------------------------------
        Console.WriteLine()
        Console.WriteLine(New String("="c, 70))
        Console.WriteLine("示例 C：仅检索 PMID 列表")
        Console.WriteLine(New String("="c, 70))

        Dim pmids = Await PubMedEUtilities.ESearchAsync(
            "machine learning[Title/Abstract] AND cancer[Title/Abstract]",
            retMax:=10,
            sort:="relevance")
        Console.WriteLine($"PMID 列表: {String.Join(", ", pmids)}")


        ' -------------------------------------------------------------------
        ' 示例 D：使用 ELINK 检查某 PMID 是否有 PMC 全文
        ' -------------------------------------------------------------------
        Console.WriteLine()
        Console.WriteLine(New String("="c, 70))
        Console.WriteLine("示例 D：ELINK 检查 PMC 可用性")
        Console.WriteLine(New String("="c, 70))

        Dim pmcId = Await PubMedEUtilities.FindPmcIdByPmidAsync("36652858")
        Console.WriteLine($"PMID 36652858 对应的 PMC ID: {If(String.IsNullOrEmpty(pmcId), "（无 PMC 全文）", "PMC" & pmcId)}")

        Console.WriteLine()
        Console.WriteLine("完成。PMC 全文已保存至: " & outDir)
    End Function

End Module

