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
    ''' 计算封闭代谢网络的全局稳态浓度分布（质量守恒）
    ''' </summary>
    ''' <remarks>
    ''' 认为物质在 network 中永不消耗，只是不断循环反应，直到完全混合。封闭系统充分混合后的稳态只取决于网络结构。
    ''' </remarks>
    Public Function ComputeSteadyStateClosed(net As MetabolicNetwork, totalMass As Double, Optional maxItrs As Integer = 10000) As Double()
        Dim n As Integer = net.NodeCount
        Dim P(,) As Double = BuildRowStochasticMatrix(net) ' 复用你原有的矩阵构建函数
        ' 幂迭代法求左特征向量 (平稳分布 pi^T = pi^T * P)
        Dim pi(n - 1) As Double

        For i As Integer = 0 To n - 1
            pi(i) = 1.0 / n ' 初始随机分布
        Next

        Dim iter As Integer = 0

        Do
            Dim piNew(n - 1) As Double
            For j As Integer = 0 To n - 1
                For i As Integer = 0 To n - 1
                    piNew(j) += pi(i) * P(i, j)
                Next
            Next

            ' 归一化以保证概率总和为1
            Dim sum As Double = piNew.Sum()
            For j As Integer = 0 To n - 1
                piNew(j) /= sum
            Next

            ' 收敛判断
            Dim diff As Double = 0.0
            For i As Integer = 0 To n - 1
                diff += Math.Abs(piNew(i) - pi(i))
            Next

            If diff < 0.00000001 OrElse iter > maxItrs Then
                Exit Do
            Else
                pi = piNew
            End If
        Loop

        ' 乘以初始总质量
        For i As Integer = 0 To n - 1
            pi(i) *= totalMass
        Next

        Return pi
    End Function

End Module