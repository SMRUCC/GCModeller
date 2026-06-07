Imports std = System.Math

''' <summary>
''' Personalized PageRank（PPR）+ 代谢扩散稳态计算
''' </summary>
Public Module PPRSolver

    ''' <summary>
    ''' 将代谢网络转换为行随机化转移矩阵 P
    ''' </summary>
    Public Function BuildRowStochasticMatrix(net As MetabolicNetwork) As Double(,)
        Dim n As Integer = net.NodeCount
        Dim P(n - 1, n - 1) As Double

        For i As Integer = 0 To n - 1
            Dim totalOutWeight As Double = 0.0
            For Each edge In net.Adjacency(i)
                totalOutWeight += edge.Weight
            Next

            If totalOutWeight > 0 Then
                For Each edge In net.Adjacency(i)
                    P(i, edge.Target) = edge.Weight / totalOutWeight
                Next
            Else
                ' 无出边：自环（避免死端）
                P(i, i) = 1.0
            End If
        Next

        Return P
    End Function

    ''' <summary>
    ''' 计算 Personalized PageRank 稳态分布
    ''' </summary>
    ''' <param name="P">行随机化转移矩阵</param>
    ''' <param name="seedNode">种子节点索引</param>
    ''' <param name="alpha">阻尼因子（通常 0.85）</param>
    ''' <param name="tolerance">收敛阈值</param>
    Public Function ComputePPR(P As Double(,), seedNode As Integer, Optional alpha As Double = 0.85, Optional tolerance As Double = 0.00000001) As Double()
        Dim n As Integer = P.GetLength(0)
        Dim piPrev(n - 1) As Double
        Dim piCurr(n - 1) As Double
        Dim s(n - 1) As Double

        ' 初始化种子向量
        s(seedNode) = 1.0

        ' 初始分布
        For i As Integer = 0 To n - 1
            piPrev(i) = 1.0 / n
        Next

        ' 幂迭代
        Do
            ' piCurr = alpha * piPrev * P + (1 - alpha) * s
            For j As Integer = 0 To n - 1
                Dim sum As Double = 0.0
                For i As Integer = 0 To n - 1
                    sum += piPrev(i) * P(i, j)
                Next
                piCurr(j) = alpha * sum + (1 - alpha) * s(j)
            Next

            ' 检查收敛
            Dim diff As Double = 0.0
            For i As Integer = 0 To n - 1
                diff += Math.Abs(piCurr(i) - piPrev(i))
            Next

            If diff < tolerance Then Exit Do

            Array.Copy(piCurr, piPrev, n)
        Loop

        Return piCurr
    End Function

    ''' <summary>
    ''' 带 Drain 项的稳态解（简单 Jacobi 迭代）
    ''' </summary>
    Public Function SolveWithDrain(P As Double(,), seedNode As Integer, drain() As Double, Optional alpha As Double = 0.85) As Double()
        Dim n As Integer = P.GetLength(0)
        Dim x(n - 1) As Double
        Dim b(n - 1) As Double

        ' 右侧向量
        For i As Integer = 0 To n - 1
            b(i) = If(i = seedNode, 1.0 - alpha, 0.0)
        Next

        ' 初始化
        For i As Integer = 0 To n - 1
            x(i) = 1.0 / n
        Next

        ' Jacobi 迭代
        For iter As Integer = 1 To 10000
            Dim xNew(n - 1) As Double

            For i As Integer = 0 To n - 1
                Dim sum As Double = 0.0
                For j As Integer = 0 To n - 1
                    If i <> j Then
                        sum += alpha * P(j, i) * x(j)
                    End If
                Next
                Dim diag = 1.0 + drain(i) - alpha * P(i, i)
                xNew(i) = (b(i) + sum) / diag
            Next

            Dim err As Double = 0.0
            For i As Integer = 0 To n - 1
                err += std.Abs(xNew(i) - x(i))
            Next

            x = xNew
            If err < 0.0000000001 Then Exit For
        Next

        Return x
    End Function
End Module