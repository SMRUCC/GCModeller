Imports System.Runtime.CompilerServices

Namespace Clustal

    ''' <summary>
    ''' 两条 profile HMM 之间的 4 状态(MM/GD/DG/GG 的等价三矩阵形式)比对器。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 与 HHalign 同源的实现思路：先在 profile 之间计算位置特异的替换打分
    ''' (把替换矩阵按发射概率加权成对数几率)，再在对数空间做前向/后向动态规划求后验概率，
    ''' 最后在"后验加权"的路径空间里搜索最优路径。
    ''' </para>
    ''' <para>
    ''' 关于解码：HHalign 的 MAC 解码直接对后验和做最大化，这要求模型是严格归一化的概率模型；
    ''' 本实现保留了两端自由空位(与 clustalo 的终端空位处理一致)，此时纯后验和最大化会倾向
    ''' 沿着边界行走(边界格的 alpha 恒为 0，后验被抬高)。因此这里采用工程化的近似：
    ''' 路径搜索仍然使用真实的打分与仿射空位罚分，后验概率只作为每个格子的"可信度奖励"，
    ''' 用来在多个近似等价的路径之间挑出与整体后验最一致的那一条。
    ''' 当 <see cref="ClustalOmegaOptions.Decode"/> 为 <see cref="DecodeMode.Viterbi"/> 时
    ''' 完全不计算后验，等价于纯 Viterbi 解码。
    ''' </para>
    ''' </remarks>
    Public Class PairHMMAligner

        ''' <summary>
        ''' 对数空间中的负无穷哨兵值。所有比较与加法都会显式跳过它，
        ''' 避免出现"哨兵参与算术运算后反而变成最优路径"的错误。
        ''' </summary>
        Private Const NEG As Double = -1.0E+30

        ''' <summary>
        ''' 每个 DP 单元的平均内存占用(3 个 alpha + 3 个 beta 的 Double，加上 3 个量化的后验 Byte)
        ''' </summary>
        Private Const BytesPerCell As Double = 51.0R

        ''' <summary>
        ''' 后验概率对路径搜索的奖励权重(单位 bit，对应后验概率为 1 的格子)
        ''' </summary>
        Private Const PosteriorBonus As Double = 1.0R

        Private ReadOnly options As ClustalOmegaOptions

        Sub New(options As ClustalOmegaOptions)
            Me.options = options
        End Sub

        ''' <summary>
        ''' 一次 profile-profile 比对的统计数据
        ''' </summary>
        Public Class PairHMMStatistics
            ''' <summary>
            ''' DP 单元数量
            ''' </summary>
            Public Property Cells As Long
            ''' <summary>
            ''' 是否因为内存超限而退化为 Viterbi 解码
            ''' </summary>
            Public Property FallbackToViterbi As Boolean
            ''' <summary>
            ''' 前向算法的配分函数(对数尺度)
            ''' </summary>
            Public Property PartitionFunction As Double
            ''' <summary>
            ''' 估计的内存峰值(字节)
            ''' </summary>
            Public Property MemoryBytes As Long

            Public Overrides Function ToString() As String
                Return $"cells={Cells}, fallback={FallbackToViterbi}, Z={Math.Round(PartitionFunction, 3)}"
            End Function
        End Class

        ''' <summary>
        ''' 对齐两条 profile，返回按输出列顺序排列的 (A 的列, B 的列) 序列，
        ''' 其中 -1 表示该侧在此列上是空位。
        ''' </summary>
        Public Function Align(q As ProfileHMM, t As ProfileHMM,
                              Optional ByRef stats As PairHMMStatistics = Nothing) As (Integer, Integer)()

            Dim M1 As Integer = q.Columns
            Dim M2 As Integer = t.Columns

            If M1 = 0 AndAlso M2 = 0 Then
                Dim none As (Integer, Integer)() = {}

                stats = New PairHMMStatistics()
                Return none
            End If

            ' 退化路径：一侧为空，另一侧整体作为空位插入
            If M1 = 0 Then
                Dim onlyB(M2 - 1) As (Integer, Integer)

                For j As Integer = 0 To M2 - 1
                    onlyB(j) = (-1, j)
                Next

                stats = New PairHMMStatistics With {.Cells = M2}
                Return onlyB
            End If

            If M2 = 0 Then
                Dim onlyA(M1 - 1) As (Integer, Integer)

                For i As Integer = 0 To M1 - 1
                    onlyA(i) = (i, -1)
                Next

                stats = New PairHMMStatistics With {.Cells = M1}
                Return onlyA
            End If

            stats = New PairHMMStatistics With {.Cells = CLng(M1 + 1) * (M2 + 1)}
            stats.MemoryBytes = CLng(stats.Cells * BytesPerCell)

            Dim limit As Long = CLng(options.MacRamMb) * 1024L * 1024L
            Dim usePosterior As Boolean = options.Decode = DecodeMode.MaximumAccuracy AndAlso stats.MemoryBytes <= limit

            If Not usePosterior Then
                stats.FallbackToViterbi = options.Decode = DecodeMode.MaximumAccuracy
                Return PathAlign(q, t, Nothing, Nothing, Nothing, stats)
            Else
                Return AlignWithPosterior(q, t, stats)
            End If
        End Function

        ''' <summary>
        ''' 计算位置特异的替换打分矩阵(单位 bit)，结果按 A 的列优先展平
        ''' </summary>
        ''' <remarks>
        ''' <para>
        ''' 打分采用 HHalign 同源的"对数平均几率"(log-average of odds)形式：
        ''' </para>
        ''' <para>
        ''' <c>S(i,j) = sum_a pA(i,a) * log2( sum_b pB(j,b) * 2^M(a,b) )</c>
        ''' </para>
        ''' <para>
        ''' 其中 <c>M</c> 是替换矩阵的对数几率。之所以不能直接计算
        ''' <c>sum_a sum_b pA * pB * M</c> 这样的线性期望，是因为 profile 的发射概率带有伪计数，
        ''' 分布被"抹平"之后线性期望恒为负值，会让动态规划选择"完全不比"的退化路径。
        ''' 先对 B 的列做一次加权求和(预计算 logOdds)，之后每个 DP 单元只需要 O(A) 的点积。
        ''' </para>
        ''' </remarks>
        Private Shared Function SubstitutionScores(q As ProfileHMM, t As ProfileHMM) As Double()
            Dim alphaSize As Integer = q.AlphabetSize
            Dim M1 As Integer = q.Columns
            Dim M2 As Integer = t.Columns
            Dim matrix As ScoringMatrix = q.Matrix

            ' 预先换算几率比 ratio(a,b) = 2^M(a,b)，避免在热路径里反复调用 Math.Pow
            Dim ratio As Double() = New Double(alphaSize * alphaSize - 1) {}

            For a As Integer = 0 To alphaSize - 1
                For b As Integer = 0 To alphaSize - 1
                    ratio(a * alphaSize + b) = Math.Pow(2.0R, matrix.Scores(a * alphaSize + b))
                Next
            Next

            ' logOdds(j, a) = log2( sum_b pB(j, b) * ratio(a, b) )
            Dim logOdds As Double() = New Double(M2 * alphaSize - 1) {}

            For j As Integer = 0 To M2 - 1
                Dim baseB As Integer = j * alphaSize

                For ai As Integer = 0 To alphaSize - 1
                    Dim acc As Double = 0
                    Dim ratioRow As Integer = ai * alphaSize

                    For bi As Integer = 0 To alphaSize - 1
                        acc += t.Emit(baseB + bi) * ratio(ratioRow + bi)
                    Next

                    logOdds(baseB + ai) = Math.Log(Math.Max(acc, 1.0E-12), 2.0R)
                Next
            Next

            ' score(i, j) = sum_a pA(i, a) * logOdds(j, a)
            Dim score As Double() = New Double(M1 * M2 - 1) {}

            For i As Integer = 0 To M1 - 1
                Dim baseA As Integer = i * alphaSize
                Dim row As Integer = i * M2

                For j As Integer = 0 To M2 - 1
                    Dim baseB As Integer = j * alphaSize
                    Dim acc As Double = 0

                    For ai As Integer = 0 To alphaSize - 1
                        acc += q.Emit(baseA + ai) * logOdds(baseB + ai)
                    Next

                    score(row + j) = acc
                Next
            Next

            Return score
        End Function

        ''' <summary>
        ''' 前向 + 后向求后验概率，然后在后验加权的路径空间里解码
        ''' </summary>
        Private Function AlignWithPosterior(q As ProfileHMM, t As ProfileHMM,
                                            stats As PairHMMStatistics) As (Integer, Integer)()

            Dim M1 As Integer = q.Columns
            Dim M2 As Integer = t.Columns
            Dim W As Integer = M2 + 1
            Dim cells As Integer = (M1 + 1) * W
            Dim S As Double() = SubstitutionScores(q, t)

            Dim alphaM As Double() = New Double(cells - 1) {}
            Dim alphaX As Double() = New Double(cells - 1) {}
            Dim alphaY As Double() = New Double(cells - 1) {}

            Call Fill(alphaM, NEG)
            Call Fill(alphaX, NEG)
            Call Fill(alphaY, NEG)

            ' 首行/首列：终端空位按 EndGapScale 打折罚分
            Dim endOpen As Double = options.GapOpenBits * options.EndGapScale
            Dim endExt As Double = options.GapExtendBits * options.EndGapScale

            alphaM(0) = 0

            For j As Integer = 1 To M2
                alphaY(j) = -endOpen - (j - 1) * endExt
            Next

            For i As Integer = 1 To M1
                alphaX(i * W) = -endOpen - (i - 1) * endExt
            Next

            For i As Integer = 1 To M1
                Dim row As Integer = i * W
                Dim openA As Double = q.OpenBits(i - 1)
                Dim extA As Double = q.ExtendBits(i - 1)

                For j As Integer = 1 To M2
                    Dim k As Integer = row + j
                    Dim prev As Integer = (i - 1) * W + (j - 1)

                    Dim m As Double = LogAdd2(alphaM(prev), alphaX(prev), alphaY(prev))

                    If m > NEG / 2 Then
                        alphaM(k) = m + S((i - 1) * M2 + (j - 1))
                    End If

                    ' GD：A 的第 i 列对空位，B 停在 j 列
                    If j = M2 Then
                        alphaX(k) = LogAdd2(alphaM((i - 1) * W + j) - endOpen, alphaX((i - 1) * W + j) - endExt)
                    Else
                        alphaX(k) = LogAdd2(alphaM((i - 1) * W + j) - openA, alphaX((i - 1) * W + j) - extA)
                    End If

                    ' DG：B 的第 j 列对空位，A 停在 i 列
                    If i = M1 Then
                        alphaY(k) = LogAdd2(alphaM(k - 1) - endOpen, alphaY(k - 1) - endExt)
                    Else
                        alphaY(k) = LogAdd2(alphaM(k - 1) - t.OpenBits(j - 1), alphaY(k - 1) - t.ExtendBits(j - 1))
                    End If
                Next
            Next

            Dim last As Integer = M1 * W + M2
            Dim Z As Double = LogAdd2(alphaM(last), alphaX(last), alphaY(last))

            stats.PartitionFunction = Z

            If Z <= NEG / 2 Then
                stats.FallbackToViterbi = True
                Return PathAlign(q, t, Nothing, Nothing, Nothing, stats)
            End If

            ' 后向
            Dim betaM As Double() = New Double(cells - 1) {}
            Dim betaX As Double() = New Double(cells - 1) {}
            Dim betaY As Double() = New Double(cells - 1) {}

            Call Fill(betaM, NEG)
            Call Fill(betaX, NEG)
            Call Fill(betaY, NEG)

            betaM(last) = 0
            betaX(last) = 0
            betaY(last) = 0

            For i As Integer = M1 To 0 Step -1
                Dim row As Integer = i * W

                For j As Integer = M2 To 0 Step -1
                    If i = M1 AndAlso j = M2 Then
                        Continue For
                    End If

                    Dim k As Integer = row + j
                    Dim subScore As Double = If(i < M1 AndAlso j < M2, S(i * M2 + j), NEG)
                    Dim diag As Integer = (i + 1) * W + (j + 1)
                    Dim bm As Double = NEG
                    Dim bx As Double = NEG
                    Dim by As Double = NEG

                    If i < M1 AndAlso j < M2 Then
                        bm = subScore + betaM(diag)
                        bx = subScore + betaM(diag)
                        by = subScore + betaM(diag)
                    End If

                    If i < M1 Then
                        Dim penOpen As Double = If(j = M2, endOpen, q.OpenBits(i))
                        Dim penExt As Double = If(j = M2, endExt, q.ExtendBits(i))
                        Dim down As Integer = (i + 1) * W + j

                        bm = LogAdd2(bm, -penOpen + betaX(down))
                        bx = LogAdd2(bx, -penExt + betaX(down))
                    End If

                    If j < M2 Then
                        Dim penOpen As Double = If(i = M1, endOpen, t.OpenBits(j))
                        Dim penExt As Double = If(i = M1, endExt, t.ExtendBits(j))
                        Dim right As Integer = row + j + 1

                        bm = LogAdd2(bm, -penOpen + betaY(right))
                        by = LogAdd2(by, -penExt + betaY(right))
                    End If

                    betaM(k) = bm
                    betaX(k) = bx
                    betaY(k) = by
                Next
            Next

            ' 后验量化：共享一套 min/max，保证三个状态之间的相对大小不丢失
            Dim postM As Byte() = New Byte(cells - 1) {}
            Dim postX As Byte() = New Byte(cells - 1) {}
            Dim postY As Byte() = New Byte(cells - 1) {}
            Dim minValue As Double = Double.MaxValue
            Dim maxValue As Double = Double.MinValue

            For k As Integer = 0 To cells - 1
                Call TrackPosterior(alphaM, betaM, k, Z, minValue, maxValue)
                Call TrackPosterior(alphaX, betaX, k, Z, minValue, maxValue)
                Call TrackPosterior(alphaY, betaY, k, Z, minValue, maxValue)
            Next

            If maxValue <= minValue Then
                maxValue = minValue + 1.0R
            End If

            Dim span As Double = maxValue - minValue

            For k As Integer = 0 To cells - 1
                postM(k) = Quantize(alphaM, betaM, k, Z, minValue, span)
                postX(k) = Quantize(alphaX, betaX, k, Z, minValue, span)
                postY(k) = Quantize(alphaY, betaY, k, Z, minValue, span)
            Next

            ' 释放 alpha / beta，降低峰值内存
            alphaM = Nothing
            alphaX = Nothing
            alphaY = Nothing
            betaM = Nothing
            betaX = Nothing
            betaY = Nothing

            Return PathAlign(q, t, postM, postX, postY, stats)
        End Function

        Private Shared Sub TrackPosterior(alpha As Double(), beta As Double(), k As Integer,
                                          Z As Double, ByRef minValue As Double, ByRef maxValue As Double)

            If alpha(k) <= NEG / 2 OrElse beta(k) <= NEG / 2 Then
                Return
            End If

            Dim value As Double = alpha(k) + beta(k) - Z

            If value < minValue Then
                minValue = value
            End If
            If value > maxValue Then
                maxValue = value
            End If
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function Quantize(alpha As Double(), beta As Double(), k As Integer,
                                         Z As Double, minValue As Double, span As Double) As Byte

            If alpha(k) <= NEG / 2 OrElse beta(k) <= NEG / 2 Then
                Return 0
            End If

            Dim v As Double = (alpha(k) + beta(k) - Z - minValue) / span

            If v <= 0 Then
                Return 0
            ElseIf v >= 1 Then
                Return Byte.MaxValue
            Else
                Return CByte(Math.Round(v * 255.0R))
            End If
        End Function

        ''' <summary>
        ''' 路径解码：真实的替换打分 + 仿射空位罚分，后验概率(如果提供)作为每个格子的可信度奖励。
        ''' </summary>
        Private Function PathAlign(q As ProfileHMM, t As ProfileHMM,
                                   postM As Byte(), postX As Byte(), postY As Byte(),
                                   stats As PairHMMStatistics) As (Integer, Integer)()

            Dim M1 As Integer = q.Columns
            Dim M2 As Integer = t.Columns
            Dim W As Integer = M2 + 1
            Dim cells As Integer = (M1 + 1) * W
            Dim S As Double() = SubstitutionScores(q, t)

            Dim m As Double() = New Double(cells - 1) {}
            Dim x As Double() = New Double(cells - 1) {}
            Dim y As Double() = New Double(cells - 1) {}

            Call Fill(m, NEG)
            Call Fill(x, NEG)
            Call Fill(y, NEG)

            m(0) = 0

            For j As Integer = 1 To M2
                y(j) = 0
            Next

            For i As Integer = 1 To M1
                x(i * W) = 0
            Next

            For i As Integer = 1 To M1
                Dim row As Integer = i * W
                Dim openA As Double = q.OpenBits(i - 1)
                Dim extA As Double = q.ExtendBits(i - 1)

                For j As Integer = 1 To M2
                    Dim k As Integer = row + j
                    Dim prev As Integer = (i - 1) * W + (j - 1)
                    Dim bonusM As Double = If(postM Is Nothing, 0.0, postM(k) / 255.0R * PosteriorBonus)
                    Dim bonusX As Double = If(postX Is Nothing, 0.0, postX(k) / 255.0R * PosteriorBonus)
                    Dim bonusY As Double = If(postY Is Nothing, 0.0, postY(k) / 255.0R * PosteriorBonus)

                    m(k) = MaxOf(m(prev), x(prev), y(prev)) + S((i - 1) * M2 + (j - 1)) + bonusM

                    If j = M2 Then
                        x(k) = Math.Max(m((i - 1) * W + j), x((i - 1) * W + j)) + bonusX
                    Else
                        x(k) = Math.Max(m((i - 1) * W + j) - openA, x((i - 1) * W + j) - extA) + bonusX
                    End If

                    If i = M1 Then
                        y(k) = Math.Max(m(k - 1), y(k - 1)) + bonusY
                    Else
                        y(k) = Math.Max(m(k - 1) - t.OpenBits(j - 1), y(k - 1) - t.ExtendBits(j - 1)) + bonusY
                    End If
                Next
            Next

            Dim last As Integer = M1 * W + M2

            If postM Is Nothing Then
                stats.PartitionFunction = MaxOf(m(last), x(last), y(last))
            End If

            Dim state As Integer = ArgMax(m(last), x(last), y(last))
            Dim pairs As New List(Of (Integer, Integer))()
            Dim ci As Integer = M1
            Dim cj As Integer = M2

            While ci > 0 OrElse cj > 0
                Dim k As Integer = ci * W + cj

                ' 边界守卫：任意一侧已经走完时，路径只能沿另一侧的空位继续前进，
                ' 这样可以避免回溯到负索引的非法单元
                If ci = 0 Then
                    state = 2
                ElseIf cj = 0 Then
                    state = 1
                End If

                Select Case state
                    Case 0 ' MM
                        Call pairs.Add((ci - 1, cj - 1))

                        Dim prev As Integer = (ci - 1) * W + (cj - 1)
                        state = ArgMax(m(prev), x(prev), y(prev))
                        ci -= 1
                        cj -= 1
                    Case 1 ' GD
                        Call pairs.Add((ci - 1, -1))

                        Dim down As Integer = (ci - 1) * W + cj
                        state = If(x(down) >= m(down), 1, 0)
                        ci -= 1
                    Case Else ' DG
                        Call pairs.Add((-1, cj - 1))

                        Dim right As Integer = ci * W + cj - 1
                        state = If(y(right) >= m(right), 2, 0)
                        cj -= 1
                End Select
            End While

            Call pairs.Reverse()

            Return pairs.ToArray
        End Function

        ''' <summary>
        ''' 对数空间的加法：log2(2^a + 2^b)
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function LogAdd2(a As Double, b As Double) As Double
            If a <= NEG / 2 Then
                Return b
            ElseIf b <= NEG / 2 Then
                Return a
            End If

            Dim hi As Double = Math.Max(a, b)
            Dim lo As Double = Math.Min(a, b)

            Return hi + Math.Log(1.0R + Math.Pow(2.0R, lo - hi), 2.0R)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function LogAdd2(a As Double, b As Double, c As Double) As Double
            Return LogAdd2(LogAdd2(a, b), c)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function MaxOf(a As Double, b As Double, c As Double) As Double
            Return Math.Max(a, Math.Max(b, c))
        End Function

        Private Shared Sub Fill(data As Double(), value As Double)
            For i As Integer = 0 To data.Length - 1
                data(i) = value
            Next
        End Sub

        ''' <summary>
        ''' 取两个数值中较大者的索引(0 或 1)
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function ArgMax(a As Double, b As Double) As Integer
            Return If(a >= b, 0, 1)
        End Function

        ''' <summary>
        ''' 取三个数值中较大者的索引(0/1/2)
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function ArgMax(a As Double, b As Double, c As Double) As Integer
            If a >= b Then
                Return If(a >= c, 0, 2)
            Else
                Return If(b >= c, 1, 2)
            End If
        End Function
    End Class
End Namespace
