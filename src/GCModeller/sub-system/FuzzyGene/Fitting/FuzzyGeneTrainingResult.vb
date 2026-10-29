Imports System.Text
Imports SMRUCC.genomics.Analysis.FuzzyGene.Config

Namespace Fitting

    ''' <summary>
    ''' 单个靶基因的拟合诊断记录。
    ''' </summary>
    Public Class GeneFitMetric

        ''' <summary>靶基因名称。</summary>
        Public Property Gene As String

        ''' <summary>实际使用的调控因子（规则预算降级之后）。</summary>
        Public Property Regulators As String()

        ''' <summary>训练集决定系数。</summary>
        Public Property R2 As Double

        ''' <summary>训练集均方根误差（z 空间）。</summary>
        Public Property RMSE As Double

        ''' <summary>最终规则数量。</summary>
        Public Property RuleCount As Integer

        ''' <summary>实际使用的模糊划分数（降级后可能小于配置值）。</summary>
        Public Property MfCount As Integer

        ''' <summary>训练行数。</summary>
        Public Property SampleCount As Integer

        ''' <summary>训练轮数。</summary>
        Public Property Iterations As Integer

        ''' <summary>状态：<c>ok</c> / <c>skipped</c> / <c>failed</c>。</summary>
        Public Property Status As String

        ''' <summary>跳过或失败的原因。</summary>
        Public Property Message As String

        Public Overrides Function ToString() As String
            If Status <> "ok" Then
                Return $"{Gene}: {Status} ({Message})"
            End If

            Return $"{Gene} <- {{{String.Join(", ", If(Regulators, New String() {}))}}}: " &
                   $"R2={R2:G4}, RMSE={RMSE:G4}, rules={RuleCount}, mf={MfCount}, n={SampleCount}"
        End Function

    End Class

    ''' <summary>
    ''' 整次拟合的汇总结果（诊断信息，不参与推理）。
    ''' </summary>
    Public Class FuzzyGeneTrainingResult

        ''' <summary>实际采用的拟合路线。</summary>
        Public Property Mode As FuzzyTargetMode

        ''' <summary>参与建模的基因总数。</summary>
        Public Property NGene As Integer

        ''' <summary>成功训练出模型的基因数。</summary>
        Public Property NTrained As Integer

        ''' <summary>被跳过的基因数（常表达、缺少候选因子等）。</summary>
        Public Property NSkipped As Integer

        ''' <summary>训练失败的基因数。</summary>
        Public Property NFailed As Integer

        ''' <summary>成功基因的平均决定系数。</summary>
        Public Property MeanR2 As Double

        ''' <summary>成功基因的平均训练 RMSE。</summary>
        Public Property MeanRmse As Double

        ''' <summary>导出的带符号调控边数量。</summary>
        Public Property EdgeCount As Integer

        ''' <summary>训练样本（状态行）数量（行采样之后）。</summary>
        Public Property NSample As Integer

        ''' <summary>原始样本数量（行采样之前）。</summary>
        Public Property NSampleRaw As Integer

        ''' <summary>相关性筛选实际使用的候选基因数量。</summary>
        Public Property NCandidatePool As Integer

        ''' <summary>预处理（统计量 + 列构建 + 列统计）耗时。</summary>
        Public Property PreprocessElapsed As TimeSpan

        ''' <summary>候选筛选累计耗时（CPU 时间，跨所有并行线程求和）。</summary>
        Public Property SelectionElapsed As TimeSpan

        ''' <summary>单基因训练累计耗时（CPU 时间，跨所有并行线程求和）。</summary>
        Public Property TrainingElapsed As TimeSpan

        ''' <summary>实际使用的并行 worker 线程数（诊断 128 核机器是否真的跑满）。</summary>
        Public Property ParallelWorkers As Integer

        ''' <summary>拟合阶段的总分配字节数（MB）；用于衡量缓冲复用的效果。</summary>
        Public Property AllocatedMB As Double

        ''' <summary>拟合阶段的 Gen2 GC 次数；大对象分配是否被消除，看这个最直接。</summary>
        Public Property Gen2Collections As Integer

        ''' <summary>逐基因诊断记录。</summary>
        Public Property GeneMetrics As GeneFitMetric()

        ''' <summary>降级、异常与数据质量告警。</summary>
        Public Property Warnings As String()

        ''' <summary>耗时。</summary>
        Public Property Elapsed As TimeSpan

        ''' <summary>按 R2 取训练最好的前 n 个基因。</summary>
        Public Function GetTopGenes(n As Integer) As GeneFitMetric()
            Return GeneMetrics _
                .Where(Function(m) m.Status = "ok") _
                .OrderByDescending(Function(m) m.R2) _
                .Take(n) _
                .ToArray()
        End Function

        ''' <summary>多行摘要。</summary>
        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()

            Dim sampleText As String = If(NSampleRaw > 0 AndAlso NSampleRaw <> NSample,
                $"{NSample} (sampled from {NSampleRaw})", NSample.ToString)

            Call sb.AppendLine($"FuzzyGene fitting: mode={Mode}, samples={sampleText}, genes={NGene}, candidate pool={NCandidatePool}")
            Call sb.AppendLine($"  trained={NTrained}, skipped={NSkipped}, failed={NFailed}, edges={EdgeCount}")
            Call sb.AppendLine($"  mean R2={MeanR2:G4}, mean RMSE={MeanRmse:G4}, elapsed={Elapsed.TotalSeconds:F2}s")
            Call sb.AppendLine($"  time: preprocessing={PreprocessElapsed.TotalSeconds:F2}s, " &
                               $"selection={SelectionElapsed.TotalSeconds:F1}s (cpu), " &
                               $"per-gene training={TrainingElapsed.TotalSeconds:F1}s (cpu)")
            Call sb.AppendLine($"  runtime: workers={ParallelWorkers}, allocated={AllocatedMB:F0} MB, gen2={Gen2Collections}")

            If Warnings IsNot Nothing AndAlso Warnings.Length > 0 Then
                Call sb.AppendLine($"  warnings: {Warnings.Length}")

                For Each message As String In Warnings.Take(10)
                    Call sb.AppendLine($"    - {message}")
                Next
            End If

            Return sb.ToString().TrimEnd()
        End Function

    End Class

    ''' <summary>拟合输出：网络模型 + 诊断报告。</summary>
    Public Class FuzzyGeneFitOutput

        ''' <summary>拟合出的调控网络。</summary>
        Public Property Network As FuzzyGeneNetwork

        ''' <summary>训练诊断。</summary>
        Public Property Report As FuzzyGeneTrainingResult

    End Class

End Namespace
