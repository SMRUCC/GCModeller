Imports System.Diagnostics
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.BioSystem.Intervention

Namespace Perturbation.Analysis

    ''' <summary>
    ''' 一键分析导出的网格与开关。
    ''' </summary>
    ''' <remarks>
    ''' 所有默认值与各分析器的默认值保持一致；改这里只影响
    ''' <see cref="AnalysisExporter.ExportAll"/> 一次编排的产物。
    ''' </remarks>
    Public Class AnalysisOptions

        ''' <summary>四个单扰动分析共同使用的干预模式。</summary>
        Public Property Mode As InterventionMode = InterventionMode.Knockout

        ''' <summary>ω 扫描网格；<c>Nothing</c> 时取分析器默认值。</summary>
        Public Property Omegas As Double()

        ''' <summary>步数扫描网格；<c>Nothing</c> 时取分析器默认值。</summary>
        Public Property StepsList As Integer()

        ''' <summary>ω 扫描与 lag 分析使用的迭代步数。</summary>
        Public Property SweepSteps As Integer = 50

        ''' <summary>剂量扫描网格；<c>Nothing</c> 时取分析器默认值。</summary>
        Public Property Doses As Double()

        ''' <summary>剂量扫描使用的迭代步数。</summary>
        Public Property DoseSteps As Integer = 60

        ''' <summary>组合分析使用的迭代步数。</summary>
        Public Property CombinationSteps As Integer = 60

        ''' <summary>剂量-响应与组合分析保留的基因数（0 表示全部）。</summary>
        Public Property TopGenes As Integer = 50

        ''' <summary>剂量-时间曲面保留的基因数。</summary>
        Public Property SurfaceTopGenes As Integer = 20

        ''' <summary>lag 的触发阈值比例。</summary>
        Public Property LagThresholdRatio As Double = 0.1

        ''' <summary>判定「不变」的相对偏差阈值。</summary>
        Public Property InvariantTolerance As Double = 0.001

        ''' <summary>单行摘要。</summary>
        Public Overrides Function ToString() As String
            Return $"{Mode}, sweepSteps={SweepSteps}, doseSteps={DoseSteps}, top={TopGenes}, surfaceTop={SurfaceTopGenes}"
        End Function

    End Class

    ''' <summary>
    ''' 把四类分析结果写成 CSV 与一份 JSON 清单。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 产出的表格都是长表（每行一个观测），列名与 R 侧 <c>data.frame</c> 的构造一一对应，
    ''' 方便直接用 <c>read.csv</c> 读入或与 R# 导出的 dataframe 对拍。
    ''' </para>
    ''' <para>
    ''' 数值一律用 <c>R</c> 格式（可往返）与不变文化写出，避免区域设置把小节写成逗号。
    ''' 布尔值写成 <c>TRUE</c> / <c>FALSE</c>。
    ''' </para>
    ''' </remarks>
    Public Module AnalysisExporter

        ''' <summary>ω 与步数扫描的合并表。</summary>
        Public Const SensitivityFile As String = "analysis.sensitivity.csv"

        ''' <summary>lag 宽表（行 = 基因，列 = ω）。</summary>
        Public Const LagFile As String = "analysis.lag.csv"

        ''' <summary>Spearman 秩相关矩阵。</summary>
        Public Const SpearmanFile As String = "analysis.spearman.csv"

        ''' <summary>剂量-响应长表。</summary>
        Public Const DoseResponseFile As String = "analysis.dose_response.csv"

        ''' <summary>剂量-时间曲面长表。</summary>
        Public Const SurfaceFile As String = "analysis.dose_time_surface.csv"

        ''' <summary>组合非加性长表。</summary>
        Public Const CombinationFile As String = "analysis.combination.csv"

        ''' <summary>汇总清单。</summary>
        Public Const ReportFile As String = "analysis.report.json"

        ''' <summary>
        ''' 一次跑完四项分析并写出全部产物。
        ''' </summary>
        ''' <param name="model">已训练的模型</param>
        ''' <param name="gene">单扰动分析的靶基因</param>
        ''' <param name="geneA">组合分析的第一个基因；为空时跳过组合部分</param>
        ''' <param name="geneB">组合分析的第二个基因；为空时跳过组合部分</param>
        ''' <param name="dir">输出目录（不存在时自动创建）</param>
        ''' <param name="options">网格与开关；<c>Nothing</c> 时用默认值</param>
        ''' <returns>实际写出的文件路径</returns>
        Public Function ExportAll(model As FuzzyGeneModel,
                                  gene As String,
                                  geneA As String,
                                  geneB As String,
                                  dir As String,
                                  Optional options As AnalysisOptions = Nothing) As String()

            If model Is Nothing Then
                Throw New ArgumentNullException(NameOf(model))
            End If
            If model.Network Is Nothing Then
                Throw New InvalidOperationException("the model has not been trained yet")
            End If
            If String.IsNullOrEmpty(gene) Then
                Throw New ArgumentException("a target gene is required", NameOf(gene))
            End If
            If String.IsNullOrWhiteSpace(dir) Then
                Throw New ArgumentException("the output directory is empty", NameOf(dir))
            End If

            Dim settings As AnalysisOptions = If(options, New AnalysisOptions())
            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim files As New List(Of String)
            Dim runs As Integer = 0
            Dim notes As New List(Of String)

            Call dir.MakeDir

            ' ---------------- 1. ω 不变性 ----------------
            Dim omega As OmegaSweepResult = SolverStability.SweepOmega(
                model, gene, settings.Mode, settings.Omegas, settings.SweepSteps,
                Double.NaN, settings.InvariantTolerance)

            runs += omega.Runs

            ' ---------------- 2. 步数不变性 ----------------
            Dim steps As StepSweepResult = SolverStability.SweepSteps(
                model, gene, settings.Mode, settings.StepsList,
                Double.NaN, settings.InvariantTolerance)

            runs += steps.Runs

            ' ---------------- 3. lag 稳健性 ----------------
            Dim lag As LagRobustnessResult = SolverStability.LagRobustness(
                model, gene, settings.Mode, settings.Omegas, settings.SweepSteps, settings.LagThresholdRatio)

            runs += lag.Runs

            ' ---------------- 4. 剂量扫描 ----------------
            Dim dose As DoseResponseResult = DoseScan.DoseResponse(
                model, gene, settings.Doses, settings.DoseSteps, Double.NaN, settings.TopGenes)

            runs += dose.Runs

            Dim surface As DoseTimeSurfaceResult = DoseScan.DoseTimeSurface(
                model, gene, settings.Doses, settings.DoseSteps, Double.NaN, settings.SurfaceTopGenes)

            runs += surface.Runs

            ' ---------------- 5. 组合非加性 ----------------
            Dim combination As CombinationTraceResult = Nothing

            If Not String.IsNullOrEmpty(geneA) AndAlso Not String.IsNullOrEmpty(geneB) Then
                If String.Equals(geneA, geneB, StringComparison.OrdinalIgnoreCase) Then
                    Call notes.Add("the combination analysis was skipped: geneA and geneB are the same")
                Else
                    combination = CombinationTrace.Analyze(
                        model, geneA, geneB, settings.Mode, settings.CombinationSteps,
                        Double.NaN, settings.TopGenes)

                    runs += combination.Runs
                End If
            Else
                Call notes.Add("the combination analysis was skipped: geneA / geneB were not given")
            End If

            watch.Stop()

            ' ---------------- 6. 落盘 ----------------
            Dim sensitivityPath As String = System.IO.Path.Combine(dir, SensitivityFile)
            Dim lagPath As String = System.IO.Path.Combine(dir, LagFile)
            Dim spearmanPath As String = System.IO.Path.Combine(dir, SpearmanFile)
            Dim dosePath As String = System.IO.Path.Combine(dir, DoseResponseFile)
            Dim surfacePath As String = System.IO.Path.Combine(dir, SurfaceFile)
            Dim reportPath As String = System.IO.Path.Combine(dir, ReportFile)

            Call WriteSensitivity(omega, steps, sensitivityPath)
            Call WriteLag(lag, lagPath)
            Call WriteSpearman(lag, spearmanPath)
            Call WriteDoseResponse(dose, dosePath)
            Call WriteSurface(surface, surfacePath)

            Call files.Add(sensitivityPath)
            Call files.Add(lagPath)
            Call files.Add(spearmanPath)
            Call files.Add(dosePath)
            Call files.Add(surfacePath)

            If combination IsNot Nothing Then
                Dim combinationPath As String = System.IO.Path.Combine(dir, CombinationFile)

                Call WriteCombination(combination, combinationPath)
                Call files.Add(combinationPath)
            End If

            Call files.Add(reportPath)

            Call WriteReport(model, gene, geneA, geneB, settings, omega, steps, lag, dose, surface, combination,
                             runs, watch.Elapsed.TotalMilliseconds, notes, files.ToArray(), reportPath)

            Return files.ToArray()
        End Function

        ' ==================== 各个表的写法 ====================

        ''' <summary>ω 与步数扫描合并成一张长表：<c>kind</c> 列区分两者。</summary>
        Private Sub WriteSensitivity(omega As OmegaSweepResult, steps As StepSweepResult, path As String)
            Dim text As New StringBuilder()

            Call text.AppendLine("kind,value,converged,iterations,last_max_delta,max_abs_deviation,max_relative_deviation,invariant")

            For k As Integer = 0 To omega.Omegas.Length - 1
                Call text.AppendLine(
                    $"omega,{Num(omega.Omegas(k))},{Flag(omega.Converged(k))},{omega.Iterations(k)}," &
                    $"{Num(omega.LastMaxDelta(k))},{Num(omega.MaxAbsDeviation(k))},{Num(omega.MaxRelativeDeviation(k))},{Flag(omega.Invariant)}")
            Next

            For k As Integer = 0 To steps.StepsList.Length - 1
                Call text.AppendLine(
                    $"steps,{steps.StepsList(k)},{Flag(steps.Converged(k))},{steps.ActualIterations(k)}," &
                    $"{Num(steps.LastMaxDelta(k))},{Num(steps.MaxAbsDeviation(k))},{Num(steps.MaxRelativeDeviation(k))},{Flag(steps.Invariant)}")
            Next

            Call text.ToString().SaveTo(path, Utf8NoBom)
        End Sub

        ''' <summary>lag 宽表：行 = 基因，列 = ω。</summary>
        Private Sub WriteLag(lag As LagRobustnessResult, path As String)
            Dim text As New StringBuilder()
            Dim header As New List(Of String) From {"gene"}

            For k As Integer = 0 To lag.Omegas.Length - 1
                Call header.Add($"omega_{Num(lag.Omegas(k))}")
            Next

            Call text.AppendLine(String.Join(",", header))

            For j As Integer = 0 To lag.GeneNames.Length - 1
                Dim row As New List(Of String) From {Quote(lag.GeneNames(j))}

                For k As Integer = 0 To lag.Omegas.Length - 1
                    Call row.Add(Num(lag.Lag(k, j)))
                Next

                Call text.AppendLine(String.Join(",", row))
            Next

            Call text.ToString().SaveTo(path, Utf8NoBom)
        End Sub

        ''' <summary>Spearman 矩阵。</summary>
        Private Sub WriteSpearman(lag As LagRobustnessResult, path As String)
            Dim text As New StringBuilder()
            Dim header As New List(Of String) From {"omega"}

            For k As Integer = 0 To lag.Omegas.Length - 1
                Call header.Add($"omega_{Num(lag.Omegas(k))}")
            Next

            Call text.AppendLine(String.Join(",", header))

            For k As Integer = 0 To lag.Omegas.Length - 1
                Dim row As New List(Of String) From {Num(lag.Omegas(k))}

                For j As Integer = 0 To lag.Omegas.Length - 1
                    Call row.Add(Num(lag.Spearman(k, j)))
                Next

                Call text.AppendLine(String.Join(",", row))
            Next

            Call text.ToString().SaveTo(path, Utf8NoBom)
        End Sub

        ''' <summary>剂量-响应长表（全部基因 × 全部剂量点）。</summary>
        Private Sub WriteDoseResponse(dose As DoseResponseResult, path As String)
            Dim text As New StringBuilder()

            Call text.AppendLine("dose,clamp_value,gene,fold_change,percent_change,z_score,significant")

            For k As Integer = 0 To dose.Doses.Length - 1
                For g As Integer = 0 To dose.GeneNames.Length - 1
                    Call text.AppendLine(
                        $"{Num(dose.Doses(k))},{Num(dose.ClampValues(k))},{Quote(dose.GeneNames(g))}," &
                        $"{Num(dose.FoldChanges(k, g))},{Num(dose.PercentChanges(k, g))}," &
                        $"{Num(dose.ZScores(k, g))},{Flag(dose.Significant(k, g))}")
                Next
            Next

            Call text.ToString().SaveTo(path, Utf8NoBom)
        End Sub

        ''' <summary>剂量-时间曲面长表。</summary>
        Private Sub WriteSurface(surface As DoseTimeSurfaceResult, path As String)
            Dim text As New StringBuilder()

            Call text.AppendLine("dose,iteration,gene,delta")

            For d As Integer = 0 To surface.Doses.Length - 1
                For f As Integer = 0 To surface.NFrames - 1
                    For j As Integer = 0 To surface.GeneNames.Length - 1
                        Call text.AppendLine(
                            $"{Num(surface.Doses(d))},{f},{Quote(surface.GeneNames(j))},{Num(surface.Deltas(d, f, j))}")
                    Next
                Next
            Next

            Call text.ToString().SaveTo(path, Utf8NoBom)
        End Sub

        ''' <summary>组合非加性长表（全部基因）。</summary>
        Private Sub WriteCombination(combination As CombinationTraceResult, path As String)
            Dim text As New StringBuilder()

            Call text.AppendLine("gene,single_a,single_b,joint,additive,non_additive")

            For g As Integer = 0 To combination.GeneNames.Length - 1
                Call text.AppendLine(
                    $"{Quote(combination.GeneNames(g))},{Num(combination.SingleAMeans(g))}," &
                    $"{Num(combination.SingleBMeans(g))},{Num(combination.JointMeans(g))}," &
                    $"{Num(combination.AdditiveMeans(g))},{Num(combination.NonAdditive(g))}")
            Next

            Call text.ToString().SaveTo(path, Utf8NoBom)
        End Sub

        ''' <summary>汇总清单：网格定义 + 判定结果 + 运行次数 + 耗时。</summary>
        Private Sub WriteReport(model As FuzzyGeneModel,
                                gene As String,
                                geneA As String,
                                geneB As String,
                                settings As AnalysisOptions,
                                omega As OmegaSweepResult,
                                steps As StepSweepResult,
                                lag As LagRobustnessResult,
                                dose As DoseResponseResult,
                                surface As DoseTimeSurfaceResult,
                                combination As CombinationTraceResult,
                                runs As Integer,
                                elapsedMs As Double,
                                notes As List(Of String),
                                files As String(),
                                path As String)

            Dim report As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase) From {
                {"format", "fuzzygene analysis report"},
                {"created_at", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},
                {"target_gene", gene},
                {"gene_a", If(geneA, "")},
                {"gene_b", If(geneB, "")},
                {"mode", settings.Mode.ToString()},
                {"n_genes", model.Network.NGene},
                {"omega_sweep", SweepSummary(omega.Omegas, omega.Converged, omega.Iterations,
                                             omega.LastMaxDelta, omega.MaxRelativeDeviation, omega.Invariant,
                                             omega.ReferenceOmega)},
                {"step_sweep", SweepSummary(ToDouble(steps.StepsList), steps.Converged, steps.ActualIterations,
                                            steps.LastMaxDelta, steps.MaxRelativeDeviation, steps.Invariant,
                                            steps.ReferenceSteps)},
                {"lag", New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase) From {
                    {"threshold_ratio", lag.ThresholdRatio},
                    {"responding_genes", lag.GeneNames.Length},
                    {"mean_rho", lag.MeanRho},
                    {"omegas", lag.Omegas}
                }},
                {"dose_response", New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase) From {
                    {"doses", dose.Doses},
                    {"clamp_values", dose.ClampValues},
                    {"top_genes", dose.TopGenes.Length},
                    {"max_abs_z", MaxAbsZ(dose)}
                }},
                {"dose_time_surface", New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase) From {
                    {"doses", surface.Doses},
                    {"frames", surface.NFrames},
                    {"genes", surface.GeneNames.Length}
                }},
                {"combination", If(combination Is Nothing, Nothing,
                    New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase) From {
                        {"gene_a", combination.GeneA},
                        {"gene_b", combination.GeneB},
                        {"max_abs_non_additive", combination.MaxAbsNonAdditive},
                        {"top_genes", combination.TopGenes.Length}
                    })},
                {"runs", runs},
                {"elapsed_ms", elapsedMs},
                {"notes", notes.ToArray()},
                {"files", files}
            }

            Call File.WriteAllText(path, report.GetJson(indent:=True), Utf8NoBom)
        End Sub

        Private Function SweepSummary(values As Double(),
                                      converged As Boolean(),
                                      iterations As Integer(),
                                      lastDelta As Double(),
                                      relativeDeviation As Double(),
                                      invariant As Boolean,
                                      reference As Double) As Dictionary(Of String, Object)

            Return New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase) From {
                {"values", values},
                {"converged", converged},
                {"iterations", iterations},
                {"last_max_delta", lastDelta},
                {"max_relative_deviation", relativeDeviation},
                {"invariant", invariant},
                {"reference", reference}
            }
        End Function

        Private Function MaxAbsZ(dose As DoseResponseResult) As Double
            Dim max As Double = 0.0
            Dim last As Integer = dose.Doses.Length - 1

            For g As Integer = 0 To dose.GeneNames.Length - 1
                Dim value As Double = System.Math.Abs(dose.ZScores(last, g))

                If value > max Then
                    max = value
                End If
            Next

            Return max
        End Function

        Private Function ToDouble(values As Integer()) As Double()
            Dim result(values.Length - 1) As Double

            For i As Integer = 0 To values.Length - 1
                result(i) = values(i)
            Next

            Return result
        End Function

        ' ==================== 格式化 ====================

        ''' <summary>可往返的数值格式；NaN / Inf 写成 R 认识的字面量。</summary>
        Private Function Num(value As Double) As String
            If Double.IsNaN(value) Then
                Return "NaN"
            End If
            If Double.IsInfinity(value) Then
                Return If(value > 0, "Inf", "-Inf")
            End If

            Return value.ToString("R", Globalization.CultureInfo.InvariantCulture)
        End Function

        Private Function Flag(value As Boolean) As String
            Return If(value, "TRUE", "FALSE")
        End Function

        ''' <summary>CSV 字段转义（基因名一般是字母数字，但 ID 里可能带逗号）。</summary>
        Private Function Quote(text As String) As String
            If String.IsNullOrEmpty(text) Then
                Return """"""
            End If
            If text.IndexOfAny(New Char() {","c, """"c, ControlChars.Lf, ControlChars.Cr}) < 0 Then
                Return text
            End If

            Return """" & text.Replace("""", """""") & """"
        End Function

        Private ReadOnly Property Utf8NoBom As UTF8Encoding
            Get
                Return New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False)
            End Get
        End Property

    End Module

End Namespace
