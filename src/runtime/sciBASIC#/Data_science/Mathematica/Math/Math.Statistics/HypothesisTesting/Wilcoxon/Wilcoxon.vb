Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports Microsoft.VisualBasic.Math.Statistics.OrderStatistics
Imports Microsoft.VisualBasic.Math.Statistics.SingleCell
Imports sysmath = System.Math

Namespace Hypothesis

    ''' <summary>
    ''' Wilcoxon 秩和检验（等价于 Mann-Whitney U 检验）：两样本一次性检验、逐基因批量检验、
    ''' 以及与"对照组序统计缓存"协作的增量检验。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 约定：<c>U(x, y)</c> 表示"以 <c>y</c> 为参照、<c>x</c> 的秩和"，
    ''' 即 <c>U + n1·n2/2</c> 是 <c>x</c> 在合并样本中的秩和。因此 <c>U(x, y) + U(y, x) = n1·n2</c>。
    ''' </para>
    ''' <para>
    ''' P 值的两种口径：
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description><b>正态近似</b>：<c>z = (U - n1n2/2) / sd</c>，
    ''' <c>sd = sqrt(tc · n1·n2·(N+1)/12)</c>，<c>tc = 1 - Σ(t³-t)/(N³-N)</c>；</description></item>
    ''' <item><description><b>精确法</b>：样本量之和不超过 <see cref="RankSumOptions.ExactThreshold"/>
    ''' 且无并列时，用生成函数 <c>∏(1-q^(n1+i))/(1-q^i)</c> 的系数枚举 U 的精确分布。</description></item>
    ''' </list>
    ''' <para>
    ''' 退化情形（全并列导致 <c>sd = 0</c>）返回 <c>Z = 0</c>、<c>P = 1</c>（"没有证据"），
    ''' 并在 <see cref="WilcoxonResult.Diagnostics"/> 中注明原因，不抛异常。
    ''' </para>
    ''' </remarks>
    Public Module Wilcoxon

        ''' <summary>
        ''' 两样本秩和检验
        ''' </summary>
        ''' <param name="x">实验组观测值</param>
        ''' <param name="y">对照组观测值</param>
        ''' <param name="opt">检验参数，缺省使用默认值</param>
        ''' <returns></returns>
        Public Function Test(x As Double(), y As Double(), Optional opt As RankSumOptions = Nothing) As WilcoxonResult
            If x Is Nothing Then Throw New ArgumentNullException(NameOf(x))
            If y Is Nothing Then Throw New ArgumentNullException(NameOf(y))
            If opt Is Nothing Then opt = RankSumOptions.Default()
            If x.Length = 0 OrElse y.Length = 0 Then
                Throw New ArgumentException($"both samples must be non-empty: n1={x.Length}, n2={y.Length}")
            End If

            ' 用对照组构建单基因序统计，复用与批量路径完全相同的 U 计算逻辑
            Dim control As ControlOrderStats = ControlOrderStats.FromTable(
                OrderStatisticsTable.CreateFromGenes({y}, opt.Precision))

            Dim core As RankSumCore = control.Core(0, x)
            Dim n1 As Integer = core.N1
            Dim n2 As Integer = core.N2
            Dim total As Double = n1 + n2
            Dim tieCorrection As Double = TieCorrectionFactor(core.PooledTie, total)
            Dim z As Double = ZValue(core.U, n1, n2, tieCorrection)
            Dim method As RankSumMethod = RankSumMethod.NormalApproximation
            Dim p As Double
            Dim diagnostics As String = Nothing

            If opt.ExactThreshold > 0 AndAlso (n1 + n2) <= opt.ExactThreshold AndAlso core.PooledTie = 0.0 Then
                method = RankSumMethod.Exact
                p = ExactPValue(core.U, n1, n2, opt.Alternative)
            ElseIf tieCorrection <= 0.0 Then
                ' 全部观测并列：秩和方差为 0，z 无定义
                p = 1.0
                z = 0.0
                diagnostics = "all observations are tied; the rank-sum variance is zero"
            Else
                p = PValueFromZ(z, opt.Alternative)
            End If

            Dim effect As Double = EffectSize(x, y, opt.EffectEpsilon)

            Return New WilcoxonResult With {
                .U = core.U,
                .Z = z,
                .PValue = p,
                .PAdjusted = p,
                .Effect = effect,
                .Significant = p < opt.Alpha,
                .Alternative = opt.Alternative,
                .Method = method,
                .TieCorrection = tieCorrection,
                .N1 = n1,
                .N2 = n2,
                .Diagnostics = diagnostics
            }
        End Function

        ''' <summary>
        ''' 只计算 Mann-Whitney U 统计量
        ''' </summary>
        ''' <param name="x">实验组</param>
        ''' <param name="y">对照组</param>
        ''' <returns></returns>
        Public Function U(x As Double(), y As Double()) As Double
            Return Test(x, y, Nothing).U
        End Function

        ''' <summary>
        ''' 只计算正态化 z 值（并列校正开启）
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <returns></returns>
        Public Function Z(x As Double(), y As Double()) As Double
            Return Test(x, y, Nothing).Z
        End Function

        ''' <summary>
        ''' 逐基因批量检验：以已缓存的对照组序统计为参照，检验一个实验组的所有基因。
        ''' </summary>
        ''' <param name="control">对照组序统计缓存</param>
        ''' <param name="sampleCpm">实验组的每细胞 CPM，形状为 [细胞, 基因]</param>
        ''' <param name="opt">检验参数</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 批量路径<b>固定使用带并列校正的正态近似</b>：逐基因做精确枚举在
        ''' "上万基因 × 数百细胞"的规模下不可行，而正态近似在该规模下的误差可以忽略。
        ''' 需要精确法请对单个基因调用 <see cref="Test"/>（并设置较小的样本量）。
        ''' </remarks>
        Public Function TestTable(control As ControlOrderStats,
                                  sampleCpm As Double()(),
                                  Optional opt As RankSumOptions = Nothing) As RankSumTable

            If control Is Nothing Then Throw New ArgumentNullException(NameOf(control))
            If sampleCpm Is Nothing Then Throw New ArgumentNullException(NameOf(sampleCpm))
            If opt Is Nothing Then opt = RankSumOptions.Default()

            Dim genes As Integer = control.GeneCount
            Dim cells As Integer = sampleCpm.Length

            For i As Integer = 0 To cells - 1
                If sampleCpm(i) Is Nothing OrElse sampleCpm(i).Length <> genes Then
                    Throw New ArgumentException(
                        $"sample row #{i} has {If(sampleCpm(i) Is Nothing, 0, sampleCpm(i).Length)} column(s) but {genes} is required",
                        NameOf(sampleCpm))
                End If
            Next

            Dim table As New RankSumTable(genes, opt)
            Dim meanCpm As Double() = control.MeanCpm
            Dim n2 As Integer = control.CellCount
            Dim total As Double = cells + n2
            Dim values As Double() = New Double(cells - 1) {}

            For g As Integer = 0 To genes - 1
                If meanCpm(g) <= opt.MinControlCpm Then
                    table.Eligible(g) = False
                    table.PValue(g) = Double.NaN
                    table.PAdjusted(g) = Double.NaN
                    table.Effect(g) = Double.NaN
                    Continue For
                End If

                table.Eligible(g) = True

                Dim sampleSum As Double = 0.0

                For i As Integer = 0 To cells - 1
                    Dim v As Double = sampleCpm(i)(g)

                    values(i) = v
                    sampleSum += v
                Next

                Dim core As RankSumCore = control.Core(g, values)
                Dim tc As Double = TieCorrectionFactor(core.PooledTie, total)
                Dim z As Double = ZValue(core.U, cells, n2, tc)

                table.U(g) = core.U
                table.Z(g) = z
                table.Method(g) = RankSumMethod.NormalApproximation
                table.Effect(g) = sysmath.Log((sampleSum / cells + opt.EffectEpsilon) /
                                              (meanCpm(g) + opt.EffectEpsilon)) / sysmath.Log(2.0)

                If tc <= 0.0 Then
                    table.PValue(g) = 1.0
                Else
                    table.PValue(g) = PValueFromZ(z, opt.Alternative)
                End If
            Next

            Call AdjustPValues(table, opt)

            Return table
        End Function

        ''' <summary>
        ''' 把 z 值按备择假设方向换算成 P 值
        ''' </summary>
        ''' <param name="z"></param>
        ''' <param name="alternative"></param>
        ''' <returns></returns>
        Public Function PValueFromZ(z As Double, Optional alternative As Hypothesis = Hypothesis.TwoSided) As Double
            If Double.IsNaN(z) Then Return Double.NaN

            Select Case alternative
                Case Hypothesis.Greater
                    Return NormalDistribution.CCDF(z)
                Case Hypothesis.Less
                    Return NormalDistribution.CDF(z)
                Case Else
                    Return NormalDistribution.TwoSidedPValue(z)
            End Select
        End Function

        ''' <summary>
        ''' 并列校正因子 <c>tc = 1 - pooledTie/(N³ - N)</c>
        ''' </summary>
        ''' <param name="pooledTie">合并后的 <c>Σ(t³ - t)</c></param>
        ''' <param name="total">合并样本量 <c>N = n1 + n2</c></param>
        ''' <returns>落在 <c>[0, 1]</c>；<c>N &lt; 2</c> 时返回 1</returns>
        Public Function TieCorrectionFactor(pooledTie As Double, total As Double) As Double
            Dim denominator As Double = total * total * total - total

            If denominator <= 0.0 Then Return 1.0

            Dim tc As Double = 1.0 - pooledTie / denominator

            If tc < 0.0 Then Return 0.0
            If tc > 1.0 Then Return 1.0

            Return tc
        End Function

        ''' <summary>
        ''' 由 U 与并列校正因子计算 z 值
        ''' </summary>
        ''' <param name="u"></param>
        ''' <param name="n1"></param>
        ''' <param name="n2"></param>
        ''' <param name="tieCorrection"></param>
        ''' <returns>方差退化时返回 0</returns>
        Public Function ZValue(u As Double, n1 As Integer, n2 As Integer, tieCorrection As Double) As Double
            Dim variance As Double = tieCorrection * n1 * n2 * (n1 + n2 + 1.0) / 12.0

            If variance <= 0.0 Then Return 0.0

            Return (u - n1 * n2 / 2.0) / sysmath.Sqrt(variance)
        End Function

        ''' <summary>
        ''' 效应量 <c>log2((mean(x) + eps)/(mean(y) + eps))</c>
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <param name="epsilon"></param>
        ''' <returns></returns>
        Public Function EffectSize(x As Double(), y As Double(), Optional epsilon As Double = 0.000000001) As Double
            Dim meanX As Double = Mean(x)
            Dim meanY As Double = Mean(y)

            Return sysmath.Log((meanX + epsilon) / (meanY + epsilon)) / sysmath.Log(2.0)
        End Function

        ''' <summary>
        ''' U 统计量的精确分布：返回 P 值
        ''' </summary>
        ''' <param name="u">观测到的 U（无并列时必为整数或半整数）</param>
        ''' <param name="n1"></param>
        ''' <param name="n2"></param>
        ''' <param name="alternative"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 用生成函数 <c>∏_{i=1..n2} (1 - q^(n1+i)) / (1 - q^i)</c> 的系数作为"取到该 U 值的组合数"，
        ''' 复杂度 <c>O(n1·n2·(n1·n2))</c>；只适合小样本（默认上限 20）。
        ''' </remarks>
        Public Function ExactPValue(u As Double, n1 As Integer, n2 As Integer, Optional alternative As Hypothesis = Hypothesis.TwoSided) As Double
            If n1 <= 0 OrElse n2 <= 0 Then Throw New ArgumentException("both sample sizes must be positive")

            Dim maxU As Integer = n1 * n2
            Dim previous As Double() = New Double(maxU) {}
            Dim current As Double() = New Double(maxU) {}

            previous(0) = 1.0

            For j As Integer = 1 To n2
                Call Array.Clear(current, 0, current.Length)

                For s As Integer = 0 To maxU
                    Dim v As Double = previous(s)

                    If s >= j Then v += current(s - j)
                    If s >= n1 + j Then v -= previous(s - (n1 + j))

                    current(s) = v
                Next

                Dim swap As Double() = previous

                previous = current
                current = swap
            Next

            Dim totalWays As Double = 0.0

            For s As Integer = 0 To maxU
                totalWays += previous(s)
            Next

            If totalWays <= 0.0 Then Return Double.NaN

            ' 无并列时 U 为整数；有并列会传入半整数，这里按四舍五入处理
            Dim observed As Integer = CInt(sysmath.Round(u))
            Dim doubleMean As Integer = n1 * n2
            Dim doubledDeviation As Integer = sysmath.Abs(2 * observed - doubleMean)
            Dim accumulator As Double = 0.0

            For s As Integer = 0 To maxU
                Dim include As Boolean

                Select Case alternative
                    Case Hypothesis.Greater
                        include = s >= observed
                    Case Hypothesis.Less
                        include = s <= observed
                    Case Else
                        include = sysmath.Abs(2 * s - doubleMean) >= doubledDeviation
                End Select

                If include Then accumulator += previous(s)
            Next

            Dim p As Double = accumulator / totalWays

            If p > 1.0 Then Return 1.0
            If p < 0.0 Then Return 0.0

            Return p
        End Function

        ''' <summary>
        ''' 对批量结果做多重校正（只校正参与检验的基因）
        ''' </summary>
        ''' <param name="table"></param>
        ''' <param name="opt"></param>
        Private Sub AdjustPValues(table As RankSumTable, opt As RankSumOptions)
            Dim genes As Integer = table.GeneCount
            Dim index As New List(Of Integer)(genes)
            Dim values As New List(Of Double)(genes)

            For g As Integer = 0 To genes - 1
                If Not table.Eligible(g) Then
                    table.PAdjusted(g) = Double.NaN
                    table.Significant(g) = False
                    Continue For
                End If

                Call index.Add(g)
                Call values.Add(table.PValue(g))
            Next

            If index.Count = 0 Then Return

            Dim adjusted As Double() = FalseDiscoveryRate.PValueAdjust(values, opt.AdjustMethod, index.Count)

            For i As Integer = 0 To index.Count - 1
                Dim g As Integer = index(i)

                table.PAdjusted(g) = adjusted(i)
                table.Significant(g) = adjusted(i) < opt.Alpha
            Next
        End Sub

        Private Function Mean(values As Double()) As Double
            If values Is Nothing OrElse values.Length = 0 Then Return 0.0

            Dim sum As Double = 0.0

            For i As Integer = 0 To values.Length - 1
                sum += values(i)
            Next

            Return sum / values.Length
        End Function

    End Module
End Namespace
