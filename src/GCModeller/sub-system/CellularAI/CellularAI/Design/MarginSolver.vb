Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Model
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.SingleCell
Imports std = System.Math

Namespace Design

    ''' <summary>边距求解的结果。</summary>
    Public Structure MarginSolution

        ''' <summary>边距 <c>m ≥ 0</c>。</summary>
        Public Margin As Double

        ''' <summary>是否真的把 <c>realized(m)</c> 打到目标上（相对误差 ≤ <c>SOLVE_RTOL</c>）。</summary>
        Public Reached As Boolean

    End Structure

    ''' <summary>幅度求解的结果。</summary>
    Public Structure AmplitudeSolution

        ''' <summary>幅度 <c>a</c>。</summary>
        Public Amplitude As Double

        ''' <summary>是否因为"即使 64 倍也不够"而被截断。</summary>
        Public Capped As Boolean

    End Structure

    ''' <summary>
    ''' 一维求根（边距 / 幅度）与控制侧的"信用"抽样。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>割线优先、二分兜底</b>：<c>realized(m)</c>（整数总量离散度）对边距近似线性，
    ''' 割线法通常两三次就命中；但它在"目标不可达"或"函数在某段平坦"时会退化，
    ''' 所以每一步都检查相对误差，失败后转成区间扩张 + 二分，并按 <c>2^SOLVE_STEPS</c> 设上限。
    ''' </para>
    ''' <para>
    ''' <b>不可达必须显式暴露</b>：目标超出上限时返回夹紧后的边距并置
    ''' <c>Reached = False</c>，由调用方在报告里标注——静默接受一个打不到的目标会让下游的
    ''' 信用校验（<c>credit_band</c>）失去意义。
    ''' </para>
    ''' <para>
    ''' <b>信用抽样</b>：<c>C_p</c> 是"一个扰动的真实剖面离散度"的样本，由对照细胞随机抽
    ''' <c>n_cells</c> 个后取 jackknife 矩得到。<c>c_hat</c> 是"我们合成出来的细胞"的同一个矩，
    ''' 信用取 <c>Σ_p w_p·E[min(Ĉ_p, C_p)]</c>——即"合成得越接近真实越好，但超过真实不加分"。
    ''' </para>
    ''' </remarks>
    Public Module MarginSolver

        ''' <summary>边距求解的相对容差。</summary>
        Public Const SolveRtol As Double = 0.0001

        ''' <summary>边距求解的迭代步数。</summary>
        Public Const SolveSteps As Integer = 12

        ''' <summary>
        ''' 求满足 <c>realized(m) = target</c> 的边距：先割线、再区间扩张 + 二分。
        ''' </summary>
        ''' <param name="realized"><c>m ↦ 实测离散度</c></param>
        ''' <param name="target">目标离散度</param>
        ''' <param name="start">初始步长（通常沿用上一轮的边距）</param>
        Public Function SolveMargin(realized As Func(Of Double, Double), target As Double, start As Double) As MarginSolution
            If target <= 0.0 Then
                Return New MarginSolution With {.Margin = 0.0, .Reached = False}
            End If

            Dim m0 As Double = If(start > 0.0, start, 1.0)
            Dim r0 As Double = realized(m0)
            Dim m1 As Double = If(r0 > 0.0, m0 * target / r0, -1.0)

            For iteration As Integer = 1 To SolveSteps
                If Not (m1 > 0.0) Then
                    Exit For
                End If

                Dim r1 As Double = realized(m1)

                If std.Abs(r1 / target - 1.0) <= SolveRtol Then
                    Return New MarginSolution With {.Margin = m1, .Reached = True}
                End If
                If r1 = r0 Then
                    Exit For
                End If

                Dim next1 As Double = m1 + (target - r1) * (m1 - m0) / (r1 - r0)

                m0 = m1
                r0 = r1
                m1 = next1
            Next

            ' 二分兜底：区间 [0, start]，必要时扩张到 start·2^SolveSteps
            Dim lo As Double = 0.0
            Dim hi As Double = m0

            If realized(lo) >= target Then
                Return New MarginSolution With {.Margin = lo, .Reached = False}
            End If

            Dim rhi As Double = realized(hi)

            While rhi < target
                If hi >= m0 * std.Pow(2.0, SolveSteps) Then
                    Return New MarginSolution With {.Margin = hi, .Reached = False}
                End If

                lo = hi
                hi *= 2.0
                rhi = realized(hi)
            End While

            For iteration As Integer = 1 To 4 * SolveSteps
                Dim mid As Double = 0.5 * (lo + hi)
                Dim r As Double = realized(mid)

                If std.Abs(r / target - 1.0) <= SolveRtol Then
                    Return New MarginSolution With {.Margin = mid, .Reached = True}
                End If

                If r < target Then
                    lo = mid
                Else
                    hi = mid
                    rhi = r
                End If
            Next

            Return New MarginSolution With {
                .Margin = hi,
                .Reached = std.Abs(rhi / target - 1.0) <= SolveRtol
            }
        End Function

        ''' <summary>
        ''' 求幅度：使 <c>spread(design(a))</c> 达到目标；上限 64 倍仍不够时返回 <c>Capped = True</c>。
        ''' </summary>
        ''' <param name="spread"><c>设计矩阵 ↦ 离散度</c></param>
        ''' <param name="design"><c>幅度 ↦ 设计矩阵</c></param>
        ''' <param name="target">目标离散度</param>
        Public Function BisectAmp(spread As Func(Of Double()(), Double),
                                  design As Func(Of Double, Double()()),
                                  target As Double) As AmplitudeSolution
            Dim lo As Double = 0.0
            Dim hi As Double = 1.0
            Dim s As Double = spread(design(hi))

            While s < target AndAlso hi < 64.0
                hi *= 2.0
                s = spread(design(hi))
            End While

            Dim capped As Boolean = s < target

            For iteration As Integer = 1 To 30
                Dim mid As Double = 0.5 * (lo + hi)

                If spread(design(mid)) < target Then
                    lo = mid
                Else
                    hi = mid
                End If
            Next

            Return New AmplitudeSolution With {.Amplitude = hi, .Capped = capped}
        End Function

        ''' <summary>
        ''' 控制侧的信用抽样：抽 <c>credit_draws</c> 组各 <c>n_cells</c> 个对照细胞，
        ''' 每组算一次 jackknife 矩，得到"扰动真实离散度"的样本。
        ''' </summary>
        ''' <param name="controls">对照细胞矩阵</param>
        ''' <param name="options">总量设计参数（提供抽样次数与种子）</param>
        ''' <param name="nCells">每组抽多少个细胞</param>
        Public Function ControlMoments(controls As CsrMatrix, options As TotalsOptions, nCells As Integer) As Double()
            Dim n As Integer = controls.RowDimension
            Dim draws(If(options.CreditDraws <= 0, -1, options.CreditDraws - 1)) As Double

            If n = 0 OrElse nCells <= 0 Then
                Return draws
            End If

            Dim rnd As New Random(options.CreditSeed)
            Dim replace As Boolean = n < nCells

            For d As Integer = 0 To draws.Length - 1
                Dim indices As Integer() = SampleIndices(rnd, n, nCells, replace)
                Dim block As CsrMatrix = controls.Slice(indices)

                draws(d) = CountStatistics.JackknifeMoment(block, 512, ProfileSpread.TS)
            Next

            Return draws
        End Function

        ''' <summary>
        ''' 期望信用：<c>Σ_p w_p·E[min(Ĉ_p, C_p)]</c>（<c>C_p</c> 取抽样样本的均值）。
        ''' </summary>
        ''' <param name="cHat">我们合成出来的细胞的逐扰动矩</param>
        ''' <param name="draws">真实侧的矩样本</param>
        ''' <param name="w">逐扰动权重 <c>1/G_p</c></param>
        Public Function ExpectedCredit(cHat As Double(), draws As Double(), w As Double()) As Double
            If draws Is Nothing OrElse draws.Length = 0 Then
                Return 0.0
            End If

            Dim total As Double = 0.0

            For Each draw As Double In draws
                Dim accumulated As Double = 0.0

                For p As Integer = 0 To cHat.Length - 1
                    accumulated += w(p) * std.Min(cHat(p), draw)
                Next

                total += accumulated
            Next

            Return total / draws.Length
        End Function

        ''' <summary>无放回（或按需有放回）抽样，返回<b>升序</b>下标。</summary>
        Public Function SampleIndices(rnd As Random, n As Integer, size As Integer, replace As Boolean) As Integer()
            Dim take As Integer = std.Min(size, n)
            Dim result(take - 1) As Integer

            If replace Then
                For i As Integer = 0 To take - 1
                    result(i) = rnd.Next(n)
                Next

                Return result
            End If

            ' 部分 Fisher–Yates：只打乱前 take 个位置
            Dim pool As Integer() = Enumerable.Range(0, n).ToArray

            For i As Integer = 0 To take - 1
                Dim j As Integer = rnd.Next(i, n)
                Dim swap As Integer = pool(i)

                pool(i) = pool(j)
                pool(j) = swap
                result(i) = pool(i)
            Next

            Call Array.Sort(result)

            Return result
        End Function

    End Module
End Namespace
