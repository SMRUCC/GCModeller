Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports std = System.Math

Namespace Residual

    ''' <summary>
    ''' 低秩"响应基"：把一份基因×扰动的效应矩阵分解成"基因载荷 U × 扰动坐标"。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 拟合前先把<b>自身靶</b>的位置（基因名等于扰动名的对角元）替换成"该行其余扰动的均值"：
    ''' 自身靶的效应是"敲低本身"，量级远大于真实的上下游响应，
    ''' 直接参与分解会让第一主成分退化成"敲低强度"，把真正的共调控结构挤到后面的成分里。
    ''' </para>
    ''' <para>
    ''' 分解用随机化 SVD（<see cref="RandomizedSVD"/>）：秩取 <c>residual_rank</c>，
    ''' 过采样 8、两次幂迭代，与上游的数值路径一致；不把 <c>G×P</c> 的中间矩阵稠密化两次。
    ''' </para>
    ''' <para>
    ''' 每个成分再除以它的"响应尺度"（<c>Uᵀ·centred</c> 的均方根），
    ''' 使后续的逐基因岭回归在同一个量纲上进行。
    ''' </para>
    ''' </remarks>
    Public Class ResponseBasis

        ''' <summary>基因载荷（基因×秩）。</summary>
        Public Property Basis As Double()()

        ''' <summary>每个基因的截距（逐行非对角均值）。</summary>
        Public Property Intercept As Double()

        ''' <summary>中心化后的矩阵（基因×扰动）。</summary>
        Public Property Centered As Double()()

        ''' <summary>每个成分的响应尺度（长度 = 实际秩）。</summary>
        Public Property ResponseScale As Double()

        ''' <summary>训练坐标 <c>Uᵀ·centred / scale</c>（秩×扰动）。</summary>
        Public Property Coordinates As Double()()

        ''' <summary>实际保留的秩。</summary>
        Public ReadOnly Property Rank As Integer
            Get
                Return If(Basis Is Nothing, 0, Basis(0).Length)
            End Get
        End Property

        ''' <summary>
        ''' 拟合一族效应矩阵的响应基。
        ''' </summary>
        ''' <param name="matrix">基因×扰动的效应矩阵（训练行上的反方差均值）</param>
        ''' <param name="rows">"自身靶"所在的基因下标</param>
        ''' <param name="columns">对应的扰动下标</param>
        ''' <param name="rank">请求的秩</param>
        ''' <param name="seed">随机化 SVD 的种子</param>
        Public Shared Function Fit(matrix As Double()(), rows As Integer(), columns As Integer(),
                                   rank As Integer, seed As Integer) As ResponseBasis
            If matrix Is Nothing Then
                Throw New ArgumentNullException(NameOf(matrix))
            End If

            Dim nGenes As Integer = matrix.Length
            Dim nPerts As Integer = If(nGenes = 0, 0, matrix(0).Length)
            Dim training As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)

            For i As Integer = 0 To nGenes - 1
                For j As Integer = 0 To nPerts - 1
                    training(i)(j) = matrix(i)(j)
                Next
            Next

            ' 自身靶置零后求"行非对角均值"
            Dim counts(nGenes - 1) As Integer

            For i As Integer = 0 To nGenes - 1
                counts(i) = nPerts
            Next

            For i As Integer = 0 To rows.Length - 1
                Dim at As Integer = rows(i)

                If at >= 0 AndAlso at < nGenes AndAlso columns(i) >= 0 AndAlso columns(i) < nPerts Then
                    training(at)(columns(i)) = 0.0
                    counts(at) -= 1
                End If
            Next

            Dim intercept(nGenes - 1) As Double

            For i As Integer = 0 To nGenes - 1
                Dim sum As Double = 0.0

                For j As Integer = 0 To nPerts - 1
                    sum += training(i)(j)
                Next

                intercept(i) = If(counts(i) > 0, sum / counts(i), 0.0)
            Next

            For i As Integer = 0 To rows.Length - 1
                Dim at As Integer = rows(i)

                If at >= 0 AndAlso at < nGenes AndAlso columns(i) >= 0 AndAlso columns(i) < nPerts Then
                    training(at)(columns(i)) = intercept(at)
                End If
            Next

            Dim centered As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)

            For i As Integer = 0 To nGenes - 1
                For j As Integer = 0 To nPerts - 1
                    centered(i)(j) = training(i)(j) - intercept(i)
                Next
            Next

            Dim request As Integer = std.Min(rank, std.Min(nGenes, nPerts))

            If request <= 0 Then
                Return New ResponseBasis With {
                    .Basis = RectangularArray.Matrix(Of Double)(nGenes, 0),
                    .Intercept = intercept,
                    .Centered = centered,
                    .ResponseScale = New Double(-1) {},
                    .Coordinates = RectangularArray.Matrix(Of Double)(0, nPerts)
                }
            End If

            Dim svd As New RandomizedSVD(centered, request, oversampling:=8, powerIterations:=2, seed:=seed)
            Dim basis As Double()() = svd.U
            Dim kept As Integer = svd.RetainedRank
            Dim trimmed As Double()() = RectangularArray.Matrix(Of Double)(nGenes, kept)

            For i As Integer = 0 To nGenes - 1
                For c As Integer = 0 To kept - 1
                    trimmed(i)(c) = basis(i)(c)
                Next
            Next

            Dim scale(If(kept = 0, -1, kept - 1)) As Double

            For c As Integer = 0 To kept - 1
                Dim acc As Double = 0.0

                For j As Integer = 0 To nPerts - 1
                    Dim value As Double = 0.0

                    For i As Integer = 0 To nGenes - 1
                        value += trimmed(i)(c) * centered(i)(j)
                    Next

                    acc += value * value
                Next

                Dim rms As Double = std.Sqrt(acc / std.Max(nPerts, 1))

                scale(c) = If(rms > 0.0, rms, 1.0)
            Next

            Dim coordinates As Double()() = RectangularArray.Matrix(Of Double)(kept, nPerts)

            For c As Integer = 0 To kept - 1
                For j As Integer = 0 To nPerts - 1
                    Dim value As Double = 0.0

                    For i As Integer = 0 To nGenes - 1
                        value += trimmed(i)(c) * centered(i)(j)
                    Next

                    coordinates(c)(j) = value / scale(c)
                Next
            Next

            Return New ResponseBasis With {
                .Basis = trimmed,
                .Intercept = intercept,
                .Centered = centered,
                .ResponseScale = scale,
                .Coordinates = coordinates
            }
        End Function

    End Class
End Namespace
