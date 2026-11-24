Imports Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork

Namespace test.Tests

    ''' <summary>
    ''' 数值工具(<see cref="NumericLogSpace"/>)的解析解对照。
    ''' </summary>
    ''' <remarks>
    ''' Gamma 函数与行列式都有精确的解析值可以对照，因此这里的断言全部使用
    ''' 手工计算的常数，而不是用被测函数自己算出来的结果。
    ''' </remarks>
    Module Numeric

        Public Sub Run()
            Call LogGammaChecks()
            Call LogSumExpChecks()
            Call LogDetChecks()
        End Sub

        Private Sub LogGammaChecks()
            ' Γ(1/2) = sqrt(π)
            Call Support.Near(0.5 * Math.Log(Math.PI), NumericLogSpace.LogGamma(0.5), 1.0E-12, "LogGamma(1/2)")

            ' Γ(1) = 1
            Call Support.Near(0.0, NumericLogSpace.LogGamma(1.0), 1.0E-12, "LogGamma(1)")

            ' Γ(2) = 1
            Call Support.Near(0.0, NumericLogSpace.LogGamma(2.0), 1.0E-12, "LogGamma(2)")

            ' Γ(5) = 24
            Call Support.Near(Math.Log(24.0), NumericLogSpace.LogGamma(5.0), 1.0E-12, "LogGamma(5) = ln 24")

            ' 递推关系 Γ(x+1) = x Γ(x)
            For Each x As Double In {0.5, 1.7, 3.25, 10.0, 57.5}
                Call Support.Near(
                    Math.Log(x) + NumericLogSpace.LogGamma(x),
                    NumericLogSpace.LogGamma(x + 1.0),
                    1.0E-10,
                    $"LogGamma recurrence at x={x}")
            Next

            ' 反射公式 Γ(x)Γ(1-x) = π / sin(πx)
            Dim reflected As Double = NumericLogSpace.LogGamma(0.3) + NumericLogSpace.LogGamma(0.7)
            Call Support.Near(Math.Log(Math.PI / Math.Sin(Math.PI * 0.3)), reflected, 1.0E-10, "LogGamma reflection formula")

            Call Support.Throws(Of ArgumentOutOfRangeException)(
                Sub() NumericLogSpace.LogGamma(0.0),
                "LogGamma must reject non-positive arguments")
        End Sub

        Private Sub LogSumExpChecks()
            Dim a As Double = Math.Log(3.0)
            Dim b As Double = Math.Log(4.0)

            Call Support.Near(Math.Log(7.0), NumericLogSpace.LogSumExp(a, b), 1.0E-12, "LogSumExp of two values")
            Call Support.Near(Math.Log(7.0), NumericLogSpace.LogSumExp({a, b}), 1.0E-12, "LogSumExp of an array")

            ' 其中一个为 0 概率的时候结果等于另一个
            Call Support.Near(a, NumericLogSpace.LogSumExp(a, Double.NegativeInfinity), 1.0E-12, "LogSumExp with zero")

            ' 全部为 0 概率
            Call Support.Near(
                Double.NegativeInfinity,
                NumericLogSpace.LogSumExp({Double.NegativeInfinity, Double.NegativeInfinity}),
                1.0E-12,
                "LogSumExp of all zeroes")

            ' 归一化
            Dim probs As Double() = NumericLogSpace.NormalizeLog({Math.Log(1.0), Math.Log(3.0)})
            Call Support.Near(0.25, probs(0), 1.0E-12, "NormalizeLog[0]")
            Call Support.Near(0.75, probs(1), 1.0E-12, "NormalizeLog[1]")
            Call Support.Near(1.0, probs(0) + probs(1), 1.0E-12, "NormalizeLog sums to one")
        End Sub

        Private Sub LogDetChecks()
            ' 对角矩阵的行列式等于对角元素之积
            Dim diagonal As Double(,) = {{2.0, 0.0, 0.0}, {0.0, 3.0, 0.0}, {0.0, 0.0, 4.0}}
            Call Support.Near(Math.Log(24.0), NumericLogSpace.LogDet(diagonal), 1.0E-12, "LogDet of diag(2,3,4)")

            ' 2×2：det = 1*4 - 2*3 = -2，取绝对值之后为 2
            Dim twoByTwo As Double(,) = {{1.0, 2.0}, {3.0, 4.0}}
            Call Support.Near(Math.Log(2.0), NumericLogSpace.LogDet(twoByTwo), 1.0E-12, "LogDet of a 2x2 matrix")

            ' 奇异矩阵
            Dim singular As Double(,) = {{1.0, 2.0}, {2.0, 4.0}}
            Call Support.Near(
                Double.NegativeInfinity,
                NumericLogSpace.LogDet(singular),
                1.0E-12,
                "LogDet of a singular matrix")

            ' 1×1
            Call Support.Near(Math.Log(7.5), NumericLogSpace.LogDet(New Double(,) {{7.5}}), 1.0E-12, "LogDet of a 1x1 matrix")

            ' 3×3 上三角矩阵的行列式等于对角元素之积(用来检查部分主元的影响)
            Dim triangular As Double(,) = {{2.0, 5.0, 1.0}, {0.0, -3.0, 4.0}, {0.0, 0.0, 0.5}}
            Call Support.Near(Math.Log(3.0), NumericLogSpace.LogDet(triangular), 1.0E-12, "LogDet of an upper triangular matrix")
        End Sub
    End Module
End Namespace
