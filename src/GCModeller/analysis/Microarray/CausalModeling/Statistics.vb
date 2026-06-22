Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis

''' <summary>
''' 统计运算模块 - 使用 VB.NET 基础数学函数实现统计分析
''' 包含：均值、方差、相关、标准化、OLS 多元回归、Bootstrap 重采样等
''' </summary>
Public Module Statistics

    ''' <summary>计算列均值</summary>
    Public Function ColMean(data As Double(,), col As Integer) As Double
        Dim n = data.GetLength(0)
        Dim sum = 0.0
        For i = 0 To n - 1
            sum += data(i, col)
        Next
        Return sum / n
    End Function

    ''' <summary>计算列标准差（样本标准差，自由度 n-1）</summary>
    Public Function ColStd(data As Double(,), col As Integer) As Double
        Dim n = data.GetLength(0)
        Dim m = ColMean(data, col)
        Dim sumSq = 0.0
        For i = 0 To n - 1
            sumSq += (data(i, col) - m) ^ 2
        Next
        Return Math.Sqrt(sumSq / (n - 1))
    End Function

    ''' <summary>计算列方差</summary>
    Public Function ColVar(data As Double(,), col As Integer) As Double
        Dim s = ColStd(data, col)
        Return s * s
    End Function

    ''' <summary>计算两列的 Pearson 相关系数</summary>
    Public Function Pearson(x As Double(), y As Double()) As Double
        Dim n = x.Length
        Dim mx = 0.0, my = 0.0
        For i = 0 To n - 1
            mx += x(i) : my += y(i)
        Next
        mx /= n : my /= n

        Dim sxy = 0.0, sxx = 0.0, syy = 0.0
        For i = 0 To n - 1
            Dim dx = x(i) - mx
            Dim dy = y(i) - my
            sxy += dx * dy
            sxx += dx * dx
            syy += dy * dy
        Next

        If sxx < 1.0E-30 OrElse syy < 1.0E-30 Then Return 0.0
        Return sxy / Math.Sqrt(sxx * syy)
    End Function

    ''' <summary>计算两列的 Pearson 相关系数（矩阵版）</summary>
    Public Function PearsonMat(data As Double(,), col1 As Integer, col2 As Integer) As Double
        Dim n = data.GetLength(0)
        Dim x(n - 1) As Double
        Dim y(n - 1) As Double
        For i = 0 To n - 1
            x(i) = data(i, col1)
            y(i) = data(i, col2)
        Next
        Return Pearson(x, y)
    End Function

    ''' <summary>计算相关系数矩阵</summary>
    Public Function CorrelationMatrix(data As Double(,)) As Double(,)
        Dim n = data.GetLength(0)
        Dim p = data.GetLength(1)
        Dim corr(p - 1, p - 1) As Double
        For i = 0 To p - 1
            For j = 0 To p - 1
                If i = j Then
                    corr(i, j) = 1.0
                Else
                    corr(i, j) = PearsonMat(data, i, j)
                End If
            Next
        Next
        Return corr
    End Function

    ''' <summary>计算协方差矩阵（样本协方差，自由度 n-1）</summary>
    Public Function CovarianceMatrix(data As Double(,)) As Double(,)
        Dim n = data.GetLength(0)
        Dim p = data.GetLength(1)
        Dim means(p - 1) As Double
        For j = 0 To p - 1
            means(j) = ColMean(data, j)
        Next

        Dim cov(p - 1, p - 1) As Double
        For i = 0 To p - 1
            For j = 0 To p - 1
                Dim sum = 0.0
                For k = 0 To n - 1
                    sum += (data(k, i) - means(i)) * (data(k, j) - means(j))
                Next
                cov(i, j) = sum / (n - 1)
            Next
        Next
        Return cov
    End Function

    ''' <summary>对数据进行 Z-score 标准化（每列减均值除以标准差）</summary>
    Public Function Standardize(data As Double(,)) As Double(,)
        Dim n = data.GetLength(0)
        Dim p = data.GetLength(1)
        Dim result(n - 1, p - 1) As Double
        For j = 0 To p - 1
            Dim m = ColMean(data, j)
            Dim s = ColStd(data, j)
            If s < 1.0E-30 Then s = 1.0
            For i = 0 To n - 1
                result(i, j) = (data(i, j) - m) / s
            Next
        Next
        Return result
    End Function

    ''' <summary>构造带截距项的设计矩阵</summary>
    Public Function AddIntercept(X As Double(,)) As Double(,)
        Dim n = X.GetLength(0)
        Dim p = X.GetLength(1)
        Dim result(n - 1, p) As Double  ' p+1 列
        For i = 0 To n - 1
            result(i, 0) = 1.0
            For j = 0 To p - 1
                result(i, j + 1) = X(i, j)
            Next
        Next
        Return result
    End Function

    ''' <summary>
    ''' OLS 多元线性回归: y = X * beta + epsilon
    ''' 返回回归系数 beta、标准误、t 值、p 值、R²、调整 R²
    ''' X 第一列应为 1（截距项），如不需要截距请预先处理
    ''' </summary>
    Public Function OLSRegression(y As Double(), X As Double(,), strict As Boolean, makeWarn As Boolean) As OLSResult
        Dim n = y.Length
        Dim k = X.GetLength(1)  ' 自变量个数（含截距）
        Dim result As New OLSResult() With {
            .Coefficients = New Double(k - 1) {}
        }

        If n <= k Then
            Dim msg As String = $"样本量 必须严格大于自变量个数(含截距) 才能进行 OLS 回归。当前 n={n}, k={k}。"

            If strict Then
                Throw New ArgumentException(msg)
            ElseIf makeWarn Then
                ' do nothing
                Call msg.warning
                Call App.LogException(msg,)
            End If
        End If

        ' 计算 beta = (X'X)^{-1} X'y
        Dim Xt = MatrixOps.Transpose(X)
        Dim XtX = MatrixOps.Multiply(Xt, X)
        Dim XtXInv = MatrixOps.Inverse(XtX, strict)
        Dim yMat(n - 1, 0) As Double
        For i = 0 To n - 1
            yMat(i, 0) = y(i)
        Next
        Dim Xty = MatrixOps.Multiply(Xt, yMat)
        Dim betaMat = MatrixOps.Multiply(XtXInv, Xty)

        For i = 0 To k - 1
            result.Coefficients(i) = betaMat(i, 0)
        Next

        ' 计算残差与 R²
        Dim residuals(n - 1) As Double
        Dim yMean = 0.0
        For i = 0 To n - 1
            yMean += y(i)
        Next
        yMean /= n

        Dim ssRes = 0.0, ssTot = 0.0
        For i = 0 To n - 1
            Dim pred = 0.0
            For j = 0 To k - 1
                pred += X(i, j) * result.Coefficients(j)
            Next
            residuals(i) = y(i) - pred
            ssRes += residuals(i) ^ 2
            ssTot += (y(i) - yMean) ^ 2
        Next

        result.R2 = 1.0 - ssRes / ssTot
        result.AdjR2 = 1.0 - (1.0 - result.R2) * (n - 1) / (n - k)

        ' 残差方差
        Dim sigma2 = ssRes / (n - k)

        ' 系数标准误 = sqrt(diag(sigma2 * (X'X)^{-1}))
        Dim varCov = MatrixOps.Scale(XtXInv, sigma2)
        result.StdErrors = New Double(k - 1) {}
        result.TValues = New Double(k - 1) {}
        result.PValues = New Double(k - 1) {}
        For i As Integer = 0 To k - 1
            result.StdErrors(i) = Math.Sqrt(Math.Max(varCov(i, i), 0.0))
            If result.StdErrors(i) > 1.0E-30 Then
                result.TValues(i) = result.Coefficients(i) / result.StdErrors(i)
                ' result.PValues(i) = TDistTwoTail(result.TValues(i), n - k)
                result.PValues(i) = t.Pvalue(
                    t:=result.TValues(i),     ' t value
                    df:=n - k,                ' degree of freedom
                    hyp:=Hypothesis.TwoSided  ' alternative
                )
            Else
                result.TValues(i) = 0.0
                result.PValues(i) = 1.0
            End If
        Next

        result.SampleSize = n
        result.NumPredictors = k
        result.Residuals = residuals
        Return result
    End Function

    ''' <summary>
    ''' 学生 t 分布的双侧 p 值（使用正态近似 + 修正）
    ''' df 为自由度
    ''' </summary>
    Public Function TDistTwoTail(t As Double, df As Integer) As Double
        ' 使用正态分布近似 + 自由度修正
        ' 对于大样本（df > 30）正态近似很好
        ' 对于小样本使用 Cornish-Fisher 展开修正
        Dim z = Math.Abs(t)
        Dim pNormal = 2.0 * (1.0 - NormalCDF(z))

        If df > 200 Then
            Return pNormal
        End If

        ' 使用基于不完全贝塔函数的精确 t 分布
        ' p = I_x(df/2, 1/2)，其中 x = df / (df + t^2)
        Dim x = df / (df + t * t)
        Return IncompleteBeta(x, df / 2.0, 0.5)
    End Function

    ''' <summary>标准正态分布累积分布函数</summary>
    Public Function NormalCDF(z As Double) As Double
        ' 使用 Abramowitz & Stegun 近似公式 7.1.26
        Dim a1 = 0.254829592
        Dim a2 = -0.284496736
        Dim a3 = 1.421413741
        Dim a4 = -1.453152027
        Dim a5 = 1.061405429
        Dim p = 0.3275911

        Dim sign = If(z < 0, -1, 1)
        z = Math.Abs(z) / Math.Sqrt(2.0)
        Dim t = 1.0 / (1.0 + p * z)
        Dim y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-z * z)
        Return 0.5 * (1.0 + sign * y)
    End Function

    ''' <summary>
    ''' 正则化不完全贝塔函数 I_x(a, b)
    ''' 用于计算 t 分布、F 分布的 p 值
    ''' </summary>
    Public Function IncompleteBeta(x As Double, a As Double, b As Double) As Double
        If x <= 0 Then Return 0.0
        If x >= 1 Then Return 1.0

        Dim lbeta = LogGamma(a) + LogGamma(b) - LogGamma(a + b)
        Dim front = Math.Exp(Math.Log(x) * a + Math.Log(1 - x) * b - lbeta) / a

        ' 使用连分数展开（Lentz 算法）
        Return front * BetaCF(x, a, b) * 2.0  ' 乘 2 得到双侧
    End Function

    ''' <summary>不完全贝塔函数的连分数（Numerical Recipes 风格）</summary>
    Private Function BetaCF(x As Double, a As Double, b As Double) As Double
        Dim MAXIT = 300
        Dim EPS = 0.000000000001
        Dim FPMIN = 1.0E-300
        Dim qab = a + b
        Dim qap = a + 1.0
        Dim qam = a - 1.0
        Dim c = 1.0
        Dim d = 1.0 - qab * x / qap
        If Math.Abs(d) < FPMIN Then d = FPMIN
        d = 1.0 / d
        Dim h = d

        For m = 1 To MAXIT
            Dim m2 = 2 * m
            Dim aa = m * (b - m) * x / ((qam + m2) * (a + m2))
            d = 1.0 + aa * d
            If Math.Abs(d) < FPMIN Then d = FPMIN
            c = 1.0 + aa / c
            If Math.Abs(c) < FPMIN Then c = FPMIN
            d = 1.0 / d
            h *= d * c
            aa = -(a + m) * (qab + m) * x / ((a + m2) * (qap + m2))
            d = 1.0 + aa * d
            If Math.Abs(d) < FPMIN Then d = FPMIN
            c = 1.0 + aa / c
            If Math.Abs(c) < FPMIN Then c = FPMIN
            d = 1.0 / d
            Dim del = d * c
            h *= del
            If Math.Abs(del - 1.0) < EPS Then Exit For
        Next
        Return h
    End Function

    ''' <summary>Log Gamma 函数（Lanczos 近似）</summary>
    Public Function LogGamma(x As Double) As Double
        Dim cof() As Double = {76.18009172947146, -86.50532032941677,
                                24.01409824083091, -1.231739572450155,
                                0.1208650973866179E-2, -0.5395239384953E-5}
        Dim stp = 2.5066282746310005
        Dim x1 = x + 1.0
        Dim tmp = x + 5.5
        tmp -= (x + 0.5) * Math.Log(tmp)
        Dim ser = 1.000000000190015
        For j = 0 To 5
            ser += cof(j) / x1
            x1 += 1.0
        Next
        Return -tmp + Math.Log(stp * ser / x)
    End Function

    ''' <summary>
    ''' Bootstrap 重采样 - 有放回地抽取 n 个样本
    ''' 使用传入的 rng 以保证可复现
    ''' </summary>
    Public Function BootstrapSample(data As Double(,), rng As Random) As Double(,)
        Dim n = data.GetLength(0)
        Dim p = data.GetLength(1)
        Dim sample(n - 1, p - 1) As Double
        For i = 0 To n - 1
            Dim idx = rng.Next(0, n)
            For j = 0 To p - 1
                sample(i, j) = data(idx, j)
            Next
        Next
        Return sample
    End Function

    ''' <summary>计算均值</summary>
    Public Function Mean(x As Double()) As Double
        Dim sum = 0.0
        For Each v In x
            sum += v
        Next
        Return sum / x.Length
    End Function

    ''' <summary>计算分位数（线性插值法）</summary>
    Public Function Quantile(x As Double(), q As Double) As Double
        Dim sorted(x.Length - 1) As Double
        Array.Copy(x, sorted, x.Length)
        Array.Sort(sorted)
        Dim pos = q * (sorted.Length - 1)
        Dim lower = CInt(Math.Floor(pos))
        Dim upper = CInt(Math.Ceiling(pos))
        If lower = upper Then Return sorted(lower)
        Dim frac = pos - lower
        Return sorted(lower) * (1 - frac) + sorted(upper) * frac
    End Function

End Module

''' <summary>OLS 回归结果</summary>
Public Class OLSResult
    Public Property Coefficients As Double()      ' 回归系数（含截距）
    Public Property StdErrors As Double()         ' 标准误
    Public Property TValues As Double()           ' t 统计量
    Public Property PValues As Double()           ' p 值
    Public Property R2 As Double                  ' 决定系数
    Public Property AdjR2 As Double               ' 调整决定系数
    Public Property Residuals As Double()         ' 残差
    Public Property SampleSize As Integer         ' 样本量
    Public Property NumPredictors As Integer      ' 自变量个数（含截距）
End Class
