' ============================================================================
' LogSumExp.vb - 稳定的对数域运算
'
' 统一收口模型中所有 log / exp 的计算入口，取代散落在各模块里的
' "+ 0.0000001" 补丁（GMVAE.vb L380/L400/L507、GMM.vb L438/L450/L465）。
' ============================================================================

Imports std = System.Math

Namespace Core.Numerics

    ''' <summary>
    ''' 稳定的对数域运算工具。
    ''' </summary>
    Public Module LogSumExp

        ''' <summary>对数域中「零概率」的下界，用于避免 log(0)。</summary>
        Public Const TinyProbability As Double = 1.0E-300

        ''' <summary>exp 的安全上界，超过之后直接视为 +Infinity 前的最保险值。</summary>
        Public Const MaxExpArgument As Double = 700.0

        ''' <summary>
        ''' 计算 ln(Σ exp(values[start .. start+length-1]))。
        ''' 全部为 -Infinity 时返回 -Infinity，不会产生 NaN。
        ''' </summary>
        Public Function Compute(values As Double(), Optional start As Integer = 0, Optional length As Integer = -1) As Double
            If values Is Nothing Then
                Return Double.NegativeInfinity
            End If

            If length < 0 Then
                length = values.Length - start
            End If

            If length <= 0 Then
                Return Double.NegativeInfinity
            End If

            Dim maxValue As Double = Double.NegativeInfinity

            For i As Integer = start To start + length - 1
                If values(i) > maxValue Then
                    maxValue = values(i)
                End If
            Next

            If Double.IsNegativeInfinity(maxValue) Then
                Return Double.NegativeInfinity
            End If

            If Double.IsPositiveInfinity(maxValue) Then
                Return Double.PositiveInfinity
            End If

            Dim acc As Double = 0

            For i As Integer = start To start + length - 1
                acc += std.Exp(values(i) - maxValue)
            Next

            Return maxValue + std.Log(acc)
        End Function

        ''' <summary>
        ''' 计算二维数组指定行的 ln(Σ exp(row))。行内全部为 -Infinity 时返回 -Infinity。
        ''' </summary>
        Public Function ComputeRow(logValues As Double(,), row As Integer) As Double
            Dim columns As Integer = logValues.GetLength(1)
            Dim maxValue As Double = Double.NegativeInfinity

            For j As Integer = 0 To columns - 1
                If logValues(row, j) > maxValue Then
                    maxValue = logValues(row, j)
                End If
            Next

            If Double.IsNegativeInfinity(maxValue) Then
                Return Double.NegativeInfinity
            End If

            If Double.IsPositiveInfinity(maxValue) Then
                Return Double.PositiveInfinity
            End If

            Dim acc As Double = 0

            For j As Integer = 0 To columns - 1
                acc += std.Exp(logValues(row, j) - maxValue)
            Next

            Return maxValue + std.Log(acc)
        End Function

        ''' <summary>
        ''' 按行做 log-softmax：返回与输入同形的二维数组，每行的 exp 之和为 1。
        ''' </summary>
        Public Function LogSoftmaxRows(logValues As Double(,)) As Double(,)
            Dim rows As Integer = logValues.GetLength(0)
            Dim columns As Integer = logValues.GetLength(1)
            Dim result As Double(,) = New Double(rows - 1, columns - 1) {}

            For i As Integer = 0 To rows - 1
                Dim norm As Double = ComputeRow(logValues, i)

                For j As Integer = 0 To columns - 1
                    result(i, j) = logValues(i, j) - norm
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 带下界的 ln。用于出现过小概率时的兜底，而不是静默产生 -Infinity。
        ''' </summary>
        Public Function SafeLog(value As Double, Optional floor As Double = TinyProbability) As Double
            If Double.IsNaN(value) Then
                Return Double.NaN
            End If

            If value <= 0 Then
                Return std.Log(floor)
            End If

            Return std.Log(value)
        End Function

        ''' <summary>带上界的 exp，避免溢出为 +Infinity。</summary>
        Public Function SafeExp(value As Double) As Double
            If Double.IsNaN(value) Then
                Return Double.NaN
            End If

            If value > MaxExpArgument Then
                Return std.Exp(MaxExpArgument)
            End If

            Return std.Exp(value)
        End Function

        ''' <summary>
        ''' 把对数方差夹取到安全区间 [MinLogVariance, MaxLogVariance]。
        ''' </summary>
        Public Function ClipLogVariance(value As Double,
                                        Optional lower As Double = LinearAlgebraOps.MinLogVariance,
                                        Optional upper As Double = LinearAlgebraOps.MaxLogVariance) As Double
            If Double.IsNaN(value) Then
                Return 0
            End If

            Return std.Max(lower, std.Min(upper, value))
        End Function

        ''' <summary>数值稳定的 ln(1 + exp(x))。</summary>
        Public Function Log1pExp(x As Double) As Double
            If x > 0 Then
                Return x + std.Log(1.0 + std.Exp(-x))
            End If

            Return std.Log(1.0 + std.Exp(x))
        End Function

        ''' <summary>
        ''' 将一组未归一化的对数权重转换为概率（逐行 softmax），返回与输入同形数组。
        ''' </summary>
        Public Function SoftmaxRows(logValues As Double(,)) As Double(,)
            Dim rows As Integer = logValues.GetLength(0)
            Dim columns As Integer = logValues.GetLength(1)
            Dim result As Double(,) = New Double(rows - 1, columns - 1) {}

            For i As Integer = 0 To rows - 1
                Dim norm As Double = ComputeRow(logValues, i)

                For j As Integer = 0 To columns - 1
                    result(i, j) = std.Exp(logValues(i, j) - norm)
                Next
            Next

            Return result
        End Function

    End Module

End Namespace
