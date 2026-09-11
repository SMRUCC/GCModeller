Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Statistics.Distributions
Imports std = System.Math

Namespace Statistics

    ''' <summary>
    ''' 单条链（正链或负链）的滑动窗口分析结果。
    ''' </summary>
    Public Class StrandPValueResult

        ''' <summary>
        ''' 每个位置多窗口几何平均之后的 p 值；没有任何窗口贡献时为 <see cref="Double.NaN"/>。
        ''' </summary>
        Public Property PValue As Double()
        ''' <summary>
        ''' [+] 文库在该链上的原始 read 起始覆盖度。
        ''' </summary>
        Public Property PlusCoverage As Double()
        ''' <summary>
        ''' [-] 文库在该链上的原始 read 起始覆盖度。
        ''' </summary>
        Public Property MinusCoverage As Double()
        ''' <summary>
        ''' 该位置是否落入至少一个能够成功建模的滑动窗口。
        ''' </summary>
        Public Property SeenRegion As Boolean()
    End Class

    ''' <summary>
    ''' 滑动窗口核心算法：局部零膨胀 Poisson 背景估计 + Skellam 显著性检验。
    ''' </summary>
    ''' <remarks>
    ''' 本模块严格移植 ``Resources\TSSAR.pl`` 所生成的 R 脚本逻辑：
    ''' 
    ''' 1. 以 ``winSize`` 为窗口、``ceil(winSize/10)`` 为步长滑动；
    ''' 2. 窗口内分别对 [+] / [-] 文库执行 Winsorize 与零膨胀 Poisson 建模，
    '''    得到期望的结构零个数；任何一侧建模失败（MLE 不收敛）则整个窗口被丢弃；
    ''' 3. 取两侧期望结构零个数的平均值作为参考，按概率随机剔除各文库的结构零，
    '''    对剩余的"采样零 + 正计数"求均值得到 Skellam 的 lambda 参数，并做文库归一化；
    ''' 4. 逐位置计算计数差 ``D``，使用 Skellam 分布计算 ``P(X &gt;= D)`` 作为 p 值，
    '''    但仅记录 [+] 库原始计数不小于噪声阈值的位置；
    ''' 5. 每个位置被多个窗口覆盖，最终 p 值取这些窗口 p 值的几何平均。
    ''' </remarks>
    Public Module SlidingWindow

        ''' <summary>
        ''' 表示"未建模"的哨兵值，与 R 参考实现之中的 ``9999`` 一致。
        ''' </summary>
        Private Const NotModeled As Double = 9999.0

        Private Structure WindowFit
            Public Lambda As Double
            Public Expected As Double
            Public Modeled As Boolean
        End Structure

        ''' <summary>
        ''' 对单条链执行完整的滑动窗口 Skellam 分析。
        ''' </summary>
        ''' <param name="plus">[+] 文库该链的逐位置 read 起始覆盖度（1-based）。</param>
        ''' <param name="minus">[-] 文库该链的逐位置 read 起始覆盖度（1-based）。</param>
        ''' <param name="normalizePlus">[+] 文库的归一化因子。</param>
        ''' <param name="normalizeMinus">[-] 文库的归一化因子。</param>
        ''' <param name="genomeSize">基因组长度。</param>
        ''' <param name="winSize">滑动窗口大小。</param>
        ''' <param name="minPeakSize">噪声阈值。</param>
        ''' <param name="seed">结构零随机剔除所用的随机数种子。</param>
        Public Function Run(plus As Double(),
                            minus As Double(),
                            normalizePlus As Double,
                            normalizeMinus As Double,
                            genomeSize As Integer,
                            winSize As Integer,
                            minPeakSize As Integer,
                            Optional seed As Integer = 12345) As StrandPValueResult

            Dim stepSize As Integer = CInt(std.Ceiling(winSize / 10.0))

            If stepSize < 1 Then
                stepSize = 1
            End If
            If winSize < 1 Then
                winSize = 1
            End If

            Dim logSum As Double() = New Double(genomeSize) {}
            Dim contributions As Integer() = New Integer(genomeSize) {}
            Dim seen As Boolean() = New Boolean(genomeSize) {}

            ' 可复现的随机数：结构零的随机剔除
            RandomExtensions.SetSeed(seed)

            Dim pBuffer As Double() = New Double(winSize - 1) {}
            Dim mBuffer As Double() = New Double(winSize - 1) {}
            Dim ppBuffer As Double() = New Double(winSize - 1) {}
            Dim mmBuffer As Double() = New Double(winSize - 1) {}

            Dim pos As Integer = 1

            While pos < genomeSize
                Dim windowEnd As Integer = std.Min(genomeSize, pos + winSize - 1)
                Dim winLength As Integer = windowEnd - pos + 1

                Array.Copy(plus, pos, pBuffer, 0, winLength)
                Array.Copy(minus, pos, mBuffer, 0, winLength)

                Dim fitPlus As WindowFit = FitWindow(pBuffer, ppBuffer, winLength)
                Dim fitMinus As WindowFit = FitWindow(mBuffer, mmBuffer, winLength)

                If fitPlus.Modeled AndAlso fitMinus.Modeled Then
                    For i As Integer = 0 To winLength - 1
                        seen(pos + i) = True
                    Next

                    Dim expectedTotal As Double = (fitPlus.Expected + fitMinus.Expected) / 2.0

                    Dim lambdaPlus As Double = RemoveStructuralZeros(ppBuffer, winLength, expectedTotal) * normalizePlus
                    Dim lambdaMinus As Double = RemoveStructuralZeros(mmBuffer, winLength, expectedTotal) * normalizeMinus
                    Dim degenerate As Boolean = lambdaPlus = 0.0 AndAlso lambdaMinus = 0.0

                    For i As Integer = 0 To winLength - 1
                        Dim genomePosition As Integer = pos + i

                        ' 只有 [+] 库原始起始计数达到噪声阈值的位置才记录 p 值
                        If plus(genomePosition) >= minPeakSize Then
                            Dim pvalue As Double

                            If degenerate Then
                                pvalue = 1.0
                            Else
                                Dim difference As Double = pBuffer(i) * normalizePlus - mBuffer(i) * normalizeMinus
                                pvalue = 1.0 - Skellam.pskellam(difference - Skellam.DoubleXMin, lambdaPlus, lambdaMinus)
                            End If

                            If pvalue < 0.0 Then
                                pvalue = 0.0
                            ElseIf pvalue > 1.0 Then
                                pvalue = 1.0
                            End If

                            logSum(genomePosition) += std.Log(pvalue)
                            contributions(genomePosition) += 1
                        End If
                    Next
                Else
                    ' 窗口不可建模：全部位置贡献 p = 1（对数贡献为 0）
                    For i As Integer = 0 To winLength - 1
                        contributions(pos + i) += 1
                    Next
                End If

                pos += stepSize
            End While

            ' 模型收敛所产生的最末端位置必须被视为"已建模"
            seen(genomeSize) = True

            Dim pvalues As Double() = New Double(genomeSize) {}

            For i As Integer = 1 To genomeSize
                If contributions(i) > 0 Then
                    pvalues(i) = std.Exp(logSum(i) / contributions(i))
                Else
                    pvalues(i) = Double.NaN
                End If
            Next

            Return New StrandPValueResult With {
                .PValue = pvalues,
                .PlusCoverage = plus,
                .MinusCoverage = minus,
                .SeenRegion = seen
            }
        End Function

        ''' <summary>
        ''' 窗口内的零膨胀 Poisson 建模（对应 R 实现之中的三个分支）。
        ''' </summary>
        ''' <param name="source">窗口内的原始计数。</param>
        ''' <param name="winsorized">输出缓冲区：Winsorize 之后的计数。</param>
        ''' <param name="n">窗口内有效元素个数。</param>
        Private Function FitWindow(source As Double(), winsorized As Double(), n As Integer) As WindowFit
            Dim zeros As Integer = 0

            For i As Integer = 0 To n - 1
                If source(i) = 0.0 Then
                    zeros += 1
                End If
            Next

            Winsorize(source, winsorized, n)

            Dim fit As New WindowFit With {
                .Lambda = 0.0,
                .Expected = NotModeled,
                .Modeled = False
            }

            If zeros > 0 AndAlso zeros < n Then
                ' 既有结构零又有采样零/正计数：执行 ZIP 的 EM 极大似然估计
                Dim estimate As ZipEstimate = ZipRegression.Fit(winsorized, n)

                If Not estimate.Converged Then
                    Return fit
                End If

                fit.Lambda = estimate.Lambda
                fit.Expected = estimate.ExpectedStructuralZeros
                fit.Modeled = True
            ElseIf zeros = 0 Then
                ' 窗口内没有零：lambda 取 Winsorize 之后的均值
                Dim sum As Double = 0

                For i As Integer = 0 To n - 1
                    sum += winsorized(i)
                Next

                fit.Lambda = If(n > 0, sum / n, 0.0)
                fit.Expected = 0.0
                fit.Modeled = True
            Else
                ' 窗口内全为零
                fit.Lambda = 0.0
                fit.Expected = n
                fit.Modeled = True
            End If

            Return fit
        End Function

        ''' <summary>
        ''' Winsorize 缩尾处理：把最大值替换为第二大值、最小值替换为第二小值，
        ''' 以抵抗错配 read 或 rRNA 等高丰度离群值。
        ''' </summary>
        Private Sub Winsorize(source As Double(), destination As Double(), n As Integer)
            Array.Copy(source, destination, n)

            If n < 2 Then
                Return
            End If

            Dim sorted As Double() = New Double(n - 1) {}
            Array.Copy(source, sorted, n)
            Array.Sort(sorted)

            Dim maxIndex As Integer = 0
            Dim minIndex As Integer = 0

            For i As Integer = 1 To n - 1
                If source(i) > source(maxIndex) Then
                    maxIndex = i
                End If
                If source(i) < source(minIndex) Then
                    minIndex = i
                End If
            Next

            destination(maxIndex) = sorted(n - 2)
            destination(minIndex) = sorted(1)
        End Sub

        ''' <summary>
        ''' 按概率随机剔除窗口内的结构零，返回剩余"采样零 + 正计数"的均值，
        ''' 作为 Skellam 分布的 lambda 参数（尚未乘归一化因子）。
        ''' </summary>
        ''' <param name="winsorized">Winsorize 之后的窗口计数。</param>
        ''' <param name="n">窗口长度。</param>
        ''' <param name="expectedTotal">两侧文库期望结构零个数的平均值。</param>
        Private Function RemoveStructuralZeros(winsorized As Double(), n As Integer, expectedTotal As Double) As Double
            Dim zeros As Integer = 0

            For i As Integer = 0 To n - 1
                If winsorized(i) = 0.0 Then
                    zeros += 1
                End If
            Next

            If zeros = 0 Then
                Return 0.0
            End If

            ' 每一个零是结构零的概率（期望结构零个数 / 观测到的零个数）
            Dim structuralRatio As Double = expectedTotal / zeros

            Dim sum As Double = 0.0
            Dim count As Integer = 0

            For i As Integer = 0 To n - 1
                If winsorized(i) > 0.0 Then
                    sum += winsorized(i)
                    count += 1
                End If
            Next

            ' 零：以 structuralRatio 的概率被剔除（保留 rand >= structuralRatio 的采样零）
            For i As Integer = 0 To n - 1
                If winsorized(i) = 0.0 Then
                    If RandomExtensions.NextDouble(0.0, 1.0) >= structuralRatio Then
                        count += 1
                    End If
                End If
            Next

            If count = 0 Then
                Return 0.0
            End If

            Return sum / count
        End Function
    End Module
End Namespace
