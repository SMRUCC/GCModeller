Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Statistics
Imports Microsoft.VisualBasic.DataMining.Kernel.BayesianBeliefNetwork

Namespace Bayesian.Tree

    ''' <summary>
    ''' 用 Matrix-Tree 定理(基尔霍夫矩阵树定理)对全部生成树求和。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 论文要求对依赖树 <c>T</c> 的边缘化而不是取最大：
    ''' </para>
    ''' <code>
    ''' P(D | a) = Σ_T P(D | a, T) = [ Π_i P(D_i) ] · Σ_T Π_(i,j)∈T R_ij
    ''' </code>
    ''' <para>
    ''' 其中对生成树的求和可以用 Matrix-Tree 定理转化为一个行列式：以 <c>R_ij</c> 为边权构造
    ''' Laplacian 矩阵 <c>M</c>(对角线为行和、非对角线为 <c>-R_ij</c>)，则
    ''' <c>Σ_T Π R_ij = det(M_reduced)</c>，<c>M_reduced</c> 是去掉任意一行一列之后的余子式。
    ''' 于是 <c>P(a | D) ∝ det(M)</c>。
    ''' </para>
    ''' <para>
    ''' <b>数值稳定性</b>：<c>R_ij</c> 的量级会随样本数指数增长(数百条序列时可达 <c>e^100</c>)，
    ''' 直接构造 Laplacian 会立刻溢出。这里先把所有边权统一除以 <c>exp(max ln R)</c> 缩放到
    ''' <c>[0, 1]</c>，求出余子式的对数行列式之后再补回 <c>(n-1)·max ln R</c>。
    ''' 这一步既保证了数值稳定，又保持了结果的严格等价。
    ''' </para>
    ''' </remarks>
    Public Class MatrixTree

        ''' <summary>
        ''' 计算 <c>ln Σ_T Π_(i,j)∈T R_ij</c>
        ''' </summary>
        ''' <param name="logR">似然比矩阵(对数形式)</param>
        ''' <param name="variables">参与求和的列集合，为 <c>Nothing</c> 的时候使用全部列</param>
        Public Shared Function LogTreeSum(logR As LikelihoodRatioMatrix, Optional variables As Integer() = Nothing) As Double
            If logR Is Nothing Then
                Throw New ArgumentNullException(NameOf(logR))
            End If

            Dim nodes As Integer() = If(variables, Enumerable.Range(0, logR.Size).ToArray())

            If nodes.Length <= 1 Then
                ' 只有一个节点时只有一个「空树」，其乘积为 1
                Return 0.0
            End If

            Dim logWeights As Double(,) = New Double(nodes.Length - 1, nodes.Length - 1) {}

            For i As Integer = 0 To nodes.Length - 1
                For j As Integer = 0 To nodes.Length - 1
                    logWeights(i, j) = If(i = j, 0.0, logR.LogR(nodes(i), nodes(j)))
                Next
            Next

            Return LogTreeSum(logWeights)
        End Function

        ''' <summary>
        ''' 在给定的对数边权矩阵之上计算 <c>ln Σ_T Π R_ij</c>
        ''' </summary>
        ''' <param name="logWeights">
        ''' 对称的对数边权矩阵，对角线元素被忽略
        ''' </param>
        Public Shared Function LogTreeSum(logWeights As Double(,)) As Double
            If logWeights Is Nothing Then
                Throw New ArgumentNullException(NameOf(logWeights))
            End If

            Dim n As Integer = logWeights.GetLength(0)

            If n <> logWeights.GetLength(1) Then
                Throw New ArgumentException("the weight matrix must be square", NameOf(logWeights))
            End If
            If n <= 1 Then
                Return 0.0
            End If

            ' 找到最大的对数边权，用于数值缩放
            Dim maxLog As Double = Double.NegativeInfinity

            For i As Integer = 0 To n - 2
                For j As Integer = i + 1 To n - 1
                    Dim w As Double = logWeights(i, j)

                    If Not Double.IsNegativeInfinity(w) AndAlso w > maxLog Then
                        maxLog = w
                    End If
                Next
            Next

            If Double.IsNegativeInfinity(maxLog) Then
                ' 所有边权都是 0：不存在任何有正权重的生成树
                Return Double.NegativeInfinity
            End If

            ' 用「最大生成树的平均边权」作为缩放因子，而不是单条边的最大权重。
            ' 只按单条边的最大值缩放会让其余边在 R_ij 的量级很大时全部下溢为 0，
            ' 使 Laplacian 近似奇异、行列式严重失真。以最大生成树的平均边权缩放之后，
            ' 最优树上的权重都落在 1 附近，矩阵条件数良好，而且 log Σ_T Π R_ij
            ' 必然不小于最大生成树的权重(因为最优树那一项的缩放乘积恰好为 1)。
            Dim reference As Double = maxLog

            If n > 2 Then
                Dim tree As DependencyTree = DependencyTree.Prim(logWeights)
                reference = tree.TotalLogWeight / (n - 1)

                If Double.IsNegativeInfinity(reference) OrElse Double.IsNaN(reference) Then
                    reference = maxLog
                End If
            End If

            Dim laplacian As Double(,) = New Double(n - 2, n - 2) {}

            For i As Integer = 0 To n - 2
                Dim diagonal As Double = 0.0

                For j As Integer = 0 To n - 1
                    If i = j Then
                        Continue For
                    End If

                    Dim scaled As Double = Scale(logWeights(i, j), reference)
                    diagonal += scaled

                    If j <= n - 2 Then
                        laplacian(i, j) = -scaled
                    End If
                Next

                laplacian(i, i) = diagonal
            Next

            ' Laplacian 的余子式是对称正定的，因此使用 Cholesky 分解计算对数行列式，
            ' 这样在边权跨越多个数量级的时候也不会出现相消误差。
            Dim logDet As Double = NumericLogSpace.LogDetSpd(laplacian)

            If Double.IsNegativeInfinity(logDet) Then
                Return Double.NegativeInfinity
            End If

            Return logDet + (n - 1) * reference
        End Function

        ''' <summary>
        ''' 计算 <c>Σ_T Π_(i,j)∈T R_ij</c> 的线性值
        ''' </summary>
        Public Shared Function TreeSum(logR As LikelihoodRatioMatrix, Optional variables As Integer() = Nothing) As Double
            Return SafeExp(LogTreeSum(logR, variables))
        End Function

        ''' <summary>
        ''' 直接枚举全部生成树求和，仅用于小规模数值对照测试
        ''' </summary>
        Public Shared Function BruteForceTreeSum(logWeights As Double(,)) As Double
            Dim n As Integer = logWeights.GetLength(0)

            If n <= 1 Then
                Return 1.0
            End If
            If n > 7 Then
                Throw New ArgumentException("brute force enumeration is limited to at most 7 nodes")
            End If

            Dim pairs As New List(Of (i As Integer, j As Integer))

            For i As Integer = 0 To n - 2
                For j As Integer = i + 1 To n - 1
                    Call pairs.Add((i, j))
                Next
            Next

            Dim total As Double = 0.0

            For mask As Long = 0 To (1L << pairs.Count) - 1
                Dim selected As New List(Of Integer)

                For k As Integer = 0 To pairs.Count - 1
                    If (mask >> k And 1L) = 1L Then
                        Call selected.Add(k)
                    End If
                Next

                If selected.Count <> n - 1 Then
                    Continue For
                End If

                Dim parent As Integer() = New Integer(n - 1) {}

                For i As Integer = 0 To n - 1
                    parent(i) = i
                Next

                Dim product As Double = 1.0
                Dim valid As Boolean = True

                For Each k As Integer In selected
                    Dim a As Integer = Root(parent, pairs(k).i)
                    Dim b As Integer = Root(parent, pairs(k).j)

                    If a = b Then
                        valid = False
                        Exit For
                    End If

                    parent(a) = b
                    product *= Math.Exp(logWeights(pairs(k).i, pairs(k).j))
                Next

                If valid Then
                    total += product
                End If
            Next

            Return total
        End Function

        Private Shared Function Root(parent As Integer(), x As Integer) As Integer
            While parent(x) <> x
                x = parent(x)
            End While

            Return x
        End Function

        Private Shared Function Scale(logWeight As Double, maxLog As Double) As Double
            If Double.IsNegativeInfinity(logWeight) Then
                Return 0.0
            Else
                Return Math.Exp(logWeight - maxLog)
            End If
        End Function

        Private Shared Function SafeExp(value As Double) As Double
            If Double.IsNegativeInfinity(value) Then
                Return 0.0
            ElseIf value > 709.0 Then
                ' 超出双精度浮点数的表示范围，返回正无穷以明确表示「溢出了」
                Return Double.PositiveInfinity
            Else
                Return Math.Exp(value)
            End If
        End Function
    End Class
End Namespace
