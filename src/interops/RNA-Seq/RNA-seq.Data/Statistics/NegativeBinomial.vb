' /********************************************************************************/
'
'  NegativeBinomial —— 负二项分布概率质量函数
'
'  忠实移植自 Rockhopper（Brian Tjaden, 2013）原始 Java 源码 Math_lib/NegativeBinomial.java。
'  原始实现基于 Catherine Loader (2000) 的鞍点近似算法：
'    "Fast and Accurate Computation of Binomial Probabilities".
'
'  已移除 Oracle.Java.* 等失效依赖，改用 .NET BCL 与 System.Math。
'  针对退化输入（r < 1、p 越界、k 越界）返回 0，避免产生 NaN。
'
' /********************************************************************************/

Imports System

Namespace RNA_Seq.Statistics

    ''' <summary>
    ''' 负二项分布概率质量函数。
    ''' 基于 Catherine Loader (2000) 的鞍点近似算法。
    ''' </summary>
    Public Module NegativeBinomial

        Private Const S0 As Double = 0.0833333333333333
        Private Const S1 As Double = 0.00277777777777778
        Private Const S2 As Double = 0.000793650793650794
        Private Const S3 As Double = 0.000595238095238095
        Private Const S4 As Double = 0.000841750841750842

        Private ReadOnly sfe As Double() = {
            0.0, 0.0810614667953273, 0.0413406959554093, 0.0276779256849983, 0.0207906721037651, 0.0166446911898212,
            0.0138761288230707, 0.0118967099458918, 0.0104112652619721, 0.00925546218271273, 0.00833056343336287,
            0.00757367548795184, 0.00694284010720953, 0.00640899418800421, 0.00595137011275885, 0.0055547335519628}

        ''' <summary>
        ''' 负二项分布概率质量函数（对应原 Rockhopper NegativeBinomial.pmf(k, r, p)）。
        ''' 对 r &lt; 1、p 越界、k 越界等退化输入返回 0，以规避 NaN。
        ''' </summary>
        ''' <param name="k">取值（原实现的 k 参数）。</param>
        ''' <param name="r">试验规模参数（原实现的 n 参数）。</param>
        ''' <param name="p">概率，须位于 [0, 1]。</param>
        Public Function PMF(k As Double, r As Double, p As Double) As Double
            If Double.IsNaN(k) OrElse Double.IsNaN(r) OrElse Double.IsNaN(p) Then Return 0.0
            If p < 0.0 OrElse p > 1.0 Then Return 0.0
            ' r < 1 属于退化输入，直接返回 0，避免 log / sqrt 产生 NaN
            If r < 1.0 Then Return 0.0
            If p = 0.0 Then Return If(k = 0.0, 1.0, 0.0)
            If p = 1.0 Then Return If(k = r, 1.0, 0.0)
            If k < 0.0 OrElse k > r Then Return 0.0
            If k = 0.0 Then Return Math.Exp(r * Math.Log(1.0 - p))
            If k = r Then Return Math.Exp(r * Math.Log(p))

            Dim lc As Double = stirlerr(r) - stirlerr(k) - stirlerr(r - k) - bd0(k, r * p) - bd0(r - k, r * (1.0 - p))
            ' 这里乘以 "p"
            Dim value As Double = p * Math.Exp(lc) * Math.Sqrt(r / (2.0 * Math.PI * k * (r - k)))

            If Double.IsNaN(value) OrElse Double.IsInfinity(value) Then Return 0.0
            Return value
        End Function

        ''' <summary>
        ''' log(n!) - log(sqrt(2*pi*n)*(n/e)^n)
        ''' </summary>
        Private Function stirlerr(n As Double) As Double
            If n < 16.0 Then
                Dim idx As Integer = CInt(Math.Truncate(n))
                If idx < 0 Then idx = 0
                If idx > sfe.Length - 1 Then idx = sfe.Length - 1
                Return sfe(idx)
            End If

            Dim nn As Double = n * n
            If n > 500.0 Then Return (S0 - S1 / nn) / n
            If n > 80.0 Then Return (S0 - (S1 / S2 / nn) / nn) / n
            If n > 35.0 Then Return (S0 - (S1 - (S2 - S3 / nn) / nn) / nn) / n
            Return (S0 - (S1 - (S2 - (S3 - S4 / nn) / nn) / nn) / nn) / n
        End Function

        ''' <summary>
        ''' 偏差项：k*log(k/np) + np - k
        ''' </summary>
        Private Function bd0(k As Double, np As Double) As Double
            If np <= 0.0 Then Return 0.0

            If Math.Abs(k - np) < 0.1 * (k + np) Then
                Dim s As Double = (k - np) * (k - np) / (k + np)
                Dim v As Double = (k - np) / (k + np)
                Dim ej As Double = 2.0 * k * v
                Dim j As Integer = 1

                While True
                    ej = ej * v * v
                    Dim s1 As Double = s + ej / (2 * j + 1)
                    If s1 = s Then Return s
                    s = s1
                    j += 1
                    ' 数值保护：避免极端输入下死循环
                    If j > 1000 Then Return s
                End While
            End If

            Return k * Math.Log(k / np) + np - k
        End Function

    End Module

End Namespace
