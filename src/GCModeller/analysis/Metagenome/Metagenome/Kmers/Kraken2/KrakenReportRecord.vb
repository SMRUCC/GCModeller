
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.ComponentModel

Namespace Kmers.Kraken2

    ''' <summary>
    ''' 用于存储 --report 文件中每一行的数据。这个文件提供了整个样本的分类汇总统计，非常直观。
    ''' </summary>
    ''' <remarks>
    ''' --report 文件是 taxon-centric 的，关注点是“每个分类单元包含了多少 reads”。
    ''' </remarks>
    Public Class KrakenReportRecord : Implements IExpressionValue, ITaxonomyAbundance

        ' [百分比]\t[该节点及子节点读数]\t[直接分配到该节点的读数]\t[等级代码]\t[TaxID]\t[分类名称]

        ''' <summary>
        ''' 分配到该分类单元（及其所有后代）的 reads 数量占样本中总分类 reads 数量的百分比。
        ''' </summary>
        ''' <returns></returns>
        Public Property Percentage As Double Implements IExpressionValue.ExpressionValue
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
        Public Property TaxID As UInteger Implements ITaxonomyAbundance.ncbi_taxid
        ''' <summary>
        ''' 该分类单元的科学名称。
        ''' </summary>
        ''' <returns></returns>
        Public Property ScientificName As String

        Private ReadOnly Property uniqueId As String Implements IReadOnlyId.Identity
            Get
                Return $"{TaxID}.{ScientificName}"
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{uniqueId} [rank:{RankCode} {Percentage}%]"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseDocument(filepath As String) As IEnumerable(Of KrakenReportRecord)
            Return KrakenParser.ParseReportFile(filepath)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function FilterHost(report As IEnumerable(Of KrakenReportRecord), hostIDs As Long()) As KrakenReportRecord()
            Return ReportFilter.FilterHumanReadsAndRecalculate(report.ToArray, hostIDs)
        End Function
    End Class
End Namespace