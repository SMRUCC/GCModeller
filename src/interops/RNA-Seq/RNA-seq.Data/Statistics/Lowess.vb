' /********************************************************************************/
'
'  Lowess —— 局部加权回归（Robust Locally Weighted Regression）
'
'  忠实移植自 Rockhopper（Brian Tjaden, 2013）原始 Java 源码 Math_lib/Lowess.java：
'    * 对 x > 0 的点基于次序统计量 r 计算邻域带宽 h；
'    * 使用 tricube（tri-square）权重 w = (1 - clip(d/h, 0, 1)^3)^3；
'    * 对每个点做局部加权线性回归（2x2 正规方程求解）；
'    * 迭代进行鲁棒残差重加权（默认 1 次迭代）。
'
'  已移除 Oracle.Java.* / Java.Matrix / Output(...) 等失效依赖，改用 .NET BCL 与 System.Math。
'  返回数组与输入等长，且下标与输入顺序一一对应。
'
' /********************************************************************************/

Imports System
Imports System.Collections.Generic
Imports System.Linq

Namespace RNA_Seq.Statistics

    ''' <summary>
    ''' Lowess 平滑器。对 mean-variance 关系做稳健局部加权回归。
    ''' </summary>
    Public Module Lowess

        ''' <summary>
        ''' 局部加权回归(lowess)对 mean-variance 关系做平滑，输入按 x 升序或乱序均可（内部排序）。
        ''' 返回与输入等长的平滑值数组，数组下标与输入顺序一一对应。
        ''' </summary>
        ''' <param name="x">自变量（例如基因在各条件下的平均表达量）。</param>
        ''' <param name="y">因变量（例如对应的方差）。</param>
        ''' <returns>与输入等长的平滑值数组；参数为空或长度不一致时返回空数组。</returns>
        Public Function Fit(x As IEnumerable(Of Double), y As IEnumerable(Of Double)) As Double()
            If x Is Nothing OrElse y Is Nothing Then Return New Double() {}

            Dim xs As Double() = x.ToArray()
            Dim ys As Double() = y.ToArray()
            If xs.Length <> ys.Length Then Return New Double() {}

            Dim n As Integer = xs.Length
            Dim result As Double() = New Double(n - 1) {}
            If n = 0 Then Return result

            ' 原实现仅对 x > 0 的点参与拟合（零表达量的基因不参与）
            Dim X1 As New List(Of Double)()
            Dim Y1 As New List(Of Double)()
            For i As Integer = 0 To n - 1
                If xs(i) > 0.0 Then
                    X1.Add(xs(i))
                    Y1.Add(ys(i))
                End If
            Next

            If X1.Count < 2 Then
                ' 可用点不足，退化为原始 y 值（保持等长）
                For i As Integer = 0 To n - 1
                    result(i) = ys(i)
                Next
                Return result
            End If

            Dim h As List(Of Double) = computeBandwidths(X1)
            Dim w As Double()() = process(X1, h)
            cube(w)
            oneMinus(w)
            cube(w)

            Dim count As Integer = X1.Count
            Dim yest As Double() = New Double(count - 1) {}
            Dim delta As Double() = New Double(count - 1) {}

            ' 原实现默认只做 1 次迭代（无参重载内部调用 lowess(x, y, 0.1, 1)）
            Const iterations As Integer = 1

            For z As Integer = 0 To iterations - 1
                For i As Integer = 0 To count - 1
                    Dim A1 As Double = 0.0
                    Dim A2 As Double = 0.0
                    Dim A4 As Double = 0.0
                    Dim b1 As Double = 0.0
                    Dim b2 As Double = 0.0
                    For j As Integer = 0 To w.Length - 1
                        Dim wj As Double = w(j)(i)
                        A1 += wj
                        A2 += wj * X1(j)
                        A4 += wj * X1(j) * X1(j)
                        b1 += wj * Y1(j)
                        b2 += wj * Y1(j) * X1(j)
                    Next

                    ' 求解 2x2 正规方程 A * beta = b（等价于原 Java.Matrix.solve）
                    Dim det As Double = A1 * A4 - A2 * A2
                    If System.Math.Abs(det) < 1.0E-12 Then
                        yest(i) = If(A1 <> 0.0, b1 / A1, X1(i))
                    Else
                        Dim beta0 As Double = (b1 * A4 - b2 * A2) / det
                        Dim beta1 As Double = (A1 * b2 - A2 * b1) / det
                        yest(i) = beta0 + beta1 * X1(i)
                    End If
                Next

                Dim residuals As Double() = New Double(Y1.Count - 1) {}
                Dim residuals_absValue As New List(Of Double)()
                For i As Integer = 0 To Y1.Count - 1
                    residuals(i) = Y1(i) - yest(i)
                    residuals_absValue.Add(System.Math.Abs(residuals(i)))
                Next

                Dim s As Double = 1.0
                If residuals.Length > 0 Then
                    s = selectByOrder(residuals_absValue.ToArray(), 1 + (residuals.Length \ 2))
                End If
                If s <= 0.0 Then s = 1.0

                For i As Integer = 0 To delta.Length - 1
                    delta(i) = residuals(i) / (6.0 * s)
                    If delta(i) < -1.0 Then delta(i) = -1.0
                    If delta(i) > 1.0 Then delta(i) = 1.0
                Next

                ' 与原实现保持一致：此处重新推导 delta（鲁棒权重）。
                ' 注意：原实现的 delta 并未回写到 w，故 yest 在多次迭代间保持不变。
                Call oneMinus(product(delta, delta))
                delta = product(delta, delta)
            Next

            ' 将平滑结果映射回原始输入顺序（x <= 0 的点取正值中的最小值）
            Const threshold As Double = 0.0
            Dim minPositive As Double = Double.MaxValue
            For i As Integer = 0 To yest.Length - 1
                If yest(i) > threshold AndAlso yest(i) < minPositive Then
                    minPositive = yest(i)
                End If
            Next
            If minPositive = Double.MaxValue Then
                For i As Integer = 0 To yest.Length - 1
                    If yest(i) < minPositive Then minPositive = yest(i)
                Next
                If minPositive = Double.MaxValue Then minPositive = 0.0
            End If

            Dim k As Integer = 0
            For i As Integer = 0 To n - 1
                If xs(i) <= threshold Then
                    result(i) = minPositive
                Else
                    result(i) = yest(k)
                    k += 1
                End If
            Next

            Return result
        End Function

        ''' <summary>
        ''' 计算每个点的邻域带宽 h（原文中由次序统计量 r 决定）。
        ''' </summary>
        Private Function computeBandwidths(X1 As List(Of Double)) As List(Of Double)
            Dim X1_sorted As Double() = X1.ToArray()
            System.Array.Sort(X1_sorted)
            System.Array.Reverse(X1_sorted)
            ' 降序排列

            ' 复刻 List.IndexOf 的“首次出现”语义
            Dim firstIndex As New Dictionary(Of Double, Integer)()
            For i As Integer = 0 To X1_sorted.Length - 1
                If Not firstIndex.ContainsKey(X1_sorted(i)) Then
                    firstIndex(X1_sorted(i)) = i
                End If
            Next

            Dim h As New List(Of Double)()
            For i As Integer = 0 To X1.Count - 1
                Dim p As Double = X1(i)
                Dim fraction As Double = (X1_sorted.Length - firstIndex(p)) / CDbl(X1_sorted.Length)
                Dim r As Integer = CInt(System.Math.Truncate(System.Math.Ceiling(System.Math.Min(2.0 * fraction, 0.4) * X1.Count)))

                Dim dist As Double() = New Double(X1.Count - 1) {}
                For j As Integer = 0 To X1.Count - 1
                    dist(j) = System.Math.Abs(p - X1(j))
                Next
                System.Array.Sort(dist)

                If r < dist.Length Then
                    h.Add(dist(r))
                Else
                    h.Add(dist(dist.Length - 1))
                End If
            Next

            Return h
        End Function

        ''' <summary>
        ''' clip(abs((x - transpose(x)) / h), 0.0, 1.0)，结果以 w(j)(i) 存放。
        ''' 对 h(i) = 0（x 含重复值）做保护：相同点权重为 0，不同点权重为 1。
        ''' </summary>
        Private Function process(x As List(Of Double), h As List(Of Double)) As Double()()
            Dim w As Double()() = New Double(x.Count - 1)() {}
            For i As Integer = 0 To x.Count - 1
                w(i) = New Double(x.Count - 1) {}
            Next

            For i As Integer = 0 To x.Count - 1
                For j As Integer = 0 To x.Count - 1
                    Dim value As Double
                    If h(i) <= 0.0 Then
                        value = If(x(i) - x(j) = 0.0, 0.0, 1.0)
                    Else
                        value = System.Math.Abs((x(i) - x(j)) / h(i))
                    End If
                    If value < 0.0 Then value = 0.0
                    If value > 1.0 Then value = 1.0
                    w(j)(i) = value
                Next
            Next

            Return w
        End Function

        ''' <summary>对二维数组的每个元素求立方。</summary>
        Private Sub cube(w As Double()())
            For i As Integer = 0 To w.Length - 1
                For j As Integer = 0 To w(0).Length - 1
                    w(i)(j) = w(i)(j) * w(i)(j) * w(i)(j)
                Next
            Next
        End Sub

        ''' <summary>w = 1 - w（二维数组）。</summary>
        Private Sub oneMinus(w As Double()())
            For i As Integer = 0 To w.Length - 1
                For j As Integer = 0 To w(0).Length - 1
                    w(i)(j) = 1.0 - w(i)(j)
                Next
            Next
        End Sub

        ''' <summary>w = 1 - w（一维数组）。</summary>
        Private Sub oneMinus(w As Double())
            For i As Integer = 0 To w.Length - 1
                w(i) = 1.0 - w(i)
            Next
        End Sub

        ''' <summary>逐元素相乘。</summary>
        Private Function product(x As Double(), y As Double()) As Double()
            Dim result As Double() = New Double(x.Length - 1) {}
            For i As Integer = 0 To x.Length - 1
                result(i) = x(i) * y(i)
            Next
            Return result
        End Function

        ''' <summary>
        ''' 返回已排序数组中的第 orderStatistic 个次序统计量（1 起始）。
        ''' </summary>
        Private Function selectByOrder(a As Double(), orderStatistic As Integer) As Double
            Dim sorted As Double() = CType(a.Clone(), Double())
            System.Array.Sort(sorted)
            Dim idx As Integer = orderStatistic - 1
            If idx < 0 Then idx = 0
            If idx > sorted.Length - 1 Then idx = sorted.Length - 1
            Return sorted(idx)
        End Function

    End Module

End Namespace
