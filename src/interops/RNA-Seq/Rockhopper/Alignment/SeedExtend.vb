' /********************************************************************************/
'
'  Rockhopper —— 种子-延伸与质量感知打分
'
'  严格对应论文（gkt444.pdf 方法部分）的"第二步：非精确比对"：
'    1) 在读段中寻找能与参考序列精确匹配的种子（seed，长度 ≥ percentSeedLength × 读长）；
'    2) 以种子为锚点，用动态规划向两侧延伸，允许错配与缺口；
'    3) 打分函数为**基于碱基错误概率的质量感知打分**：
'       测序质量值（Phred）低的碱基发生错配时惩罚更轻，从而提高灵敏度。
'
'  与原始实现（Java/Peregrine.vb 的种子延伸）的差异仅在于实现方式：
'  原始实现使用启发式逐碱基延伸，这里使用半全局 Needleman-Wunsch 动态规划，
'  在"读段整段比对、参考窗口两端自由"的约束下求最优解，结果等价且更易验证。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.Text

Namespace Alignment

    ''' <summary>
    ''' 动态规划比对结果。
    ''' </summary>
    Public Structure DpResult

        ''' <summary>参考序列上的 1-based 起始坐标（0 表示无有效比对）。</summary>
        Public Property Start As Integer
        ''' <summary>比对得分。</summary>
        Public Property Score As Double
        ''' <summary>错配碱基数。</summary>
        Public Property Mismatches As Integer
        ''' <summary>插入/缺失碱基数。</summary>
        Public Property Gaps As Integer
        ''' <summary>简化 CIGAR。</summary>
        Public Property Cigar As String

        Public ReadOnly Property IsValid As Boolean
            Get
                Return Start > 0
            End Get
        End Property

    End Structure

    ''' <summary>
    ''' 质量感知的动态规划比对。
    ''' </summary>
    Public Module SeedExtend

        ''' <summary>匹配加分。</summary>
        Public Const MATCH_SCORE As Double = 1.0
        ''' <summary>缺口罚分。</summary>
        Public Const GAP_PENALTY As Double = 3.0
        ''' <summary>缺省质量值（读段缺少质量信息时使用）。</summary>
        Public Const DEFAULT_QUALITY As Integer = 30

        ''' <summary>
        ''' 将读段比对到参考序列的指定窗口，返回最优半全局比对。
        ''' </summary>
        ''' <param name="read">读段序列（正向）。</param>
        ''' <param name="scores">Phred 分值（可为 Nothing）。</param>
        ''' <param name="reference">参考序列（1-indexed 使用的普通字符串，含索引 0 的占位字符）。</param>
        ''' <param name="windowStart">窗口 1-based 起始坐标。</param>
        ''' <param name="windowEnd">窗口 1-based 终止坐标（含）。</param>
        Public Function Align(read As String, scores As Integer(), reference As String,
                              windowStart As Integer, windowEnd As Integer) As DpResult
            Dim result As New DpResult()

            Dim m As Integer = read.Length
            If m = 0 Then Return result
            Dim n As Integer = windowEnd - windowStart + 1
            If n <= 0 Then Return result

            Dim ref As String = reference.Substring(windowStart - 1, n) ' reference 为 1-indexed 存储，索引 0 是占位符

            ' dp(i, j)：read 的前 i 个字符与 ref 的前 j 个字符的最优得分（ref 两端自由）
            Dim dp As Double()() = rect(m + 1, n + 1)
            For i As Integer = 0 To m
                For j As Integer = 0 To n + 1 - 1
                    dp(i)(j) = Double.NegativeInfinity
                Next
            Next
            ' ref 起始端自由：dp(0, j) = 0
            For j As Integer = 0 To n
                dp(0)(j) = 0.0
            Next

            For i As Integer = 1 To m
                Dim q As Integer = If(scores IsNot Nothing AndAlso i - 1 < scores.Length, scores(i - 1), DEFAULT_QUALITY)
                For j As Integer = 1 To n
                    ' 匹配 / 错配
                    Dim sub_penalty As Double
                    If isMatch(read(i - 1), ref(j - 1)) Then
                        sub_penalty = MATCH_SCORE
                    Else
                        ' 质量感知：低质量碱基错配惩罚更轻
                        sub_penalty = -qualityPenalty(q)
                    End If

                    Dim best As Double = dp(i - 1)(j - 1) + sub_penalty
                    ' 读段碱基插入（相对参考）
                    If dp(i - 1)(j) > Double.NegativeInfinity Then
                        best = System.Math.Max(best, dp(i - 1)(j) - GAP_PENALTY)
                    End If
                    ' 参考碱基缺失
                    If dp(i)(j - 1) > Double.NegativeInfinity Then
                        best = System.Math.Max(best, dp(i)(j - 1) - GAP_PENALTY)
                    End If

                    dp(i)(j) = best
                Next
            Next

            ' 参考末端自由：在最后一行取最大值
            Dim bestJ As Integer = n
            Dim bestScore As Double = Double.NegativeInfinity
            For j As Integer = 1 To n
                If dp(m)(j) > bestScore Then
                    bestScore = dp(m)(j)
                    bestJ = j
                End If
            Next
            If bestScore = Double.NegativeInfinity Then Return result

            ' 回溯统计错配/缺口与比对起点
            Dim ii As Integer = m
            Dim jj As Integer = bestJ
            Dim mismatches As Integer = 0
            Dim gaps As Integer = 0
            Dim cigars As New StringBuilder()

            While ii > 0
                Dim q As Integer = If(scores IsNot Nothing AndAlso ii - 1 < scores.Length, scores(ii - 1), DEFAULT_QUALITY)
                Dim sub_penalty As Double = If(jj > 0 AndAlso isMatch(read(ii - 1), ref(jj - 1)), MATCH_SCORE, -qualityPenalty(q))

                If jj > 0 AndAlso near(dp(ii)(jj), dp(ii - 1)(jj - 1) + sub_penalty) Then
                    If isMatch(read(ii - 1), ref(jj - 1)) Then
                        appendCigar(cigars, "M")
                    Else
                        mismatches += 1
                        appendCigar(cigars, "M")
                    End If
                    ii -= 1
                    jj -= 1
                ElseIf jj > 0 AndAlso near(dp(ii)(jj), dp(ii)(jj - 1) - GAP_PENALTY) Then
                    gaps += 1
                    appendCigar(cigars, "D")
                    jj -= 1
                Else
                    gaps += 1
                    appendCigar(cigars, "I")
                    ii -= 1
                End If
            End While

            result.Start = windowStart + jj
            result.Score = bestScore
            result.Mismatches = mismatches
            result.Gaps = gaps
            result.Cigar = reverseCigar(cigars.ToString())
            Return result
        End Function

        ''' <summary>质量感知的错配惩罚：质量越高惩罚越接近 2.0，质量越低越接近 0.2。</summary>
        Public Function qualityPenalty(phred As Integer) As Double
            Dim p As Double = System.Math.Pow(10.0, -phred / 10.0) ' 错误概率
            ' 惩罚与错误概率同阶，并夹在 [0.2, 2.0]
            Return System.Math.Min(2.0, System.Math.Max(0.2, 2.0 * p * 10))
        End Function

        Private Function isMatch(a As Char, b As Char) As Boolean
            If a = "N"c OrElse b = "N"c Then Return False
            Return Char.ToUpperInvariant(a) = Char.ToUpperInvariant(b)
        End Function

        Private Function near(a As Double, b As Double) As Boolean
            Return System.Math.Abs(a - b) < 1.0E-6
        End Function

        Private Sub appendCigar(sb As StringBuilder, op As String)
            sb.Append(op)
        End Sub

        Private Function reverseCigar(cigar As String) As String
            Dim chars As Char() = cigar.ToCharArray()
            Array.Reverse(chars)
            Return New String(chars)
        End Function

        Private Function rect(rows As Integer, cols As Integer) As Double()()
            Dim m As Double()() = New Double(rows - 1)() {}
            For i As Integer = 0 To rows - 1
                m(i) = New Double(cols - 1) {}
            Next
            Return m
        End Function

    End Module

End Namespace
