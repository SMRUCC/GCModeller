' /********************************************************************************/
'
'  Rockhopper —— 差异基因的 KEGG 通路富集（调控关系预测的一部分）
'
'  复刻原始 Rockhopper `API/TSSsAnalysis.KEGGDifferent` 的报表结构，但：
'    * 去除对 `LANS.SystemsBiology.Assembly.KEGG.DBGET.BriteHEntry` 的依赖
'      （该类在当前框架已不存在），改为直接按通路分组统计；
'    * 输出为可复用的行数据（<see cref="EnrichmentRow"/>），由调用方决定写 CSV 还是其它格式。
'
'  分组逻辑与原始实现一致：
'    1) 只统计"同时预测到 TSS 与 TTS"的基因；
'    2) 分别统计 TSS 与 TTS 同时改变 / 仅 TSS 改变 / 仅 TTS 改变；
'    3) 对每一类，按 KEGG 通路聚合基因，并按基因数降序输出。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection

Namespace Regulation

    ''' <summary>
    ''' 富集报表的一行。
    ''' </summary>
    Public Class EnrichmentRow

        ''' <summary>分组标题（例如 "TSSs & TTSs both changed:"）。</summary>
        Public Property Group As String
        ''' <summary>KEGG 通路编号。</summary>
        Public Property Pathway As String
        ''' <summary>该通路中发生变化的基因数。</summary>
        Public Property Count As Integer
        ''' <summary>该通路中发生变化的基因列表。</summary>
        <Collection("Genes", "; ")> Public Property Genes As String()

        Public Overrides Function ToString() As String
            Return $"{Group}{vbTab}{Pathway}{vbTab}{Count}{vbTab}{String.Join("; ", If(Genes, New String() {}))}"
        End Function

    End Class

    ''' <summary>
    ''' 差异 TSS/TTS 基因的 KEGG 通路富集。
    ''' </summary>
    Public Module KEGGEnrichment

        ''' <summary>
        ''' 生成分组的富集报表。
        ''' </summary>
        ''' <param name="data">条件间 TSS/TTS 差异结果（其 <see cref="TSSsDifferent.Pathway"/> 应为 KEGG 通路编号）。</param>
        Public Function BuildReport(data As IEnumerable(Of TSSsDifferent)) As List(Of EnrichmentRow)
            Dim rows As New List(Of EnrichmentRow)()
            Dim items As TSSsDifferent() = data.ToArray()

            Dim haveBoth As TSSsDifferent() = items.Where(Function(x) x.HaveBoth()).ToArray()

            rows.Add(New EnrichmentRow With {.Group = "Genes have both TSSs & TTSs predicted:", .Count = haveBoth.Length, .Pathway = ""})
            rows.AddRange(groupByPathway(haveBoth.Where(Function(x) x.TSSChanged() AndAlso x.TTSChanged()), "TSSs & TTSs both changed:"))
            rows.AddRange(groupByPathway(haveBoth.Where(Function(x) x.TSSChanged() AndAlso Not x.TTSChanged()), "TSSs changed:"))
            rows.AddRange(groupByPathway(haveBoth.Where(Function(x) x.TTSChanged() AndAlso Not x.TSSChanged()), "TTSs changed:"))

            Dim onlyTss As TSSsDifferent() = items.Where(Function(x) x.HaveTSSs() AndAlso Not x.HaveTTSs()).ToArray()
            rows.Add(New EnrichmentRow With {.Group = "Genes only have TSSs predicted:", .Count = onlyTss.Length, .Pathway = ""})
            rows.AddRange(groupByPathway(onlyTss.Where(Function(x) x.TSSChanged()), "TSSs changed (TSS only):"))

            Dim onlyTts As TSSsDifferent() = items.Where(Function(x) x.HaveTTSs() AndAlso Not x.HaveTSSs()).ToArray()
            rows.Add(New EnrichmentRow With {.Group = "Genes only have TTSs predicted:", .Count = onlyTts.Length, .Pathway = ""})
            rows.AddRange(groupByPathway(onlyTts.Where(Function(x) x.TTSChanged()), "TTSs changed (TTS only):"))

            Return rows
        End Function

        Private Function groupByPathway(items As IEnumerable(Of TSSsDifferent), groupTitle As String) As List(Of EnrichmentRow)
            Dim rows As New List(Of EnrichmentRow)()
            Dim flattened As New List(Of (pathway As String, gene As String))()

            For Each item As TSSsDifferent In items
                If item.Pathway Is Nothing OrElse item.Pathway.Length = 0 Then
                    flattened.Add(("(unmapped)", item.GeneID))
                Else
                    For Each pathway As String In item.Pathway
                        flattened.Add((pathway, item.GeneID))
                    Next
                End If
            Next

            For Each group In flattened.GroupBy(Function(x) x.pathway).OrderByDescending(Function(g) g.Count())
                rows.Add(New EnrichmentRow With {
                    .Group = groupTitle,
                    .Pathway = group.Key,
                    .Count = group.Count(),
                    .Genes = group.Select(Function(x) x.gene).Distinct().ToArray()
                })
            Next

            Return rows
        End Function

    End Module

End Namespace
