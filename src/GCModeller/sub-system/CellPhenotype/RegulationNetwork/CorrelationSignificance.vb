Imports Microsoft.VisualBasic.Math.Correlations

Namespace RegulationNetwork

    ''' <summary>
    ''' 相关显著性检验与多证据置信度融合
    ''' </summary>
    ''' <remarks>
    ''' 相关性的显著性由学生 t 检验给出：
    ''' <code>
    ''' t = r · sqrt( df / (1 - r²) ),  df = n - 2
    ''' p = I_{ df / (df + t²) }( df/2, 1/2 )
    ''' </code>
    ''' 其中 I 为不完全 beta 函数（等价于 t 分布的双尾概率）。
    '''
    ''' 置信度融合把量纲不同的证据统一到 0-1：
    ''' 共表达强度 |r|、显著性 1 - q/α、偏相关衰减折扣、STRING combined_score 归一化、无向候选折扣。
    ''' </remarks>
    Public Module CorrelationSignificance

        ''' <summary>
        ''' 相关系数的双尾显著性 p 值
        ''' </summary>
        ''' <param name="r">相关系数</param>
        ''' <param name="n">样本数（有效样本数）</param>
        ''' <returns>p 值；样本数不足或计算失败时返回 1（最保守）</returns>
        Public Function PValue(r As Double, n As Integer) As Double
            If n <= 3 OrElse Double.IsNaN(r) Then
                Return 1.0
            End If

            Dim df As Double = n - 2
            Dim rr As Double = r * r

            If rr >= 1.0 Then
                rr = 1.0 - 0.000000000001
            End If
            If rr <= 0.0 Then
                Return 1.0
            End If

            Dim t2 As Double = rr * df / (1.0 - rr)
            Dim x As Double = df / (df + t2)

            If Double.IsNaN(x) OrElse x <= 0.0 OrElse x > 1.0 Then
                Return 1.0
            End If

            Try
                Dim p As Double = Beta.betai(0.5 * df, 0.5, x, throwMaxIterError:=False)

                If Double.IsNaN(p) OrElse Double.IsInfinity(p) Then
                    Return 1.0
                End If

                Return Clamp01(p)
            Catch ex As Exception
                Return 1.0
            End Try
        End Function

        ''' <summary>
        ''' 批量计算相关显著性的 p 值
        ''' </summary>
        ''' <param name="r">相关系数数组</param>
        ''' <param name="n">样本数</param>
        ''' <returns>与输入等长的 p 值数组</returns>
        Public Function PValues(r As Double(), n As Integer) As Double()
            Dim p(r.Length - 1) As Double

            For i As Integer = 0 To r.Length - 1
                p(i) = PValue(r(i), n)
            Next

            Return p
        End Function

        ''' <summary>
        ''' Benjamini-Hochberg FDR 校正
        ''' </summary>
        ''' <param name="pvalues">原始 p 值</param>
        ''' <returns>与输入顺序一致、单调的 q 值数组；空输入返回空数组</returns>
        Public Function BH(pvalues As Double()) As Double()
            If pvalues Is Nothing OrElse pvalues.Length = 0 Then
                Return {}
            End If

            ' 顺序无关的朴素实现：rank 相同的 p 值共享同一个 q 值
            Dim n As Integer = pvalues.Length
            Dim order As Integer() = Enumerable.Range(0, n) _
                .OrderBy(Function(i) pvalues(i)) _
                .ToArray()
            Dim q(n - 1) As Double
            Dim prev As Double = 1.0

            For rank As Integer = order.Length - 1 To 0 Step -1
                Dim idx As Integer = order(rank)
                Dim value As Double = pvalues(idx) * n / (rank + 1)

                If value < prev Then prev = value
                q(idx) = If(prev > 1.0, 1.0, prev)
            Next

            Return q
        End Function

        ''' <summary>
        ''' 由共表达强度与显著性计算表达证据的置信度
        ''' </summary>
        ''' <param name="absCor">相关绝对值（0-1）</param>
        ''' <param name="qvalue">BH 校正后的 FDR</param>
        ''' <param name="fdrThreshold">FDR 阈值，用于把 q 值映射为显著因子</param>
        ''' <returns>0-1 的表达证据置信度</returns>
        Public Function ExpressionConfidence(absCor As Double, qvalue As Double, fdrThreshold As Double) As Double
            Dim base As Double = Clamp01(System.Math.Abs(absCor))
            Dim alpha As Double = If(fdrThreshold > 0, fdrThreshold, 0.05)
            Dim q As Double = If(Double.IsNaN(qvalue), 1.0, Clamp01(qvalue))
            Dim sig As Double = Clamp01(1.0 - q / alpha)

            Return Clamp01(0.7 * base + 0.3 * sig)
        End Function

        ''' <summary>
        ''' 把 STRING 证据融合进已有的表达证据置信度
        ''' </summary>
        ''' <param name="confExpr">表达证据置信度（0-1）</param>
        ''' <param name="stringScore">STRING combined_score（0-1000）；0 或负数表示无 PPI 支持</param>
        ''' <param name="weight">融合权重 w（0 表示不做加权）</param>
        ''' <param name="minScore">STRING 分数的有效下限</param>
        ''' <returns>融合后的置信度（0-1）</returns>
        Public Function MixString(confExpr As Double, stringScore As Double, weight As Double, minScore As Double) As Double
            Dim conf As Double = Clamp01(confExpr)

            If stringScore <= 0 OrElse weight <= 0 Then
                Return conf
            End If

            Dim norm As Double = NormalizeStringScore(stringScore, minScore)
            Dim w As Double = If(weight > 1.0, 1.0, weight)

            Return Clamp01((1.0 - w) * conf + w * norm)
        End Function

        ''' <summary>
        ''' 仅由 STRING 产生的边的置信度（无共表达支持，符号未知）
        ''' </summary>
        ''' <param name="stringScore">STRING combined_score（0-1000）</param>
        ''' <param name="minScore">STRING 分数的有效下限</param>
        ''' <returns>0-1 的置信度；上限 0.5，保证纯 PPI 边不会超过有表达支持的边</returns>
        Public Function StringOnlyConfidence(stringScore As Double, minScore As Double) As Double
            If stringScore <= 0 Then
                Return 0.0
            End If

            Return Clamp01(0.5 * NormalizeStringScore(stringScore, minScore))
        End Function

        ''' <summary>
        ''' STRING combined_score 归一化到 0-1
        ''' </summary>
        ''' <param name="score">combined_score（0-1000）</param>
        ''' <param name="minScore">有效下限，低于该值视为无证据</param>
        ''' <returns></returns>
        Public Function NormalizeStringScore(score As Double, minScore As Double) As Double
            If score <= minScore Then
                Return 0.0
            End If

            Dim span As Double = 1000.0 - minScore

            If span <= 0 Then
                Return Clamp01(score / 1000.0)
            End If

            Return Clamp01((score - minScore) / span)
        End Function

        ''' <summary>
        ''' 把任意数值钳制到 [0,1]
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Public Function Clamp01(x As Double) As Double
            If Double.IsNaN(x) Then
                Return 0.0
            End If
            If x < 0.0 Then
                Return 0.0
            End If
            If x > 1.0 Then
                Return 1.0
            End If

            Return x
        End Function
    End Module
End Namespace
