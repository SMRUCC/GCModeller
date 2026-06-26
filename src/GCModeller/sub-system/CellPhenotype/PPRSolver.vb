Imports System.IO
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Parallel

''' <summary>
''' Personalized PageRank（PPR）+ 代谢扩散稳态计算
''' </summary>
Public Module PPRSolver

    Private Class VectorSum : Inherits VectorTask

        Friend piNew As Double()
        Friend pi As Double()
        Friend P As Double(,)

        Public Sub New(pi As Double(), p As Double(,), Optional verbose As Boolean = False, Optional workers As Integer? = Nothing)
            MyBase.New(pi.Length, verbose, workers)

            Me.pi = pi
            Me.P = p
            Me.piNew = New Double(pi.Length - 1) {}
        End Sub

        Protected Overrides Sub Solve(start As Integer, ends As Integer, cpu_id As Integer)
            Dim n As Integer = P.GetLength(0)

            For j As Integer = start To ends
                Dim pnew As Double = 0

                For i As Integer = 0 To n - 1
                    pnew += pi(i) * P(i, j)
                Next

                piNew(j) = pnew
            Next
        End Sub
    End Class

    ''' <summary>
    ''' 计算封闭代谢网络的全局稳态浓度分布（质量守恒）
    ''' </summary>
    ''' <remarks>
    ''' 认为物质在 network 中永不消耗，只是不断循环反应，直到完全混合。封闭系统充分混合后的稳态只取决于网络结构。
    ''' </remarks>
    Public Function ComputeSteadyStateClosed(net As MetabolicNetwork, Optional totalMass As Double = 999, Optional maxItrs As Integer = 10000) As Double()
        Dim P(,) As Double = net.BuildRowStochasticMatrix()
        Dim n As Integer = P.GetLength(0)
        ' 幂迭代法求左特征向量 (平稳分布 pi^T = pi^T * P)
        Dim pi(n - 1) As Double

        For i As Integer = 0 To n - 1
            pi(i) = 1.0 / n ' 初始随机分布
        Next

        Dim iter As Integer = 0

        Do
            Dim piNew As Double()

            If n < 1000 Then
                Dim piNew_1(n - 1) As Double

                For j As Integer = 0 To n - 1
                    For i As Integer = 0 To n - 1
                        piNew_1(j) += pi(i) * P(i, j)
                    Next
                Next

                piNew = piNew_1
            Else
                Dim vecSum As New VectorSum(pi, P)
                Call vecSum.Run()
                piNew = vecSum.piNew
            End If

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
        Return New Vector(pi) * totalMass
    End Function

    ''' <summary>
    ''' 计算 Personalized PageRank 稳态分布
    ''' </summary>
    ''' <param name="net">行随机化转移矩阵</param>
    ''' <param name="seedNode">种子节点索引</param>
    ''' <param name="alpha">阻尼因子（通常 0.85）</param>
    ''' <param name="tolerance">收敛阈值</param>
    ''' <remarks>
    ''' 用途：同位素示踪 / 碳流 / 模拟 营养注入点
    ''' </remarks>
    Public Function ComputePPR(net As MetabolicNetwork, seedNode As Integer, Optional alpha As Double = 0.85, Optional tolerance As Double = 0.00000001) As Double()
        Dim P(,) As Double = net.BuildRowStochasticMatrix
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
    Public Function SolveWithDrain(net As MetabolicNetwork, seedNode As Integer, drain() As Double, Optional alpha As Double = 0.85, Optional maxItrs As Integer = 10000) As Double()
        Dim P(,) As Double = net.BuildRowStochasticMatrix
        Dim n As Integer = P.GetLength(0)

        If drain Is Nothing Then
            Throw New NullReferenceException("drain should not be nothing!")
        End If
        If drain.Length <> n Then
            Throw New InvalidDataException($"the network metabolite size({n}) is not matched with the size of drain({drain.Length}) vector!")
        End If

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
        For iter As Integer = 1 To maxItrs
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
                err += Math.Abs(xNew(i) - x(i))
            Next

            x = xNew

            If err < 0.0000000001 Then
                Exit For
            End If
        Next

        Return x
    End Function
End Module