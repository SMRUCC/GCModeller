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

        ''' <summary>主成分方向，<c>[k][dimension]</c>（每行是一个单位主成分向量）。</summary>
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

        ''' <summary>训练时各维的均值。</summary>
        Public ReadOnly Property Mean As Double()
            Get
                Return _mean
            End Get
        End Property

        ''' <summary>拟合主成分。</summary>
        ''' <param name="samples">样本矩阵 <c>[n][dimension]</c>。</param>
        ''' <param name="components">保留的主成分个数。</param>
        Public Shared Function Fit(samples As Double()(), components As Integer) As PcaProjection
            If samples Is Nothing OrElse samples.Length = 0 Then
                Throw New ArgumentException("样本矩阵为空", NameOf(samples))
            End If

            Dim n = samples.Length
            Dim dimension = samples(0).Length
            Dim k = std.Max(1, std.Min(components, dimension))

            ' 中心化
            Dim mean(dimension - 1) As Double
            For i As Integer = 0 To n - 1
                For j As Integer = 0 To dimension - 1
                    mean(j) += samples(i)(j)
                Next
            Next
            For j As Integer = 0 To dimension - 1
                mean(j) /= n
            Next

            Dim centered As Double()() = New Double(n - 1)() {}
            For i As Integer = 0 To n - 1
                Dim row(dimension - 1) As Double
                For j As Integer = 0 To dimension - 1
                    row(j) = samples(i)(j) - mean(j)
                Next
                centered(i) = row
            Next

            ' 协方差矩阵（对称）
            Dim covariance(dimension - 1, dimension - 1) As Double
            For a As Integer = 0 To dimension - 1
                For b As Integer = a To dimension - 1
                    Dim total As Double = 0.0
                    For i As Integer = 0 To n - 1
                        total += centered(i)(a) * centered(i)(b)
                    Next

                    Dim value = total / std.Max(1, n - 1)
                    covariance(a, b) = value
                    covariance(b, a) = value
                Next
            Next

            ' Jacobi 对称特征分解
            Dim eigenvectors As Double()() = Identity(dimension)
            Dim eigenvalues As Double() = Jacobi(covariance, eigenvectors, dimension)

            ' 按特征值降序排序
            Dim order = Enumerable.Range(0, dimension).ToArray()
            Array.Sort(order, Function(a, b) eigenvalues(b).CompareTo(eigenvalues(a)))

            Dim varianceTotal As Double = 0.0
            For Each value In eigenvalues
                If value > 0.0 Then varianceTotal += value
            Next

            Dim picked As Double()() = New Double(k - 1)() {}
            Dim ratios(k - 1) As Double
            For i As Integer = 0 To k - 1
                picked(i) = eigenvectors(order(i))
                ratios(i) = If(varianceTotal > 0.0, std.Max(0.0, eigenvalues(order(i))) / varianceTotal, 0.0)
            Next

            Return New PcaProjection(picked, mean, ratios)
        End Function

        ''' <summary>由张量 <c>[n, dimension]</c> 拟合。</summary>
        Public Shared Function Fit(tensor As Tensor, components As Integer) As PcaProjection
            Return Fit(ToRows(tensor), components)
        End Function

        ''' <summary>把样本矩阵投影到全部已保留的主成分。</summary>
        Public Function Project(samples As Double()()) As Double()()
            Dim result As Double()() = New Double(samples.Length - 1)() {}

            For i As Integer = 0 To samples.Length - 1
                Dim row(_components.Length - 1) As Double
                For c As Integer = 0 To _components.Length - 1
                    Dim total As Double = 0.0
                    For j As Integer = 0 To _mean.Length - 1
                        total += (samples(i)(j) - _mean(j)) * _components(c)(j)
                    Next
                    row(c) = total
                Next
                result(i) = row
            Next

            Return result
        End Function

        ''' <summary>把张量 <c>[n, dimension]</c> 投影到二维（不足两个主成分时第二维补 0）。</summary>
        Public Function Project2D(tensor As Tensor) As Double()()
            Dim projected = Project(ToRows(tensor))
            Dim result As Double()() = New Double(projected.Length - 1)() {}

            For i As Integer = 0 To projected.Length - 1
                Dim x = projected(i)(0)
                Dim y = If(projected(i).Length > 1, projected(i)(1), 0.0)
                result(i) = New Double() {x, y}
            Next

            Return result
        End Function

        ''' <summary>张量 <c>[n, dimension]</c> → 行数组。</summary>
        Public Shared Function ToRows(tensor As Tensor) As Double()()
            Dim rows = tensor.Shape(0)
            Dim columns = tensor.Shape(1)
            Dim data = tensor.Data
            Dim result As Double()() = New Double(rows - 1)() {}

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

        Private Shared Function Identity(size As Integer) As Double()()
            Dim result As Double()() = New Double(size - 1)() {}
            For i As Integer = 0 To size - 1
                Dim row(size - 1) As Double
                row(i) = 1.0
                result(i) = row
            Next
            Return result
        End Function

        ''' <summary>
        ''' Jacobi 特征分解（原地对 <paramref name="matrix"/> 做旋转变换）。
        ''' 输出：<paramref name="eigenvectors"/> 被改写为按**行**存放的主成分方向，返回特征值。
        ''' </summary>
        Private Shared Function Jacobi(matrix As Double(,), eigenvectors As Double()(), size As Integer) As Double()
            Dim maxSweeps = 100
            Dim tolerance = 1.0E-12

            For sweep As Integer = 1 To maxSweeps
                Dim offDiagonal As Double = 0.0
                For p As Integer = 0 To size - 2
                    For q As Integer = p + 1 To size - 1
                        offDiagonal += matrix(p, q) * matrix(p, q)
                    Next
                Next

                If std.Sqrt(offDiagonal) < tolerance Then Exit For

                For p As Integer = 0 To size - 2
                    For q As Integer = p + 1 To size - 1
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

                        ' 列更新：A ← A·J
                        For k As Integer = 0 To size - 1
                            Dim akp = matrix(k, p)
                            Dim akq = matrix(k, q)
                            matrix(k, p) = c * akp - s * akq
                            matrix(k, q) = s * akp + c * akq
                        Next

                        ' 行更新：A ← Jᵀ·A
                        For k As Integer = 0 To size - 1
                            Dim apk = matrix(p, k)
                            Dim aqk = matrix(q, k)
                            matrix(p, k) = c * apk - s * aqk
                            matrix(q, k) = s * apk + c * aqk
                        Next

                        ' 累积特征向量：V ← V·J
                        For k As Integer = 0 To size - 1
                            Dim vkp = eigenvectors(k)(p)
                            Dim vkq = eigenvectors(k)(q)
                            eigenvectors(k)(p) = c * vkp - s * vkq
                            eigenvectors(k)(q) = s * vkp + c * vkq
                        Next
                    Next
                Next
            Next

            ' 特征值取对角元；特征向量由「按列」转成「按行」，便于直接取主成分方向
            Dim eigenvalues(size - 1) As Double
            Dim byRow As Double()() = New Double(size - 1)() {}

            For j As Integer = 0 To size - 1
                eigenvalues(j) = matrix(j, j)

                Dim row(size - 1) As Double
                For i As Integer = 0 To size - 1
                    row(i) = eigenvectors(i)(j)
                Next
                byRow(j) = row
            Next

            For i As Integer = 0 To size - 1
                eigenvectors(i) = byRow(i)
            Next

            Return eigenvalues
        End Function
    End Class
End Namespace
