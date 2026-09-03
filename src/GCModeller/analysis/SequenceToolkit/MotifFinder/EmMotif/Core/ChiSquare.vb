' ============================================================================
' ChiSquare.vb — χ² 生存函数（正则化上不完全伽马）与 motif E-value
' ----------------------------------------------------------------------------
' [em.md §9] 关注 E-value 与 log-likelihood ratio。
' LLR 检验统计量：LLR = 2·Σ_sites Σ_k log(θ_{k,a}/θ_0,a)（软指派期望），
' 零假设下近似 χ²，自由度 df = (K−1)·W（每列 K−1 个自由参数）。
' E-value = 候选窗口总数 × p-value（保守近似；MEME 精确 E-value 基于词序统计，
' 本实现为文档化简化）。
' 不完全伽马：级数展开（x < s+1）+ 连分数（x ≥ s+1），Numerical Recipes 标准式，
' Python 镜像已对 χ²(1,2,4,10) 文献分位数验证至 5e-4。
' ============================================================================

Namespace EmMotif.Core

    Public Module ChiSquare

        ''' <summary>正则化上不完全伽马 Q(s, x)</summary>
        Public Function GammaQ(s As Double, x As Double) As Double
            If x < 0 OrElse s <= 0 Then Return 1.0
            If x = 0 Then Return 1.0
            If x < s + 1.0 Then
                ' 级数展开 P(s,x) → Q = 1 − P
                Dim ap = s
                Dim summ = 1.0 / s
                Dim delt = summ
                For i = 1 To 500
                    ap += 1.0
                    delt *= x / ap
                    summ += delt
                    If Math.Abs(delt) < Math.Abs(summ) * 0.000000000000001 Then Exit For
                Next
                Dim p = summ * Math.Exp(-x + s * Math.Log(x) - GammaLn(s))
                Return Math.Max(0.0, Math.Min(1.0, 1.0 - p))
            Else
                ' 连分数直接算 Q
                Dim b = x + 1.0 - s
                Dim c = 1.0E+300
                Dim d = 1.0 / b
                Dim h = d
                For i = 1 To 500
                    Dim an = -i * (i - s)
                    b += 2.0
                    d = an * d + b
                    If Math.Abs(d) < 1.0E-300 Then d = 1.0E-300
                    c = b + an / c
                    If Math.Abs(c) < 1.0E-300 Then c = 1.0E-300
                    d = 1.0 / d
                    Dim delt = d * c
                    h *= delt
                    If Math.Abs(delt - 1.0) < 0.000000000000001 Then Exit For
                Next
                Dim q = Math.Exp(-x + s * Math.Log(x) - GammaLn(s)) * h
                Return Math.Max(0.0, Math.Min(1.0, q))
            End If
        End Function

        ''' <summary>χ² 生存函数 P(X ≥ x)，df 自由度</summary>
        Public Function ChiSquareSf(df As Double, x As Double) As Double
            If x <= 0 Then Return 1.0
            Return GammaQ(df / 2.0, x / 2.0)
        End Function

        ''' <summary>log Γ(x)（Lanczos 近似）</summary>
        Public Function GammaLn(x As Double) As Double
            Dim g = 7.0
            Dim coef() As Double = {
                0.99999999999980993, 676.5203681218851, -1259.1392167224028,
                771.32342877765313, -176.61502916214059, 12.507343278686905,
                -0.13857109526572012, 0.0000099843695780195716, 0.00000015056327351493116}
            If x < 0.5 Then
                Return Math.Log(Math.PI / Math.Sin(Math.PI * x)) - GammaLn(1.0 - x)
            End If
            x -= 1.0
            Dim a = coef(0)
            Dim t = x + g + 0.5
            For i = 1 To 8
                a += coef(i) / (x + i)
            Next
            Return 0.5 * Math.Log(2.0 * Math.PI) + (x + 0.5) * Math.Log(t) - t + Math.Log(a)
        End Function

        ''' <summary>
        ''' E-value = 候选窗口总数 × χ²p 值。[em.md §9] E &lt; 0.05 视为显著。
        ''' </summary>
        Public Function MotifEValue(llr As Double, df As Double, totalWindows As Double) As Double
            Dim p = ChiSquareSf(df, Math.Max(0.0, llr))
            Dim e = totalWindows * p
            If e < 1.0E-300 Then e = 1.0E-300
            Return e
        End Function

    End Module

End Namespace
