Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace Evaluation

    ''' <summary>
    ''' 主成分分析（自实现）。
    '''
    ''' 仓库内没有可复用的降维实现（全库检索 PCA / UMAP / 聚类均无实质命中），
    ''' 因此这里用「协方差矩阵 + Jacobi 对称特征分解」实现一个足够稳健的主成分分析，
    ''' 仅用于把高维表达谱 / 语义隐空间投影到二维以便导出可视化数据。
    '''
    ''' 算法：中心化 → 协方差 → Jacobi 迭代求特征值/特征向量 → 按特征值降序取前 k 个主成分。
    ''' </summary>
    Public Class PcaProjection

        Private ReadOnly _components As Double()()
        Private ReadOnly _mean As Double()
        Private ReadOnly _explainedRatio As Double()

        Private Sub New(components As Double()(), mean As Double(), explainedRatio As Double())
            Me._components = components
            Me._mean = mean
            Me._explainedRatio = explainedRatio
        End Sub

        ''' <summary>主成分方向，<c>[k][dim]</c>（每行是一个单位主成分向量）。</summary>
        Public ReadOnly Property Components As Double()()
            Get
                Return _components
            End Get
        End Property

        ''' <summary>各主成分解释的方差占比。</summary>
        Public ReadOnly Property ExplainedVarianceRatio As Double()
            Get
                Return _explainedRatio
            End Get
        End Property

        ''' <summary>主成分个数。</summary>
        Public ReadOnly Property ComponentCount As Integer
            Get
                Return _components.Length
            End Get
        End Property

        ''' <summary>训练时各维的均值（投影时需要减去）。</summary>
        Public ReadOnly Property Mean As Double()
            Get
                Return _mean
            End Get
        End Property

        ''' <summary>
        ''' 拟合主成分。
        ''' </summary>
        ''' <param name="samples">样本矩阵 <c>[n][dim]</c>。</param>
        ''' <param name="components">保留的主成分个数。</param>
        Public Shared Function Fit(samples As Double()(), components As Integer) As PcaProjection
            If samples Is Nothing OrElse samples.Length = 0 Then
                Throw New ArgumentException("样本矩阵为空", NameOf(samples))
            End If

            Dim n = samples.Length
            Dim dim = samples(0).Length
            Dim k = std.Max(1, std.Min(components, dim))

            ' 中心化
            Dim mean(dim - 1) As Double
            For i As Integer = 0 To n - 1
                For j As Integer = 0 To dim - 1
                    mean(j) += samples(i)(j)
                Next
            Next
            For j Integer = 0 To dim - 1
                mean(j) /= n
            Next

            Dim centered(n - 1)() As Double
            For i As Integer = 0 To n - 1
                Dim row(dim - 1) As Double
                For j As Integer = 0 To dim - 1
                    row(j) = samples(i)(j) - mean(j)
                Next
                centered(i) = row
            Next

            ' 协方差矩阵（对称）
            Dim covariance(dim - 1, dim - 1) As Double
            For a As Integer = 0 To dim - 1
                For b As Integer = a To dim - 1
                    Dim sum As Double = 0.0
                    For i As Integer = 0 To n - 1
                        sum += centered(i)(a) * centered(i)(b)
                    Next

                    Dim value = sum / std.Max(1, n - 1)
                    covariance(a, b) = value
                    covariance(b, a) = value
                Next
            Next

            ' Jacobi 对称特征分解
            Dim eigenvectors As Double()() = Identity(dim)
            Dim eigenvalues As Double() = Jacobi(covariance, eigenvectors, dim)

            ' 按特征值降序排序
            Dim order = Enumerable.Range(0, dim).ToArray()
            Array.Sort(order, Function(a, b) eigenvalues(b).CompareTo(eigenvalues(a)))

            Dim total As Double = 0.0
            For Each value In eigenvalues
                If value > 0.0 Then total += value
            Next

            Dim picked(k - 1)() As Double
            Dim ratios(k - 1) As Double
            For i Integer = 0 To k - 1
                picked(i) = eigenvectors(order(i))
                ratios(i) = If(total > 0.0, std.Max(0.0, eigenvalues(order(i))) / total, 0.0)
            Next

            Return New PcaProjection(picked, mean, ratios)
        End Function

        ''' <summary>由张量 <c>[n, dim]</c> 拟合。</summary>
        Public Shared Function Fit(tensor As Tensor, components As Integer) As PcaProjection
            Return Fit(ToRows(tensor), components)
        End Function

        ''' <summary>把样本矩阵投影到前 <paramref name="components"/> 个主成分。</summary>
        Public Function Project(samples As Double()()) As Double()()
            Dim result(samples.Length - 1)() As Double

            For i As Integer = 0 To samples.Length - 1
                Dim row(_components.Length - 1) As Double
                For c As Integer = 0 To _components.Length - 1
                    Dim sum As Double = 0.0
                    For j As Integer = 0 To _mean.Length - 1
                        sum += (samples(i)(j) - _mean(j)) * _components(c)(j)
                    Next
                    row(c) = sum
                Next
                result(i) = row
            Next

            Return result
        End Function

        ''' <summary>把张量 <c>[n, dim]</c> 投影到二维（不足两个主成分时补 0）。</summary>
        Public Function Project2D(tensor As Tensor) As Double()()
            Dim projected = Project(ToRows(tensor))
            Dim result(projected.Length - 1)() As Double

            For i Integer = 0 To projected.Length - 1
                Dim x = projected(i)(0)
                Dim y = If(projected(i).Length > 1, projected(i)(1), 0.0)
                result(i) = New Double() {x, y}
            Next

            Return result
        End Function

        ''' <summary>张量 <c>[n, dim]</c> → 行数组 <c>[n][dim]</c>。</summary>
        Public Shared Function ToRows(tensor As Tensor) As Double()()
            Dim rows = tensor.Shape(0)
            Dim columns = tensor.Shape(1)
            Dim data = tensor.Data
            Dim result(rows - 1)() As Double

            For i As Integer = 0 To rows - 1
                Dim row(columns - 1) As Double
                Dim offset = i * columns
                For j As Integer = 0 To columns - 1
                    row(j) = data(offset + j)
                Next
                result(i) = row
            Next

            Return result
        End Function

        Private Shared Function Identity(dim As Integer) As Double()()
            Dim result(dim - 1)() As Double
            For i Integer = 0 To dim - 1
                Dim row(dim - 1) As Double
                row(i) = 1.0
                result(i) = row
            Next
            Return result
        End Function

        ''' <summary>
        ''' Jacobi 特征分解（原地对 <paramref name="matrix"/> 做旋转变换），
        ''' 特征向量按列写回 <paramref name="eigenvectors"/>，返回特征值。
        ''' </summary>
        Private Shared Function Jacobi(matrix As Double(,), eigenvectors As Double()(), dim As Integer) As Double()
            Dim maxSweeps = 100
            Dim tolerance = 1.0E-12

            For sweep As Integer = 1 To maxSweeps
                Dim offDiagonal As Double = 0.0
                For p As Integer = 0 To dim - 2
                    For q As Integer = p + 1 To dim - 1
                        offDiagonal += matrix(p, q) * matrix(p, q)
                    Next
                Next

                If std.Sqrt(offDiagonal) < tolerance Then Exit For

                For p As Integer = 0 To dim - 2
                    For q As Integer = p + 1 To dim - 1
                        If std.Abs(matrix(p, q)) < tolerance Then Continue For

                        Dim theta = (matrix(q, q) - matrix(p, p)) / (2.0 * matrix(p, q))
                        Dim tangent As Double
                        If std.Abs(theta) < 1.0E-300 Then
                            tangent = 1.0
                        Else
                            tangent = std.Sign(theta) / (std.Abs(theta) + std.Sqrt(theta * theta + 1.0))
                        End If

                        Dim c = 1.0 / std.Sqrt(tangent * tangent + 1.0)
                        Dim s = tangent * c

                        ' 更新对称矩阵
                        For k As Integer = 0 To dim - 1
                            Dim mkp = matrix(k, p)
                            Dim mkq = matrix(k, q)
                            matrix(k, p) = c * mkp - s * mkq
                            matrix(k, q) = s * mkp + c * mkq
                        Next

                        For k As Integer = 0 To dim - 1
                            Dim mpk = matrix(p, k)
                            Dim mqk = matrix(q, k)
                            matrix(p, k) = c * mpk - s * mqk
                            matrix(q, k) = s * mpk + c * mqk
                        Next

                        ' 累积特征向量
                        For k As Integer = 0 To dim - 1
                            Dim vkp = eigenvectors(k)(p)
                            Dim vkq = eigenvectors(k)(q)
                            eigenvectors(k)(p) = c * vkp - s * vkq
                            eigenvectors(k)(q) = s * vkp + c * vkq
                        Next
                    Next
                Next
            Next

            ' 特征值（对角元）；特征向量按列存储 → 转置成按行，便于取主成分方向
            Dim eigenvalues(dim - 1) As Double
            Dim byRow(dim - 1)() As Double
            For j As Integer = 0 To dim - 1
                eigenvalues(j) = matrix(j, j)

                Dim row(dim - 1) As Double
                For i Integer = 0 To dim - 1
                    row(i) = eigenvectors(i)(j)
                Next
                byRow(j) = row
            Next

            For i Integer = 0 To dim - 1
                eigenvectors(i) = byRow(i)
            Next

            Return eigenvalues
        End Function
    End Class
End Namespace
