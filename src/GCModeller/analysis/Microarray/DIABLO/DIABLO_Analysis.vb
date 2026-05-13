' ============================================================================
' DIABLO_Analysis.vb
' ============================================================================
' VB.NET 实现的 DIABLO (Data Integration Analysis for Biomarker discovery
' using Latent cOmponents) 多组学数据关联分析数学计算模块
'
' 参考: R语言 mixOmics 程序包中的 block.splsda / DIABLO 方法
' 论文: Singh A, et al. DIABLO: an integrative approach for identifying
'       key molecular drivers from multi-omics studies. (2019)
'
' 本模块仅实现数学算法计算核心，不包含绘图功能
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Math

Namespace MixOmics

    ' ========================================================================
    ' 1. 基础矩阵运算库
    ' ========================================================================

    ''' <summary>
    ''' 二维矩阵类，提供基础线性代数运算
    ''' 支持矩阵创建、运算、分解等操作
    ''' </summary>
    Public Class Matrix
        Private _data As Double(,)
        Private _rows As Integer
        Private _cols As Integer

        ''' <summary>矩阵行数</summary>
        Public ReadOnly Property Rows As Integer
            Get
                Return _rows
            End Get
        End Property

        ''' <summary>矩阵列数</summary>
        Public ReadOnly Property Cols As Integer
            Get
                Return _cols
            End Get
        End Property

        ''' <summary>矩阵元素访问器</summary>
        Default Public Property Item(i As Integer, j As Integer) As Double
            Get
                Return _data(i, j)
            End Get
            Set(value As Double)
                _data(i, j) = value
            End Set
        End Property

        ''' <summary>获取内部二维数组引用</summary>
        Public ReadOnly Property Data As Double(,)
            Get
                Return _data
            End Get
        End Property

        ''' <summary>构造 n×m 零矩阵</summary>
        Public Sub New(n As Integer, m As Integer)
            _rows = n
            _cols = m
            _data = New Double(n - 1, m - 1) {}
        End Sub

        ''' <summary>从二维数组构造矩阵</summary>
        Public Sub New(data As Double(,))
            _rows = data.GetLength(0)
            _cols = data.GetLength(1)
            _data = CType(data.Clone(), Double(,))
        End Sub

        ''' <summary>从一维数组构造列向量 (n×1 矩阵)</summary>
        Public Sub New(vec As Double())
            _rows = vec.Length
            _cols = 1
            _data = New Double(_rows - 1, 0) {}
            For i As Integer = 0 To _rows - 1
                _data(i, 0) = vec(i)
            Next
        End Sub

        ''' <summary>深拷贝矩阵</summary>
        Public Function Clone() As Matrix
            Return New Matrix(CType(_data.Clone(), Double(,)))
        End Function

        ''' <summary>矩阵加法: C = A + B</summary>
        Public Shared Operator +(A As Matrix, B As Matrix) As Matrix
            If A.Rows <> B.Rows OrElse A.Cols <> B.Cols Then
                Throw New ArgumentException("矩阵维度不匹配，无法相加")
            End If
            Dim C As New Matrix(A.Rows, A.Cols)
            For i As Integer = 0 To A.Rows - 1
                For j As Integer = 0 To A.Cols - 1
                    C(i, j) = A(i, j) + B(i, j)
                Next
            Next
            Return C
        End Operator

        ''' <summary>矩阵减法: C = A - B</summary>
        Public Shared Operator -(A As Matrix, B As Matrix) As Matrix
            If A.Rows <> B.Rows OrElse A.Cols <> B.Cols Then
                Throw New ArgumentException("矩阵维度不匹配，无法相减")
            End If
            Dim C As New Matrix(A.Rows, A.Cols)
            For i As Integer = 0 To A.Rows - 1
                For j As Integer = 0 To A.Cols - 1
                    C(i, j) = A(i, j) - B(i, j)
                Next
            Next
            Return C
        End Operator

        ''' <summary>标量乘法: C = alpha * A</summary>
        Public Shared Operator *(alpha As Double, A As Matrix) As Matrix
            Dim C As New Matrix(A.Rows, A.Cols)
            For i As Integer = 0 To A.Rows - 1
                For j As Integer = 0 To A.Cols - 1
                    C(i, j) = alpha * A(i, j)
                Next
            Next
            Return C
        End Operator

        ''' <summary>标量乘法: C = A * alpha</summary>
        Public Shared Operator *(A As Matrix, alpha As Double) As Matrix
            Return alpha * A
        End Operator

        ''' <summary>矩阵乘法: C = A * B</summary>
        Public Shared Operator *(A As Matrix, B As Matrix) As Matrix
            If A.Cols <> B.Rows Then
                Throw New ArgumentException($"矩阵维度不匹配，无法相乘: {A.Rows}x{A.Cols} * {B.Rows}x{B.Cols}")
            End If
            Dim C As New Matrix(A.Rows, B.Cols)
            For i As Integer = 0 To A.Rows - 1
                For j As Integer = 0 To B.Cols - 1
                    Dim sum As Double = 0.0
                    For k As Integer = 0 To A.Cols - 1
                        sum += A(i, k) * B(k, j)
                    Next
                    C(i, j) = sum
                Next
            Next
            Return C
        End Operator

        ''' <summary>矩阵取负: C = -A</summary>
        Public Shared Operator -(A As Matrix) As Matrix
            Dim C As New Matrix(A.Rows, A.Cols)
            For i As Integer = 0 To A.Rows - 1
                For j As Integer = 0 To A.Cols - 1
                    C(i, j) = -A(i, j)
                Next
            Next
            Return C
        End Operator

        ''' <summary>矩阵转置: A^T</summary>
        Public Function Transpose() As Matrix
            Dim T As New Matrix(_cols, _rows)
            For i As Integer = 0 To _rows - 1
                For j As Integer = 0 To _cols - 1
                    T(j, i) = _data(i, j)
                Next
            Next
            Return T
        End Function

        ''' <summary>提取第 col 列为列向量</summary>
        Public Function GetColumn(col As Integer) As Matrix
            Dim v As New Matrix(_rows, 1)
            For i As Integer = 0 To _rows - 1
                v(i, 0) = _data(i, col)
            Next
            Return v
        End Function

        ''' <summary>设置第 col 列的值</summary>
        Public Sub SetColumn(col As Integer, v As Matrix)
            For i As Integer = 0 To _rows - 1
                _data(i, col) = v(i, 0)
            Next
        End Sub

        ''' <summary>提取第 row 行为行向量</summary>
        Public Function GetRow(row As Integer) As Matrix
            Dim v As New Matrix(1, _cols)
            For j As Integer = 0 To _cols - 1
                v(0, j) = _data(row, j)
            Next
            Return v
        End Function

        ''' <summary>设置第 row 行的值</summary>
        Public Sub SetRow(row As Integer, v As Matrix)
            For j As Integer = 0 To _cols - 1
                _data(row, j) = v(0, j)
            Next
        End Sub

        ''' <summary>逐元素乘法 (Hadamard积)</summary>
        Public Function Hadamard(B As Matrix) As Matrix
            If _rows <> B.Rows OrElse _cols <> B.Cols Then
                Throw New ArgumentException("矩阵维度不匹配，无法进行逐元素乘法")
            End If
            Dim C As New Matrix(_rows, _cols)
            For i As Integer = 0 To _rows - 1
                For j As Integer = 0 To _cols - 1
                    C(i, j) = _data(i, j) * B(i, j)
                Next
            Next
            Return C
        End Function

        ''' <summary>计算 Frobenius 范数</summary>
        Public Function FrobeniusNorm() As Double
            Dim sum As Double = 0.0
            For i As Integer = 0 To _rows - 1
                For j As Integer = 0 To _cols - 1
                    sum += _data(i, j) * _data(i, j)
                Next
            Next
            Return Sqrt(sum)
        End Function

        ''' <summary>计算 L2 范数 (向量范数，适用于列向量)</summary>
        Public Function L2Norm() As Double
            Dim sum As Double = 0.0
            For i As Integer = 0 To _rows - 1
                For j As Integer = 0 To _cols - 1
                    sum += _data(i, j) * _data(i, j)
                Next
            Next
            Return Sqrt(sum)
        End Function

        ''' <summary>计算 L1 范数</summary>
        Public Function L1Norm() As Double
            Dim sum As Double = 0.0
            For i As Integer = 0 To _rows - 1
                For j As Integer = 0 To _cols - 1
                    sum += Abs(_data(i, j))
                Next
            Next
            Return sum
        End Function

        ''' <summary>归一化为单位向量 (L2范数=1)</summary>
        Public Function Normalize() As Matrix
            Dim n As Double = L2Norm()
            If n < 0.000000000000001 Then Return Clone()
            Return (1.0 / n) * Me
        End Function

        ''' <summary>计算列均值向量</summary>
        Public Function ColumnMeans() As Matrix
            Dim means As New Matrix(1, _cols)
            For j As Integer = 0 To _cols - 1
                Dim s As Double = 0.0
                For i As Integer = 0 To _rows - 1
                    s += _data(i, j)
                Next
                means(0, j) = s / _rows
            Next
            Return means
        End Function

        ''' <summary>计算行均值向量</summary>
        Public Function RowMeans() As Matrix
            Dim means As New Matrix(_rows, 1)
            For i As Integer = 0 To _rows - 1
                Dim s As Double = 0.0
                For j As Integer = 0 To _cols - 1
                    s += _data(i, j)
                Next
                means(i, 0) = s / _cols
            Next
            Return means
        End Function

        ''' <summary>计算列标准差向量</summary>
        Public Function ColumnStdDevs() As Matrix
            Dim means As Matrix = ColumnMeans()
            Dim stds As New Matrix(1, _cols)
            For j As Integer = 0 To _cols - 1
                Dim s As Double = 0.0
                For i As Integer = 0 To _rows - 1
                    s += (_data(i, j) - means(0, j)) ^ 2
                Next
                stds(0, j) = Sqrt(s / (_rows - 1))
                ' 防止除零
                If stds(0, j) < 0.000000000000001 Then stds(0, j) = 1.0
            Next
            Return stds
        End Function

        ''' <summary>中心化: 每列减去均值</summary>
        Public Function Center() As Matrix
            Dim means As Matrix = ColumnMeans()
            Dim result As Matrix = Clone()
            For i As Integer = 0 To _rows - 1
                For j As Integer = 0 To _cols - 1
                    result(i, j) -= means(0, j)
                Next
            Next
            Return result
        End Function

        ''' <summary>标准化: 中心化 + 缩放为单位方差</summary>
        Public Function Scale() As Matrix
            Dim centered As Matrix = Center()
            Dim stds As Matrix = centered.ColumnStdDevs()
            For i As Integer = 0 To _rows - 1
                For j As Integer = 0 To _cols - 1
                    centered(i, j) /= stds(0, j)
                Next
            Next
            Return centered
        End Function

        ''' <summary>矩阵的迹 (Trace)</summary>
        Public Function Trace() As Double
            If _rows <> _cols Then
                Throw New ArgumentException("迹只能对方阵计算")
            End If
            Dim sum As Double = 0.0
            For i As Integer = 0 To _rows - 1
                sum += _data(i, i)
            Next
            Return sum
        End Function

        ''' <summary>创建 n×n 单位矩阵</summary>
        Public Shared Function Identity(n As Integer) As Matrix
            Dim I As New Matrix(n, n)
            For i As Integer = 0 To n - 1
                i(i, i) = 1.0
            Next
            Return I
        End Function

        ''' <summary>创建 n×m 全1矩阵</summary>
        Public Shared Function Ones(n As Integer, m As Integer) As Matrix
            Dim M As New Matrix(n, M)
            For i As Integer = 0 To n - 1
                For j As Integer = 0 To M - 1
                    M(i, j) = 1.0
                Next
            Next
            Return M
        End Function

        ''' <summary>创建 n×m 全0矩阵</summary>
        Public Shared Function Zeros(n As Integer, m As Integer) As Matrix
            Return New Matrix(n, m)
        End Function

        ''' <summary>从多个列向量水平拼接矩阵</summary>
        Public Shared Function HConcat(matrices As Matrix()) As Matrix
            If matrices Is Nothing OrElse matrices.Length = 0 Then
                Throw New ArgumentException("输入矩阵数组不能为空")
            End If
            Dim totalCols As Integer = 0
            Dim nRows As Integer = matrices(0).Rows
            For Each m In matrices
                If m.Rows <> nRows Then
                    Throw New ArgumentException("所有矩阵的行数必须一致")
                End If
                totalCols += m.Cols
            Next
            Dim result As New Matrix(nRows, totalCols)
            Dim colOffset As Integer = 0
            For Each m In matrices
                For j As Integer = 0 To m.Cols - 1
                    For i As Integer = 0 To nRows - 1
                        result(i, colOffset + j) = m(i, j)
                    Next
                Next
                colOffset += m.Cols
            Next
            Return result
        End Function

        ''' <summary>从多个矩阵垂直拼接</summary>
        Public Shared Function VConcat(matrices As Matrix()) As Matrix
            If matrices Is Nothing OrElse matrices.Length = 0 Then
                Throw New ArgumentException("输入矩阵数组不能为空")
            End If
            Dim totalRows As Integer = 0
            Dim nCols As Integer = matrices(0).Cols
            For Each m In matrices
                If m.Cols <> nCols Then
                    Throw New ArgumentException("所有矩阵的列数必须一致")
                End If
                totalRows += m.Rows
            Next
            Dim result As New Matrix(totalRows, nCols)
            Dim rowOffset As Integer = 0
            For Each m In matrices
                For i As Integer = 0 To m.Rows - 1
                    For j As Integer = 0 To nCols - 1
                        result(rowOffset + i, j) = m(i, j)
                    Next
                Next
                rowOffset += m.Rows
            Next
            Return result
        End Function

        ''' <summary>提取子矩阵</summary>
        Public Function SubMatrix(startRow As Integer, endRow As Integer,
                                  startCol As Integer, endCol As Integer) As Matrix
            Dim r As Integer = endRow - startRow + 1
            Dim c As Integer = endCol - startCol + 1
            Dim sub As New Matrix(r, c)
            For i As Integer = 0 To r - 1
                For j As Integer = 0 To c - 1
                    Sub(i, j) = _data(startRow + i, startCol + j)
                Next
            Next
            Return Sub()
                       End Function

        ''' <summary>矩阵字符串表示 (调试用)</summary>
        Public Overrides Function ToString() As String
            Dim sb As New System.Text.StringBuilder()
            Dim maxRows As Integer = Math.Min(_rows, 10)
            Dim maxCols As Integer = Math.Min(_cols, 10)
            For i As Integer = 0 To maxRows - 1
                For j As Integer = 0 To maxCols - 1
                    sb.Append(_data(i, j).ToString("F6").PadLeft(12))
                Next
                If _cols > maxCols Then sb.Append("  ...")
                sb.AppendLine()
            Next
            If _rows > maxRows Then sb.AppendLine("...")
            Return sb.ToString()
        End Function

    End Class

    ' ========================================================================
    ' 2. 线性代数工具类
    ' ========================================================================

    ''' <summary>
    ''' 线性代数工具类，提供SVD分解、特征值分解等高级运算
    ''' </summary>
    Public Class LinearAlgebra

        ''' <summary>
        ''' 幂迭代法计算矩阵最大特征值及对应特征向量
        ''' 用于提取主成分方向
        ''' </summary>
        ''' <param name="A">对称方阵</param>
        ''' <param name="maxIter">最大迭代次数</param>
        ''' <param name="tol">收敛容差</param>
        ''' <returns>特征值和归一化特征向量</returns>
        Public Shared Function PowerIteration(A As Matrix,
                                             Optional maxIter As Integer = 1000,
                                             Optional tol As Double = 0.0000000001) As (eigenValue As Double, eigenVector As Matrix)
            Dim n As Integer = A.Rows
            ' 随机初始化
            Dim rng As New Random(42)
            Dim v As New Matrix(n, 1)
            For i As Integer = 0 To n - 1
                v(i, 0) = rng.NextDouble() - 0.5
            Next
            v = v.Normalize()

            Dim eigenValue As Double = 0.0
            For iter As Integer = 0 To maxIter - 1
                Dim w As Matrix = A * v
                Dim newEigenValue As Double = (v.Transpose() * w)(0, 0)
                Dim norm As Double = w.L2Norm()
                If norm < 0.000000000000001 Then Exit For
                Dim newV As Matrix = (1.0 / norm) * w

                ' 检查收敛
                If Abs(newEigenValue - eigenValue) < tol Then
                    eigenValue = newEigenValue
                    v = newV
                    Exit For
                End If
                eigenValue = newEigenValue
                v = newV
            Next

            Return (eigenValue, v)
        End Function

        ''' <summary>
        ''' 紧凑SVD分解 (Singular Value Decomposition)
        ''' A = U * S * V^T
        ''' 使用Golub-Kahan双对角化 + 幂迭代的简化实现
        ''' </summary>
        Public Shared Function SVD(A As Matrix,
                                   Optional maxIter As Integer = 500,
                                   Optional tol As Double = 0.00000001) As (U As Matrix, S As Double(), VT As Matrix)
            Dim m As Integer = A.Rows
            Dim n As Integer = A.Cols
            Dim k As Integer = Math.Min(m, n)

            ' 计算 A^T * A 的特征值分解来获取 V 和奇异值
            Dim ATA As Matrix = A.Transpose() * A

            Dim singularValues As New List(Of Double)
            Dim vVectors As New List(Of Matrix)
            Dim deflated As Matrix = ATA.Clone()

            For i As Integer = 0 To k - 1
                Dim result = PowerIteration(deflated, maxIter, tol)
                If result.eigenValue < tol Then Exit For
                singularValues.Add(Sqrt(Math.Abs(result.eigenValue)))
                vVectors.Add(result.eigenVector)
                ' 矩阵缩减 (Hotelling缩减)
                deflated = deflated - result.eigenValue * (result.eigenVector * result.eigenVector.Transpose())
            Next

            Dim r As Integer = singularValues.Count
            If r = 0 Then
                Return (New Matrix(m, 1), New Double() {}, New Matrix(1, n))
            End If

            ' 构建 V 矩阵
            Dim V As New Matrix(n, r)
            For j As Integer = 0 To r - 1
                For i As Integer = 0 To n - 1
                    V(i, j) = vVectors(j)(i, 0)
                Next
            Next

            ' 构建奇异值数组
            Dim S As Double() = singularValues.ToArray()

            ' 计算 U = A * V * S^(-1)
            Dim U As New Matrix(m, r)
            For j As Integer = 0 To r - 1
                Dim Av As Matrix = A * V.GetColumn(j)
                If S(j) > 0.000000000000001 Then
                    For i As Integer = 0 To m - 1
                        U(i, j) = Av(i, 0) / S(j)
                    Next
                End If
            Next

            Return (U, S, V.Transpose())
        End Function

        ''' <summary>
        ''' 计算矩阵伪逆 (Moore-Penrose Pseudoinverse)
        ''' 基于 SVD: A+ = V * S+ * U^T
        ''' </summary>
        Public Shared Function PseudoInverse(A As Matrix, Optional tol As Double = 0.0000000001) As Matrix
            Dim svdResult = SVD(A)
            Dim U As Matrix = svdResult.U
            Dim S As Double() = svdResult.S
            Dim VT As Matrix = svdResult.VT

            If S.Length = 0 Then Return Matrix.Zeros(A.Cols, A.Rows)

            ' 构建 S+ (奇异值取倒数)
            Dim Splus As New Matrix(S.Length, S.Length)
            Dim maxS As Double = S.Max()
            For i As Integer = 0 To S.Length - 1
                If S(i) > tol * maxS Then
                    Splus(i, i) = 1.0 / S(i)
                Else
                    Splus(i, i) = 0.0
                End If
            Next

            ' A+ = V * S+ * U^T
            Return VT.Transpose() * Splus * U.Transpose()
        End Function

        ''' <summary>
        ''' 计算两个矩阵列向量之间的相关系数矩阵
        ''' R[i,j] = cor(X[,i], Y[,j])
        ''' </summary>
        Public Shared Function CorrelationMatrix(X As Matrix, Y As Matrix) As Matrix
            Dim xc As Matrix = X.Center()
            Dim yc As Matrix = Y.Center()

            Dim xNorms As New Matrix(1, xc.Cols)
            For j As Integer = 0 To xc.Cols - 1
                Dim s As Double = 0.0
                For i As Integer = 0 To xc.Rows - 1
                    s += xc(i, j) ^ 2
                Next
                xNorms(0, j) = Sqrt(s)
                If xNorms(0, j) < 0.000000000000001 Then xNorms(0, j) = 1.0
            Next

            Dim yNorms As New Matrix(1, yc.Cols)
            For j As Integer = 0 To yc.Cols - 1
                Dim s As Double = 0.0
                For i As Integer = 0 To yc.Rows - 1
                    s += yc(i, j) ^ 2
                Next
                yNorms(0, j) = Sqrt(s)
                If yNorms(0, j) < 0.000000000000001 Then yNorms(0, j) = 1.0
            Next

            Dim R As Matrix = (1.0 / (X.Rows - 1)) * (xc.Transpose() * yc)
            For i As Integer = 0 To R.Rows - 1
                For j As Integer = 0 To R.Cols - 1
                    R(i, j) /= (xNorms(0, i) * yNorms(0, j))
                    ' 限制在 [-1, 1]
                    R(i, j) = Math.Max(-1.0, Math.Min(1.0, R(i, j)))
                Next
            Next
            Return R
        End Function

        ''' <summary>
        ''' 计算两个列向量之间的皮尔逊相关系数
        ''' </summary>
        Public Shared Function PearsonCorrelation(x As Matrix, y As Matrix) As Double
            If x.Rows <> y.Rows Then
                Throw New ArgumentException("向量长度不一致")
            End If
            Dim n As Integer = x.Rows
            Dim xMean As Double = 0.0, yMean As Double = 0.0
            For i As Integer = 0 To n - 1
                xMean += x(i, 0)
                yMean += y(i, 0)
            Next
            xMean /= n
            yMean /= n

            Dim cov As Double = 0.0, varX As Double = 0.0, varY As Double = 0.0
            For i As Integer = 0 To n - 1
                Dim dx As Double = x(i, 0) - xMean
                Dim dy As Double = y(i, 0) - yMean
                cov += dx * dy
                varX += dx * dx
                varY += dy * dy
            Next

            If varX < 0.000000000000001 OrElse varY < 0.000000000000001 Then Return 0.0
            Return cov / Sqrt(varX * varY)
        End Function

    End Class

    ' ========================================================================
    ' 3. 稀疏化工具类
    ' ========================================================================

    ''' <summary>
    ''' 稀疏化与变量选择工具
    ''' 实现L1惩罚的软阈值函数，用于DIABLO中的变量选择
    ''' </summary>
    Public Class SparseUtils

        ''' <summary>
        ''' 软阈值函数 (Soft Thresholding)
        ''' S_lambda(x) = sign(x) * max(|x| - lambda, 0)
        ''' 这是L1惩罚 (Lasso) 的近端算子
        ''' </summary>
        ''' <param name="x">输入值</param>
        ''' <param name="lambda">阈值参数</param>
        Public Shared Function SoftThreshold(x As Double, lambda As Double) As Double
            If x > lambda Then
                Return x - lambda
            ElseIf x < -lambda Then
                Return x + lambda
            Else
                Return 0.0
            End If
        End Function

        ''' <summary>
        ''' 对向量应用软阈值
        ''' </summary>
        Public Shared Function SoftThresholdVector(v As Matrix, lambda As Double) As Matrix
            Dim result As Matrix = v.Clone()
            For i As Integer = 0 To v.Rows - 1
                For j As Integer = 0 To v.Cols - 1
                    result(i, j) = SoftThreshold(v(i, j), lambda)
                Next
            Next
            Return result
        End Function

        ''' <summary>
        ''' L1稀疏化: 保留绝对值最大的keepX个分量，其余置零
        ''' 模拟 mixOmics 中 keepX 参数的效果
        ''' 这是DIABLO/sPLS-DA中核心的变量选择机制
        ''' </summary>
        ''' <param name="v">输入权重向量 (列向量)</param>
        ''' <param name="keepX">保留的变量数</param>
        ''' <returns>稀疏化后的向量</returns>
        Public Shared Function SparsifyVector(v As Matrix, keepX As Integer) As Matrix
            If keepX >= v.Rows Then Return v.Clone()
            If keepX <= 0 Then Return New Matrix(v.Rows, 1)

            ' 获取绝对值排序索引
            Dim absValues As New List(Of (index As Integer, absVal As Double))
            For i As Integer = 0 To v.Rows - 1
                absValues.Add((i, Math.Abs(v(i, 0))))
            Next
            absValues = absValues.OrderByDescending(Function(x) x.absVal).ToList()

            ' 找到第keepX大的绝对值作为阈值
            Dim threshold As Double = absValues(keepX - 1).absVal

            ' 保留绝对值 >= threshold 的分量
            ' 如果有并列，只保留前keepX个
            Dim result As New Matrix(v.Rows, 1)
            Dim kept As Integer = 0
            For Each item In absValues
                If kept < keepX Then
                    result(item.index, 0) = v(item.index, 0)
                    kept += 1
                Else
                    Exit For
                End If
            Next

            Return result
        End Function

        ''' <summary>
        ''' 基于L1范数约束的稀疏化
        ''' 给定目标L1范数，通过迭代软阈值实现
        ''' 用于更精细的稀疏度控制
        ''' </summary>
        Public Shared Function SparsifyByL1Norm(v As Matrix, targetL1Norm As Double,
                                                Optional maxIter As Integer = 100,
                                                Optional tol As Double = 0.000001) As Matrix
            Dim currentL1 As Double = v.L1Norm()
            If currentL1 <= targetL1Norm Then Return v.Clone()

            ' 二分搜索最优lambda
            Dim lambdaLow As Double = 0.0
            Dim lambdaHigh As Double = Math.Abs(v(0, 0))
            For i As Integer = 0 To v.Rows - 1
                lambdaHigh = Math.Max(lambdaHigh, Math.Abs(v(i, 0)))
            Next

            Dim result As Matrix = v.Clone()
            For iter As Integer = 0 To maxIter - 1
                Dim lambdaMid As Double = (lambdaLow + lambdaHigh) / 2.0
                result = SoftThresholdVector(v, lambdaMid)
                currentL1 = result.L1Norm()

                If Math.Abs(currentL1 - targetL1Norm) < tol Then Exit For

                If currentL1 > targetL1Norm Then
                    lambdaLow = lambdaMid
                Else
                    lambdaHigh = lambdaMid
                End If
            Next

            Return result
        End Function

    End Class

    ' ========================================================================
    ' 4. DIABLO 核心算法类
    ' ========================================================================

    ''' <summary>
    ''' DIABLO (Data Integration Analysis for Biomarker discovery using Latent cOmponents)
    ''' 多组学数据关联分析核心算法
    '''
    ''' 数学原理:
    ''' DIABLO 是一种基于稀疏多块偏最小二乘判别分析 (sparse multi-block PLS-DA) 的
    ''' 多组学整合方法。其核心思想是在多个数据块之间寻找最大化协方差的潜在成分，
    ''' 同时通过L1惩罚实现变量选择，并确保这些成分能够区分已知的样本类别。
    '''
    ''' 目标函数 (对于第h个成分):
    '''   max  sum_{k,l} design[k,l] * cov(X_k * u_kh, X_l * u_lh)
    '''        + cov(X_k * u_kh, Y * c_h)
    '''   s.t. ||u_kh||_2 = 1,  ||u_kh||_1 <= lambda_k
    '''
    ''' 其中 design[k,l] 是块间连接设计矩阵，lambda_k 是稀疏度参数
    ''' </summary>
    Public Class DIABLO

        ' ---- 输入数据 ----
        Private _X As List(Of Matrix)        ' 多组学数据块列表
        Private _Y As Matrix                  ' 响应矩阵 (one-hot编码)
        Private _YLabels As Integer()         ' 原始类别标签
        Private _classLabels As String()      ' 类别名称
        Private _nBlocks As Integer           ' 数据块数量
        Private _nSamples As Integer          ' 样本数
        Private _nClasses As Integer          ' 类别数

        ' ---- 算法参数 ----
        Private _ncomp As Integer             ' 成分数
        Private _design As Matrix             ' 块间设计矩阵 (含Y)
        Private _keepX As List(Of Integer())  ' 每个块每个成分保留的变量数
        Private _maxIter As Integer           ' 最大迭代次数
        Private _tol As Double                ' 收敛容差

        ' ---- 计算结果 ----
        Private _loadings As List(Of Matrix())   ' 每个块每个成分的载荷向量
        Private _latentVars As List(Of Matrix()) ' 每个块每个成分的潜在变量
        Private _YLoadings As Matrix()           ' Y每个成分的载荷
        Private _YLatentVars As Matrix           ' Y的潜在变量
        Private _explainedVar As List(Of Double()) ' 每个块每个成分的解释方差
        Private _isFitted As Boolean = False

        ' ---- 预处理信息 (用于新数据预测) ----
        Private _XMeans As List(Of Matrix)    ' 每个块的列均值
        Private _XStds As List(Of Matrix)     ' 每个块的列标准差
        Private _deflationMode As String      ' 缩减模式

        ''' <summary>数据块数量</summary>
        Public ReadOnly Property NBlocks As Integer
            Get
                Return _nBlocks
            End Get
        End Property

        ''' <summary>成分数</summary>
        Public ReadOnly Property NComp As Integer
            Get
                Return _ncomp
            End Get
        End Property

        ''' <summary>设计矩阵</summary>
        Public ReadOnly Property Design As Matrix
            Get
                Return _design
            End Get
        End Property

        ''' <summary>载荷向量 (loadings): _loadings(block)(component)</summary>
        Public ReadOnly Property Loadings As List(Of Matrix())
            Get
                Return _loadings
            End Get
        End Property

        ''' <summary>潜在变量 (latent variables / variates)</summary>
        Public ReadOnly Property LatentVars As List(Of Matrix())
            Get
                Return _latentVars
            End Get
        End Property

        ''' <summary>Y的载荷向量</summary>
        Public ReadOnly Property YLoadings As Matrix()
            Get
                Return _YLoadings
            End Get
        End Property

        ''' <summary>Y的潜在变量</summary>
        Public ReadOnly Property YLatentVars As Matrix
            Get
                Return _YLatentVars
            End Get
        End Property

        ''' <summary>每个块每个成分的解释方差比例</summary>
        Public ReadOnly Property ExplainedVariance As List(Of Double())
            Get
                Return _explainedVar
            End Get
        End Property

        ''' <summary>
        ''' 构造DIABLO分析对象
        ''' </summary>
        ''' <param name="X">多组学数据块列表，每个矩阵为 n×p_k (样本×变量)</param>
        ''' <param name="YLabels">样本类别标签 (整数编码，从0开始)</param>
        ''' <param name="classLabels">类别名称数组</param>
        ''' <param name="ncomp">提取的成分数</param>
        ''' <param name="design">块间设计矩阵 (nBlocks+1) × (nBlocks+1)，最后一行/列为Y</param>
        ''' <param name="keepX">每个块每个成分保留的变量数列表，keepX(block)(component)</param>
        ''' <param name="maxIter">迭代算法最大迭代次数</param>
        ''' <param name="tol">收敛容差</param>
        Public Sub New(X As List(Of Matrix),
                       YLabels As Integer(),
                       classLabels As String(),
                       ncomp As Integer,
                       Optional design As Matrix = Nothing,
                       Optional keepX As List(Of Integer()) = Nothing,
                       Optional maxIter As Integer = 500,
                       Optional tol As Double = 0.000001)

            If X Is Nothing OrElse X.Count = 0 Then
                Throw New ArgumentException("数据块列表不能为空")
            End If

            _nSamples = X(0).Rows
            _nBlocks = X.Count
            _ncomp = ncomp
            _maxIter = maxIter
            _tol = tol
            _classLabels = classLabels
            _nClasses = classLabels.Length

            ' 验证数据维度一致性
            For k As Integer = 0 To _nBlocks - 1
                If X(k).Rows <> _nSamples Then
                    Throw New ArgumentException($"数据块 {k} 的样本数 ({X(k).Rows}) 与第一个块 ({_nSamples}) 不一致")
                End If
            Next

            ' 存储原始数据 (后续会中心化)
            _X = New List(Of Matrix)()
            For Each block In X
                _X.Add(block.Clone())
            Next

            ' 将类别标签转为one-hot编码
            _YLabels = CType(YLabels.Clone(), Integer())
            _Y = OneHotEncode(YLabels, _nClasses)

            ' 设置设计矩阵 (默认全1连接)
            If design IsNot Nothing Then
                _design = design.Clone()
            Else
                ' 默认: 所有块之间以及与Y之间完全连接
                Dim d As Integer = _nBlocks + 1
                _design = Matrix.Ones(d, d)
                For i As Integer = 0 To d - 1
                    _design(i, i) = 0.0
                Next
            End If

            ' 设置稀疏度参数 (默认保留所有变量)
            If keepX IsNot Nothing Then
                _keepX = keepX
            Else
                _keepX = New List(Of Integer())()
                For k As Integer = 0 To _nBlocks - 1
                    Dim kx(_ncomp - 1) As Integer
                    For h As Integer = 0 To _ncomp - 1
                        kx(h) = _X(k).Cols  ' 保留全部变量
                    Next
                    _keepX.Add(kx)
                Next
            End If

            ' 初始化结果存储
            _loadings = New List(Of Matrix())()
            _latentVars = New List(Of Matrix())()
            _YLoadings = New Matrix(_ncomp - 1) {}
            _explainedVar = New List(Of Double())()
        End Sub

        ''' <summary>
        ''' 执行DIABLO拟合
        ''' 核心算法流程:
        ''' 1. 数据预处理 (中心化/标准化)
        ''' 2. 对每个成分h:
        '''    a. 初始化权重向量
        '''    b. 迭代优化:
        '''       - 对每个数据块k，计算与其它块的协方差加权和
        '''       - 应用稀疏化 (L1惩罚)
        '''       - 归一化权重向量
        '''       - 检查收敛
        '''    c. 计算潜在变量
        '''    d. 矩阵缩减 (deflation)
        ''' </summary>
        Public Sub Fit()
            ' ---- 步骤1: 数据预处理 ----
            PreprocessData()

            ' 创建工作副本 (迭代中会被缩减)
            Dim Xwork As New List(Of Matrix)()
            For k As Integer = 0 To _nBlocks - 1
                Xwork.Add(_X(k).Clone())
            Next
            Dim Ywork As Matrix = _Y.Clone()

            ' 初始化结果存储
            _loadings = New List(Of Matrix())()
            _latentVars = New List(Of Matrix())()
            _YLoadings = New Matrix(_ncomp - 1) {}
            _explainedVar = New List(Of Double())()

            For k As Integer = 0 To _nBlocks - 1
                Dim loadings_k(_ncomp - 1) As Matrix
                Dim lv_k(_ncomp - 1) As Matrix
                Dim ev_k(_ncomp - 1) As Double
                _loadings.Add(loadings_k)
                _latentVars.Add(lv_k)
                _explainedVar.Add(ev_k)
            Next

            ' ---- 步骤2: 逐成分提取 ----
            For h As Integer = 0 To _ncomp - 1
                ' 提取第h个成分
                Dim compResult = ExtractComponent(Xwork, Ywork, h)

                ' 存储载荷和潜在变量
                For k As Integer = 0 To _nBlocks - 1
                    _loadings(k)(h) = compResult.Loadings(k)
                    _latentVars(k)(h) = compResult.LatentVars(k)
                Next
                _YLoadings(h) = compResult.YLoading
                _YLatentVars = compResult.YLatentVar

                ' 计算解释方差
                For k As Integer = 0 To _nBlocks - 1
                    Dim totalVar As Double = 0.0
                    For j As Integer = 0 To _X(k).Cols - 1
                        For i As Integer = 0 To _nSamples - 1
                            totalVar += _X(k)(i, j) ^ 2
                        Next
                    Next
                    If totalVar > 0 Then
                        Dim compVar As Double = compResult.LatentVars(k).L2Norm() ^ 2
                        _explainedVar(k)(h) = compVar / totalVar
                    Else
                        _explainedVar(k)(h) = 0.0
                    End If
                Next

                ' ---- 步骤3: 矩阵缩减 (Deflation) ----
                DeflateMatrices(Xwork, Ywork, compResult)
            Next

            _isFitted = True
        End Sub

        ''' <summary>
        ''' 数据预处理: 中心化每个数据块和Y
        ''' </summary>
        Private Sub PreprocessData()
            _XMeans = New List(Of Matrix)()
            _XStds = New List(Of Matrix)()

            For k As Integer = 0 To _nBlocks - 1
                Dim means As Matrix = _X(k).ColumnMeans()
                Dim stds As Matrix = _X(k).ColumnStdDevs()
                _XMeans.Add(means)
                _XStds.Add(stds)

                ' 中心化 (不缩放，与mixOmics默认行为一致)
                For i As Integer = 0 To _nSamples - 1
                    For j As Integer = 0 To _X(k).Cols - 1
                        _X(k)(i, j) -= means(0, j)
                    Next
                Next
            Next
        End Sub

        ''' <summary>
        ''' 提取单个成分的核心迭代算法
        '''
        ''' 这是DIABLO算法的核心，实现了稀疏多块PLS-DA的迭代优化:
        ''' 对于每个数据块k，更新其权重向量u_k:
        '''   u_k = X_k^T * [sum_l design[k,l] * (X_l * u_l) + design[k,Y] * (Y * c)]
        ''' 然后应用稀疏化并归一化
        ''' </summary>
        Private Function ExtractComponent(Xwork As List(Of Matrix), Ywork As Matrix,
                                          compIdx As Integer) As ComponentResult
            Dim result As New ComponentResult()
            result.Loadings = New Matrix(_nBlocks - 1) {}
            result.LatentVars = New Matrix(_nBlocks - 1) {}

            ' ---- 初始化权重向量 ----
            ' 使用SVD初始化: 对每个块取第一个左奇异向量方向
            Dim weights As Matrix() = New Matrix(_nBlocks - 1) {}
            Dim yWeight As Matrix = Nothing

            For k As Integer = 0 To _nBlocks - 1
                Dim svdResult = LinearAlgebra.SVD(Xwork(k), 100, 0.000001)
                If svdResult.S.Length > 0 Then
                    weights(k) = svdResult.VT.Transpose().GetColumn(0)
                Else
                    Dim rng As New Random(compIdx * 1000 + k)
                    weights(k) = New Matrix(Xwork(k).Cols, 1)
                    For i As Integer = 0 To Xwork(k).Cols - 1
                        weights(k)(i, 0) = rng.NextDouble() - 0.5
                    Next
                End If
                weights(k) = weights(k).Normalize()
            Next

            ' Y权重初始化
            Dim ySvdResult = LinearAlgebra.SVD(Ywork, 100, 0.000001)
            If ySvdResult.S.Length > 0 Then
                yWeight = ySvdResult.VT.Transpose().GetColumn(0)
            Else
                yWeight = Matrix.Ones(_nClasses, 1)
            End If
            yWeight = yWeight.Normalize()

            ' ---- 迭代优化 ----
            Dim converged As Boolean = False
            Dim prevWeights As Matrix() = New Matrix(_nBlocks - 1) {}

            For iter As Integer = 0 To _maxIter - 1
                ' 保存上一次的权重用于收敛判断
                For k As Integer = 0 To _nBlocks - 1
                    prevWeights(k) = weights(k).Clone()
                Next

                ' 对每个数据块更新权重
                For k As Integer = 0 To _nBlocks - 1
                    ' 计算 M_k = X_k^T * [sum_l design[k,l] * t_l + design[k,Y] * t_Y]
                    ' 其中 t_l = X_l * u_l 是第l个块的潜在变量
                    Dim Mk As New Matrix(Xwork(k).Cols, 1)

                    ' 与其他数据块的协方差贡献
                    For l As Integer = 0 To _nBlocks - 1
                        If l = k Then Continue For
                        Dim designKL As Double = _design(k, l)
                        If Math.Abs(designKL) < 0.000000000000001 Then Continue For

                        Dim t_l As Matrix = Xwork(l) * weights(l)
                        Dim contrib As Matrix = Xwork(k).Transpose() * t_l
                        Mk = Mk + designKL * contrib
                    Next

                    ' 与Y的协方差贡献
                    Dim designKY As Double = _design(k, _nBlocks)
                    If Math.Abs(designKY) > 0.000000000000001 Then
                        Dim tY As Matrix = Ywork * yWeight
                        Dim yContrib As Matrix = Xwork(k).Transpose() * tY
                        Mk = Mk + designKY * yContrib
                    End If

                    ' 提取主方向 (取Mk的方向)
                    weights(k) = Mk.Normalize()

                    ' 应用稀疏化 (L1惩罚 / 变量选择)
                    Dim kx As Integer = _keepX(k)(compIdx)
                    If kx < Xwork(k).Cols Then
                        weights(k) = SparseUtils.SparsifyVector(weights(k), kx)
                        ' 稀疏化后重新归一化
                        Dim norm As Double = weights(k).L2Norm()
                        If norm > 0.000000000000001 Then
                            weights(k) = (1.0 / norm) * weights(k)
                        End If
                    End If
                Next

                ' 更新Y权重
                Dim MkY As New Matrix(_nClasses, 1)
                For l As Integer = 0 To _nBlocks - 1
                    Dim designYl As Double = _design(_nBlocks, l)
                    If Math.Abs(designYl) < 0.000000000000001 Then Continue For
                    Dim t_l As Matrix = Xwork(l) * weights(l)
                    Dim yContrib As Matrix = Ywork.Transpose() * t_l
                    MkY = MkY + designYl * yContrib
                Next
                Dim yNorm As Double = MkY.L2Norm()
                If yNorm > 0.000000000000001 Then
                    yWeight = MkY.Normalize()
                End If

                ' ---- 检查收敛 ----
                converged = True
                For k As Integer = 0 To _nBlocks - 1
                    Dim diff As Double = (weights(k) - prevWeights(k)).L2Norm()
                    If diff > _tol Then
                        converged = False
                        Exit For
                    End If
                Next

                If converged Then Exit For
            Next

            ' ---- 计算最终结果 ----
            For k As Integer = 0 To _nBlocks - 1
                result.Loadings(k) = weights(k).Clone()
                result.LatentVars(k) = Xwork(k) * weights(k)
            Next

            result.YLoading = yWeight.Clone()
            result.YLatentVar = Ywork * yWeight

            Return result
        End Function

        ''' <summary>
        ''' 矩阵缩减 (Deflation)
        '''
        ''' 在提取每个成分后，需要从原始矩阵中移除该成分的贡献，
        ''' 以便在残余矩阵上提取下一个成分。
        '''
        ''' 使用回归缩减法 (regression deflation):
        '''   X_k_new = X_k - t_k * p_k^T
        ''' 其中 p_k = X_k^T * t_k / (t_k^T * t_k) 是回归系数
        ''' </summary>
        Private Sub DeflateMatrices(Xwork As List(Of Matrix), Ywork As Matrix,
                                    compResult As ComponentResult)
            For k As Integer = 0 To _nBlocks - 1
                Dim t_k As Matrix = compResult.LatentVars(k)
                Dim tktk As Double = (t_k.Transpose() * t_k)(0, 0)

                If tktk < 0.000000000000001 Then Continue For

                ' 计算回归系数 p_k = X_k^T * t_k / (t_k^T * t_k)
                Dim p_k As Matrix = (1.0 / tktk) * (Xwork(k).Transpose() * t_k)

                ' 缩减: X_k = X_k - t_k * p_k^T
                Dim deflate As Matrix = t_k * p_k.Transpose()
                For i As Integer = 0 To Xwork(k).Rows - 1
                    For j As Integer = 0 To Xwork(k).Cols - 1
                        Xwork(k)(i, j) -= deflate(i, j)
                    Next
                Next
            Next

            ' Y的缩减 (可选，mixOmics中Y也会被缩减)
            Dim tY As Matrix = compResult.YLatentVar
            Dim tYtY As Double = (tY.Transpose() * tY)(0, 0)
            If tYtY > 0.000000000000001 Then
                Dim pY As Matrix = (1.0 / tYtY) * (Ywork.Transpose() * tY)
                Dim deflateY As Matrix = tY * pY.Transpose()
                For i As Integer = 0 To Ywork.Rows - 1
                    For j As Integer = 0 To Ywork.Cols - 1
                        Ywork(i, j) -= deflateY(i, j)
                    Next
                Next
            End If
        End Sub

        ''' <summary>
        ''' One-Hot 编码: 将整数类别标签转为指示矩阵
        ''' </summary>
        Private Function OneHotEncode(labels As Integer(), nClasses As Integer) As Matrix
            Dim Y As New Matrix(labels.Length, nClasses)
            For i As Integer = 0 To labels.Length - 1
                If labels(i) >= 0 AndAlso labels(i) < nClasses Then
                    Y(i, labels(i)) = 1.0
                End If
            Next
            Return Y
        End Function

        ''' <summary>
        ''' 预测新样本的类别
        '''
        ''' 基于最大距离法 (max.dist) 或质心法 (centroids.dist)
        ''' 将新样本投影到DIABLO成分空间，然后计算到各类质心的距离
        ''' </summary>
        ''' <param name="Xnew">新样本数据块列表</param>
        ''' <param name="distMethod">距离方法: "max_dist", "centroids_dist", "mahalanobis_dist"</param>
        ''' <returns>预测类别标签数组</returns>
        Public Function Predict(Xnew As List(Of Matrix),
                                Optional distMethod As String = "centroids_dist") As Integer()
            If Not _isFitted Then
                Throw New InvalidOperationException("模型尚未拟合，请先调用 Fit()")
            End If

            Dim nNew As Integer = Xnew(0).Rows

            ' 中心化新数据 (使用训练集的均值)
            Dim Xwork As New List(Of Matrix)()
            For k As Integer = 0 To _nBlocks - 1
                Dim Xk As Matrix = Xnew(k).Clone()
                For i As Integer = 0 To nNew - 1
                    For j As Integer = 0 To Xk.Cols - 1
                        Xk(i, j) -= _XMeans(k)(0, j)
                    Next
                Next
                Xwork.Add(Xk)
            Next

            ' 投影到成分空间
            ' 计算每个新样本在每个成分上的得分
            Dim scores As New List(Of Matrix())()
            For k As Integer = 0 To _nBlocks - 1
                Dim blockScores(_ncomp - 1) As Matrix
                For h As Integer = 0 To _ncomp - 1
                    blockScores(h) = Xwork(k) * _loadings(k)(h)
                Next
                scores.Add(blockScores)
            Next

            ' 合并所有块的得分 (简单平均)
            Dim combinedScores As New Matrix(nNew, _ncomp)
            For h As Integer = 0 To _ncomp - 1
                For i As Integer = 0 To nNew - 1
                    Dim sumScore As Double = 0.0
                    For k As Integer = 0 To _nBlocks - 1
                        sumScore += scores(k)(h)(i, 0)
                    Next
                    combinedScores(i, h) = sumScore / _nBlocks
                Next
            Next

            ' 计算训练集各类质心
            Dim centroids As Matrix() = ComputeClassCentroids()

            ' 根据距离方法进行分类
            Dim predictions(nNew - 1) As Integer

            Select Case distMethod
                Case "max_dist"
                    predictions = PredictMaxDist(combinedScores, centroids)
                Case "mahalanobis_dist"
                    predictions = PredictMahalanobisDist(combinedScores, centroids)
                Case Else  ' centroids_dist
                    predictions = PredictCentroidsDist(combinedScores, centroids)
            End Select

            Return predictions
        End Function

        ''' <summary>
        ''' 计算训练集各类在成分空间中的质心
        ''' </summary>
        Private Function ComputeClassCentroids() As Matrix()
            ' 计算训练集的合并得分
            Dim trainScores As New Matrix(_nSamples, _ncomp)
            For h As Integer = 0 To _ncomp - 1
                For i As Integer = 0 To _nSamples - 1
                    Dim sumScore As Double = 0.0
                    For k As Integer = 0 To _nBlocks - 1
                        sumScore += _latentVars(k)(h)(i, 0)
                    Next
                    trainScores(i, h) = sumScore / _nBlocks
                Next
            Next

            ' 计算各类质心
            Dim centroids(_nClasses - 1) As Matrix
            For c As Integer = 0 To _nClasses - 1
                centroids(c) = New Matrix(1, _ncomp)
                Dim count As Integer = 0
                For i As Integer = 0 To _nSamples - 1
                    If _YLabels(i) = c Then
                        For h As Integer = 0 To _ncomp - 1
                            centroids(c)(0, h) += trainScores(i, h)
                        Next
                        count += 1
                    End If
                Next
                If count > 0 Then
                    For h As Integer = 0 To _ncomp - 1
                        centroids(c)(0, h) /= count
                    Next
                End If
            Next

            Return centroids
        End Function

        ''' <summary>
        ''' 基于质心欧氏距离的分类
        ''' </summary>
        Private Function PredictCentroidsDist(scores As Matrix, centroids As Matrix()) As Integer()
            Dim n As Integer = scores.Rows
            Dim predictions(n - 1) As Integer

            For i As Integer = 0 To n - 1
                Dim minDist As Double = Double.MaxValue
                Dim bestClass As Integer = 0
                For c As Integer = 0 To _nClasses - 1
                    Dim dist As Double = 0.0
                    For h As Integer = 0 To _ncomp - 1
                        dist += (scores(i, h) - centroids(c)(0, h)) ^ 2
                    Next
                    dist = Sqrt(dist)
                    If dist < minDist Then
                        minDist = dist
                        bestClass = c
                    End If
                Next
                predictions(i) = bestClass
            Next

            Return predictions
        End Function

        ''' <summary>
        ''' 基于最大距离法的分类
        ''' 对每个成分分别计算到质心的距离，取最大值
        ''' </summary>
        Private Function PredictMaxDist(scores As Matrix, centroids As Matrix()) As Integer()
            Dim n As Integer = scores.Rows
            Dim predictions(n - 1) As Integer

            For i As Integer = 0 To n - 1
                Dim minDist As Double = Double.MaxValue
                Dim bestClass As Integer = 0
                For c As Integer = 0 To _nClasses - 1
                    Dim maxCompDist As Double = 0.0
                    For h As Integer = 0 To _ncomp - 1
                        Dim compDist As Double = Math.Abs(scores(i, h) - centroids(c)(0, h))
                        maxCompDist = Math.Max(maxCompDist, compDist)
                    Next
                    If maxCompDist < minDist Then
                        minDist = maxCompDist
                        bestClass = c
                    End If
                Next
                predictions(i) = bestClass
            Next

            Return predictions
        End Function

        ''' <summary>
        ''' 基于马氏距离的分类
        ''' 考虑各类内部的协方差结构
        ''' </summary>
        Private Function PredictMahalanobisDist(scores As Matrix, centroids As Matrix()) As Integer()
            Dim n As Integer = scores.Rows
            Dim predictions(n - 1) As Integer

            ' 计算训练集合并得分
            Dim trainScores As New Matrix(_nSamples, _ncomp)
            For h As Integer = 0 To _ncomp - 1
                For i As Integer = 0 To _nSamples - 1
                    Dim sumScore As Double = 0.0
                    For k As Integer = 0 To _nBlocks - 1
                        sumScore += _latentVars(k)(h)(i, 0)
                    Next
                    trainScores(i, h) = sumScore / _nBlocks
                Next
            Next

            ' 计算合并协方差矩阵
            Dim covMatrix As Matrix = (1.0 / (_nSamples - _nClasses)) *
                (trainScores.Transpose() * trainScores)

            ' 添加正则化以保证可逆
            For i As Integer = 0 To _ncomp - 1
                covMatrix(i, i) += 0.000001
            Next

            ' 计算协方差逆矩阵
            Dim covInv As Matrix = LinearAlgebra.PseudoInverse(covMatrix)

            ' 计算马氏距离
            For i As Integer = 0 To n - 1
                Dim minDist As Double = Double.MaxValue
                Dim bestClass As Integer = 0
                Dim xVec As Matrix = scores.GetRow(i).Transpose()

                For c As Integer = 0 To _nClasses - 1
                    Dim muVec As Matrix = centroids(c).Transpose()
                    Dim diff As Matrix = xVec - muVec
                    Dim mahalDist As Double = (diff.Transpose() * covInv * diff)(0, 0)
                    If mahalDist < minDist Then
                        minDist = mahalDist
                        bestClass = c
                    End If
                Next
                predictions(i) = bestClass
            Next

            Return predictions
        End Function

        ''' <summary>
        ''' 计算块间成分相关性矩阵
        ''' 返回各数据块潜在变量之间的相关系数
        ''' </summary>
        Public Function ComputeBlockCorrelation() As Matrix(,)
            If Not _isFitted Then
                Throw New InvalidOperationException("模型尚未拟合，请先调用 Fit()")
            End If

            Dim corMat(_nBlocks, _nBlocks) As Matrix  ' 包含Y，共 nBlocks+1

            ' 计算所有块(含Y)两两之间的成分相关性
            For k1 As Integer = 0 To _nBlocks
                For k2 As Integer = 0 To _nBlocks
                    corMat(k1, k2) = New Matrix(_ncomp, _ncomp)

                    For h1 As Integer = 0 To _ncomp - 1
                        For h2 As Integer = 0 To _ncomp - 1
                            Dim v1 As Matrix, v2 As Matrix

                            If k1 < _nBlocks Then
                                v1 = _latentVars(k1)(h1)
                            Else
                                v1 = _YLatentVars.GetColumn(h1)
                            End If

                            If k2 < _nBlocks Then
                                v2 = _latentVars(k2)(h2)
                            Else
                                v2 = _YLatentVars.GetColumn(h2)
                            End If

                            corMat(k1, k2)(h1, h2) = LinearAlgebra.PearsonCorrelation(v1, v2)
                        Next
                    Next
                Next
            Next

            Return corMat
        End Function

        ''' <summary>
        ''' 计算每个变量的载荷重要性 (Variable Importance)
        ''' 类似于mixOmics中的loadings贡献度
        ''' </summary>
        Public Function ComputeVariableImportance() As List(Of Double())
            If Not _isFitted Then
                Throw New InvalidOperationException("模型尚未拟合，请先调用 Fit()")
            End If

            Dim importance As New List(Of Double())()

            For k As Integer = 0 To _nBlocks - 1
                Dim p As Integer = _X(k).Cols
                Dim imp(p - 1) As Double

                For j As Integer = 0 To p - 1
                    Dim totalContrib As Double = 0.0
                    For h As Integer = 0 To _ncomp - 1
                        totalContrib += _loadings(k)(h)(j, 0) ^ 2
                    Next
                    imp(j) = totalContrib
                Next

                ' 归一化到 [0, 1]
                Dim maxImp As Double = imp.Max()
                If maxImp > 0 Then
                    For j As Integer = 0 To p - 1
                        imp(j) /= maxImp
                    Next
                End If

                importance.Add(imp)
            Next

            Return importance
        End Function

        ''' <summary>
        ''' 获取每个块中被选中的变量索引 (非零载荷对应的变量)
        ''' </summary>
        Public Function GetSelectedVariables(Optional threshold As Double = 0.0000000001) As List(Of List(Of Integer()))
            If Not _isFitted Then
                Throw New InvalidOperationException("模型尚未拟合，请先调用 Fit()")
            End If

            Dim selected As New List(Of List(Of Integer()))()

            For k As Integer = 0 To _nBlocks - 1
                Dim blockSelected As New List(Of Integer())()
                For h As Integer = 0 To _ncomp - 1
                    Dim vars As New List(Of Integer)
                    For j As Integer = 0 To _X(k).Cols - 1
                        If Math.Abs(_loadings(k)(h)(j, 0)) > threshold Then
                            vars.Add(j)
                        End If
                    Next
                    blockSelected.Add(vars.ToArray())
                Next
                selected.Add(blockSelected)
            Next

            Return selected
        End Function

        ''' <summary>
        ''' 组件结果辅助类
        ''' </summary>
        Private Class ComponentResult
            Public Loadings As Matrix()      ' 每个块的载荷向量
            Public LatentVars As Matrix()    ' 每个块的潜在变量
            Public YLoading As Matrix        ' Y的载荷向量
            Public YLatentVar As Matrix      ' Y的潜在变量
        End Class

    End Class

    ' ========================================================================
    ' 5. 交叉验证模块
    ' ========================================================================

    ''' <summary>
    ''' DIABLO 交叉验证模块
    ''' 用于选择最优的成分数 (ncomp) 和稀疏度参数 (keepX)
    '''
    ''' 支持两种交叉验证策略:
    ''' 1. K折交叉验证: 选择ncomp
    ''' 2. 留一法交叉验证: 精细调优keepX
    '''
    ''' 评估指标:
    ''' - 分类错误率 (Balanced Error Rate, BER)
    ''' - 总体准确率
    ''' </summary>
    Public Class DIABLOCrossValidation

        Private _X As List(Of Matrix)
        Private _YLabels As Integer()
        Private _classLabels As String()
        Private _nClasses As Integer
        Private _nBlocks As Integer
        Private _nSamples As Integer

        ''' <summary>
        ''' 构造交叉验证对象
        ''' </summary>
        Public Sub New(X As List(Of Matrix), YLabels As Integer(), classLabels As String())
            _X = X
            _YLabels = YLabels
            _classLabels = classLabels
            _nClasses = classLabels.Length
            _nBlocks = X.Count
            _nSamples = X(0).Rows
        End Sub

        ''' <summary>
        ''' K折交叉验证选择最优成分数
        ''' </summary>
        ''' <param name="ncompRange">候选成分数范围</param>
        ''' <param name="nFolds">折数</param>
        ''' <param name="design">设计矩阵</param>
        ''' <param name="keepX">每个块的keepX参数 (固定)</param>
        ''' <param name="distMethod">距离方法</param>
        ''' <returns>每个成分数对应的平均BER和准确率</returns>
        Public Function TuneNComp(ncompRange As Integer(),
                                  Optional nFolds As Integer = 5,
                                  Optional design As Matrix = Nothing,
                                  Optional keepX As List(Of Integer()) = Nothing,
                                  Optional distMethod As String = "centroids_dist") As CVNCompResult

            Dim result As New CVNCompResult()
            result.NCompValues = ncompRange
            result.BERValues = New Double(ncompRange.Length - 1) {}
            result.AccuracyValues = New Double(ncompRange.Length - 1) {}
            result.BERStdErrors = New Double(ncompRange.Length - 1) {}

            ' 创建K折索引
            Dim foldIndices As List(Of Integer()) = CreateFolds(nFolds)

            For idx As Integer = 0 To ncompRange.Length - 1
                Dim ncomp As Integer = ncompRange(idx)
                Dim foldBERs As New List(Of Double)
                Dim foldAccs As New List(Of Double)

                For fold As Integer = 0 To nFolds - 1
                    ' 划分训练集和测试集
                    Dim trainTest = SplitTrainTest(foldIndices, fold)
                    Dim Xtrain As List(Of Matrix) = trainTest.Xtrain
                    Dim Xtest As List(Of Matrix) = trainTest.Xtest
                    Dim Ytrain As Integer() = trainTest.Ytrain
                    Dim Ytest As Integer() = trainTest.Ytest

                    ' 拟合DIABLO模型
                    Dim model As New DIABLO(Xtrain, Ytrain, _classLabels,
                                            ncomp, design, keepX)
                    model.Fit()

                    ' 预测测试集
                    Dim predLabels As Integer() = model.Predict(Xtest, distMethod)

                    ' 计算BER和准确率
                    Dim evalResult = ComputeBER(predLabels, Ytest, _nClasses)
                    foldBERs.Add(evalResult.BER)
                    foldAccs.Add(evalResult.Accuracy)
                Next

                result.BERValues(idx) = foldBERs.Average()
                result.AccuracyValues(idx) = foldAccs.Average()

                ' 标准误差
                Dim meanBER As Double = result.BERValues(idx)
                Dim sumSqDiff As Double = 0.0
                For Each ber In foldBERs
                    sumSqDiff += (ber - meanBER) ^ 2
                Next
                result.BERStdErrors(idx) = Sqrt(sumSqDiff / (foldBERs.Count - 1)) / Sqrt(foldBERs.Count)
            Next

            ' 找到最优成分数 (最小BER)
            Dim minBER As Double = result.BERValues.Min()
            result.OptimalNComp = ncompRange(Array.IndexOf(result.BERValues, minBER))

            Return result
        End Function

        ''' <summary>
        ''' 交叉验证选择最优keepX参数 (稀疏度调优)
        ''' 使用网格搜索 + K折交叉验证
        ''' </summary>
        ''' <param name="ncomp">固定的成分数</param>
        ''' <param name="keepXGrid">每个块的候选keepX值网格</param>
        ''' <param name="nFolds">折数</param>
        ''' <param name="design">设计矩阵</param>
        ''' <param name="distMethod">距离方法</param>
        Public Function TuneKeepX(ncomp As Integer,
                                  keepXGrid As List(Of Integer()),
                                  Optional nFolds As Integer = 5,
                                  Optional design As Matrix = Nothing,
                                  Optional distMethod As String = "centroids_dist") As CVKeepXResult

            Dim result As New CVKeepXResult()

            ' 生成所有keepX组合
            Dim combinations As List(Of Integer()) = GenerateKeepXCombinations(keepXGrid)

            Dim foldIndices As List(Of Integer()) = CreateFolds(nFolds)

            Dim allBERs As New List(Of Double)
            Dim allKeepXCombos As New List(Of Integer())

            For Each combo In combinations
                ' 将组合转为keepX格式
                Dim keepX As New List(Of Integer())()
                For k As Integer = 0 To _nBlocks - 1
                    Dim kx(_ncomp - 1) As Integer
                    For h As Integer = 0 To _ncomp - 1
                        kx(h) = combo(k)
                    Next
                    keepX.Add(kx)
                Next

                Dim foldBERs As New List(Of Double)

                For fold As Integer = 0 To nFolds - 1
                    Dim trainTest = SplitTrainTest(foldIndices, fold)

                    Try
                        Dim model As New DIABLO(trainTest.Xtrain, trainTest.Ytrain,
                                                _classLabels, ncomp, design, keepX)
                        model.Fit()
                        Dim predLabels As Integer() = model.Predict(trainTest.Xtest, distMethod)
                        Dim evalResult = ComputeBER(predLabels, trainTest.Ytest, _nClasses)
                        foldBERs.Add(evalResult.BER)
                    Catch ex As Exception
                        ' 某些keepX组合可能导致数值问题，跳过
                        foldBERs.Add(1.0)
                    End Try
                Next

                allBERs.Add(foldBERs.Average())
                allKeepXCombos.Add(combo)
            Next

            ' 找到最优组合
            Dim minIdx As Integer = 0
            Dim minBER As Double = allBERs(0)
            For i As Integer = 1 To allBERs.Count - 1
                If allBERs(i) < minBER Then
                    minBER = allBERs(i)
                    minIdx = i
                End If
            Next

            result.OptimalKeepX = allKeepXCombos(minIdx)
            result.OptimalBER = minBER
            result.AllBERs = allBERs.ToArray()
            result.AllKeepXCombinations = allKeepXCombos

            Return result
        End Function

        ''' <summary>
        ''' 创建K折索引
        ''' </summary>
        Private Function CreateFolds(nFolds As Integer) As List(Of Integer())
            Dim indices As Integer() = Enumerable.Range(0, _nSamples).ToArray()

            ' 随机打乱 (使用固定种子保证可重复性)
            Dim rng As New Random(42)
            For i As Integer = _nSamples - 1 To 1 Step -1
                Dim j As Integer = rng.Next(0, i + 1)
                Dim temp As Integer = indices(i)
                indices(i) = indices(j)
                indices(j) = temp
            Next

            Dim folds As New List(Of Integer())
            Dim foldSize As Integer = _nSamples \ nFolds
            Dim remainder As Integer = _nSamples Mod nFolds

            Dim startIdx As Integer = 0
            For f As Integer = 0 To nFolds - 1
                Dim size As Integer = foldSize + (If(f < remainder, 1, 0))
                Dim fold(size - 1) As Integer
                Array.Copy(indices, startIdx, fold, 0, size)
                folds.Add(fold)
                startIdx += size
            Next

            Return folds
        End Function

        ''' <summary>
        ''' 划分训练集和测试集
        ''' </summary>
        Private Function SplitTrainTest(folds As List(Of Integer()), testFold As Integer) As TrainTestSplit
            ' 测试集索引
            Dim testIndices As Integer() = folds(testFold)
            Dim testSet As New HashSet(Of Integer)(testIndices)

            ' 训练集索引
            Dim trainIndices As New List(Of Integer)
            For i As Integer = 0 To _nSamples - 1
                If Not testSet.Contains(i) Then
                    trainIndices.Add(i)
                End If
            Next

            ' 构建训练集数据块
            Dim Xtrain As New List(Of Matrix)()
            Dim Xtest As New List(Of Matrix)()
            For k As Integer = 0 To _nBlocks - 1
                Dim Xtr As New Matrix(trainIndices.Count, _X(k).Cols)
                Dim Xte As New Matrix(testIndices.Length, _X(k).Cols)
                For i As Integer = 0 To trainIndices.Count - 1
                    For j As Integer = 0 To _X(k).Cols - 1
                        Xtr(i, j) = _X(k)(trainIndices(i), j)
                    Next
                Next
                For i As Integer = 0 To testIndices.Length - 1
                    For j As Integer = 0 To _X(k).Cols - 1
                        Xte(i, j) = _X(k)(testIndices(i), j)
                    Next
                Next
                Xtrain.Add(Xtr)
                Xtest.Add(Xte)
            Next

            ' 构建标签
            Dim Ytrain(trainIndices.Count - 1) As Integer
            Dim Ytest(testIndices.Length - 1) As Integer
            For i As Integer = 0 To trainIndices.Count - 1
                Ytrain(i) = _YLabels(trainIndices(i))
            Next
            For i As Integer = 0 To testIndices.Length - 1
                Ytest(i) = _YLabels(testIndices(i))
            Next

            Return New TrainTestSplit With {
                .Xtrain = Xtrain,
                .Xtest = Xtest,
                .Ytrain = Ytrain,
                .Ytest = Ytest
            }
        End Function

        ''' <summary>
        ''' 生成keepX参数的所有组合
        ''' </summary>
        Private Function GenerateKeepXCombinations(keepXGrid As List(Of Integer())) As List(Of Integer())
            If keepXGrid.Count = 0 Then Return New List(Of Integer())

            Dim results As New List(Of Integer())
            Dim current(keepXGrid.Count - 1) As Integer
            GenerateCombinationsRecursive(keepXGrid, 0, current, results)
            Return results
        End Function

        Private Sub GenerateCombinationsRecursive(keepXGrid As List(Of Integer()),
                                                   idx As Integer,
                                                   current As Integer(),
                                                   results As List(Of Integer()))
            If idx = keepXGrid.Count Then
                results.Add(CType(current.Clone(), Integer()))
                Return
            End If

            For Each Val In keepXGrid(idx)
                current(idx) = Val()
                GenerateCombinationsRecursive(keepXGrid, idx + 1, current, results)
            Next
        End Sub

        ''' <summary>
        ''' 计算平衡错误率 (Balanced Error Rate)
        ''' BER = 平均(每类错误率)
        ''' </summary>
        Public Shared Function ComputeBER(predicted As Integer(), actual As Integer(),
                                          nClasses As Integer) As BERResult
            Dim classErrors(nClasses - 1) As Double
            Dim classCounts(nClasses - 1) As Integer
            Dim correct As Integer = 0

            For i As Integer = 0 To predicted.Length - 1
                classCounts(actual(i)) += 1
                If predicted(i) = actual(i) Then
                    correct += 1
                Else
                    classErrors(actual(i)) += 1
                End If
            Next

            ' 计算每类错误率
            Dim sumClassErrorRate As Double = 0.0
            Dim validClasses As Integer = 0
            For c As Integer = 0 To nClasses - 1
                If classCounts(c) > 0 Then
                    sumClassErrorRate += classErrors(c) / classCounts(c)
                    validClasses += 1
                End If
            Next

            Dim ber As Double = If(validClasses > 0, sumClassErrorRate / validClasses, 1.0)
            Dim accuracy As Double = If(predicted.Length > 0, CDbl(correct) / predicted.Length, 0.0)

            Return New BERResult With {
                .BER = ber,
                .Accuracy = accuracy,
                .ClassErrorRates = classErrors,
                .ClassCounts = classCounts
            }
        End Function

        ''' <summary>
        ''' 训练/测试集划分辅助结构
        ''' </summary>
        Private Class TrainTestSplit
            Public Xtrain As List(Of Matrix)
            Public Xtest As List(Of Matrix)
            Public Ytrain As Integer()
            Public Ytest As Integer()
        End Class

    End Class

    ' ========================================================================
    ' 6. 性能评估结果类
    ' ========================================================================

    ''' <summary>
    ''' BER (平衡错误率) 计算结果
    ''' </summary>
    Public Class BERResult
        ''' <summary>平衡错误率</summary>
        Public BER As Double
        ''' <summary>总体准确率</summary>
        Public Accuracy As Double
        ''' <summary>每类错误数</summary>
        Public ClassErrorRates As Double()
        ''' <summary>每类样本数</summary>
        Public ClassCounts As Integer()
    End Class

    ''' <summary>
    ''' 成分数调优结果
    ''' </summary>
    Public Class CVNCompResult
        ''' <summary>候选成分数值</summary>
        Public NCompValues As Integer()
        ''' <summary>每个成分数对应的平均BER</summary>
        Public BERValues As Double()
        ''' <summary>每个成分数对应的平均准确率</summary>
        Public AccuracyValues As Double()
        ''' <summary>BER标准误差</summary>
        Public BERStdErrors As Double()
        ''' <summary>最优成分数</summary>
        Public OptimalNComp As Integer
    End Class

    ''' <summary>
    ''' keepX参数调优结果
    ''' </summary>
    Public Class CVKeepXResult
        ''' <summary>最优keepX组合 (每个块一个值)</summary>
        Public OptimalKeepX As Integer()
        ''' <summary>最优组合的BER</summary>
        Public OptimalBER As Double
        ''' <summary>所有组合的BER</summary>
        Public AllBERs As Double()
        ''' <summary>所有keepX组合</summary>
        Public AllKeepXCombinations As List(Of Integer())
    End Class

    ' ========================================================================
    ' 7. 辅助工具类
    ' ========================================================================

    ''' <summary>
    ''' DIABLO 辅助工具类
    ''' 提供设计矩阵构建、数据模拟等功能
    ''' </summary>
    Public Class DIABLOUtils

        ''' <summary>
        ''' 创建全连接设计矩阵 (所有块之间以及与Y之间完全连接)
        ''' design[i,j] = 1 表示块i和块j之间有连接
        ''' </summary>
        Public Shared Function CreateFullDesign(nBlocks As Integer) As Matrix
            Dim d As Integer = nBlocks + 1  ' 加上Y
            Dim design As Matrix = Matrix.Ones(d, d)
            For i As Integer = 0 To d - 1
                design(i, i) = 0.0
            Next
            Return design
        End Function

        ''' <summary>
        ''' 创建自定义设计矩阵
        ''' </summary>
        ''' <param name="connections">连接对列表，每对为 (block_i, block_j, weight)</param>
        ''' <param name="nBlocks">数据块数量</param>
        Public Shared Function CreateCustomDesign(connections As List(Of (i As Integer, j As Integer, w As Double)),
                                                   nBlocks As Integer) As Matrix
            Dim d As Integer = nBlocks + 1
            Dim design As New Matrix(d, d)

            ' 默认所有块与Y连接
            For k As Integer = 0 To nBlocks - 1
                design(k, nBlocks) = 1.0
                design(nBlocks, k) = 1.0
            Next

            ' 设置自定义连接
            For Each conn In connections
                If conn.i < d AndAlso conn.j < d Then
                    design(conn.i, conn.j) = conn.w
                    design(conn.j, conn.i) = conn.w
                End If
            Next

            Return design
        End Function

        ''' <summary>
        ''' 模拟多组学数据 (用于测试和验证算法)
        ''' 生成具有已知类别结构和块间相关性的多组学数据集
        ''' </summary>
        ''' <param name="nSamples">样本数</param>
        ''' <param name="nClasses">类别数</param>
        ''' <param name="blockSizes">每个数据块的变量数</param>
        ''' <param name="nInformative">每个块中与类别相关的信息变量数</param>
        ''' <param name="correlationStrength">块间相关强度 [0, 1]</param>
        ''' <param name="noiseLevel">噪声水平</param>
        ''' <param name="seed">随机种子</param>
        Public Shared Function SimulateMultiOmicsData(nSamples As Integer,
                                                      nClasses As Integer,
                                                      blockSizes As Integer(),
                                                      nInformative As Integer(),
                                                      Optional correlationStrength As Double = 0.8,
                                                      Optional noiseLevel As Double = 0.5,
                                                      Optional seed As Integer = 42) As SimulatedData
            Dim rng As New Random(seed)
            Dim nBlocks As Integer = blockSizes.Length

            ' 生成类别标签 (均匀分布)
            Dim labels(nSamples - 1) As Integer
            For i As Integer = 0 To nSamples - 1
                labels(i) = i * nClasses \ nSamples
            Next
            ' 打乱
            For i As Integer = nSamples - 1 To 1 Step -1
                Dim j As Integer = rng.Next(0, i + 1)
                Dim temp As Integer = labels(i)
                labels(i) = labels(j)
                labels(j) = temp
            Next

            Dim classLabels(nClasses - 1) As String
            For c As Integer = 0 To nClasses - 1
                classLabels(c) = $"Class_{c}"
            Next

            ' 生成共享的潜在因子 (确保块间相关性)
            Dim latentFactors As New Matrix(nSamples, nClasses - 1)
            For i As Integer = 0 To nSamples - 1
                For c As Integer = 0 To nClasses - 2
                    If labels(i) = c Then
                        latentFactors(i, c) = 1.0 + correlationStrength * (rng.NextDouble() - 0.5)
                    Else
                        latentFactors(i, c) = -0.5 * correlationStrength + (rng.NextDouble() - 0.5) * (1 - correlationStrength)
                    End If
                Next
            Next

            ' 生成每个数据块
            Dim X As New List(Of Matrix)()
            For k As Integer = 0 To nBlocks - 1
                Dim Xk As New Matrix(nSamples, blockSizes(k))
                Dim nInfo As Integer = Math.Min(nInformative(k), blockSizes(k))

                ' 信息变量: 与类别结构相关
                For j As Integer = 0 To nInfo - 1
                    ' 随机生成载荷
                    Dim loadings As New Matrix(nClasses - 1, 1)
                    For c As Integer = 0 To nClasses - 2
                        loadings(c, 0) = (rng.NextDouble() - 0.5) * 2.0
                    Next

                    ' 生成变量值
                    Dim col As Matrix = latentFactors * loadings
                    For i As Integer = 0 To nSamples - 1
                        Xk(i, j) = col(i, 0) + noiseLevel * (rng.NextDouble() - 0.5) * 2.0
                    Next
                Next

                ' 噪声变量: 与类别无关
                For j As Integer = nInfo To blockSizes(k) - 1
                    For i As Integer = 0 To nSamples - 1
                        Xk(i, j) = rng.NextDouble() * 2.0 - 1.0
                    Next
                Next

                X.Add(Xk)
            Next

            Return New SimulatedData With {
                .X = X,
                .YLabels = labels,
                .ClassLabels = classLabels,
                .NInformative = nInformative
            }
        End Function

        ''' <summary>
        ''' 计算AUC (Area Under the ROC Curve) - 二分类情况
        ''' 使用梯形法计算ROC曲线下面积
        ''' </summary>
        Public Shared Function ComputeAUC(scores As Double(), labels As Integer(), positiveClass As Integer) As Double
            Dim n As Integer = scores.Length
            Dim pairs As New List(Of (score As Double, label As Integer))
            For i As Integer = 0 To n - 1
                pairs.Add((scores(i), labels(i)))
            Next
            pairs = pairs.OrderByDescending(Function(p) p.score).ToList()

            Dim tp As Integer = 0, fp As Integer = 0
            Dim totalPos As Integer = labels.Count(Function(l) l = positiveClass)
            Dim totalNeg As Integer = n - totalPos

            If totalPos = 0 OrElse totalNeg = 0 Then Return 0.5

            Dim auc As Double = 0.0
            Dim prevFPR As Double = 0.0, prevTPR As Double = 0.0

            For Each pair In pairs
                If pair.label = positiveClass Then
                    tp += 1
                Else
                    fp += 1
                End If

                Dim fpr As Double = CDbl(fp) / totalNeg
                Dim tpr As Double = CDbl(tp) / totalPos

                auc += (fpr - prevFPR) * (tpr + prevTPR) / 2.0
                prevFPR = fpr
                prevTPR = tpr
            Next

            Return auc
        End Function

        ''' <summary>
        ''' 多分类AUC (One-vs-Rest 平均)
        ''' </summary>
        Public Shared Function ComputeMulticlassAUC(scores As Matrix, labels As Integer(), nClasses As Integer) As Double
            Dim totalAUC As Double = 0.0
            For c As Integer = 0 To nClasses - 1
                Dim classScores(labels.Length - 1) As Double
                For i As Integer = 0 To labels.Length - 1
                    classScores(i) = scores(i, c)
                Next
                totalAUC += ComputeAUC(classScores, labels, c)
            Next
            Return totalAUC / nClasses
        End Function

    End Class

    ''' <summary>
    ''' 模拟数据结果
    ''' </summary>
    Public Class SimulatedData
        ''' <summary>多组学数据块列表</summary>
        Public X As List(Of Matrix)
        ''' <summary>类别标签</summary>
        Public YLabels As Integer()
        ''' <summary>类别名称</summary>
        Public ClassLabels As String()
        ''' <summary>每个块的信息变量数</summary>
        Public NInformative As Integer()
    End Class

    ' ========================================================================
    ' 8. 多块整合分析扩展模块
    ' ========================================================================

    ''' <summary>
    ''' 多块整合分析扩展工具
    ''' 提供DIABLO之外的多组学整合辅助分析方法
    ''' </summary>
    Public Class MultiBlockIntegration

        ''' <summary>
        ''' 计算RGCCA (Regularized Generalized Canonical Correlation Analysis)
        ''' 风格的块间协方差矩阵
        '''
        ''' 这是DIABLO的理论基础之一，用于量化多个数据块之间的整体关联程度
        ''' </summary>
        ''' <param name="X">数据块列表 (已中心化)</param>
        ''' <param name="design">设计矩阵</param>
        ''' <param name="tau">正则化参数 (0=最大协方差, 1=最大相关)</param>
        Public Shared Function ComputeRGCCACovariance(X As List(Of Matrix),
                                                      design As Matrix,
                                                      Optional tau As Double() = Nothing) As Matrix
            Dim nBlocks As Integer = X.Count
            Dim nSamples As Integer = X(0).Rows

            ' 计算块间协方差矩阵 C
            Dim C As New Matrix(nBlocks, nBlocks)
            For i As Integer = 0 To nBlocks - 1
                For j As Integer = 0 To nBlocks - 1
                    If i = j Then
                        C(i, j) = 1.0
                    Else
                        ' 计算块间协方差: tr(X_i^T * X_j) / (n-1)
                        Dim cov As Matrix = (1.0 / (nSamples - 1)) * (X(i).Transpose() * X(j))
                        ' 取Frobenius范数作为块间关联度量
                        C(i, j) = cov.FrobeniusNorm() * design(i, j)
                    End If
                Next
            Next

            ' 应用正则化
            If tau IsNot Nothing Then
                For i As Integer = 0 To nBlocks - 1
                    If i < tau.Length Then
                        Dim diagVal As Double = C(i, i)
                        C(i, i) = (1 - tau(i)) * diagVal + tau(i)
                    End If
                Next
            End If

            Return C
        End Function

        ''' <summary>
        ''' 计算共识矩阵 (Consensus Matrix)
        ''' 用于评估多组学整合的稳定性
        ''' 基于多次bootstrap采样的变量选择一致性
        ''' </summary>
        Public Shared Function ComputeConsensusMatrix(selectedVars As List(Of List(Of Boolean()))) As Matrix
            If selectedVars Is Nothing OrElse selectedVars.Count = 0 Then
                Throw New ArgumentException("变量选择结果不能为空")
            End If

            Dim nVars As Integer = selectedVars(0)(0).Length
            Dim nRuns As Integer = selectedVars.Count

            ' 计算变量对共选频率
            Dim consensus As New Matrix(nVars, nVars)
            For run As Integer = 0 To nRuns - 1
                For comp As Integer = 0 To selectedVars(run).Count - 1
                    Dim sel As Boolean() = selectedVars(run)(comp)
                    For i As Integer = 0 To nVars - 1
                        For j As Integer = i To nVars - 1
                            If sel(i) AndAlso sel(j) Then
                                consensus(i, j) += 1.0
                                If i <> j Then consensus(j, i) += 1.0
                            End If
                        Next
                    Next
                Next
            Next

            ' 归一化到 [0, 1]
            For i As Integer = 0 To nVars - 1
                For j As Integer = 0 To nVars - 1
                    consensus(i, j) /= nRuns
                Next
            Next

            Return consensus
        End Function

        ''' <summary>
        ''' 计算网络拓扑指标
        ''' 用于评估多组学变量选择结果的网络特性
        ''' </summary>
        ''' <param name="adjacency">邻接矩阵 (加权)</param>
        ''' <param name="threshold">边的权重阈值</param>
        Public Shared Function ComputeNetworkMetrics(adjacency As Matrix,
                                                     Optional threshold As Double = 0.5) As NetworkMetrics
            Dim n As Integer = adjacency.Rows
            Dim metrics As New NetworkMetrics()

            ' 计算度 (degree)
            Dim degrees(n - 1) As Integer
            Dim weightedDegrees(n - 1) As Double
            Dim nEdges As Integer = 0

            For i As Integer = 0 To n - 1
                For j As Integer = i + 1 To n - 1
                    If adjacency(i, j) >= threshold Then
                        degrees(i) += 1
                        degrees(j) += 1
                        nEdges += 1
                    End If
                    weightedDegrees(i) += adjacency(i, j)
                    weightedDegrees(j) += adjacency(i, j)
                Next
            Next

            metrics.Degrees = degrees
            metrics.WeightedDegrees = weightedDegrees
            metrics.NEdges = nEdges
            metrics.Density = 2.0 * nEdges / (n * (n - 1))

            ' 计算hub变量 (度最高的变量)
            Dim sortedIndices As Integer() = Enumerable.Range(0, n).OrderByDescending(Function(i) weightedDegrees(i)).ToArray()
            Dim nHubs As Integer = Math.Max(1, n \ 10)
            Dim hubs(nHubs - 1) As Integer
            Array.Copy(sortedIndices, hubs, nHubs)
            metrics.HubVariables = hubs

            ' 计算聚类系数
            Dim clusterCoeffs(n - 1) As Double
            For i As Integer = 0 To n - 1
                Dim neighbors As New List(Of Integer)
                For j As Integer = 0 To n - 1
                    If j <> i AndAlso adjacency(i, j) >= threshold Then
                        neighbors.Add(j)
                    End If
                Next

                If neighbors.Count < 2 Then
                    clusterCoeffs(i) = 0.0
                Else
                    Dim triangles As Integer = 0
                    Dim possible As Integer = neighbors.Count * (neighbors.Count - 1) \ 2
                    For a As Integer = 0 To neighbors.Count - 2
                        For b As Integer = a + 1 To neighbors.Count - 1
                            If adjacency(neighbors(a), neighbors(b)) >= threshold Then
                                triangles += 1
                            End If
                        Next
                    Next
                    clusterCoeffs(i) = CDbl(triangles) / possible
                End If
            Next
            metrics.ClusteringCoefficients = clusterCoeffs
            metrics.AvgClusteringCoefficient = clusterCoeffs.Average()

            Return metrics
        End Function

        ''' <summary>
        ''' 计算组学块间的Procrustes分析
        ''' 评估两个数据块在成分空间中的配置一致性
        ''' </summary>
        Public Shared Function ProcrustesAnalysis(X1Scores As Matrix, X2Scores As Matrix) As ProcrustesResult
            Dim n As Integer = X1Scores.Rows
            Dim p As Integer = Math.Min(X1Scores.Cols, X2Scores.Cols)

            ' 中心化
            Dim X1c As Matrix = X1Scores.Center()
            Dim X2c As Matrix = X2Scores.Center()

            ' 计算最优旋转: R = V * U^T (来自 SVD(X2^T * X1))
            Dim M As Matrix = X2c.Transpose() * X1c
            Dim svdResult = LinearAlgebra.SVD(M)
            Dim R As Matrix = svdResult.VT.Transpose() * svdResult.U.Transpose()

            ' 旋转X2
            Dim X2rotated As Matrix = X2c * R

            ' 计算Procrustes统计量 (残差平方和)
            Dim ss As Double = 0.0
            For i As Integer = 0 To n - 1
                For j As Integer = 0 To p - 1
                    ss += (X1c(i, j) - X2rotated(i, j)) ^ 2
                Next
            Next

            ' 计算Procrustes相关系数 (m^2)
            Dim norm1 As Double = X1c.FrobeniusNorm()
            Dim norm2 As Double = X2c.FrobeniusNorm()
            Dim m2 As Double = 1.0 - ss / (norm1 * norm1 + norm2 * norm2)

            Return New ProcrustesResult With {
                .RotationMatrix = R,
                .SumOfSquares = ss,
                .ProcrustesCorrelation = Sqrt(Math.Max(0, m2)),
                .X2Rotated = X2rotated
            }
        End Function

    End Class

    ''' <summary>
    ''' 网络拓扑指标
    ''' </summary>
    Public Class NetworkMetrics
        ''' <summary>每个变量的度</summary>
        Public Degrees As Integer()
        ''' <summary>每个变量的加权度</summary>
        Public WeightedDegrees As Double()
        ''' <summary>边数</summary>
        Public NEdges As Integer
        ''' <summary>网络密度</summary>
        Public Density As Double
        ''' <summary>Hub变量索引</summary>
        Public HubVariables As Integer()
        ''' <summary>每个变量的聚类系数</summary>
        Public ClusteringCoefficients As Double()
        ''' <summary>平均聚类系数</summary>
        Public AvgClusteringCoefficient As Double
    End Class

    ''' <summary>
    ''' Procrustes分析结果
    ''' </summary>
    Public Class ProcrustesResult
        ''' <summary>旋转矩阵</summary>
        Public RotationMatrix As Matrix
        ''' <summary>残差平方和</summary>
        Public SumOfSquares As Double
        ''' <summary>Procrustes相关系数</summary>
        Public ProcrustesCorrelation As Double
        ''' <summary>旋转后的X2</summary>
        Public X2Rotated As Matrix
    End Class

    ' ========================================================================
    ' 9. DIABLO 完整分析流程封装
    ' ========================================================================

    ''' <summary>
    ''' DIABLO 完整分析流程
    ''' 封装从数据预处理、参数调优到模型拟合和评估的完整流程
    ''' </summary>
    Public Class DIABLOPipeline

        ''' <summary>
        ''' 执行完整的DIABLO分析流程
        ''' </summary>
        ''' <param name="X">多组学数据块列表</param>
        ''' <param name="YLabels">类别标签</param>
        ''' <param name="classLabels">类别名称</param>
        ''' <param name="ncomp">成分数 (若为0则自动调优)</param>
        ''' <param name="keepX">稀疏度参数 (若为Nothing则自动调优)</param>
        ''' <param name="design">设计矩阵 (若为Nothing则全连接)</param>
        ''' <param name="autoTune">是否自动调优参数</param>
        ''' <param name="nFolds">交叉验证折数</param>
        Public Shared Function Run(X As List(Of Matrix),
                                   YLabels As Integer(),
                                   classLabels As String(),
                                   Optional ncomp As Integer = 0,
                                   Optional keepX As List(Of Integer()) = Nothing,
                                   Optional design As Matrix = Nothing,
                                   Optional autoTune As Boolean = True,
                                   Optional nFolds As Integer = 5) As DIABLOPipelineResult

            Dim result As New DIABLOPipelineResult()
            Dim nBlocks As Integer = X.Count
            Dim nClasses As Integer = classLabels.Length

            ' ---- 步骤1: 参数调优 ----
            If autoTune Then
                Dim cv As New DIABLOCrossValidation(X, YLabels, classLabels)

                ' 调优成分数
                If ncomp <= 0 Then
                    Dim ncompRange As Integer() = {1, 2, 3, Math.Min(5, X(0).Rows \ nClasses)}
                    ncompRange = ncompRange.Distinct().ToArray()
                    Dim ncompResult = cv.TuneNComp(ncompRange, nFolds, design, keepX)
                    ncomp = ncompResult.OptimalNComp
                    result.NCompTuning = ncompResult
                End If

                ' 调优keepX
                If keepX Is Nothing Then
                    Dim keepXGrid As New List(Of Integer())()
                    For k As Integer = 0 To nBlocks - 1
                        Dim p As Integer = X(k).Cols
                        Dim grid As Integer() = {
                            Math.Max(1, p \ 50),
                            Math.Max(1, p \ 20),
                            Math.Max(1, p \ 10),
                            Math.Max(1, p \ 5),
                            Math.Min(p, Math.Max(1, p \ 2))
                        }.Distinct().OrderBy(Function(x) x).ToArray()
                        keepXGrid.Add(grid)
                    Next

                    Dim keepXResult = cv.TuneKeepX(ncomp, keepXGrid, nFolds, design)
                    keepX = New List(Of Integer())()
                    For k As Integer = 0 To nBlocks - 1
                        Dim kx(ncomp - 1) As Integer
                        For h As Integer = 0 To ncomp - 1
                            kx(h) = keepXResult.OptimalKeepX(k)
                        Next
                        keepX.Add(kx)
                    Next
                    result.KeepXTuning = keepXResult
                End If
            Else
                ' 使用默认参数
                If ncomp <= 0 Then ncomp = 2
                If keepX Is Nothing Then
                    keepX = New List(Of Integer())()
                    For k As Integer = 0 To nBlocks - 1
                        Dim kx(ncomp - 1) As Integer
                        For h As Integer = 0 To ncomp - 1
                            kx(h) = X(k).Cols
                        Next
                        keepX.Add(kx)
                    Next
                End If
            End If

            result.NComp = ncomp
            result.KeepX = keepX

            ' ---- 步骤2: 拟合DIABLO模型 ----
            Dim model As New DIABLO(X, YLabels, classLabels, ncomp, design, keepX)
            model.Fit()
            result.Model = model

            ' ---- 步骤3: 模型评估 ----
            ' 训练集预测
            Dim trainPred As Integer() = model.Predict(X, "centroids_dist")
            Dim trainEval = DIABLOCrossValidation.ComputeBER(trainPred, YLabels, nClasses)
            result.TrainBER = trainEval.BER
            result.TrainAccuracy = trainEval.Accuracy

            ' ---- 步骤4: 计算块间相关性 ----
            result.BlockCorrelations = model.ComputeBlockCorrelation()

            ' ---- 步骤5: 变量重要性 ----
            result.VariableImportance = model.ComputeVariableImportance()

            ' ---- 步骤6: 选中的变量 ----
            result.SelectedVariables = model.GetSelectedVariables()

            ' ---- 步骤7: 解释方差 ----
            result.ExplainedVariance = New List(Of Double())()
            For k As Integer = 0 To nBlocks - 1
                Dim ev(ncomp - 1) As Double
                For h As Integer = 0 To ncomp - 1
                    ev(h) = model.ExplainedVariance(k)(h)
                Next
                result.ExplainedVariance.Add(ev)
            Next

            Return result
        End Function

    End Class

    ''' <summary>
    ''' DIABLO完整分析流程结果
    ''' </summary>
    Public Class DIABLOPipelineResult
        ''' <summary>使用的成分数</summary>
        Public NComp As Integer
        ''' <summary>使用的keepX参数</summary>
        Public KeepX As List(Of Integer())
        ''' <summary>拟合的DIABLO模型</summary>
        Public Model As DIABLO
        ''' <summary>训练集BER</summary>
        Public TrainBER As Double
        ''' <summary>训练集准确率</summary>
        Public TrainAccuracy As Double
        ''' <summary>块间相关性矩阵</summary>
        Public BlockCorrelations As Matrix(,)
        ''' <summary>变量重要性</summary>
        Public VariableImportance As List(Of Double())
        ''' <summary>选中的变量索引</summary>
        Public SelectedVariables As List(Of List(Of Integer()))
        ''' <summary>每个块每个成分的解释方差</summary>
        Public ExplainedVariance As List(Of Double())
        ''' <summary>成分数调优结果</summary>
        Public NCompTuning As CVNCompResult
        ''' <summary>keepX调优结果</summary>
        Public KeepXTuning As CVKeepXResult
    End Class

End Namespace
