#Region "Microsoft.VisualBasic::9a43f7d2646994f8514927f159c70c53, annotations\GSEA\GSVA\C\kernel_estimation.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 148
    '    Code Lines: 97 (65.54%)
    ' Comment Lines: 19 (12.84%)
    '    - Xml Docs: 84.21%
    ' 
    '   Blank Lines: 32 (21.62%)
    '     File Size: 5.08 KB


    '     Module kernel_estimation
    ' 
    '         Function: _precomputed_cdf, matrix_density_R, ppois, row_d, sd1
    ' 
    '         Sub: initCdfs
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.Distributions
Imports std = System.Math

Namespace C

    Module kernel_estimation

        Public Function matrix_density_R(X As Double()(),
                                         Y As Double()(),
                                         dims As (m%, n%),
                                         n_density_samples As Integer,
                                         n_test_samples As Integer,
                                         n_genes As Integer,
                                         rnaseq As Boolean) As Double()()

            Dim R As Double()() = RectangularArray.Matrix(Of Double)(dims.n, dims.m)

            For j As Integer = 0 To n_genes - 1
                'Dim offset_density = j * n_density_samples
                'Dim offset_test = j * n_test_samples

                ' row_d(X(offset_density), Y(offset_test), R(offset_test), n_density_samples, n_test_samples, rnaseq)
                row_d(X(j), Y(j), R(j), n_density_samples, n_test_samples, rnaseq)
            Next

            Return R
        End Function

        Const SIGMA_FACTOR = 4.0
        Const PRECOMPUTE_RESOLUTION = 10000
        Const MAX_PRECOMPUTE = 10.0

        ' 2 / sqrt(pi)，erf 幂级数所用的归一化常数
        Const TWO_OVER_SQRTPI As Double = 1.1283791670955126
        ' sqrt(2)
        Const SQRT2 As Double = 1.4142135623730951

        Dim is_precomputed As Integer = 0
        Dim precomputed_cdf As Double() = New Double(PRECOMPUTE_RESOLUTION) {}

        ''' <summary>
        ''' for resampling, x are the resampled points and y are the
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <param name="r"></param>
        ''' <param name="size_density_n"></param>
        ''' <param name="size_test_n"></param>
        ''' <param name="rnaseq"></param>
        ''' <returns></returns>
        Private Function row_d(x As Double(),
                               y As Double(),
                               ByRef r As Double(),
                               size_density_n As Integer,
                               size_test_n As Integer,
                               rnaseq As Boolean)

            Dim bw As Double = If(rnaseq, 0.5, (sd1(x, size_density_n) / SIGMA_FACTOR))
            Dim left_tail As Double

            ' 与官方 C 实现保持一致：带宽为 0 或者非数值时兜底为一个极小值，
            ' 否则后续核估计会退化为阶跃函数或者产生除零
            If Double.IsNaN(bw) OrElse bw = 0 Then
                bw = 0.001
            End If

            If Not rnaseq AndAlso is_precomputed = 0 Then
                initCdfs()
                is_precomputed = 1
            End If

            For j As Integer = 0 To size_test_n - 1
                left_tail = 0

                For i As Integer = 0 To size_density_n - 1
                    left_tail += If(rnaseq, ppois(y(j), x(i) + bw, True, False), _precomputed_cdf(y(j) - x(i), bw))
                Next

                left_tail = left_tail / size_density_n
                r(j) = -1 * std.Log((1 - left_tail) / left_tail)
            Next

            Return r
        End Function

        Private Function _precomputed_cdf(x As Double, sigma As Double) As Double
            Dim v As Double = x / sigma

            If v < -1 * MAX_PRECOMPUTE Then
                Return 0
            ElseIf v > MAX_PRECOMPUTE Then
                Return 1
            Else
                Dim i As Integer = std.Abs(v) / MAX_PRECOMPUTE * PRECOMPUTE_RESOLUTION
                Dim cdf As Double = precomputed_cdf(i)

                If v < 0 Then
                    Return 1 - cdf
                Else
                    Return cdf
                End If
            End If
        End Function

        ''' <summary>
        ''' 泊松分布的累积分布函数，对应 R 的 ``ppois(q, lambda, lower.tail, log.p)``
        ''' </summary>
        ''' <param name="y">观测计数 q</param>
        ''' <param name="x">泊松分布的期望 lambda</param>
        ''' <param name="f1">lower.tail，TRUE 时返回 P(X &lt;= q)</param>
        ''' <param name="f2">log.p，TRUE 时返回概率的对数值</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 泊松分布的累积分布函数可以经由正则化的上不完全 Gamma 函数表示：
        ''' 
        '''   P(X &lt;= q) = Q(floor(q) + 1, lambda)
        ''' 
        ''' 其中 Q(a, x) = Γ(a, x) / Γ(a) 即为 <see cref="RegularizedGammaQ"/>。
        ''' 这与 R 的 ``ppois()`` 内部实现（src/nmath/ppois.c）完全一致。
        ''' </remarks>
        Private Function ppois(y As Double, x As Double, f1 As Boolean, f2 As Boolean) As Double
            Dim p As Double

            If y < 0 Then
                p = 0.0
            ElseIf x < 0 Then
                p = Double.NaN
            Else
                p = RegularizedGammaQ(std.Floor(y) + 1.0, x)
            End If

            If Not f1 Then
                p = 1 - p
            End If
            If f2 Then
                p = std.Log(p)
            End If

            Return p
        End Function

        Private Sub initCdfs()
            Dim divisor = PRECOMPUTE_RESOLUTION * 1.0

            For i As Integer = 0 To PRECOMPUTE_RESOLUTION
                precomputed_cdf(i) = pnorm5(MAX_PRECOMPUTE * i / divisor)
            Next
        End Sub

        ''' <summary>
        ''' 标准正态分布的累积分布函数，对应 R 的 ``pnorm(q, 0, 1, lower.tail = TRUE, log.p = FALSE)``
        ''' </summary>
        ''' <param name="q">分位数</param>
        ''' <remarks>
        ''' 经由误差函数实现：
        ''' 
        '''   Phi(q) = 0.5 * (1 + erf(q / sqrt(2)))    q &gt;= 0
        '''   Phi(q) = 0.5 * erfc(-q / sqrt(2))        q &lt; 0
        ''' 
        ''' 下尾统一用互补误差函数计算，以保证小概率尾部的相对精度。
        ''' 本实现自成一体，不依赖运行时库里的 <see cref="pnorm"/>（其基于梯形积分
        ''' 并在 4.1 倍标准差处截断，存在约 2e-5 量级的系统性误差），
        ''' 也不依赖不完全 Gamma 函数中的对数 Gamma 近似。
        ''' 
        ''' 之所以对精度如此敏感，是因为 GSVA 只使用密度值的秩：
        ''' 当表达矩阵中存在大量取值并列的行时，密度值之间的间隔本身就在 1e-5 量级上，
        ''' 任何超出 1e-12 的误差都会打乱秩，进而使富集分数产生显著偏差。
        ''' </remarks>
        Friend Function pnorm5(q As Double) As Double
            If Double.IsNaN(q) Then
                Return Double.NaN
            End If
            If q = 0 Then
                Return 0.5
            End If

            ' erfc 的自变量取绝对值，保证落在 [0, +inf)
            Dim u As Double = std.Abs(q) / SQRT2
            Dim p As Double

            If u < 1.0 Then
                ' |q| < sqrt(2)：erfc(u) = 1 - erf(u)，用幂级数计算 erf
                p = 0.5 + 0.5 * erfSeries(u)
                Return If(q > 0, p, 1.0 - p)
            Else
                ' |q| >= sqrt(2)：尾部概率用连分式计算，保持相对精度
                Dim tail As Double = 0.5 * erfcCF(u)
                Return If(q > 0, 1.0 - tail, tail)
            End If
        End Function

        ''' <summary>
        ''' 误差函数 erf(x) 的幂级数实现，适用于 |x| &lt; 1
        ''' </summary>
        ''' <remarks>
        ''' ``erf(x) = (2 / sqrt(pi)) * sum_{n&gt;=0} (-1)^n x^(2n+1) / (n! (2n+1))``
        ''' </remarks>
        Private Function erfSeries(x As Double) As Double
            Dim term As Double = x
            Dim sum As Double = x
            Dim n As Integer = 0

            Do While n < 1000
                n += 1
                term *= -x * x / n
                Dim add As Double = term / (2 * n + 1)

                sum += add

                If std.Abs(add) <= std.Abs(sum) * 1.0E-18 Then
                    Exit Do
                End If
            Loop

            Return TWO_OVER_SQRTPI * sum
        End Function

        ''' <summary>
        ''' 互补误差函数 erfc(x) 的连分式实现（修正 Lentz 算法），适用于 x &gt;= 1
        ''' </summary>
        ''' <remarks>
        ''' ``erfc(x) = Q(1/2, x^2)``，即形状参数为 1/2 的正则化上不完全 Gamma 函数。
        ''' 连分式部分与 ``RegularizedGammaQ`` 相同，但归一化常数直接使用解析值
        ''' ``ln Gamma(1/2) = 0.5 * ln(pi)``，避免引入对数 Gamma 近似的误差。
        ''' </remarks>
        Private Function erfcCF(x As Double) As Double
            Const a As Double = 0.5
            Const tiny As Double = 1.0E-300

            Dim u As Double = x * x
            Dim b As Double = u + 1.0 - a
            Dim c As Double = 1.0 / tiny
            Dim d As Double = 1.0 / b
            Dim h As Double = d
            Dim i As Integer = 1

            Do While i < 1000000
                Dim an As Double = -i * (i - a)

                b += 2.0
                d = an * d + b

                If std.Abs(d) < tiny Then
                    d = tiny
                End If

                c = b + an / c

                If std.Abs(c) < tiny Then
                    c = tiny
                End If

                d = 1.0 / d

                Dim del As Double = d * c

                h *= del

                If std.Abs(del - 1.0) < 1.0E-17 Then
                    Exit Do
                End If

                i += 1
            Loop

            ' ln Gamma(1/2) = 0.5 * ln(pi)
            Dim logNorm As Double = -u + a * std.Log(u) - 0.5 * std.Log(std.PI)

            Return h * std.Exp(logNorm)
        End Function

        ''' <summary>
        ''' calculates standard deviation, largely borrowed from C code in R's src/main/cov.c */
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="n"></param>
        ''' <returns></returns>
        Private Function sd1(x As Double(), n As Integer) As Double
            Dim n1 As Integer
            Dim mean, sd As Double
            Dim sum As Double = 0.0
            Dim tmp As Double

            ' 少于两个观测值时样本标准差没有定义，与 R 的 sd() 一致返回 NA，
            ' 避免 n - 1 = 0 造成的除零
            If n < 2 Then
                Return Double.NaN
            End If

            For i As Integer = 0 To n - 1
                sum += x(i)
            Next

            tmp = sum / n

            If Not tmp.IsNaNImaginary Then
                sum = 0

                For i As Integer = 0 To n - 1
                    sum += x(i) - tmp
                Next

                tmp = tmp + sum / n
            End If

            mean = tmp
            n1 = n - 1
            sum = 0

            For i As Integer = 0 To n - 1
                sum += (x(i) - mean) ^ 2
            Next

            sd = std.Sqrt(sum / n1)

            Return sd
        End Function
    End Module
End Namespace
