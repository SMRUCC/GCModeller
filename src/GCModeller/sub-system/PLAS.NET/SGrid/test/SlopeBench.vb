Imports System.Diagnostics
Imports System.Text
Imports SMRUCC.genomics.Analysis.SGrid.GRN
Imports std = System.Math

Namespace SelfTest

    ''' <summary>
    ''' 数值微分（<see cref="SlopeEstimator"/>）的并行加速比基准。
    ''' </summary>
    ''' <remarks>
    ''' 该入口用合成的平滑轨迹（正弦 + 噪声，规模默认与完整的表达矩阵一致：
    ''' 18179 个基因 × 121 个时间点）对比串行与并行两种模式的耗时，
    ''' 并校验两种模式的结果是否逐位一致（每个基因独立计算，理论上必须完全相同）。
    ''' </remarks>
    Public Module SlopeBench

        ''' <summary>
        ''' 执行基准，返回退出码（零表示通过）
        ''' </summary>
        ''' <param name="genes">合成的基因数量</param>
        ''' <param name="points">合成的时间点数量</param>
        ''' <param name="threads">并发上限，零表示使用运行时的默认值</param>
        ''' <returns></returns>
        Public Function Run(genes As Integer, points As Integer, threads As Integer) As Integer
            If genes < 8 Then
                genes = 8
            End If
            If points < 8 Then
                points = 8
            End If

            Dim series As ExpressionTimeSeries = synthetic(genes, points)
            Dim report As New StringBuilder()
            Dim failures As New List(Of String)

            Call report.AppendLine("=========== slope estimator benchmark ===========")
            Call report.AppendLine($"series : {series.ToString}, window=9")

            ' 预热（JIT 与线程池初始化不应该被计入对比）
            Call SlopeEstimator.Estimate(series, denoise:=True, smoothWindow:=9, parallel:=False)

            Dim serialSeconds As Double = 0
            Dim parallelSeconds As Double = 0
            Dim serial As Double()() = timed(series, parallel:=False, threads:=0, seconds:=serialSeconds)
            Dim concurrent As Double()() = timed(series, parallel:=True, threads:=threads, seconds:=parallelSeconds)

            Dim diff As Double = 0

            For i As Integer = 0 To serial.Length - 1
                For k As Integer = 0 To serial(i).Length - 1
                    diff = std.Max(diff, std.Abs(serial(i)(k) - concurrent(i)(k)))
                Next
            Next

            Dim speedup As Double = If(parallelSeconds > 0, serialSeconds / parallelSeconds, 0)
            Dim label As String = If(threads > 0, $"{threads} thread(s)", $"auto ({Environment.ProcessorCount} logical core(s))")

            Call report.AppendLine($"serial : {serialSeconds.ToString("F3")}s")
            Call report.AppendLine($"parallel: {parallelSeconds.ToString("F3")}s ({label}), speedup={speedup.ToString("F2")}x")
            Call report.AppendLine($"bitwise : max |serial - parallel| = {diff.ToString("G3")}")

            If diff <> 0 Then
                Call failures.Add("the parallel result differs from the serial one")
            End If

            Call report.AppendLine()
            Call report.AppendLine(If(failures.Count = 0, "PASS: the parallel result is identical to the serial one.", $"FAIL: {failures.Count} assertion(s) were violated:"))

            For Each fail As String In failures
                Call report.AppendLine($"  {fail}")
            Next

            Call Console.WriteLine(report.ToString)

            Return If(failures.Count = 0, 0, 1)
        End Function

        ''' <summary>
        ''' 计时一次求导
        ''' </summary>
        Private Function timed(series As ExpressionTimeSeries,
                               parallel As Boolean,
                               threads As Integer,
                               ByRef seconds As Double) As Double()()
            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim result As Double()() = SlopeEstimator.Estimate(
                series,
                denoise:=True,
                smoothWindow:=9,
                parallel:=parallel,
                threads:=threads)

            watch.Stop()
            seconds = watch.Elapsed.TotalSeconds

            Return result
        End Function

        ''' <summary>
        ''' 构造一批合成的平滑轨迹（正弦 + 噪声，保证会触发滑动平均）
        ''' </summary>
        ''' <param name="genes"></param>
        ''' <param name="points"></param>
        ''' <returns></returns>
        Private Function synthetic(genes As Integer, points As Integer) As ExpressionTimeSeries
            Dim t As Double() = New Double(points - 1) {}

            For k As Integer = 0 To points - 1
                t(k) = k
            Next

            Dim rnd As New Random(1234567)
            Dim names As String() = New String(genes - 1) {}
            Dim X As Double()() = New Double(genes - 1)() {}

            For i As Integer = 0 To genes - 1
                names(i) = $"GENE{i + 1}"

                Dim row As Double() = New Double(points - 1) {}
                Dim amplitude As Double = 0.5 + rnd.NextDouble()
                Dim phase As Double = rnd.NextDouble() * 3
                Dim frequency As Double = 0.05 + rnd.NextDouble() * 0.2

                For k As Integer = 0 To points - 1
                    row(k) = 2 + amplitude * std.Sin(frequency * k + phase) + (rnd.NextDouble() - 0.5) * 0.3
                Next

                X(i) = row
            Next

            Return New ExpressionTimeSeries With {
                .GeneNames = names,
                .OriginalNames = names,
                .TimePoints = t,
                .X = X
            }
        End Function

    End Module
End Namespace
