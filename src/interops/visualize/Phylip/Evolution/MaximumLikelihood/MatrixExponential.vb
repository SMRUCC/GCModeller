Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Decomposition

Namespace Evolution.MaximumLikelihood

    ''' <summary>
    ''' 时间可逆（time-reversible）替换模型的转移概率矩阵计算：<c>P(t) = e^{Qt}</c>。
    ''' </summary>
    ''' <remarks>
    ''' 基础数学库中没有矩阵指数实现，这里利用时间可逆模型的对称性把 Q 对称化之后再做一次特征分解：
    '''
    ''' 令 <c>D = diag(π)</c>，<c>B = D^{1/2} Q D^{-1/2}</c> 为对称矩阵（因为 <c>Q_ij = S_ij π_j</c> 且
    ''' 交换率矩阵 S 对称），于是：
    '''
    ''' <c>Q = D^{-1/2} B D^{1/2}</c>，<c>B = U Λ Uᵀ</c>，<c>P(t) = e^{Qt} = D^{-1/2} U diag(e^{λt}) Uᵀ D^{1/2}</c>
    '''
    ''' 这样每个替换模型只需要做**一次**特征分解，之后各分支可以按分支长度 t 快速求出 P(t)。
    ''' 特征分解复用基础库 <see cref="MatrixOps.JacobiEigen"/>（Jacobi 旋转法，专门针对对称矩阵）。
    ''' </remarks>
    Public Class ReversibleMatrixExponential

        Private ReadOnly _n As Integer
        Private ReadOnly _lambda As Double()
        Private ReadOnly _a As Double()()      ' A(i,k) = U(i,k) / sqrt(pi_i)
        Private ReadOnly _b As Double()()      ' B(k,j) = U(j,k) * sqrt(pi_j)

        ''' <summary>
        ''' 特征值（应为非正值，其中一个为 0）
        ''' </summary>
        Public ReadOnly Property Eigenvalues As Double()
            Get
                Return _lambda
            End Get
        End Property

        Public ReadOnly Property Dimension As Integer
            Get
                Return _n
            End Get
        End Property

        ''' <summary>
        ''' 由对称交换率矩阵（对角线为零）与平衡频率构建。
        ''' </summary>
        ''' <param name="exchangeability">对称的交换率矩阵 S（对角为 0）</param>
        ''' <param name="pi">平衡频率向量（和为 1，全部为正）</param>
        Public Sub New(exchangeability As Double()(), pi As Double())
            Dim n As Integer = pi.Length
            Me._n = n

            Dim sqrtPi(n - 1) As Double

            For i As Integer = 0 To n - 1
                If pi(i) <= 0 Then
                    Throw New ArgumentException($"平衡频率必须为正数（第 {i} 个为 {pi(i)}）。")
                End If

                sqrtPi(i) = Math.Sqrt(pi(i))
            Next

            ' B(i,j) = S(i,j) * sqrt(pi_i * pi_j)（i ≠ j），对角线保持与 Q 一致
            ' B(i,i) = Q(i,i) = -Σ_{j≠i} S(i,j) π_j
            Dim bmat(n - 1)() As Double

            For i As Integer = 0 To n - 1
                bmat(i) = New Double(n - 1) {}

                Dim diagonal As Double = 0

                For j As Integer = 0 To n - 1
                    If i <> j Then
                        bmat(i)(j) = exchangeability(i)(j) * sqrtPi(i) * sqrtPi(j)
                        diagonal += exchangeability(i)(j) * pi(j)
                    End If
                Next

                bmat(i)(i) = -diagonal
            Next

            ' 对称矩阵的特征分解（JAMA 移植的 tred2/tql2）
            Dim eigen As EigenvalueDecomposition = New NumericMatrix(bmat).Eigen()
            Dim lambda As Double() = eigen.RealEigenvalues
            Dim u As NumericMatrix = eigen.V

            Dim a(n - 1)() As Double
            Dim b(n - 1)() As Double

            For i As Integer = 0 To n - 1
                a(i) = New Double(n - 1) {}
                b(i) = New Double(n - 1) {}
            Next

            For i As Integer = 0 To n - 1
                For k As Integer = 0 To n - 1
                    a(i)(k) = u(i, k) / sqrtPi(i)
                Next
            Next

            For k As Integer = 0 To n - 1
                For j As Integer = 0 To n - 1
                    b(k)(j) = u(j, k) * sqrtPi(j)
                Next
            Next

            Me._lambda = lambda
            Me._a = a
            Me._b = b
        End Sub

        ''' <summary>
        ''' 计算转移概率矩阵 <c>P(t)</c>。
        ''' </summary>
        Public Function Evaluate(t As Double) As Double()()
            Dim n As Integer = _n
            Dim e(n - 1) As Double

            For k As Integer = 0 To n - 1
                e(k) = Math.Exp(_lambda(k) * t)
            Next

            Dim p(n - 1)() As Double

            For i As Integer = 0 To n - 1
                p(i) = New Double(n - 1) {}

                For j As Integer = 0 To n - 1
                    Dim sum As Double = 0

                    For k As Integer = 0 To n - 1
                        sum += _a(i)(k) * e(k) * _b(k)(j)
                    Next

                    ' 数值噪声可能产生极小的负概率
                    p(i)(j) = If(sum < 0, 0, sum)
                Next

                ' 行归一化，保证概率矩阵的数值稳定性
                Dim rowSum As Double = 0

                For j As Integer = 0 To n - 1
                    rowSum += p(i)(j)
                Next

                If rowSum > 0 Then
                    For j As Integer = 0 To n - 1
                        p(i)(j) /= rowSum
                    Next
                End If
            Next

            Return p
        End Function
    End Class

End Namespace
