Imports System.Diagnostics
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports std = System.Math

''' <summary>
''' 与 GNU R 的对拍测试：用 Bioconductor（limma / edgeR / DESeq2）生成参考值，
''' 再逐值比较 <c>HTS_matrix</c> 之中数学计算 API 的结果。
''' </summary>
''' <remarks>
''' <para>
''' 数据流：
''' </para>
''' <code>
''' VB 生成 counts.csv + design.csv
'''        ↓
''' Rscript test/R/hts_matrix_reference.R input output
'''        ↓
''' R 用 limma/edgeR/DESeq2 写出 y.csv
'''        ↓
''' VB 读回 y.csv 并与自己的 API 输出逐值比较（DebugAudit 记录）
''' </code>
''' <para>
''' Rscript 的路径可以通过环境变量 <c>RSCRIPT_EXE</c> 覆盖，默认使用
''' <c>C:\Program Files\R\R-4.5.0\bin\Rscript.exe</c>；找不到 R 时整个用例集会被跳过
''' （记录一条 Mark，而不是把测试判为失败）。
''' </para>
''' </remarks>
Friend Module RCrossCheckTest

    Const DefaultRscript As String = "C:\Program Files\R\R-4.5.0\bin\Rscript.exe"

    ''' <summary>当前文件的失败次数</summary>
    Private failed As Integer = 0
    Private cases As Integer = 0

    ''' <summary>
    ''' 执行全部对拍用例
    ''' </summary>
    ''' <returns>失败的断言数量</returns>
    Friend Function Run() As Integer
        failed = 0
        cases = 0

        Dim originMute As Boolean = VBDebugger.Mute

        VBDebugger.Mute = True

        Try
            Return runImpl()
        Finally
            VBDebugger.Mute = originMute
        End Try
    End Function

    Private Function runImpl() As Integer
        Dim rscript As String = Environment.GetEnvironmentVariable("RSCRIPT_EXE")

        If String.IsNullOrEmpty(rscript) Then
            rscript = DefaultRscript
        End If

        If Not File.Exists(rscript) Then
            Call DebugAudit.Mark("r.crosscheck", $"skipped: Rscript not found at '{rscript}'")
            Return 0
        End If

        Dim script As String = findUp(Path.Combine("R", "hts_matrix_reference.R"))

        If script Is Nothing Then
            Call check(False, "the R reference script 'test/R/hts_matrix_reference.R' could not be located")
            Return failed
        End If

        Dim folder As String = Path.Combine(
            Path.GetTempPath(),
            "hts_matrix_r_crosscheck_" & Guid.NewGuid().ToString("N").Substring(0, 8)
        )

        Dim inputDir As String = Path.Combine(folder, "input")
        Dim outputDir As String = Path.Combine(folder, "output")

        Call Directory.CreateDirectory(inputDir)

        Try
            Call DebugAudit.Mark("r.crosscheck.script", script)

            Dim counts As Matrix = buildCounts()
            Dim samples As String() = counts.sampleID
            Dim groups As String() = sampleGroups(samples)

            Call counts.SaveMatrix(Path.Combine(inputDir, "counts.csv"))

            Using design As StreamWriter = New StreamWriter(Path.Combine(inputDir, "design.csv"), append:=False, Encoding.UTF8)
                Call design.WriteLine("sample,group")

                For i As Integer = 0 To samples.Length - 1
                    Call design.WriteLine($"{samples(i)},{groups(i)}")
                Next
            End Using

            Dim result = runRscript(rscript, script, inputDir, outputDir)

            If result.exitCode <> 0 Then
                Call check(False, $"Rscript failed (exit={result.exitCode}): {result.output}")
                Return failed
            End If

            Dim reference As String() = Directory.GetFiles(outputDir, "*.csv")

            Call DebugAudit.Mark("r.crosscheck.reference", $"{reference.Length} reference file(s) generated")

            Call crossCheckNormalizations(counts, outputDir)
            Call crossCheckHvgAndScaling(counts, outputDir)
            Call crossCheckLimma(counts, samples, groups, outputDir)
        Catch ex As Exception
            Call check(False, $"R cross check threw {ex.GetType().Name}: {ex.Message}")
        Finally
            Try
                Call Directory.Delete(folder, recursive:=True)
            Catch innerEx As Exception
                ' 清理失败不影响结论
            End Try
        End Try

        Call DebugAudit.Mark("r.crosscheck", $"{cases} case(s) executed, {failed} failure(s)")

        Return failed
    End Function

    Private Sub check(ok As Boolean, msg As String)
        If Not ok Then
            failed += 1
        End If

        Call DebugAudit.Test(ok, msg)
    End Sub

    ' ==================== 输入数据 ====================

    ''' <summary>
    ''' 构造一份确定的 counts 矩阵：800 个基因 × 8 个样本（4 control / 4 treat，列交错排列）
    ''' </summary>
    ''' <remarks>
    ''' 所有计数都 ≥ 1（DESeq2 的 size factor 只能使用"全样本都表达"的基因），
    ''' 并且有 30% 的基因带有处理组效应，保证 limma 有真实信号可检验。
    ''' </remarks>
    Private Function buildCounts() As Matrix
        Const nGene As Integer = 800
        Dim samples As String() = {
            "ctrl_1", "treat_1", "ctrl_2", "treat_2",
            "ctrl_3", "treat_3", "ctrl_4", "treat_4"
        }
        Dim isTreat As Boolean() = {False, True, False, True, False, True, False, True}
        ' 样本之间刻意给出不同的测序深度，用于检验文库大小校正
        Dim depth As Double() = {1.0, 1.4, 0.85, 1.2, 1.1, 1.55, 0.92, 1.3}

        Dim rnd As New Random(20261116)
        Dim rows As DataFrameRow() = New DataFrameRow(nGene - 1) {}

        For i As Integer = 0 To nGene - 1
            Dim baseline As Double = 60.0 + rnd.NextDouble() * 5000.0
            Dim effect As Double = If(i Mod 10 < 3, 2.0, 1.0)
            Dim values As Double() = New Double(samples.Length - 1) {}

            For j As Integer = 0 To samples.Length - 1
                Dim mu As Double = baseline * depth(j) * If(isTreat(j), effect, 1.0)
                ' 加一点乘性噪声，避免所有基因的比例完全相同
                Dim noise As Double = 0.85 + rnd.NextDouble() * 0.3
                Dim value As Double = std.Round(mu * noise)

                If value < 1 Then
                    value = 1
                End If

                values(j) = value
            Next

            rows(i) = New DataFrameRow With {
                .geneID = $"gene_{i + 1}",
                .experiments = values
            }
        Next

        Return New Matrix With {
            .tag = "r_crosscheck",
            .sampleID = samples,
            .expression = rows
        }
    End Function

    Private Function sampleGroups(samples As String()) As String()
        Return samples _
            .Select(Function(id) If(id.StartsWith("treat"), "treat", "control")) _
            .ToArray
    End Function

    ' ==================== 归一化 ====================

    Private Sub crossCheckNormalizations(counts As Matrix, outputDir As String)
        ' ---- 分位数归一化
        Dim quantile As Matrix = counts.QuantileNormalize

        Call compareMatrices(
            "quantile normalization (limma::normalizeBetweenArrays)",
            quantile,
            readMatrix(Path.Combine(outputDir, "norm_quantile.csv")),
            relative:=1.0E-09
        )

        ' ---- DESeq2
        Dim deseq2 As Matrix = counts.DESeq2Normalize

        Call compareMatrices(
            "DESeq2 median-of-ratios normalization",
            deseq2,
            readMatrix(Path.Combine(outputDir, "norm_deseq2.csv")),
            relative:=1.0E-09
        )

        ' ---- edgeR TMM 因子
        Dim tmm = counts.CalcTMMFactors
        Dim referenceFactors As Matrix = readMatrix(Path.Combine(outputDir, "norm_tmm_factors.csv"))
        Dim expectedRef As String = File.ReadAllText(Path.Combine(outputDir, "norm_tmm_ref.txt")).Trim()

        cases += 1

        Dim factorMismatch As Integer = 0
        Dim worst As Double = 0

        For j As Integer = 0 To tmm.normFactors.Length - 1
            Dim expect As Double = referenceFactors.expression(0).experiments(j)
            Dim actual As Double = tmm.normFactors(j)
            Dim deviation As Double = relativeDeviation(actual, expect)

            If deviation > worst Then
                worst = deviation
            End If
            If deviation > 1.0E-09 Then
                factorMismatch += 1
            End If
        Next

        Call check(
            factorMismatch = 0,
            $"edgeR TMM norm factors ({factorMismatch} mismatch(es), worst relative deviation {worst:R})"
        )
        Call check(
            expectedRef = tmm.referenceSampleIndex.ToString(),
            $"edgeR TMM reference column should be {expectedRef} but actually is {tmm.referenceSampleIndex}"
        )

        ' ---- edgeR CPM（有效文库大小）
        Call compareMatrices(
            "edgeR TMM normalized CPM (edgeR::cpm)",
            counts.EdgeRTMMNormalize,
            readMatrix(Path.Combine(outputDir, "norm_tmm_cpm.csv")),
            relative:=1.0E-09
        )

        ' ---- TPM.Normalize（按文库大小中位数缩放）
        Call compareMatrices(
            "TPM.Normalize (median library size scaling)",
            counts.Normalize,
            readMatrix(Path.Combine(outputDir, "norm_tpm.csv")),
            relative:=1.0E-09
        )
    End Sub

    ' ==================== HVG / 缩放 ====================

    Private Sub crossCheckHvgAndScaling(counts As Matrix, outputDir As String)
        ' ---- 逐基因方差
        Dim nGene As Integer = counts.size
        Dim nSample As Integer = counts.sample_count
        Dim raw(nGene - 1, nSample - 1) As Double

        For i As Integer = 0 To nGene - 1
            For j As Integer = 0 To nSample - 1
                raw(i, j) = counts.expression(i).experiments(j)
            Next
        Next

        Dim variances As Double() = HVG.GeneVariances(raw, nGene, degreeOfParallelism:=1)
        Dim referenceVariance As Matrix = readMatrix(Path.Combine(outputDir, "hvg_variance.csv"))
        Dim mismatches As Integer = 0
        Dim worst As Double = 0

        cases += 1

        For i As Integer = 0 To nGene - 1
            Dim expect As Double = referenceVariance.expression(i).experiments(0)
            Dim deviation As Double = relativeDeviation(variances(i), expect)

            If deviation > worst Then
                worst = deviation
            End If
            If deviation > 1.0E-08 Then
                mismatches += 1
            End If
        Next

        Call check(mismatches = 0, $"HVG per-gene variance ({mismatches} mismatch(es), worst relative deviation {worst:R})")

        ' ---- RelativeScale(max)
        Dim byMax As Matrix = counts.scaleEach(Function(row) row.RelativeScale(median:=False))

        Call compareMatrices(
            "ExpressionScale.RelativeScale(max)",
            byMax,
            readMatrix(Path.Combine(outputDir, "scale_max.csv")),
            relative:=1.0E-09
        )

        ' ---- RelativeScale(median)
        Dim byMedian As Matrix = counts.scaleEach(Function(row) row.RelativeScale(median:=True))

        Call compareMatrices(
            "ExpressionScale.RelativeScale(median)",
            byMedian,
            readMatrix(Path.Combine(outputDir, "scale_median.csv")),
            relative:=1.0E-09
        )

        ' ---- LogScale(base=2)
        Call compareMatrices(
            "Matrix.log(base=2) / ExpressionScale.LogScale",
            counts.log(2),
            readMatrix(Path.Combine(outputDir, "logscale2.csv")),
            relative:=1.0E-09,
            absolute:=1.0E-12
        )
    End Sub

    <System.Runtime.CompilerServices.Extension>
    Private Function scaleEach(counts As Matrix, transform As Func(Of DataFrameRow, DataFrameRow)) As Matrix
        Return New Matrix With {
            .tag = "scaled",
            .sampleID = counts.sampleID,
            .expression = counts.expression.Select(transform).ToArray
        }
    End Function

    ' ==================== limma ====================

    Private Sub crossCheckLimma(counts As Matrix, samples As String(), groups As String(), outputDir As String)
        ' 让 treat 成为 experiment 组：CreateDataGroups 按第一次出现的顺序排列分组
        Dim sampleInfo As New List(Of SampleInfo)

        For Each groupName As String In {"treat", "control"}
            For i As Integer = 0 To samples.Length - 1
                If groups(i) = groupName Then
                    Call sampleInfo.Add(New SampleInfo(samples(i), groupName))
                End If
            Next
        Next

        Dim design As New DataAnalysis(sampleInfo)
        Dim tables As LimmaTable() = counts.LmFit(design).ToArray
        Dim reference As Matrix = readMatrix(Path.Combine(outputDir, "limma.csv"))

        If reference Is Nothing Then
            Call check(False, "limma reference matrix is missing")
            Return
        End If

        Dim columns As String() = {"logFC", "AveExpr", "t", "P.Value", "adj.P.Val", "B"}
        Dim tolerances As Double() = {1.0E-08, 1.0E-08, 1.0E-06, 1.0E-06, 1.0E-06, 1.0E-05}
        Dim mismatches As Integer() = New Integer(columns.Length - 1) {}
        Dim worst As Double() = New Double(columns.Length - 1) {}

        cases += 1

        For Each table As LimmaTable In tables
            Dim rowIndex As Integer = -1

            For i As Integer = 0 To reference.size - 1
                If reference.expression(i).geneID = table.id Then
                    rowIndex = i
                    Exit For
                End If
            Next

            If rowIndex < 0 Then
                Call check(False, $"limma reference does not contain gene '{table.id}'")
                Continue For
            End If

            Dim values As Double() = {
                table.logFC, table.AveExpr, table.t, table.P_Value, table.adj_P_Val, table.B
            }

            For k As Integer = 0 To values.Length - 1
                Dim expect As Double = reference.expression(rowIndex).experiments(k)
                Dim deviation As Double = relativeDeviation(values(k), expect)

                If deviation > worst(k) Then
                    worst(k) = deviation
                End If
                If deviation > tolerances(k) Then
                    mismatches(k) += 1
                End If
            Next
        Next

        For k As Integer = 0 To columns.Length - 1
            Call DebugAudit.Mark($"limma.{columns(k)}.worst", worst(k))
            Call check(
                mismatches(k) = 0,
                $"limma {columns(k)} ({mismatches(k)}/{tables.Length} mismatch(es), worst relative deviation {worst(k):R})"
            )
        Next

        ' 中间量诊断：var.prior 是 B 统计量的唯一外部输入，单独比较便于定位偏差
        Dim diagnostics As Dictionary(Of String, Double) = readKeyValues(Path.Combine(outputDir, "limma_diag.txt"))

        If diagnostics.ContainsKey("var_prior") Then
            Dim seUnscaled As Double = std.Sqrt(1.0 / 4.0 + 1.0 / 4.0)
            Dim tstat As Double() = tables.Select(Function(t) t.t).ToArray
            Dim ourVarPrior As Double = LimmaStatistics.VarPrior(
                tstat,
                diagnostics("df_total"),
                seUnscaled,
                diagnostics("s2_prior"),
                diagnostics("df_prior")
            )

            Call DebugAudit.Mark(
                "limma.varPrior",
                $"VB={ourVarPrior:R} R={diagnostics("var_prior"):R} (s2.prior={diagnostics("s2_prior"):R}, df.prior={diagnostics("df_prior"):R})"
            )
            ' tmixture 内部要做 4 次 t 分布分位数反解，容差放宽到 1e-6
            Call check(
                relativeDeviation(ourVarPrior, diagnostics("var_prior")) < 1.0E-06,
                $"limma var.prior: VB={ourVarPrior:R} R={diagnostics("var_prior"):R}"
            )
        End If

        ' 结构一致性：显著基因数量
        Dim significant As Integer = tables.Count(Function(t) t.adj_P_Val < 0.05)
        Dim expectedSignificant As Integer = 0

        For i As Integer = 0 To reference.size - 1
            If reference.expression(i).experiments(4) < 0.05 Then
                expectedSignificant += 1
            End If
        Next

        Call check(
            significant = expectedSignificant,
            $"limma significant gene count: expected {expectedSignificant} but actually is {significant}"
        )
    End Sub

    ' ==================== 比较工具 ====================

    ''' <summary>
    ''' 逐值比较两个矩阵（按基因 id 与样本 id 对齐）
    ''' </summary>
    Private Sub compareMatrices(label As String,
                                actual As Matrix,
                                expected As Matrix,
                                relative As Double,
                                Optional absolute As Double = 0)

        cases += 1

        If expected Is Nothing Then
            Call check(False, $"{label}: reference matrix is missing")
            Return
        End If

        Dim expectedRows As New Dictionary(Of String, Dictionary(Of String, Double))

        For Each row As DataFrameRow In expected.expression
            Dim values As New Dictionary(Of String, Double)

            For j As Integer = 0 To expected.sample_count - 1
                values(expected.sampleID(j)) = row.experiments(j)
            Next

            expectedRows(row.geneID) = values
        Next

        Dim mismatches As Integer = 0
        Dim worst As Double = 0
        Dim sample As String = ""

        For Each row As DataFrameRow In actual.expression
            Dim expect As Dictionary(Of String, Double) = Nothing

            If Not expectedRows.TryGetValue(row.geneID, expect) Then
                mismatches += 1
                Continue For
            End If

            For j As Integer = 0 To actual.sample_count - 1
                Dim id As String = actual.sampleID(j)
                Dim expectValue As Double

                If Not expect.TryGetValue(id, expectValue) Then
                    mismatches += 1
                    Continue For
                End If

                Dim deviation As Double = relativeDeviation(row.experiments(j), expectValue)

                If deviation > worst Then
                    worst = deviation
                    sample = $"{row.geneID}/{id}: {row.experiments(j):R} vs {expectValue:R}"
                End If

                If deviation > relative AndAlso std.Abs(row.experiments(j) - expectValue) > absolute Then
                    mismatches += 1
                End If
            Next
        Next

        Call DebugAudit.Mark($"r.{label}.worst", worst)

        Call check(mismatches = 0, $"{label} ({mismatches} mismatch(es), worst relative deviation {worst:R} at {sample})")
    End Sub

    ''' <summary>
    ''' 相对偏差：<c>|a − b| / max(1, |b|)</c>，对 0 附近的数值退化为绝对偏差
    ''' </summary>
    Private Function relativeDeviation(actual As Double, expected As Double) As Double
        If Double.IsNaN(actual) AndAlso Double.IsNaN(expected) Then
            Return 0
        End If
        If Double.IsNaN(actual) OrElse Double.IsNaN(expected) Then
            Return Double.PositiveInfinity
        End If

        Return std.Abs(actual - expected) / std.Max(1.0, std.Abs(expected))
    End Function

    Private Function readKeyValues(path As String) As Dictionary(Of String, Double)
        Dim values As New Dictionary(Of String, Double)

        If Not File.Exists(path) Then
            Return values
        End If

        For Each line As String In File.ReadAllLines(path)
            Dim i As Integer = line.IndexOf("="c)

            If i <= 0 Then
                Continue For
            End If

            Dim value As Double
            Dim text As String = line.Substring(i + 1)

            ' R 写出来的 Inf/-Inf/NaN 需要单独处理
            Select Case text
                Case "Inf"
                    value = Double.PositiveInfinity
                Case "-Inf"
                    value = Double.NegativeInfinity
                Case "NaN"
                    value = Double.NaN
                Case Else
                    If Not Double.TryParse(text, Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture, value) Then
                        Continue For
                    End If
            End Select

            values(line.Substring(0, i)) = value
        Next

        Return values
    End Function

    Private Function readMatrix(path As String) As Matrix
        If Not File.Exists(path) Then
            Return Nothing
        End If

        Return Document.LoadMatrixDocument(path, DirectCast(Nothing, Index(Of String)), tqdm:=False)
    End Function

    ' ==================== 进程调用 ====================

    Private Function runRscript(rscript As String, script As String, inputDir As String, outputDir As String) As (exitCode As Integer, output As String)
        Dim info As New ProcessStartInfo(rscript) With {
            .UseShellExecute = False,
            .RedirectStandardOutput = True,
            .RedirectStandardError = True,
            .CreateNoWindow = True
        }

        Call info.ArgumentList.Add(script)
        Call info.ArgumentList.Add(inputDir)
        Call info.ArgumentList.Add(outputDir)

        Using process As Process = Process.Start(info)
            Dim stdout As String = process.StandardOutput.ReadToEnd()
            Dim stderr As String = process.StandardError.ReadToEnd()

            Call process.WaitForExit()

            Return (process.ExitCode, stdout & stderr)
        End Using
    End Function

    ''' <summary>
    ''' 从测试程序的工作目录向上查找项目内的资源文件
    ''' </summary>
    Private Function findUp(relativePath As String) As String
        Dim folder As New DirectoryInfo(AppContext.BaseDirectory)

        For level As Integer = 0 To 6
            If folder Is Nothing Then
                Exit For
            End If

            Dim candidate As String = Path.Combine(folder.FullName, relativePath)

            If File.Exists(candidate) Then
                Return candidate
            End If

            folder = folder.Parent
        Next

        Return Nothing
    End Function

End Module
