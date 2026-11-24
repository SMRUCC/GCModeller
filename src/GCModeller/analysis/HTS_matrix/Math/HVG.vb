Public Module HVG

    ''' <summary>
    ''' 逐基因的原始表达方差（基因过滤用）。
    ''' </summary>
    ''' <param name="raw">原始表达矩阵 <c>[基因, 样本]</c></param>
    ''' <param name="nGene">基因数量</param>
    ''' <param name="degreeOfParallelism">并行度：0 = 使用全部逻辑核心，1 = 串行</param>
    ''' <remarks>
    ''' 每个基因只写自己的 <c>variances(i)</c> 槽位：没有共享运行状态、也没有跨迭代的归约，
    ''' 因此按基因并行天然无竞争，且结果与线程调度无关（逐位可复现）。
    ''' 单遍扫描里同时累加 <c>Σx</c> 与 <c>Σx²</c>，方差取
    ''' <c>(Σx² − Σx²/n)/(count−1)</c>——这里只做基因排序，精度余量充足。
    ''' </remarks>
    Public Function GeneVariances(raw As Double(,), nGene As Integer, Optional degreeOfParallelism As Integer = 0) As Double()
        Dim nSample As Integer = raw.GetLength(1)
        Dim variances(nGene - 1) As Double

        Dim body As Action(Of Integer) =
            Sub(i)
                Dim sum As Double = 0.0
                Dim sumSq As Double = 0.0
                Dim count As Integer = 0

                For s As Integer = 0 To nSample - 1
                    Dim v As Double = raw(i, s)

                    If Not v.IsNaNImaginary Then
                        sum += v
                        sumSq += v * v
                        count += 1
                    End If
                Next

                If count > 1 Then
                    Dim variance As Double = (sumSq - sum * sum / count) / (count - 1)

                    If variance < 0.0 Then variance = 0.0

                    variances(i) = variance
                Else
                    variances(i) = 0.0
                End If
            End Sub

        Call RunGenes(body, nGene, degreeOfParallelism, nSample)

        Return variances
    End Function

    ''' <summary>
    ''' 按基因并行执行一个 per-gene 的动作。
    ''' </summary>
    ''' <param name="body">动作，参数是基因下标</param>
    ''' <param name="count">基因数量</param>
    ''' <param name="degreeOfParallelism">并行度：0 = 全部逻辑核心，1 = 串行</param>
    ''' <param name="workPerItem">每个基因的样本数，用于判断值不值得并行</param>
    ''' <remarks>
    ''' 每个基因只写自己的槽位（统计量、列），因此没有锁、没有归约、也没有
    ''' 「并行改变累加顺序」的问题——无论多少线程，写进数组的都是同样的值。
    ''' 小于 200 万个元素时直接串行：线程分区与调度的开销会盖过收益。
    ''' </remarks>
    Public Sub RunGenes(body As Action(Of Integer), count As Integer, degreeOfParallelism As Integer, Optional workPerItem As Integer = 0)
        If count <= 0 Then
            Return
        End If

        Dim work As Long = CLng(count) * If(workPerItem > 0, workPerItem, 1)

        If degreeOfParallelism = 1 OrElse count < 2 OrElse work < 2000000L Then
            For i As Integer = 0 To count - 1
                Call body(i)
            Next
        Else
            Dim options As New ParallelOptions()
            ' Parallel 必须全限定：Microsoft.VisualBasic.Parallel 会遮蔽同名类型
            options.MaxDegreeOfParallelism = If(degreeOfParallelism > 0, degreeOfParallelism, -1)
            Call Parallel.For(0, count, options, body)
        End If
    End Sub
End Module
