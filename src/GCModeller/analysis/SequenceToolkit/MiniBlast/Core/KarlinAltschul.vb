' ============================================================================
' KarlinAltschul.vb — Karlin-Altschul 极值分布统计参数
' ----------------------------------------------------------------------------
' 核心公式（对应 README 第五节）：
'   [式5-1]  E = K·m·n·e^(-λS)
'   [式5-2]  S' = (λS - ln K) / ln 2        （Bit Score）
'   [式5-3]  E = m·n·2^(-S')                （与 5-1/5-2 恒等，用于自检）
'
' 参数来源策略（与 NCBI blast_stat.c 预计算表同思路）：
'   1. 核酸：λ 由 F(λ) = Σ_v prob(v)·e^(λv) = 1 数值精确求解（均匀背景），
'      K 以 (2,-3) 系统的文献锚点值 0.41 为基准，其他 reward/penalty 组合
'      按首达概率常数 C 的比值缩放（启发式，文档化）。
'   2. 蛋白矩阵：λ/K 直接内嵌 NCBI 文献表值：
'      BLOSUM62 λ=0.3176 K=0.1341（高置信）
'      BLOSUM45 λ=0.3795 K=0.15
'      BLOSUM80 λ=0.3430  K=0.1938
'      PAM250   λ=0.2291  K=0.0931
'      H 由数值式 H = λ·Σ v·prob(v)·e^(λv) 计算（仅信息展示用）。
' ============================================================================

Imports System
Imports System.Collections.Generic

