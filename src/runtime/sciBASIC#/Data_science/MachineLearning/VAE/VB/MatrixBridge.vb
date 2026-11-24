' ============================================================================
' MatrixBridge.vb - 张量与数值矩阵之间的桥接层
'
' 变分贝叶斯模型的公共数据接口统一使用 Tensor（与项目内 GMM/GMVAE 一致），
' 而矩阵分解（求逆 / 行列式 / Cholesky）则下沉到 LinearAlgebra.Matrix 的
' NumericMatrix，以获得更稳定的数值表现。本模块是两者之间唯一的转换边界，
' 模型层不直接引用 LinearAlgebra.Matrix。
'
' 转换只发生在参数更新与下界评估的边界（每次迭代 O(K·D²) 次），
' 不会出现在逐样本的循环内部，因此不构成性能热点。
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports std = System.Math

Namespace VariationalBayes

    ''' <summary>
    ''' <see cref="Tensor"/> 与 <see cref="NumericMatrix"/> 之间的无状态转换与线性代数辅助。
    ''' </summary>
    Public Module MatrixBridge

        ''' <summary>
        ''' 把一个二维张量转换为 <see cref="NumericMatrix"/>。
        ''' </summary>
        ''' <exception cref="ArgumentException">张量的秩不等于 2 的时候抛出</exception>
        Public Function ToMatrix(a As Tensor) As NumericMatrix
            If a Is Nothing Then
                Throw New ArgumentNullException(NameOf(a))
            End If

            If a.Rank <> 2 Then
                Throw New ArgumentException($"需要二维张量 [N, D]，实际秩为 {a.Rank}", NameOf(a))
            End If

            Return New NumericMatrix(a.To2DArrayDouble())
        End Function

        ''' <summary>
        ''' 从一个 <see cref="GeneralMatrix"/> 构造 <see cref="Tensor"/>（行优先展平）。
        ''' </summary>
        Public Function FromMatrix(a As GeneralMatrix) As Tensor
            If a Is Nothing Then
                Throw New ArgumentNullException(NameOf(a))
            End If

            Dim rows As Integer = a.RowDimension
            Dim cols As Integer = a.ColumnDimension
            Dim buffer As Double()() = a.ArrayPack(deepcopy:=False)
            Dim data As Double() = New Double(rows * cols - 1) {}

            For i As Integer = 0 To rows - 1
                Dim row As Double() = buffer(i)

                For j As Integer = 0 To cols - 1
                    data(i * cols + j) = row(j)
                Next
            Next

            Return New Tensor(data, rows, cols)
        End Function

        ''' <summary>
        ''' 二维张量转矩形数组。
        ''' </summary>
        Public Function ToRectangular(a As Tensor) As Double(,)
            If a Is Nothing Then
                Throw New ArgumentNullException(NameOf(a))
            End If

            If a.Rank <> 2 Then
                Throw New ArgumentException($"需要二维张量 [N, D]，实际秩为 {a.Rank}", NameOf(a))
            End If

            Return a.To2DArrayDouble()
        End Function

        ''' <summary>
        ''' 矩形数组转锯齿数组（<see cref="NumericMatrix"/> 的构造需要）。
        ''' </summary>
        Public Function ToJagged(a As Double(,)) As Double()()
            If a Is Nothing Then
                Throw New ArgumentNullException(NameOf(a))
            End If

            Dim rows As Integer = a.GetLength(0)
            Dim cols As Integer = a.GetLength(1)
            Dim result As Double()() = New Double(rows - 1)() {}

            For i As Integer = 0 To rows - 1
                Dim row As Double() = New Double(cols - 1) {}

                For j As Integer = 0 To cols - 1
                    row(j) = a(i, j)
                Next

                result(i) = row
            Next

            Return result
        End Function

        ''' <summary>
        ''' 二维数组转张量。
        ''' </summary>
        Public Function FromRectangular(a As Double(,)) As Tensor
            If a Is Nothing Then
                Throw New ArgumentNullException(NameOf(a))
            End If

            Return New Tensor(a)
        End Function

        ''' <summary>
        ''' 方阵的行列式的对数值 ln|A|。
        ''' </summary>
        ''' <remarks>
        ''' 优先尝试 Cholesky 分解（正定矩阵的情形），失败之后回退到 LU 分解。
        ''' 奇异矩阵返回 <see cref="Double.NegativeInfinity"/>，表示该方向的精度为 0。
        ''' </remarks>
        Public Function LogDet(a As Tensor) As Double
            Dim matrix As NumericMatrix = ToMatrix(a)
            Dim n As Integer = matrix.RowDimension

            If n <> matrix.ColumnDimension Then
                Throw New ArgumentException("行列式要求方阵", NameOf(a))
            End If

            If n = 0 Then
                Return 0
            End If

            Try
                Dim chol As New CholeskyDecomposition(matrix)

                If chol.SPD Then
                    Dim l As GeneralMatrix = chol.GetL()
                    Dim acc As Double = 0

                    For i As Integer = 0 To n - 1
                        Dim value As Double = std.Abs(l(i, i))

                        If value <= 0 Then
                            Exit Try
                        End If

                        acc += std.Log(value)
                    Next

                    Return 2.0R * acc
                End If
            Catch ex As Exception
                ' 非正定矩阵，回退到 LU
            End Try

            Dim lu As New LUDecomposition(matrix)
            Dim det As Double = lu.Determinant()

            If std.Abs(det) < VBMath.TinyValue Then
                Return Double.NegativeInfinity
            End If

            Return std.Log(std.Abs(det))
        End Function

        ''' <summary>
        ''' 判断一个方阵是否正定（通过 Cholesky 分解）。
        ''' </summary>
        Public Function IsPositiveDefinite(a As Tensor) As Boolean
            Try
                Dim matrix As NumericMatrix = ToMatrix(a)

                If matrix.RowDimension <> matrix.ColumnDimension Then
                    Return False
                End If

                Dim chol As New CholeskyDecomposition(matrix)

                Return chol.SPD
            Catch ex As Exception
                Return False
            End Try
        End Function

        ''' <summary>
        ''' 方阵求逆；当矩阵奇异的时候先做对角线抖动，再失败则回退到
        ''' <see cref="MathUtils.MatrixInverse"/> 的高斯-约当实现。
        ''' </summary>
        ''' <param name="a">需要求逆的方阵</param>
        ''' <param name="jitter">对角线抖动的幅度</param>
        Public Function InverseOrPseudo(a As Tensor, Optional jitter As Double = VBMath.DefaultJitter) As Tensor
            Dim matrix As NumericMatrix = ToMatrix(a)
            Dim n As Integer = matrix.RowDimension

            If n <> matrix.ColumnDimension Then
                Throw New ArgumentException("矩阵求逆要求方阵", NameOf(a))
            End If

            Try
                Return FromMatrix(matrix.Inverse())
            Catch ex As Exception
                ' 奇异矩阵，继续尝试抖动
            End Try

            Dim jittered As Double(,) = New Double(n - 1, n - 1) {}

            For i As Integer = 0 To n - 1
                For j As Integer = 0 To n - 1
                    jittered(i, j) = matrix(i, j)
                Next

                jittered(i, i) += jitter
            Next

            Try
                Return FromMatrix(New NumericMatrix(jittered).Inverse())
            Catch ex As Exception
                ' 抖动之后仍然奇异，使用项目内已有的高斯-约当实现作为最后回退
                Return MathUtils.MatrixInverse(New Tensor(jittered))
            End Try
        End Function

        ''' <summary>
        ''' 矩形数组形式的 <see cref="LogDet"/> 重载。
        ''' </summary>
        Public Function LogDet(a As Double(,)) As Double
            If a Is Nothing Then
                Throw New ArgumentNullException(NameOf(a))
            End If

            Return LogDet(New Tensor(a))
        End Function

        ''' <summary>
        ''' 矩形数组形式的 <see cref="InverseOrPseudo"/> 重载。
        ''' </summary>
        Public Function InverseOrPseudo(a As Double(,), Optional jitter As Double = VBMath.DefaultJitter) As Double(,)
            If a Is Nothing Then
                Throw New ArgumentNullException(NameOf(a))
            End If

            Return InverseOrPseudo(New Tensor(a), jitter).To2DArrayDouble()
        End Function

        ''' <summary>
        ''' 单精度标量转双精度的安全转换（避免 <c>Option Strict Off</c> 下隐式转换的不确定性）。
        ''' </summary>
        Public Function ToDouble(value As Single) As Double
            Return CDbl(value)
        End Function

        ''' <summary>
        ''' 判断张量之中是否含有 NaN 或者无穷大。
        ''' </summary>
        Public Function HasNaNOrInfinity(a As Tensor) As Boolean
            If a Is Nothing Then
                Return True
            End If

            Dim data As Double() = a.Data

            For i As Integer = 0 To data.Length - 1
                If Double.IsNaN(data(i)) OrElse Double.IsInfinity(data(i)) Then
                    Return True
                End If
            Next

            Return False
        End Function

    End Module

End Namespace
