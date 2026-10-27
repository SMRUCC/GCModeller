Imports Microsoft.VisualBasic.Math.Statistics
Imports std = System.Math

Namespace KOBAS

    ''' <summary>
    ''' 两组比较的统计量计算（均值、无偏方差、Welch t 检验）。
    ''' </summary>
    ''' <remarks>
    ''' p 值使用正则化不完全 Beta 函数精确计算：
    ''' <c>p = I_{df/(df+t²)}(df/2, 1/2)</c>，与 R 的 <c>t.test</c>（双尾）一致。
    ''' </remarks>
    Friend Module GroupStatistics

        ''' <summary>
        ''' 计算子集均值。
        ''' </summary>
        ''' <param name="values">完整的观测值。</param>
        ''' <param name="[subset]">子集下标。</param>
        ''' <returns></returns>
        Public Function Mean(values As Double(), [subset] As Integer()) As Double
            If [subset] Is Nothing OrElse [subset].Length = 0 Then
                Return 0
            End If

            Dim sum As Double = 0

            For Each i As Integer In [subset]
                sum += values(i)
            Next

            Return sum / [subset].Length
        End Function

        ''' <summary>
        ''' 计算子集的无偏方差（ddof=1）。
        ''' </summary>
        ''' <param name="values">完整的观测值。</param>
        ''' <param name="[subset]">子集下标。</param>
        ''' <param name="mean">子集均值。</param>
        ''' <returns></returns>
        Public Function Variance(values As Double(), [subset] As Integer(), mean As Double) As Double
            If [subset] Is Nothing OrElse [subset].Length < 2 Then
                Return 0
            End If

            Dim sum As Double = 0

            For Each i As Integer In [subset]
                Dim d As Double = values(i) - mean

                sum += d * d
            Next

            Return sum / ([subset].Length - 1)
        End Function

        ''' <summary>
        ''' 执行 Welch 两样本 t 检验（第二组相对第一组，双尾）。
        ''' </summary>
        ''' <param name="values">完整的观测值。</param>
        ''' <param name="group0">第一组（对照）的样本下标。</param>
        ''' <param name="group1">第二组（处理）的样本下标。</param>
        ''' <returns>元组：<c>T</c> 为 t 统计量、<c>DF</c> 为 Welch 自由度、<c>PValue</c> 为双尾 p 值。</returns>
        Public Function StudentT(values As Double(), group0 As Integer(), group1 As Integer()) As (T As Double, DF As Double, PValue As Double)
            Dim mean0 As Double = Mean(values, group0)
            Dim mean1 As Double = Mean(values, group1)
            Dim variance0 As Double = Variance(values, group0, mean0)
            Dim variance1 As Double = Variance(values, group1, mean1)
            Dim term0 As Double = variance0 / group0.Length
            Dim term1 As Double = variance1 / group1.Length
            Dim standardError As Double = std.Sqrt(term0 + term1)

            If standardError = 0 Then
                Return (0, 0, 1)
            End If

            Dim t As Double = (mean1 - mean0) / standardError
            Dim denominator As Double = 0

            If group0.Length > 1 Then
                denominator += term0 * term0 / (group0.Length - 1)
            End If
            If group1.Length > 1 Then
                denominator += term1 * term1 / (group1.Length - 1)
            End If

            Dim df As Double = If(denominator = 0, group0.Length + group1.Length - 2, (term0 + term1) * (term0 + term1) / denominator)
            Dim pvalue As Double

            If df <= 0 OrElse Double.IsNaN(t) Then
                pvalue = 1
            Else
                pvalue = SpecialFunctions.RegularizedIncompleteBetaFunction(df / 2, 0.5, df / (df + t * t))

                If Double.IsNaN(pvalue) Then
                    pvalue = 1
                Else
                    pvalue = std.Max(0.0, std.Min(1.0, pvalue))
                End If
            End If

            Return (t, df, pvalue)
        End Function
    End Module
End Namespace