Namespace MiniBlast.Core

    ''' <summary>Karlin-Altschul 参数集</summary>
    Public Class KaParams

        Public Property Lambda As Double
        Public Property K As Double
        Public Property H As Double

        ''' <summary>[式5-1] E = K·m·n·e^(-λS)</summary>
        Public Function EValue(rawScore As Double, searchSpace As Double) As Double
            Return K * searchSpace * Math.Exp(-Lambda * rawScore)
        End Function

        ''' <summary>[式5-1] m、n 分列的便捷重载（searchSpace = m·n）</summary>
        Public Function EValue(m As Double, n As Double, rawScore As Double) As Double
            Return K * m * n * Math.Exp(-Lambda * rawScore)
        End Function

        ''' <summary>[式5-2] Bit Score</summary>
        Public Function BitScore(rawScore As Double) As Double
            Return (Lambda * rawScore - Math.Log(K)) / Math.Log(2.0)
        End Function

    End Class

    Public Module KarlinAltschul

        ''' <summary>Robinson-Robinson 氨基酸背景频率（与 AaAlphabet.Std20 对齐）</summary>
        Public ReadOnly AaBackground() As Double =
            {0.074, 0.052, 0.045, 0.054, 0.013, 0.043, 0.047, 0.057, 0.024, 0.068,
             0.099, 0.058, 0.025, 0.05, 0.04, 0.069, 0.059, 0.013, 0.033, 0.066}

        ''' <summary>氨基酸矩阵得分直方图（RR 背景频率）</summary>
        Public Function BuildAaHist(scorer As AaScorer) As SortedDictionary(Of Integer, Double)
            Return BuildHist(Function(a As Int32, b As Int32) scorer.Score(a, b), 20, AaBackground)
        End Function

        ''' <summary>核酸 match/mismatch 得分直方图（均匀背景 0.25）</summary>
        Public Function BuildNtHist(reward As Double, penalty As Double) As SortedDictionary(Of Integer, Double)
            Return BuildHist(Function(a As Int32, b As Int32) If(a <= 3 AndAlso a = b, reward, penalty),
                             4, {0.25, 0.25, 0.25, 0.25})
        End Function

        ''' <summary>构建得分直方图：prob(v) = Σ_{(i,j): s(i,j)=v} p_i·p_j</summary>
        Public Function BuildHist(scoreFn As Func(Of Int32, Int32, Double),
                                  alphabetSize As Integer,
                                  freq() As Double) As SortedDictionary(Of Integer, Double)
            Dim hist As New SortedDictionary(Of Integer, Double)()
            For i As Integer = 0 To alphabetSize - 1
                For j As Integer = 0 To alphabetSize - 1
                    Dim v = CInt(scoreFn(i, j))
                    Dim p = freq(i) * freq(j)
                    If hist.ContainsKey(v) Then
                        hist(v) += p
                    Else
                        hist(v) = p
                    End If
                Next
            Next
            Return hist
        End Function

        ''' <summary>
        ''' λ 数值精确解：F(λ) = Σ prob(v)·e^(λv) = 1 的唯一正根（F 凸，二分）。
        ''' </summary>
        Public Function SolveLambda(hist As SortedDictionary(Of Integer, Double)) As Double
            Dim lo = 0.000001, hi = 1.0

            Dim evalF = Function(lam As Double)
                            Dim s As Double = 0
                            For Each kv In hist
                                s += kv.Value * Math.Exp(lam * kv.Key)
                            Next
                            Return s
                        End Function

            While evalF(hi) < 1.0
                hi *= 2.0
                If hi > 1000000.0 Then Throw New InvalidOperationException("λ 求解发散")
            End While

            For i As Integer = 1 To 200
                Dim mid = 0.5 * (lo + hi)
                If evalF(mid) < 1.0 Then
                    lo = mid
                Else
                    hi = mid
                End If
                If hi - lo < 0.00000000000001 Then Exit For
            Next
            Return 0.5 * (lo + hi)
        End Function

        ''' <summary>相对熵 H = λ·Σ v·prob(v)·e^(λv)</summary>
        Public Function SolveH(hist As SortedDictionary(Of Integer, Double), lambda As Double) As Double
            Dim s As Double = 0
            For Each kv In hist
                s += kv.Key * kv.Value * Math.Exp(lambda * kv.Key)
            Next
            Return lambda * s
        End Function

        ''' <summary>
        ''' 首达概率常数 C = lim P(max walk ≥ S)·e^(λS)（截断格点 DP，精确）。
        ''' 用于不同打分系统之间 K 值的比值缩放。
        ''' </summary>
        Public Function FirstPassageC(hist As SortedDictionary(Of Integer, Double), lambda As Double) As Double
            Dim scores(hist.Count - 1) As Integer
            Dim probs(hist.Count - 1) As Double
            Dim idx = 0
            For Each kv In hist
                scores(idx) = kv.Key : probs(idx) = kv.Value : idx += 1
            Next

            Dim maxAbs = 1
            For Each s In scores
                maxAbs = Math.Max(maxAbs, Math.Abs(s))
            Next
            Dim bigS = CInt(Math.Ceiling(30.0 / lambda)) + maxAbs
            Dim b = CInt(Math.Ceiling(30.0 / lambda)) + maxAbs
            Dim size = b + bigS

            ' f[k] = P(reach ≥ bigS | at k)，k ∈ [-b, bigS-1]，k ≥ bigS → 1
            Dim f(size - 1) As Double

            For iteration As Integer = 1 To 20000
                Dim delta As Double = 0
                For k As Integer = bigS - 1 To -b Step -1
                    Dim arrIdx = k + b
                    Dim acc As Double = 0
                    For t As Integer = 0 To scores.Length - 1
                        Dim nk = k + scores(t)
                        If nk >= bigS Then
                            acc += probs(t)
                        Else
                            Dim ni = nk + b
                            If ni >= 0 Then acc += probs(t) * f(ni)
                        End If
                    Next
                    Dim d = Math.Abs(acc - f(arrIdx))
                    If d > delta Then delta = d
                    f(arrIdx) = acc
                Next
                If delta < 0.000000000000001 Then Exit For
            Next

            Return f(0 + b) * Math.Exp(lambda * bigS)
        End Function

        ''' <summary>核酸打分系统参数：λ 精确解 + K 锚点比值缩放</summary>
        Public Function NtParams(reward As Double, penalty As Double) As KaParams
            Dim hist = BuildHist(Function(a As Int32, b As Int32) If(a <= 3 AndAlso a = b, reward, penalty), 4, {0.25, 0.25, 0.25, 0.25})
            Dim lam = SolveLambda(hist)

            ' 锚点：(2,-3) 系统的 K 文献值 0.41
            Dim histAnchor = BuildHist(Function(a As Int32, b As Int32) If(a <= 3 AndAlso a = b, 2.0, -3.0), 4, {0.25, 0.25, 0.25, 0.25})
            Dim lamAnchor = SolveLambda(histAnchor)
            Dim cAnchor = FirstPassageC(histAnchor, lamAnchor)
            Dim c = FirstPassageC(hist, lam)
            Dim k = 0.41 * c / cAnchor

            Return New KaParams With {.Lambda = lam, .K = k, .H = SolveH(hist, lam)}
        End Function

        ''' <summary>蛋白矩阵参数：内嵌 NCBI 文献表值</summary>
        Public Function ProteinParams(matrixName As String) As KaParams
            Dim lam As Double, k As Double
            Select Case matrixName.ToUpperInvariant()
                Case "BLOSUM62" : lam = 0.3176 : k = 0.1341
                Case "BLOSUM45" : lam = 0.3795 : k = 0.15
                Case "BLOSUM80" : lam = 0.343 : k = 0.1938
                Case "PAM250" : lam = 0.2291 : k = 0.0931
                Case Else
                    Throw New ArgumentException($"矩阵 '{matrixName}' 无统计参数表")
            End Select
            ' H 用 RR 背景频率数值计算（信息展示用）
            Dim scorer As New AaScorer(matrixName)
            Dim hist = BuildHist(Function(a As Int32, b As Int32) scorer.Score(a, b), 20, AaBackground)
            Return New KaParams With {.Lambda = lam, .K = k, .H = SolveH(hist, lam)}
        End Function

        ''' <summary>组成校正（comp_based_stats=1 简化实现）：
        ''' 以查询/命中组成重算 λ（K 保持不变），抑制偏组成命中。</summary>
        Public Function AdjustedParams(scorer As AaScorer, queryCodes As Int32(),
                                       hitCodes As Int32(), baseParams As KaParams) As KaParams
            Dim qFreq(19) As Double
            Dim hFreq(19) As Double
            Dim qTotal = 0, hTotal = 0
            For Each c In queryCodes
                If c < 20 Then qFreq(c) += 1 : qTotal += 1
            Next
            For Each c In hitCodes
                If c < 20 Then hFreq(c) += 1 : hTotal += 1
            Next
            If qTotal < 20 OrElse hTotal < 20 Then Return baseParams
            For i As Integer = 0 To 19
                qFreq(i) /= qTotal
                hFreq(i) /= hTotal
            Next
            ' 查询组成 × 命中组成的二维直方图
            Dim hist2 As New SortedDictionary(Of Integer, Double)()
            For i As Integer = 0 To 19
                For j As Integer = 0 To 19
                    Dim v = CInt(scorer.Score(i, j))
                    Dim p = qFreq(i) * hFreq(j)
                    If hist2.ContainsKey(v) Then hist2(v) += p Else hist2(v) = p
                Next
            Next
            Dim lam = SolveLambda(hist2)
            ' λ 比值限幅，避免极端组成导致 E 值失真
            lam = Math.Max(0.5 * baseParams.Lambda, Math.Min(1.5 * baseParams.Lambda, lam))
            Return New KaParams With {.Lambda = lam, .K = baseParams.K,
                                      .H = SolveH(hist2, lam)}
        End Function

    End Module

End Namespace
