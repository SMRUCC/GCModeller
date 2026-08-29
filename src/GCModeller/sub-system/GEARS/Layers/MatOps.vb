Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace Layers

    ''' <summary>
    ''' 基于 <see cref="Tensor"/> 底层数组实现的高性能矩阵运算工具
    ''' </summary>
    ''' <remarks>
    ''' <see cref="Tensor.MatMul"/> 的实现走的是属性索引器，在 350+ 节点 × 数十维特征的
    ''' 训练循环里（上千次前向/反向）会成为明显瓶颈。这里直接操作 <see cref="Tensor.Data"/>
    ''' 一维数组并按行优先顺序做循环展开，语义与 <see cref="Tensor.MatMul"/> 完全一致，
    ''' 但避免了逐元素的属性调用开销。
    '''
    ''' 所有 *Into 版本都写入调用方提供的缓冲区（不重新分配），用于在训练热路径上复用
    ''' 临时张量、降低 GC 压力。
    ''' </remarks>
    Public Module MatOps

        ''' <summary>
        ''' result = a @ b
        ''' </summary>
        ''' <param name="a">左矩阵 [m, k]</param>
        ''' <param name="b">右矩阵 [k, n]</param>
        ''' <param name="result">输出缓冲区 [m, n]，会被原地覆盖</param>
        Public Sub MulInto(a As Tensor, b As Tensor, result As Tensor)
            Dim m As Integer = a.Shape(0)
            Dim n As Integer = b.Shape(1)
            Dim k As Integer = a.Shape(1)
            Dim ad As Double() = a.Data
            Dim bd As Double() = b.Data
            Dim rd As Double() = result.Data

            For i As Integer = 0 To m - 1
                Dim aOff As Integer = i * k
                Dim rOff As Integer = i * n

                For p As Integer = 0 To k - 1
                    Dim av As Double = ad(aOff + p)

                    If av = 0.0 Then
                        Continue For
                    End If

                    Dim bOff As Integer = p * n

                    For j As Integer = 0 To n - 1
                        rd(rOff + j) += av * bd(bOff + j)
                    Next
                Next
            Next
        End Sub

        ''' <summary>
        ''' 计算 result = a @ b（自动分配输出张量）
        ''' </summary>
        ''' <param name="a">左矩阵 [m, k]</param>
        ''' <param name="b">右矩阵 [k, n]</param>
        ''' <returns>新的 [m, n] 张量</returns>
        Public Function Mul(a As Tensor, b As Tensor) As Tensor
            Dim result As Tensor = New Tensor(a.Shape(0), b.Shape(1))

            Call MulInto(a, b, result)

            Return result
        End Function

        ''' <summary>
        ''' result = aᵀ @ b
        ''' </summary>
        ''' <param name="a">矩阵 [m, k]</param>
        ''' <param name="b">矩阵 [m, n]</param>
        ''' <param name="result">输出缓冲区 [k, n]，会被原地覆盖</param>
        Public Sub MulATInto(a As Tensor, b As Tensor, result As Tensor)
            Dim m As Integer = a.Shape(0)
            Dim k As Integer = a.Shape(1)
            Dim n As Integer = b.Shape(1)
            Dim ad As Double() = a.Data
            Dim bd As Double() = b.Data
            Dim rd As Double() = result.Data

            For p As Integer = 0 To k - 1
                Dim rOff As Integer = p * n

                For i As Integer = 0 To m - 1
                    Dim av As Double = ad(i * k + p)

                    If av = 0.0 Then
                        Continue For
                    End If

                    Dim bOff As Integer = i * n

                    For j As Integer = 0 To n - 1
                        rd(rOff + j) += av * bd(bOff + j)
                    Next
                Next
            Next
        End Sub

        ''' <summary>
        ''' 计算 result = aᵀ @ b（自动分配输出张量）
        ''' </summary>
        ''' <param name="a">矩阵 [m, k]</param>
        ''' <param name="b">矩阵 [m, n]</param>
        ''' <returns>新的 [k, n] 张量</returns>
        Public Function MulAT(a As Tensor, b As Tensor) As Tensor
            Dim result As Tensor = New Tensor(a.Shape(1), b.Shape(1))

            Call MulATInto(a, b, result)

            Return result
        End Function

        ''' <summary>
        ''' result = a @ bᵀ
        ''' </summary>
        ''' <param name="a">矩阵 [m, k]</param>
        ''' <param name="b">矩阵 [n, k]</param>
        ''' <param name="result">输出缓冲区 [m, n]，会被原地覆盖</param>
        Public Sub MulBTInto(a As Tensor, b As Tensor, result As Tensor)
            Dim m As Integer = a.Shape(0)
            Dim n As Integer = b.Shape(0)
            Dim k As Integer = a.Shape(1)
            Dim ad As Double() = a.Data
            Dim bd As Double() = b.Data
            Dim rd As Double() = result.Data

            For i As Integer = 0 To m - 1
                Dim aOff As Integer = i * k
                Dim rOff As Integer = i * n

                For j As Integer = 0 To n - 1
                    Dim sum As Double = 0
                    Dim bOff As Integer = j * k

                    For p As Integer = 0 To k - 1
                        sum += ad(aOff + p) * bd(bOff + p)
                    Next

                    rd(rOff + j) += sum
                Next
            Next
        End Sub

        ''' <summary>
        ''' 计算 result = a @ bᵀ（自动分配输出张量）
        ''' </summary>
        ''' <param name="a">矩阵 [m, k]</param>
        ''' <param name="b">矩阵 [n, k]</param>
        ''' <returns>新的 [m, n] 张量</returns>
        Public Function MulBT(a As Tensor, b As Tensor) As Tensor
            Dim result As Tensor = New Tensor(a.Shape(0), b.Shape(0))

            Call MulBTInto(a, b, result)

            Return result
        End Function

        ''' <summary>
        ''' 沿行方向求和：result[1, n] = Σ_i a[i, n]
        ''' </summary>
        ''' <param name="a">输入矩阵 [m, n]</param>
        ''' <param name="result">输出缓冲区 [1, n]，会被原地覆盖</param>
        Public Sub ColSumInto(a As Tensor, result As Tensor)
            Dim m As Integer = a.Shape(0)
            Dim n As Integer = a.Shape(1)
            Dim ad As Double() = a.Data
            Dim rd As Double() = result.Data

            For i As Integer = 0 To m - 1
                Dim off As Integer = i * n

                For j As Integer = 0 To n - 1
                    rd(j) += ad(off + j)
                Next
            Next
        End Sub

        ''' <summary>
        ''' 将源张量的值累加到目标张量上：dst += src
        ''' </summary>
        ''' <param name="source">源张量</param>
        ''' <param name="target">目标张量（原地累加）</param>
        Public Sub Accumulate(source As Tensor, target As Tensor)
            Dim sd As Double() = source.Data
            Dim td As Double() = target.Data
            Dim n As Integer = std.Min(sd.Length, td.Length)

            For i As Integer = 0 To n - 1
                td(i) += sd(i)
            Next
        End Sub

        ''' <summary>
        ''' 将张量的所有元素清零
        ''' </summary>
        ''' <param name="x">待清零的张量</param>
        Public Sub Zero(x As Tensor)
            Call Array.Clear(x.Data, 0, x.Data.Length)
        End Sub

        ''' <summary>
        ''' 创建指定形状的全零张量
        ''' </summary>
        ''' <param name="rows">行数</param>
        ''' <param name="cols">列数</param>
        ''' <returns>[rows, cols] 的全零张量</returns>
        Public Function Zeros(rows As Integer, cols As Integer) As Tensor
            Return New Tensor(rows, cols)
        End Function
    End Module
End Namespace
