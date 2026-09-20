Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports deepOps = Microsoft.VisualBasic.MachineLearning.Transformer.TensorOps
Imports std = System.Math

Namespace NN

    ''' <summary>
    ''' 张量形状适配工具集。
    '''
    ''' ### 为什么需要这个模块
    ''' 底层 <see cref="Tensor"/> 的 <c>Operator +</c>(Tensor, Tensor) **不提供 NumPy 式广播**：
    ''' 它只特判了「行向量 <c>[1,N]</c> ⊕ 列向量 <c>[M,1]</c> → 外和 <c>[M,N]</c>」这一种情形，
    ''' 其余形状不一致的加法会直接抛 <c>ArgumentException</c>。而条件批归一化 / 残差块 / 扩散加噪
    ''' 都需要把 <c>[1,H]</c>（或 <c>[B,1]</c>）的向量广播到 <c>[B,H]</c>。
    '''
    ''' ### 实现策略
    ''' 统一用「全 1 列向量/行向量做矩阵乘」来表达广播：
    ''' <code>
    ''' [B,H] ← ones(B,1) · v(1,H)       ' 行广播（偏置、缩放因子）
    ''' [B,H] ← v(B,1)   · ones(1,H)     ' 列广播（逐样本标量）
    ''' </code>
    ''' 这两条都是 <see cref="Tensor.computeKernel"/> 的 <c>MatMul</c> 算子，因此广播本身也走
    ''' SIMD / CUDA 路径，不需要手写循环而牺牲后端加速。
    ''' </summary>
    Public Module TensorUtil

        Private ReadOnly _onesColumn As New Dictionary(Of Integer, Tensor)
        Private ReadOnly _onesRow As New Dictionary(Of Integer, Tensor)
        Private ReadOnly _cacheLock As New Object()

        ''' <summary><c>[n,1]</c> 全 1 列向量（按 n 缓存复用，<see cref="Tensor"/> 只读使用）。</summary>
        Public Function OnesColumn(n As Integer) As Tensor
            SyncLock _cacheLock
                Dim t As Tensor = Nothing
                If Not _onesColumn.TryGetValue(n, t) Then
                    t = Tensor.Ones(New Integer() {n, 1})
                    _onesColumn(n) = t
                End If
                Return t
            End SyncLock
        End Function

        ''' <summary><c>[1,n]</c> 全 1 行向量（按 n 缓存复用）。</summary>
        Public Function OnesRow(n As Integer) As Tensor
            SyncLock _cacheLock
                Dim t As Tensor = Nothing
                If Not _onesRow.TryGetValue(n, t) Then
                    t = Tensor.Ones(New Integer() {1, n})
                    _onesRow(n) = t
                End If
                Return t
            End SyncLock
        End Function

        ''' <summary>把 <c>[1,H]</c> 的行向量广播成 <c>[B,H]</c>。</summary>
        Public Function BroadcastRow(v As Tensor, batchSize As Integer) As Tensor
            If v.Rank <> 2 OrElse v.Shape(0) <> 1 Then
                Throw New ArgumentException($"BroadcastRow 需要形状为 [1,H] 的张量，实际为 [{String.Join(", ", v.Shape)}]")
            End If

            Return OnesColumn(batchSize).MatMul(v)
        End Function

        ''' <summary>把 <c>[B,1]</c> 的列向量广播成 <c>[B,cols]</c>。</summary>
        Public Function BroadcastColumn(v As Tensor, columns As Integer) As Tensor
            If v.Rank <> 2 OrElse v.Shape(1) <> 1 Then
                Throw New ArgumentException($"BroadcastColumn 需要形状为 [B,1] 的张量，实际为 [{String.Join(", ", v.Shape)}]")
            End If

            Return v.MatMul(OnesRow(columns))
        End Function

        ''' <summary><c>[B,H]</c> 逐样本标量缩放：<c>t ⊙ broadcast(column)</c>，其中 column 为 <c>[B,1]</c>。</summary>
        Public Function RowScale(t As Tensor, column As Tensor) As Tensor
            Return t.ElementwiseMultiply(BroadcastColumn(column, t.Shape(1)))
        End Function

        ''' <summary><c>[B,H] + [1,H]</c>（行向量广播加）。</summary>
        Public Function BroadcastAdd(t As Tensor, row As Tensor) As Tensor
            Return t + BroadcastRow(row, t.Shape(0))
        End Function

        ''' <summary><c>[B,H] - [1,H]</c>（行向量广播减）。</summary>
        Public Function BroadcastSubtract(t As Tensor, row As Tensor) As Tensor
            Return t - BroadcastRow(row, t.Shape(0))
        End Function

        ''' <summary>把 <see cref="Double"/> 数组打包为 <c>[n,1]</c> 列向量。</summary>
        Public Function ColumnVector(values As Double()) As Tensor
            Dim t = New Tensor(New Integer() {values.Length, 1})
            Array.Copy(values, t.Data, values.Length)
            Call t.MarkHostModified()
            Return t
        End Function

        ''' <summary>把 <see cref="Integer"/> 数组（如时间步索引）打包为 <c>[n,1]</c> 列向量。</summary>
        Public Function ColumnVector(values As Integer()) As Tensor
            Dim d(values.Length - 1) As Double
            For i As Integer = 0 To values.Length - 1
                d(i) = values(i)
            Next
            Return ColumnVector(d)
        End Function

        ''' <summary>把单个 <see cref="Double"/> 重复 <paramref name="count"/> 次得到 <c>[count,1]</c> 列向量。</summary>
        Public Function FullColumn(value As Double, count As Integer) As Tensor
            Dim d(count - 1) As Double
            For i As Integer = 0 To count - 1
                d(i) = value
            Next
            Return ColumnVector(d)
        End Function

        ''' <summary>构造 <c>[1,n]</c> 的常量行向量。</summary>
        Public Function RowConstant(value As Double, columns As Integer) As Tensor
            Dim t = New Tensor(New Integer() {1, columns})
            Dim data = t.Data
            For i As Integer = 0 To data.Length - 1
                data(i) = value
            Next
            Call t.MarkHostModified()
            Return t
        End Function

        ''' <summary>取最后一维的切片（<c>Slice(axis:=rank-1, start, length)</c>）。</summary>
        Public Function SliceLastDim(t As Tensor, start As Integer, length As Integer) As Tensor
            Return Tensor.computeKernel.Slice(t, t.Rank - 1, start, length)
        End Function

        ''' <summary>沿最后一维拼接（委托给 DeepLearning 的 <c>Transformer.TensorOps</c>）。</summary>
        Public Function ConcatLast(tensors As Tensor()) As Tensor
            Return deepOps.ConcatLastDim(tensors)
        End Function

        ''' <summary>张量整体均值（Double 精度）。</summary>
        Public Function MeanAll(t As Tensor) As Double
            Return Tensor.computeKernel.MeanAll(t)
        End Function

        ''' <summary>按标量缩放（走 <c>computeKernel.MultiplyScalar</c>）。</summary>
        Public Function Scale(t As Tensor, scalar As Double) As Tensor
            Return Tensor.computeKernel.MultiplyScalar(t, scalar)
        End Function

        ''' <summary>梯度原地累加（形状必须一致）。</summary>
        Public Sub Accumulate(target As Tensor, grad As Tensor)
            deepOps.Accumulate(target, grad)
        End Sub

        ''' <summary>深拷贝张量（用于把同一份梯度分发给两条残差分支）。</summary>
        Public Function CloneT(t As Tensor) As Tensor
            Return deepOps.CloneTensor(t)
        End Function

        ''' <summary>原地清零。</summary>
        Public Sub ZeroT(t As Tensor)
            deepOps.ZeroInPlace(t)
        End Sub

        ''' <summary>He 初始化（fan-in = 倒数第二维，走 DeepLearning 的 <c>TensorOps.HeNormalInit</c>）。</summary>
        Public Function HeNormal(shape As Integer()) As Tensor
            Return deepOps.HeNormalInit(shape)
        End Function

        ''' <summary>
        ''' 用调用方提供的 <see cref="Random"/> 生成标准正态张量（保证与训练种子一致、可复现）。
        ''' </summary>
        Public Function StandardNormal(shape As Integer(), rng As Random) As Tensor
            Dim t = New Tensor(shape)
            Dim data = t.Data
            For i As Integer = 0 To data.Length - 1
                data(i) = NextGaussian(rng)
            Next
            Call t.MarkHostModified()
            Return t
        End Function

        ''' <summary>Box-Muller 标准正态采样。</summary>
        Public Function NextGaussian(rng As Random) As Double
            Dim u1 As Double = 1.0 - rng.NextDouble()
            Dim u2 As Double = 1.0 - rng.NextDouble()
            Return std.Sqrt(-2.0 * std.Log(u1)) * std.Sin(2.0 * std.PI * u2)
        End Function
    End Module
End Namespace
