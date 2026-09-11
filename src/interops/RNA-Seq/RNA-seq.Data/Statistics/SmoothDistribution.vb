' /********************************************************************************/
'
'  RNA-seq.Data —— 平滑分布（均值-方差趋势拟合，可复用）
'
'  RNA-seq 数据存在"表达量越高方差越大"的均值-方差依赖。Rockhopper 使用局部回归
'  （lowess）对每个基因按其表达量估计平滑方差，而非简单采用泊松假设（方差=均值，
'  会低估生物学重复间的方差）。
'
'  本模块在 Lowess 之上提供直接面向"均值-方差趋势"的拟合入口，供差异表达检验复用。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.Linq

Namespace RNA_Seq.Statistics

    ''' <summary>
    ''' 均值-方差趋势的平滑拟合。
    ''' </summary>
    Public Module SmoothDistribution

        ''' <summary>
        ''' 对 (means, variances) 做局部回归平滑，返回与输入等长的平滑方差数组。
        ''' </summary>
        ''' <remarks>
        ''' 同一均值可能对应多个方差，这里按均值分组取中位数后再做 lowess，
        ''' 避免重复 x 值导致局部回归的邻域权重退化。
        ''' </remarks>
        Public Function FitVarianceTrend(means As IEnumerable(Of Double), variances As IEnumerable(Of Double)) As Double()
            Dim x As Double() = means.ToArray()
            Dim y As Double() = variances.ToArray()
            If x.Length <> y.Length Then
                Throw New ArgumentException("means 与 variances 长度必须一致。")
            End If
            If x.Length = 0 Then Return New Double() {}

            ' 按均值聚合并取方差中位数
            Dim grouped As IGrouping(Of Double, Double)() =
                x.Zip(y, Function(m, v) New With {.m = m, .v = v}) _
                 .GroupBy(Function(pair) pair.m) _
                 .Select(Function(g) g) _
                 .ToArray()

            Dim gx As Double() = grouped.Select(Function(g) g.Key).OrderBy(Function(v) v).ToArray()
            Dim gy As Double() = gx.Select(Function(m) median(grouped.First(Function(g) g.Key = m))).ToArray()

            Dim smooth As Double() = Lowess.Fit(gx, gy)
            If smooth.Length <> gx.Length Then Return smooth

            ' 映射回原始 (means, variances) 顺序
            Dim lookup As New Dictionary(Of Double, Double)()
            For i As Integer = 0 To gx.Length - 1
                lookup(gx(i)) = smooth(i)
            Next

            Dim result As Double() = New Double(x.Length - 1) {}
            For i As Integer = 0 To x.Length - 1
                Dim value As Double = 0.0
                If lookup.TryGetValue(x(i), value) Then
                    result(i) = value
                Else
                    result(i) = y(i)
                End If
            Next
            Return result
        End Function

        ''' <summary>
        ''' 中位数。
        ''' </summary>
        Public Function median(values As IEnumerable(Of Double)) As Double
            Dim sorted As Double() = values.OrderBy(Function(v) v).ToArray()
            If sorted.Length = 0 Then Return 0.0
            Dim mid As Integer = sorted.Length \ 2
            If sorted.Length Mod 2 = 1 Then Return sorted(mid)
            Return (sorted(mid - 1) + sorted(mid)) / 2.0
        End Function

    End Module

End Namespace
