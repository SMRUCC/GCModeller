Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

Namespace Kmers.Kraken2

    ''' <summary>
    ''' 用于存储 --output 文件中每一行的数据，这个文件详细列出了每一条序列（read）的分类结果。每一行对应一条 read。
    ''' </summary>
    ''' <remarks>
    ''' --output 文件是 read-centric 的，关注点是“每一条 read 被分到了哪里”。
    ''' </remarks>
    Public Class KrakenOutputRecord

        ' [状态码/分类代码]\t[Read名称]\t[TaxID]\t[Read长度]\t[LCA映射详情]

        ''' <summary>
        '''  "C" or "U"
        '''  
        ''' C: Classified (已分类)。表示 Kraken2 成功为该 read 分配了一个分类单元。
        ''' U: Unclassified (未分类)。表示该 read 未能被分配到任何数据库中的分类单元。
        ''' </summary>
        ''' <returns></returns>
        Public Property StatusCode As String
        ''' <summary>
        ''' 输入的 FASTQ 文件中该 read 的序列标识符。
        ''' </summary>
        ''' <returns></returns>
        Public Property ReadName As String
        ''' <summary>
        ''' 这是 Kraken2 最终分配给该 read 的分类单元的 NCBI Taxonomy ID。这个分类单元是该 read 上所有 k-mer 的最低共同祖先。
        ''' </summary>
        ''' <returns></returns>
        Public Property TaxID As Integer
        ''' <summary>
        ''' 该 read 的碱基数量。
        ''' </summary>
        ''' <returns></returns>
        Public Property ReadLength As Integer
        ''' <summary>
        ''' LCA映射详情, 使用字典来存储 k-mer 的分配详情，键是 TaxID，值是 k-mer 数量
        ''' 
        ''' 这是一个由空格分隔的列表，详细描述了该 read 中所有 k-mer 的分配情况。
        ''' 格式为 [TaxID]:[k-mer数量]。
        ''' 0 是一个特殊的 TaxID，通常代表未分类的 k-mer（即数据库中没有匹配的 k-mer）。0:6 表示有 6 个 k-mer 未被分类。
        ''' </summary>
        ''' <returns></returns>
        Public Property LcaMappings As New Dictionary(Of Integer, Integer)

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseDocument(filepath As String) As IEnumerable(Of KrakenOutputRecord)
            Return Kraken2.KrakenParser.ParseOutputFile(filepath)
        End Function

        Public Shared Sub Save(result As IEnumerable(Of KrakenOutputRecord), file As Stream)
            Using text As New StreamWriter(file) With {.NewLine = ASCII.LF}
                For Each line As KrakenOutputRecord In result.SafeQuery
                    Call text.WriteLine(New String() {
                         line.StatusCode,
                         line.ReadName,
                         line.TaxID,
                         line.ReadLength,
                         (From map As KeyValuePair(Of Integer, Integer)
                          In line.LcaMappings
                          Select $"{map.Key}:{map.Value}").JoinBy(" ")
                    }.JoinBy(vbTab))
                Next
            End Using
        End Sub

    End Class

    ''' <summary>
    ''' 用于存储 --report 文件中每一行的数据。这个文件提供了整个样本的分类汇总统计，非常直观。
    ''' </summary>
    ''' <remarks>
    ''' --report 文件是 taxon-centric 的，关注点是“每个分类单元包含了多少 reads”。
    ''' </remarks>
    Public Class KrakenReportRecord

        ' [百分比]\t[该节点及子节点读数]\t[直接分配到该节点的读数]\t[等级代码]\t[TaxID]\t[分类名称]

        ''' <summary>
        ''' 分配到该分类单元（及其所有后代）的 reads 数量占样本中总分类 reads 数量的百分比。
        ''' </summary>
        ''' <returns></returns>
        Public Property Percentage As Double
        ''' <summary>
        ''' 分配到该分类单元或其任何下级分类单元的 reads 总数。(该节点及子节点读数)
        ''' </summary>
        ''' <returns></returns>
        Public Property ReadsAtRank As Long
        ''' <summary>
        ''' 直接分配到该节点的读数，*直接*分配到该分类单元，而不是其任何子节点的 reads 数量。
        ''' </summary>
        ''' <returns></returns>
        Public Property ReadsDirect As Long
        ''' <summary>
        ''' 表示该分类单元在分类学树中的等级。
        ''' U: 未分类
        ''' R: 根
        ''' D: 域
        ''' K: 界
        ''' P: 门
        ''' C: 纲
        ''' O: 目
        ''' F: 科
        ''' G: 属
        ''' S: 种
        ''' S1, G1 等：有时用于表示亚种或未分类的属/种。
        ''' </summary>
        ''' <returns></returns>
        Public Property RankCode As String
        ''' <summary>
        ''' 该分类单元的 NCBI Taxonomy ID。
        ''' </summary>
        ''' <returns></returns>
        Public Property TaxID As Long
        ''' <summary>
        ''' 该分类单元的科学名称。
        ''' </summary>
        ''' <returns></returns>
        Public Property ScientificName As String

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseDocument(filepath As String) As IEnumerable(Of KrakenReportRecord)
            Return KrakenParser.ParseReportFile(filepath)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function FilterHost(report As KrakenReportRecord(), hostIDs As Long()) As KrakenReportRecord()
            Return ReportFilter.FilterHumanReadsAndRecalculate(report, hostIDs)
        End Function
    End Class
End Namespace