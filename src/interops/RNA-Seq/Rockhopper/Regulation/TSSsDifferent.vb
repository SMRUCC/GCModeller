' /********************************************************************************/
'
'  Rockhopper —— 条件间 TSS/TTS 差异（调控关系预测）
'
'  复刻自原始 Rockhopper 的 AnalysisAPI.TSSsDifferent 模型与 TSSsAnalysis.DifferentTSSs：
'  对每一对实验条件，比较同一基因的转录起始位点(TSS)与终止位点(TTS)；
'  当 |Δ| > 10 bp 时判定该位点发生改变（启动子/终止子使用发生变化）。
'
'  迁移要点：
'    * 特性命名空间改为 Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
'    * 移除已失效的 Microsoft.VisualBasic.DocumentFormat.Csv.*
'    * KEGG 通路信息保留为 Pathway 字段，供 KEGGEnrichment 使用
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection

Namespace Regulation

    ''' <summary>
    ''' 一个基因在两个实验条件之间 TSS/TTS 的差异。
    ''' </summary>
    Public Class TSSsDifferent

        ''' <summary>基因号 / locus_tag。</summary>
        Public Property GeneID As String
        ''' <summary>条件 1 中的转录起始位点。</summary>
        Public Property TSSs_Condition1 As Long
        ''' <summary>条件 2 中的转录起始位点。</summary>
        Public Property TSSs_Condition2 As Long
        ''' <summary>条件 1 中的转录终止位点。</summary>
        Public Property TTSs_Condition1 As Long
        ''' <summary>条件 2 中的转录终止位点。</summary>
        Public Property TTSs_Condition2 As Long

        ''' <summary>产生差异的两个实验条件的名称。</summary>
        Public Property Condition1 As String
        ''' <summary>产生差异的两个实验条件的名称。</summary>
        Public Property Condition2 As String

        ''' <summary>该基因参与的 KEGG 通路。</summary>
        <Collection("Pathway", "; ")> Public Property Pathway As String()

        Public Overrides Function ToString() As String
            Return GeneID
        End Function

        Public Function HaveBoth() As Boolean
            Return TSSs_Condition1 <> 0 AndAlso TSSs_Condition2 <> 0 AndAlso TTSs_Condition1 <> 0 AndAlso TTSs_Condition2 <> 0
        End Function

        Public Function HaveTSSs() As Boolean
            Return TSSs_Condition1 <> 0 AndAlso TSSs_Condition2 <> 0
        End Function

        Public Function HaveTTSs() As Boolean
            Return TTSs_Condition1 <> 0 AndAlso TTSs_Condition2 <> 0
        End Function

        ''' <summary>TSS 是否发生变化（|Δ| &gt; 10 bp）。</summary>
        Public Function TSSChanged() As Boolean
            If Not HaveTSSs() Then Return False
            Return System.Math.Abs(TSSs_Condition1 - TSSs_Condition2) > 10
        End Function

        ''' <summary>TTS 是否发生变化（|Δ| &gt; 10 bp）。</summary>
        Public Function TTSChanged() As Boolean
            If Not HaveTTSs() Then Return False
            Return System.Math.Abs(TTSs_Condition1 - TTSs_Condition2) > 10
        End Function

    End Class

    ''' <summary>
    ''' 单个转录本用于比较的位点信息（避免与具体转录本模型耦合）。
    ''' </summary>
    Public Structure TranscriptLoci

        Public Property GeneID As String
        Public Property TSSs As Long
        Public Property TTSs As Long

        Public Sub New(geneId As String, tss As Long, tts As Long)
            Me.GeneID = geneId
            Me.TSSs = tss
            Me.TTSs = tts
        End Sub

    End Structure

    ''' <summary>
    ''' 条件间 TSS/TTS 差异计算。
    ''' </summary>
    Public Module TSSsDifferentAnalysis

        ''' <summary>
        ''' 比较两个条件下同一基因的 TSS/TTS，返回发生变化或信息互补的条目。
        ''' </summary>
        ''' <param name="condition1">条件 1 的转录本位点。</param>
        ''' <param name="condition2">条件 2 的转录本位点。</param>
        ''' <param name="conditionName1">条件 1 名称。</param>
        ''' <param name="conditionName2">条件 2 名称。</param>
        Public Function DifferentTSSs(condition1 As IEnumerable(Of TranscriptLoci),
                                      condition2 As IEnumerable(Of TranscriptLoci),
                                      conditionName1 As String,
                                      conditionName2 As String) As TSSsDifferent()

            Dim index2 As Dictionary(Of String, TranscriptLoci) =
                condition2.Where(Function(t) Not String.IsNullOrEmpty(t.GeneID)) _
                          .GroupBy(Function(t) t.GeneID) _
                          .ToDictionary(Function(g) g.Key, Function(g) g.First())

            Dim results As New List(Of TSSsDifferent)()

            For Each t1 As TranscriptLoci In condition1
                If String.IsNullOrEmpty(t1.GeneID) Then Continue For

                Dim t2 As TranscriptLoci
                If Not index2.TryGetValue(t1.GeneID, t2) Then Continue For

                Dim diff As New TSSsDifferent With {
                    .GeneID = t1.GeneID,
                    .TSSs_Condition1 = t1.TSSs,
                    .TSSs_Condition2 = t2.TSSs,
                    .TTSs_Condition1 = t1.TTSs,
                    .TTSs_Condition2 = t2.TTSs,
                    .Condition1 = conditionName1,
                    .Condition2 = conditionName2
                }

                ' 仅保留确实发生改变（或某一位点信息缺失导致无法比较）的条目
                If diff.TSSChanged() OrElse diff.TTSChanged() OrElse Not diff.HaveBoth() Then
                    results.Add(diff)
                End If
            Next

            Return results.ToArray
        End Function

        ''' <summary>
        ''' 对所有条件两两比较。
        ''' </summary>
        Public Function DifferentTSSs(conditions As IReadOnlyDictionary(Of String, IEnumerable(Of TranscriptLoci))) As TSSsDifferent()
            Dim names As String() = conditions.Keys.ToArray()
            Dim results As New List(Of TSSsDifferent)()

            For x As Integer = 0 To names.Length - 2
                For y As Integer = x + 1 To names.Length - 1
                    results.AddRange(DifferentTSSs(conditions(names(x)), conditions(names(y)), names(x), names(y)))
                Next
            Next

            Return results.ToArray
        End Function

    End Module

End Namespace
