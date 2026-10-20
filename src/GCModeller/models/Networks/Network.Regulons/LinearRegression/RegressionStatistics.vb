Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports std = System.Math

Namespace LinearRegression

    ''' <summary>
    ''' 线性回归模型的显著性检验工具
    ''' 
    ''' 由于 DataFittings 之中的多元线性回归模型只给出了回归系数与拟合优度，
    ''' 这里补充 OLS 模型常用的 t 检验，用于给每一条调控边附上一个显著性 p 值。
    ''' </summary>
    Friend Module RegressionStatistics

        ''' <summary>
        ''' t 分布累积概率的数值积分在这个数量级之上已经不再可靠
        ''' </summary>
        Const MaxTabulatedT As Double = 100.0

        ''' <summary>
        ''' 计算总平方和 ``SST = sum((y - mean(y))^2)``
        ''' </summary>
        ''' <param name="y"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function TotalSumOfSquares(y As Double()) As Double
            Dim mean As Double = y.Average
            Dim sst As Double = 0

            For i As Integer = 0 To y.Length - 1
                sst += (y(i) - mean) ^ 2
            Next

            Return sst
        End Function

        ''' <summary>
        ''' 拟合优度 ``R^2 = 1 - SSE / SST``
        ''' </summary>
        ''' <param name="sse"></param>
        ''' <param name="sst"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Rsquared(sse As Double, sst As Double) As Double
            If sst <= 0 Then
                Return 0
            Else
                Return 1.0 - sse / sst
            End If
        End Function

        ''' <summary>
        ''' 计算设计矩阵的交叉乘积矩阵 ``X'X``
        ''' </summary>
        ''' <param name="X">设计矩阵，每一个行向量是一个样本点的自变量向量</param>
        ''' <param name="width">设计矩阵的列数(自变量个数)</param>
        ''' <returns></returns>
        Public Function NormalMatrix(X As Double(,), width As Integer) As Double(,)
            Dim xtx As Double(,) = New Double(width - 1, width - 1) {}

            For i As Integer = 0 To width - 1
                For j As Integer = i To width - 1
                    Dim sum As Double = 0

                    For k As Integer = 0 To X.GetLength(0) - 1
                        sum += X(k, i) * X(k, j)
                    Next

                    xtx(i, j) = sum
                    xtx(j, i) = sum
                Next
            Next

            Return xtx
        End Function

        ''' <summary>
        ''' 对最小二乘模型的每一个回归系数做 t 检验。
        ''' 
        ''' ```
        ''' se(beta_j) = sqrt( MSE * inv(X'X)[j,j] )
        ''' t_j        = beta_j / se(beta_j)
        ''' MSE        = SSE / (n - p)
        ''' ```
        ''' </summary>
        ''' <param name="X">设计矩阵，每一个行向量是一个样本点的自变量向量</param>
        ''' <param name="beta">完整的回归系数向量(第 0 个元素为截距)</param>
        ''' <param name="sse">模型的残差平方和</param>
        ''' <param name="pvalueTolerance">p 值的数值容差，小于这个阈值的 p 值会被修正为零</param>
        ''' <returns>
        ''' t 值与双尾 p 值；当 ``X'X`` 不可逆或者没有残差自由度的时候返回空引用。
        ''' </returns>
        Public Function TTest(X As Double(,), beta As Double(), sse As Double,
                              Optional pvalueTolerance As Double = 0.0000000001) As (tstat As Double(), pvalue As Double())

            Dim n As Integer = X.GetLength(0)
            Dim width As Integer = X.GetLength(1)
            Dim df As Integer = n - width

            If beta.Length <> width OrElse df <= 0 Then
                Return (Nothing, Nothing)
            End If

            Dim inv As Double(,) = MatrixOps.Inverse(NormalMatrix(X, width), strict:=True, throwSingularity:=False)

            If inv Is Nothing Then
                Return (Nothing, Nothing)
            End If

            Dim mse As Double = sse / df
            Dim dist As New StudenttDistribution(df)
            Dim tstat As Double() = New Double(width - 1) {}
            Dim pvalue As Double() = New Double(width - 1) {}

            For j As Integer = 0 To width - 1
                Dim var As Double = mse * inv(j, j)

                If Not IsFinite(var) OrElse var <= 0 Then
                    ' 残差为零(完美拟合)的时候标准误退化为零，t 值趋于无穷大，
                    ' 此时直接把 p 值写成零；回归系数本身为零的变量则保持不显著
                    If beta(j) = 0 Then
                        tstat(j) = 0
                        pvalue(j) = 1
                    Else
                        tstat(j) = If(beta(j) > 0, Double.PositiveInfinity, Double.NegativeInfinity)
                        pvalue(j) = 0
                    End If

                    Continue For
                End If

                Dim t As Double = beta(j) / std.Sqrt(var)
                Dim tAbs As Double = std.Abs(t)
                Dim p As Double

                If Not IsFinite(tAbs) OrElse tAbs >= MaxTabulatedT Then
                    ' t 值非常大的时候(例如没有观测噪声的完美拟合)，
                    ' 数值积分求出来的累积概率已经完全不可靠，这里直接给出极小值
                    p = 0
                Else
                    p = 2 * (1 - dist.cdf(tAbs, resolution:=4000))
                End If

                If Double.IsNaN(p) Then
                    p = 1
                ElseIf p < pvalueTolerance Then
                    p = 0
                ElseIf p > 1 Then
                    p = 1
                End If

                tstat(j) = t
                pvalue(j) = p
            Next

            Return (tstat, pvalue)
        End Function

        ''' <summary>
        ''' 判断一个双精度数值是否为有限值(既不是 NaN 也不是无穷大)
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function IsFinite(x As Double) As Boolean
            Return Not Double.IsNaN(x) AndAlso Not Double.IsInfinity(x)
        End Function
    End Module
End Namespace
