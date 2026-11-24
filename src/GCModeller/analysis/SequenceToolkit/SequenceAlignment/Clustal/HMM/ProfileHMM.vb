Imports System.Runtime.CompilerServices

Namespace Clustal

    ''' <summary>
    ''' 由一组已对齐的序列行构建的三态(match/insert/delete) profile HMM。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 发射概率由"序列加权频度 + Gonnet 背景先验 + 伪计数"平滑得到；空位罚分由每一列的
    ''' 空位频度反推得到位置特异的 open/extend 罚分，从而让"一个空位一旦出现就倾向于继续保留"
    ''' 的经典行为在 HMM 层面自然成立。
    ''' </para>
    ''' <para>
    ''' 与 HMMER/HHalign 的完整实现相比，本类型省略了插入状态的发射位置建模与
    ''' 二级结构信息，只保留 profile-profile 比对所必需的部分，避免把 O(M^2) 的状态空间
    ''' 展开成全转移矩阵。
    ''' </para>
    ''' </remarks>
    Public Class ProfileHMM

        ''' <summary>
        ''' profile 的列数(match 状态数)
        ''' </summary>
        Public ReadOnly Property Columns As Integer
        ''' <summary>
        ''' 字母表大小
        ''' </summary>
        Public ReadOnly Property AlphabetSize As Integer
        ''' <summary>
        ''' 每列 match 状态的发射概率，扁平的 Columns x A 数组
        ''' </summary>
        Public ReadOnly Property Emit As Double()
        ''' <summary>
        ''' 每列的空位占比 [0,1]
        ''' </summary>
        Public ReadOnly Property GapFraction As Double()
        ''' <summary>
        ''' 每列的位置特异空位开放罚分(bit，正数)
        ''' </summary>
        Public ReadOnly Property OpenBits As Double()
        ''' <summary>
        ''' 每列的位置特异空位延伸罚分(bit，正数)
        ''' </summary>
        Public ReadOnly Property ExtendBits As Double()
        ''' <summary>
        ''' 有效序列条数(Neff)
        ''' </summary>
        Public ReadOnly Property Neff As Double
        ''' <summary>
        ''' 原始行数
        ''' </summary>
        Public ReadOnly Property RowCount As Integer

        ''' <summary>
        ''' 背景残基频率
        ''' </summary>
        Public ReadOnly Property Background As Double()
        ''' <summary>
        ''' 构建 profile 所使用的替换矩阵
        ''' </summary>
        Public ReadOnly Property Matrix As ScoringMatrix

        ''' <summary>
        ''' M -> M 的转移概率(逐列)
        ''' </summary>
        Public ReadOnly Property M2M As Double()
        ''' <summary>
        ''' M -> I 的转移概率(逐列)
        ''' </summary>
        Public ReadOnly Property M2I As Double()
        ''' <summary>
        ''' M -> D 的转移概率(逐列)
        ''' </summary>
        Public ReadOnly Property M2D As Double()
        ''' <summary>
        ''' D -> M 的转移概率(逐列)
        ''' </summary>
        Public ReadOnly Property D2M As Double()
        ''' <summary>
        ''' D -> D 的转移概率(逐列)
        ''' </summary>
        Public ReadOnly Property D2D As Double()
        ''' <summary>
        ''' I -> M 的转移概率(逐列)
        ''' </summary>
        Public ReadOnly Property I2M As Double()
        ''' <summary>
        ''' I -> I 的转移概率(逐列)
        ''' </summary>
        Public ReadOnly Property I2I As Double()

        Private Sub New(columns As Integer, alphabetSize As Integer, matrix As ScoringMatrix,
                        emit As Double(), gapFraction As Double(), openBits As Double(), extendBits As Double(),
                        neff As Double, rowCount As Integer, background As Double(),
                        m2m As Double(), m2i As Double(), m2d As Double(),
                        d2m As Double(), d2d As Double(), i2m As Double(), i2i As Double())

            Me.Columns = columns
            Me.AlphabetSize = alphabetSize
            Me.Matrix = matrix
            Me.Emit = emit
            Me.GapFraction = gapFraction
            Me.OpenBits = openBits
            Me.ExtendBits = extendBits
            Me.Neff = neff
            Me.RowCount = rowCount
            Me.Background = background
            Me.M2M = m2m
            Me.M2I = m2i
            Me.M2D = m2d
            Me.D2M = d2m
            Me.D2D = d2d
            Me.I2M = i2m
            Me.I2I = i2i
        End Sub

        ''' <summary>
        ''' 空 profile(没有任何列)
        ''' </summary>
        Public Shared Function Empty(matrix As ScoringMatrix) As ProfileHMM
            Dim background As Double() = BackgroundFrequencies.ForAlphabet(matrix.Alphabet)
            Return New ProfileHMM(0, matrix.Size, matrix, New Double() {}, New Double() {},
                                  New Double() {}, New Double() {}, 0, 0, background,
                                  New Double() {}, New Double() {}, New Double() {},
                                  New Double() {}, New Double() {}, New Double() {}, New Double() {})
        End Function

        ''' <summary>
        ''' 由已对齐的序列行构建 profile HMM
        ''' </summary>
        ''' <param name="rows">已经补齐为等长的行(可以包含 <c>-</c>)</param>
        ''' <param name="matrix">替换矩阵</param>
        ''' <param name="options">流水线参数(用于换算空位罚分)</param>
        Public Shared Function FromAlignment(rows As String(), matrix As ScoringMatrix,
                                             Optional options As ClustalOmegaOptions = Nothing) As ProfileHMM

            options = If(options, New ClustalOmegaOptions())

            Dim n As Integer = If(rows Is Nothing, 0, rows.Length)

            If n = 0 Then
                Return Empty(matrix)
            End If

            Dim columns As Integer = rows(0).Length

            If columns = 0 Then
                Return Empty(matrix)
            End If

            Dim A As Integer = matrix.Size
            Dim alphabet As SequenceAlphabet = matrix.Alphabet
            Dim background As Double() = BackgroundFrequencies.ForAlphabet(alphabet)
            Dim gapChar As Char = "-"c

            ' 1. Henikoff 位置权重：高度相似的序列不应该主导发射概率
            Dim weights As Double() = PositionWeights(rows, alphabet)
            Dim weightSum As Double = 0

            For i As Integer = 0 To n - 1
                weightSum += weights(i)
            Next

            If weightSum <= 0 Then
                For i As Integer = 0 To n - 1
                    weights(i) = 1
                Next
                weightSum = n
            End If

            Dim neff As Double = weightSum

            ' 2. 加权频度统计
            Dim counts As Double() = New Double(columns * A - 1) {}
            Dim matchWeight As Double() = New Double(columns - 1) {}
            Dim openWeight As Double() = New Double(columns - 1) {}
            Dim extendWeight As Double() = New Double(columns - 1) {}
            Dim gapWeight As Double() = New Double(columns - 1) {}

            For r As Integer = 0 To n - 1
                Dim row As String = rows(r)
                Dim w As Double = weights(r)

                For c As Integer = 0 To columns - 1
                    Dim ch As Char = If(c < row.Length, row(c), gapChar)

                    If ch = gapChar OrElse ch = "."c Then
                        gapWeight(c) += w

                        Dim prevGap As Boolean = c > 0 AndAlso IsGap(row(c - 1))

                        If prevGap Then
                            extendWeight(c) += w
                        Else
                            openWeight(c) += w
                        End If
                    Else
                        Dim code As Integer = alphabet.CodeOf(ch)
                        counts(c * A + code) += w
                        matchWeight(c) += w
                    End If
                Next
            Next

            ' 3. 发射概率：频度 + Gonnet 背景先验 + 伪计数
            '    伪计数取 0.5 条序列的量级：太小会让单序列 profile 过于尖锐(过拟合)，
            '    太大会把分布抹平，使得 log-odds 打分失去区分度
            Dim pseudo As Double = 0.5R
            Dim emit As Double() = New Double(columns * A - 1) {}

            For c As Integer = 0 To columns - 1
                Dim total As Double = matchWeight(c) + pseudo

                For ai As Integer = 0 To A - 1
                    emit(c * A + ai) = (counts(c * A + ai) + pseudo * background(ai)) / total
                Next
            Next

            ' 4. 位置特异的空位罚分：由空位频度反推 p_open / p_extend
            Dim pOpenDefault As Double = Math.Pow(2.0, -options.GapOpenBits)
            Dim pExtendDefault As Double = Math.Pow(2.0, -options.GapExtendBits)
            Dim kOpen As Double = 1.0R
            Dim kExtend As Double = 1.0R
            Dim gapFraction As Double() = New Double(columns - 1) {}
            Dim openBits As Double() = New Double(columns - 1) {}
            Dim extendBits As Double() = New Double(columns - 1) {}

            Dim openLow As Double = options.GapOpenBits * 0.25R
            Dim openHigh As Double = options.GapOpenBits * 3.0R
            Dim extendLow As Double = options.GapExtendBits * 0.25R
            Dim extendHigh As Double = options.GapExtendBits * 3.0R

            For c As Integer = 0 To columns - 1
                Dim total As Double = matchWeight(c) + openWeight(c) + kOpen
                Dim pOpen As Double = (openWeight(c) + kOpen * pOpenDefault) / total

                gapFraction(c) = If(weightSum > 0, gapWeight(c) / weightSum, 0)

                Dim pExtendDen As Double = openWeight(c) + extendWeight(c) + kExtend
                Dim pExtend As Double = (extendWeight(c) + kExtend * pExtendDefault) / pExtendDen

                openBits(c) = Clamp(-Log2(Math.Max(pOpen, 1.0E-12)), openLow, openHigh)
                extendBits(c) = Clamp(-Log2(Math.Max(pExtend, 1.0E-12)), extendLow, extendHigh)
            Next

            ' 5. 转移概率(用于统计与外部消费，与 openBits/extendBits 保持一致)
            Dim m2m As Double() = New Double(columns - 1) {}
            Dim m2i As Double() = New Double(columns - 1) {}
            Dim m2d As Double() = New Double(columns - 1) {}
            Dim d2m As Double() = New Double(columns - 1) {}
            Dim d2d As Double() = New Double(columns - 1) {}
            Dim i2m As Double() = New Double(columns - 1) {}
            Dim i2i As Double() = New Double(columns - 1) {}

            For c As Integer = 0 To columns - 1
                Dim pOpen As Double = Math.Pow(2.0, -openBits(c))
                Dim pExt As Double = Math.Pow(2.0, -extendBits(c))

                m2d(c) = pOpen / 2.0R
                m2i(c) = pOpen / 2.0R
                m2m(c) = 1.0R - pOpen
                d2d(c) = pExt
                d2m(c) = 1.0R - pExt
                i2i(c) = pExtendDefault
                i2m(c) = 1.0R - pExtendDefault
            Next

            Return New ProfileHMM(columns, A, matrix, emit, gapFraction, openBits, extendBits,
                                  neff, n, background, m2m, m2i, m2d, d2m, d2d, i2m, i2i)
        End Function

        ''' <summary>
        ''' 某列某编码的发射概率
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Emission(column As Integer, code As Integer) As Double
            Return Emit(column * AlphabetSize + code)
        End Function

        Private Shared Function IsGap(ch As Char) As Boolean
            Return ch = "-"c OrElse ch = "."c
        End Function

        Private Shared Function Log2(x As Double) As Double
            Return Math.Log(x, 2.0)
        End Function

        Private Shared Function Clamp(x As Double, low As Double, high As Double) As Double
            If x < low Then
                Return low
            ElseIf x > high Then
                Return high
            Else
                Return x
            End If
        End Function

        ''' <summary>
        ''' Henikoff 位置权重：权重的和为序列条数，Neff 即为权重之和。
        ''' </summary>
        Private Shared Function PositionWeights(rows As String(), alphabet As SequenceAlphabet) As Double()
            Dim n As Integer = rows.Length
            Dim columns As Integer = rows(0).Length
            Dim weights As Double() = New Double(n - 1) {}

            If columns = 0 Then
                For i As Integer = 0 To n - 1
                    weights(i) = 1
                Next

                Return weights
            End If

            Dim distinctCount As Integer() = New Integer(columns - 1) {}
            Dim colCounts As Integer()() = New Integer(columns - 1)() {}

            For c As Integer = 0 To columns - 1
                Dim slot As Integer() = New Integer(alphabet.Size - 1) {}

                For r As Integer = 0 To n - 1
                    Dim row As String = rows(r)

                    If c < row.Length AndAlso Not IsGap(row(c)) Then
                        slot(alphabet.CodeOf(row(c))) += 1
                    End If
                Next

                colCounts(c) = slot

                Dim k As Integer = 0

                For si As Integer = 0 To slot.Length - 1
                    If slot(si) > 0 Then
                        k += 1
                    End If
                Next

                distinctCount(c) = Math.Max(1, k)
            Next

            For r As Integer = 0 To n - 1
                Dim row As String = rows(r)
                Dim w As Double = 0

                For c As Integer = 0 To columns - 1
                    If c < row.Length AndAlso Not IsGap(row(c)) Then
                        Dim code As Integer = alphabet.CodeOf(row(c))
                        Dim count As Integer = colCounts(c)(code)

                        If count > 0 Then
                            w += 1.0R / (distinctCount(c) * count)
                        End If
                    End If
                Next

                weights(r) = Math.Max(w, 0.0R)
            Next

            ' 归一化到和为 N，保证 Neff 落在 [1, N] 区间
            Dim total As Double = 0

            For i As Integer = 0 To n - 1
                total += weights(i)
            Next

            If total > 0 Then
                Dim scale As Double = n / total

                For i As Integer = 0 To n - 1
                    weights(i) *= scale
                Next
            End If

            Return weights
        End Function

        Public Overrides Function ToString() As String
            Return $"ProfileHMM(M={Columns}, rows={RowCount}, Neff={Math.Round(Neff, 2)})"
        End Function
    End Class
End Namespace
