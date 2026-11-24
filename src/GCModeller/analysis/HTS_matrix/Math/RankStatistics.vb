Imports System.Runtime.CompilerServices
Imports std = System.Math

''' <summary>
''' 与 R 语义一致的秩/分位数基元，供 TMM、分位数归一化与 limma 的秩裁剪逻辑复用。
''' </summary>
Public Module RankStatistics

    ''' <summary>
    ''' 秩（并列取平均秩），等价于 R 的 <c>rank(x)</c>（默认 <c>ties.method="average"</c>）
    ''' </summary>
    ''' <param name="values">待求秩的数值序列</param>
    ''' <returns>与输入顺序一致的秩（从 1 开始）</returns>
    Public Function AverageRanks(values As Double()) As Double()
        Dim n As Integer = values.Length
        Dim ranks As Double() = New Double(n - 1) {}
        Dim order As Integer() = Enumerable.Range(0, n) _
            .OrderBy(Function(idx) values(idx)) _
            .ToArray

        Dim i As Integer = 0

        While i < n
            Dim j As Integer = i

            While j + 1 < n AndAlso values(order(j + 1)) = values(order(j))
                j += 1
            End While

            Dim average As Double = (i + j) / 2.0 + 1.0

            For k As Integer = i To j
                ranks(order(k)) = average
            Next

            i = j + 1
        End While

        Return ranks
    End Function

    ''' <summary>
    ''' 分位数，等价于 R 的 <c>quantile(x, p)</c>（默认 <c>type = 7</c>）
    ''' </summary>
    ''' <param name="sortedValues">已经升序排序的数值序列（长度大于 0）</param>
    ''' <param name="p">概率，取值 [0,1]</param>
    Public Function QuantileType7(sortedValues As Double(), p As Double) As Double
        Dim n As Integer = sortedValues.Length

        If n = 0 Then
            Return Double.NaN
        ElseIf n = 1 Then
            Return sortedValues(0)
        End If

        Dim h As Double = (n - 1) * p
        Dim lo As Integer = CInt(std.Floor(h))

        If lo >= n - 1 Then
            Return sortedValues(n - 1)
        End If

        Dim frac As Double = h - lo

        Return sortedValues(lo) * (1.0 - frac) + sortedValues(lo + 1) * frac
    End Function

    ''' <summary>
    ''' 升序副本
    ''' </summary>
    <Extension>
    Public Function SortedAscending(values As Double()) As Double()
        Dim copy As Double() = DirectCast(values.Clone(), Double())

        Call Array.Sort(copy)

        Return copy
    End Function

    ''' <summary>
    ''' 中位数，等价于 R 的 <c>median(x)</c>
    ''' </summary>
    <Extension>
    Public Function MedianOf(values As Double()) As Double
        Dim n As Integer = values.Length

        If n = 0 Then
            Return Double.NaN
        End If

        Dim sorted As Double() = values.SortedAscending

        If n Mod 2 = 1 Then
            Return sorted(n \ 2)
        Else
            Return (sorted(n \ 2 - 1) + sorted(n \ 2)) / 2.0
        End If
    End Function

End Module
