Imports System.IO
Imports System.Linq
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.genomics.BioSystem.Intervention
Imports std = System.Math

''' <summary>
''' 上下文对齐公共组件的回归测试（console）。
''' </summary>
''' <remarks>
''' 覆盖五种对齐模式、缺少参考数据时的失败行为、单基因/常量基因/NaN/未知基因等边界，
''' 以及 <see cref="ContextAlignmentState"/> 的模式切换语义。所有断言与关键变量都通过
''' <see cref="DebugAudit"/> 记录，结束时写出一份 json 审计报告。
''' </remarks>
Module Program

    Private ReadOnly Genes As String() = {"g1", "g2", "g3", "g4"}

    Sub Main()
        Call DebugAudit.Reset()

        DebugAudit.MaxEvents = 4096

        Dim report As String = Path.Combine(AppContext.BaseDirectory, "contextAlignment.audit.json")

        Using DebugAudit.Watch("context-alignment", logOnExit:=False)
            Dim reference As ContextAlignmentReference = buildReference()

            Call testNone(reference)
            Call testAffine(reference)
            Call testLogAffine(reference)
            Call testQuantile(reference)
            Call testQuantileDelta(reference)
            Call testValidation()
            Call testState(reference)
            Call DebugAudit.SaveReport(report)
        End Using

        Console.WriteLine()
        Console.WriteLine($"audit report: {report}")
        Console.WriteLine($"failures    : {DebugAudit.FailureCount}")

        Environment.ExitCode = If(DebugAudit.FailureCount = 0, 0, 1)
    End Sub

    ''' <summary>
    ''' 构造参考统计量：每个基因的种群落在互不重叠的区间里，便于断言保序性。
    ''' </summary>
    Private Function buildReference() As ContextAlignmentReference
        Dim n As Integer = Genes.Length
        Dim population(n - 1)() As Double
        Dim means(n - 1) As Double
        Dim sds(n - 1) As Double
        Dim offsets(n - 1) As Double

        For i As Integer = 0 To n - 1
            Dim values(9) As Double
            Dim sum As Double = 0

            For k As Integer = 0 To 9
                values(k) = i * 100.0R + k
                sum += values(k)
            Next

            population(i) = values
            means(i) = sum / values.Length
            offsets(i) = (i + 1) * 10.0R

            Dim ss As Double = 0

            For k As Integer = 0 To 9
                ss += (values(k) - means(i)) ^ 2
            Next

            sds(i) = std.Sqrt(ss / 9.0R)
        Next

        Return New ContextAlignmentReference With {
            .GeneNames = Genes,
            .Means = means,
            .StandardDeviations = sds,
            .Population = population,
            .Offsets = offsets,
            .PseudoCount = 1.0R
        }
    End Function

    ''' <summary>构造 4 个基因都命中的上下文（取值跨度很大，模拟另一条归一化管线）。</summary>
    Private Function buildContext() As Dictionary(Of String, Double)
        Return New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase) From {
            {"g1", 5.0R},
            {"g2", 50.0R},
            {"g3", 500.0R},
            {"g4", 5000.0R}
        }
    End Function

    Private Sub testNone(reference As ContextAlignmentReference)
        Using DebugAudit.Watch("mode:none")
            Dim result As ContextAlignmentResult = ContextAligner.Align(buildContext(), reference, ContextAlignmentMode.None)

            Call DebugAudit.Test(result.Diagnostics.Matched = 4, $"all genes matched (actual {result.Diagnostics.Matched})")
            Call DebugAudit.Test(std.Abs(result.Values("g3") - 500.0R) < 1.0E-12, "None keeps the raw value untouched")
            Call DebugAudit.Test(Not result.Diagnostics.Aligned, "None is reported as not aligned")
        End Using
    End Sub

    Private Sub testAffine(reference As ContextAlignmentReference)
        Using DebugAudit.Watch("mode:affine")
            Dim result As ContextAlignmentResult = ContextAligner.Align(buildContext(), reference, ContextAlignmentMode.Affine)
            Dim index As Integer = Array.IndexOf(Genes, "g2")
            Dim expected As Double = (50.0R - reference.Means(index)) / reference.StandardDeviations(index)

            Call DebugAudit.Test(std.Abs(result.Values("g2") - expected) < 1.0E-12,
                                 $"Affine = (x - mean) / sd (actual {result.Values("g2"):F6}, expected {expected:F6})")
            Call DebugAudit.Test(result.Diagnostics.Aligned, "Affine is reported as aligned")
            ' 50 < 训练均值 104.5 ⇒ z 必须为负；仿射是“逐基因”的单调换算，
            ' 跨基因排序本来就可能改变，因此这里不能断言 Spearman = 1
            Call DebugAudit.Test(result.Values("g2") < 0.0R, "Affine centers each gene at the training mean")
        End Using
    End Sub

    Private Sub testLogAffine(reference As ContextAlignmentReference)
        Using DebugAudit.Watch("mode:log-affine")
            Dim result As ContextAlignmentResult = ContextAligner.Align(buildContext(), reference, ContextAlignmentMode.LogAffine)
            Dim index As Integer = Array.IndexOf(Genes, "g1")
            Dim expected As Double = std.Log(5.0R + 1.0R, 2.0) + reference.Offsets(index)

            Call DebugAudit.Test(std.Abs(result.Values("g1") - expected) < 1.0E-12,
                                 $"LogAffine = log2(x + pseudo) + offset (actual {result.Values("g1"):F6})")

            ' 负值必须在对数变换前被裁剪，而不是产生 NaN
            Dim context = buildContext()

            context("g1") = -3.0R

            Dim clipped As ContextAlignmentResult = ContextAligner.Align(context, reference, ContextAlignmentMode.LogAffine)

            Call DebugAudit.Test(Not Double.IsNaN(clipped.Values("g1")), "negative input is clipped instead of producing NaN")
            Call DebugAudit.Test(clipped.Diagnostics.Notes.Any(Function(n) n.Contains("裁剪")), "the clipping is reported")
        End Using
    End Sub

    Private Sub testQuantile(reference As ContextAlignmentReference)
        Using DebugAudit.Watch("mode:quantile")
            Dim result As ContextAlignmentResult = ContextAligner.Align(buildContext(), reference, ContextAlignmentMode.Quantile)
            Dim inRange As Boolean = True
            Dim orderedOk As Boolean = True
            Dim previous As Double = Double.MinValue

            For i As Integer = 0 To Genes.Length - 1
                Dim value As Double = result.Values(Genes(i))
                Dim sorted As Double() = reference.SortedPopulation(i)

                If value < sorted(0) - 1.0E-12 OrElse value > sorted(sorted.Length - 1) + 1.0E-12 Then
                    inRange = False
                End If

                If value <= previous Then
                    orderedOk = False
                End If

                previous = value
            Next

            Call DebugAudit.Test(inRange, "every aligned value falls inside the training population of its own gene")
            Call DebugAudit.Test(orderedOk, "the cross-gene ordering of the context is preserved")
            Call DebugAudit.Test(std.Abs(result.Diagnostics.SpearmanBeforeAfter - 1.0R) < 1.0E-09,
                                 "Quantile maps the context monotonically (Spearman = 1)")

            ' ---- 单基因上下文：跨基因分位无定义，必须原值注入并给出说明 ----
            Dim singleGene As New Dictionary(Of String, Double) From {{"g1", 123.0R}}
            Dim singleResult As ContextAlignmentResult = ContextAligner.Align(singleGene, reference, ContextAlignmentMode.Quantile)

            Call DebugAudit.Test(std.Abs(singleResult.Values("g1") - 123.0R) < 1.0E-12,
                                 "a single-gene context is injected verbatim")
            Call DebugAudit.Test(singleResult.Diagnostics.Notes.Count > 0, "the single-gene case is documented in the diagnostics")
        End Using
    End Sub

    Private Sub testQuantileDelta(reference As ContextAlignmentReference)
        Using DebugAudit.Watch("mode:quantile-delta")
            Dim context = buildContext()
            ' 关闭负值裁剪，才能逐位核对公式本身（默认会裁剪到 0）
            Dim options As New ContextAlignmentOptions With {.ClipNegative = False}
            Dim result As ContextAlignmentResult = ContextAligner.Align(context, reference, ContextAlignmentMode.QuantileDelta, options)

            ' 独立复算：aligned = x + (Q_gene(p) - Q_source(p))
            Dim source(Genes.Length - 1) As Double
            Dim means(Genes.Length - 1) As Double

            For i As Integer = 0 To Genes.Length - 1
                source(i) = std.Log(1.0R + context(Genes(i)))
                means(i) = reference.Means(i)
            Next

            Call Array.Sort(source)
            Call Array.Sort(means)

            Dim consistent As Boolean = True

            For i As Integer = 0 To Genes.Length - 1
                Dim p As Double = ContextAligner.PercentileOf(source, std.Log(1.0R + context(Genes(i))))
                Dim target As Double = ContextAligner.QuantileOf(reference.SortedPopulation(i), 10, p)
                Dim baseline As Double = ContextAligner.QuantileOf(means, means.Length, p)
                Dim expected As Double = context(Genes(i)) + (target - baseline)

                If std.Abs(result.Values(Genes(i)) - expected) > 1.0E-09 Then
                    consistent = False
                End If
            Next

            Call DebugAudit.Test(consistent, "QuantileDelta = x + (Q_gene(p) - Q_source(p))")

            ' ---- 与纯分位映射相比，分位偏移应当更接近输入的绝对量级（保留倍数） ----
            Dim quantile As ContextAlignmentResult = ContextAligner.Align(context, reference, ContextAlignmentMode.Quantile)
            Dim distanceDelta As Double = 0.0
            Dim distanceQuantile As Double = 0.0

            For i As Integer = 0 To Genes.Length - 1
                Dim raw As Double = context(Genes(i))

                distanceDelta += std.Abs(result.Values(Genes(i)) - raw)
                distanceQuantile += std.Abs(quantile.Values(Genes(i)) - raw)
            Next

            Call DebugAudit.Mark("l1-distance", $"delta={distanceDelta:F3}, quantile={distanceQuantile:F3}")
            Call DebugAudit.Test(distanceDelta < distanceQuantile,
                                 "QuantileDelta stays closer to the raw magnitude than the pure quantile mapping")
        End Using
    End Sub

    Private Sub testValidation()
        Using DebugAudit.Watch("validation")
            ' 没有 population 时不允许 Quantile
            Dim bare As New ContextAlignmentReference With {
                .GeneNames = Genes,
                .Means = {1.0R, 2.0R, 3.0R, 4.0R},
                .StandardDeviations = {1.0R, 1.0R, 1.0R, 1.0R}
            }
            Dim rejected As Boolean = False

            Try
                Call ContextAligner.Align(buildContext(), bare, ContextAlignmentMode.Quantile)
            Catch ex As InvalidOperationException
                rejected = True
            End Try

            Call DebugAudit.Test(rejected, "Quantile without a training population fails loudly")

            ' 没有 Means 时不允许 Affine
            Dim noMeans As New ContextAlignmentReference With {.GeneNames = Genes}
            Dim rejectedAffine As Boolean = False

            Try
                Call ContextAligner.Align(buildContext(), noMeans, ContextAlignmentMode.Affine)
            Catch ex As InvalidOperationException
                rejectedAffine = True
            End Try

            Call DebugAudit.Test(rejectedAffine, "Affine without the per-gene mean/sd fails loudly")

            ' NaN / 未知基因 / 覆盖率
            Dim withInvalid As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase) From {
                {"g1", 1.0R},
                {"g2", Double.NaN},
                {"zzz", 3.0R}
            }
            Dim result As ContextAlignmentResult = ContextAligner.Align(withInvalid, ContextAlignmentReference.FromStatistics(
                Genes,
                {4.5R, 104.5R, 204.5R, 304.5R},
                {3.0R, 3.0R, 3.0R, 3.0R}), ContextAlignmentMode.Affine)

            Call DebugAudit.Test(result.Diagnostics.InvalidValues = 1, $"NaN is counted as invalid (actual {result.Diagnostics.InvalidValues})")
            Call DebugAudit.Test(result.Diagnostics.UnknownGenes.Contains("zzz"), "the unknown gene is reported")
            Call DebugAudit.Test(std.Abs(result.Diagnostics.Coverage - 0.25R) < 1.0E-09, "coverage is 1/4")

            ' ToVector 用训练均值补齐
            Dim vector As Double() = result.ToVector(reference:=ContextAlignmentReference.FromStatistics(
                Genes,
                {4.5R, 104.5R, 204.5R, 304.5R},
                {3.0R, 3.0R, 3.0R, 3.0R}), fill:=ContextMissingGeneFill.Mean)

            Call DebugAudit.Test(std.Abs(vector(1) - 104.5R) < 1.0E-12, "ToVector fills the uncovered genes with the training mean")
        End Using
    End Sub

    Private Sub testState(reference As ContextAlignmentReference)
        Using DebugAudit.Watch("state")
            Dim state As New ContextAlignmentState(ContextAlignmentMode.Quantile)
            Dim context = buildContext()

            Call state.SetContext(context, reference)

            Call DebugAudit.Test(state.HasContext, "the state reports the context as applied")
            Call DebugAudit.Test(state.Mode = ContextAlignmentMode.Quantile, "the default mode is Quantile")
            Call DebugAudit.Test(Not state.IsModeExplicit, "the default mode is not marked as explicit")

            ' 切换模式时必须用同一份原始上下文重新对齐
            Call state.SetMode(ContextAlignmentMode.None)

            Call DebugAudit.Test(state.IsModeExplicit, "switching the mode marks it as explicit")
            Call DebugAudit.Test(std.Abs(state.Values("g3") - 500.0R) < 1.0E-12, "switching to None re-aligns with the raw values")

            ' per-call 覆盖模式不应改变当前模式
            Call state.SetContextWith(context, reference, ContextAlignmentMode.Affine)

            Call DebugAudit.Test(state.Mode = ContextAlignmentMode.None, "SetContextWith does not change the current mode")

            Call state.Clear()

            Call DebugAudit.Test(Not state.HasContext, "Clear() drops the context")
        End Using
    End Sub

End Module
