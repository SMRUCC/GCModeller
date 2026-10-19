Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.Transformer

Namespace Model

    ''' <summary>
    ''' 双精度批量 GRU 时序编码器（method.md §4.3 步骤 4）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 序列布局为 <c>[时间步, 基因, 特征]</c>：GRU 的 batch 维恰好是基因数，
    ''' 因此一次前向就完成"逐基因沿时间编码"，无需为每个基因单独建序列。
    ''' </para>
    ''' <para>
    ''' 之所以不复用 <c>GNN.GRULayer</c>：该层的 sigmoid/tanh 用 <c>CSng</c> 计算，
    ''' 整条门控链会降到 float32，而 GRU 的隐藏状态会沿时间步累积误差，
    ''' 与"精度敏感环节保持双精度"的精度边界相冲突。本实现的门控全部走
    ''' <see cref="Tensor.computeKernel"/> 的双精度算子，GEMM 走
    ''' <see cref="TensorOps.BatchedMatMul"/>（内部已接入混合精度外观）。
    ''' </para>
    ''' <para>
    ''' 门控约定与 <c>GRULayer</c> 一致：<c>gate = x·Wᵀ + h·Uᵀ + b</c>，
    ''' <c>h_t = (1 − z) ⊙ h_{t−1} + z ⊙ h̃</c>。
    ''' </para>
    ''' </remarks>
    Public Class BatchGRU

        Private ReadOnly inputSize As Integer
        Private ReadOnly hiddenSize As Integer

        Private ReadOnly Wz As Tensor
        Private ReadOnly Uz As Tensor
        Private ReadOnly bz As Tensor
        Private ReadOnly Wr As Tensor
        Private ReadOnly Ur As Tensor
        Private ReadOnly br As Tensor
        Private ReadOnly Wh As Tensor
        Private ReadOnly Uh As Tensor
        Private ReadOnly bh As Tensor

        Private ReadOnly wzEntry As ParameterEntry
        Private ReadOnly uzEntry As ParameterEntry
        Private ReadOnly bzEntry As ParameterEntry
        Private ReadOnly wrEntry As ParameterEntry
        Private ReadOnly urEntry As ParameterEntry
        Private ReadOnly brEntry As ParameterEntry
        Private ReadOnly whEntry As ParameterEntry
        Private ReadOnly uhEntry As ParameterEntry
        Private ReadOnly bhEntry As ParameterEntry

        ' 权重初始化随机源：种子由调用方（GRNModel 传配置种子）注入，
        ' 保证同一配置下初始化可复现——这是 checkpoint 续训轨迹一致的前提之一
        Private ReadOnly _rand As Random

        ''' <summary>单个时间步的前向中间量。</summary>
        Private Class StepCache
            Public X As Tensor
            Public HPrev As Tensor
            Public Z As Tensor
            Public R As Tensor
            Public ResetHidden As Tensor
            Public HTilde As Tensor
            Public OneMinusZ As Tensor
        End Class

        Private ReadOnly _steps As New List(Of StepCache)()

        ''' <summary>输入特征维度</summary>
        Public ReadOnly Property InputFeatureDim As Integer
            Get
                Return inputSize
            End Get
        End Property

        ''' <summary>隐藏状态维度</summary>
        Public ReadOnly Property HiddenFeatureDim As Integer
            Get
                Return hiddenSize
            End Get
        End Property

        ''' <summary>
        ''' 创建批量 GRU。
        ''' </summary>
        ''' <param name="inputSize">输入特征维度</param>
        ''' <param name="hiddenSize">隐藏状态维度</param>
        ''' <param name="store">参数登记表</param>
        ''' <param name="name">参数名前缀</param>
        ''' <param name="seed">初始化随机种子；Nothing 时退化为时间相关种子（不可复现）</param>
        Public Sub New(inputSize As Integer, hiddenSize As Integer, store As ParameterStore,
                       Optional name As String = "gru", Optional seed As Integer? = Nothing)
            If store Is Nothing Then Throw New ArgumentNullException(NameOf(store))

            Me.inputSize = inputSize
            Me.hiddenSize = hiddenSize
            Me._rand = New Random(If(seed.HasValue, seed.Value, Environment.TickCount))

            Dim stdDev As Double = System.Math.Sqrt(2.0 / (inputSize + hiddenSize))

            Wz = RandomMatrix(hiddenSize, inputSize, stdDev)
            Uz = RandomMatrix(hiddenSize, hiddenSize, stdDev)
            bz = New Tensor(hiddenSize)
            Wr = RandomMatrix(hiddenSize, inputSize, stdDev)
            Ur = RandomMatrix(hiddenSize, hiddenSize, stdDev)
            br = New Tensor(hiddenSize)
            Wh = RandomMatrix(hiddenSize, inputSize, stdDev)
            Uh = RandomMatrix(hiddenSize, hiddenSize, stdDev)
            bh = New Tensor(hiddenSize)

            wzEntry = store.Register($"{name}.Wz", Wz)
            uzEntry = store.Register($"{name}.Uz", Uz)
            bzEntry = store.Register($"{name}.bz", bz)
            wrEntry = store.Register($"{name}.Wr", Wr)
            urEntry = store.Register($"{name}.Ur", Ur)
            brEntry = store.Register($"{name}.br", br)
            whEntry = store.Register($"{name}.Wh", Wh)
            uhEntry = store.Register($"{name}.Uh", Uh)
            bhEntry = store.Register($"{name}.bh", bh)
        End Sub

        ''' <summary>
        ''' 前向传播整个序列。
        ''' </summary>
        ''' <param name="sequence">
        ''' 输入序列 <c>[时间步, 基因, 输入特征]</c>（即 <c>[T, B, d]</c>）
        ''' </param>
        ''' <returns>
        ''' <c>AllHidden</c>：各时间步隐藏状态 <c>[T, B, hidden]</c>；
        ''' <c>FinalHidden</c>：最后一步隐藏状态 <c>[B, hidden]</c>
        ''' </returns>
        Public Function ForwardSequence(sequence As Tensor) As (AllHidden As Tensor, FinalHidden As Tensor)
            If sequence Is Nothing Then Throw New ArgumentNullException(NameOf(sequence))
            If sequence.Rank <> 3 Then
                Throw New ArgumentException("GRU 输入必须是三维张量 [时间步, 基因, 特征]")
            End If

            Dim steps As Integer = sequence.Shape(0)
            Dim batch As Integer = sequence.Shape(1)

            If sequence.Shape(2) <> inputSize Then
                Throw New ArgumentException($"输入特征维度应为 {inputSize}，实际 {sequence.Shape(2)}")
            End If

            _steps.Clear()

            Dim allHidden As Tensor = New Tensor(steps, batch, hiddenSize)
            Dim h As Tensor = New Tensor(batch, hiddenSize)

            For t As Integer = 0 To steps - 1
                Dim x As Tensor = SliceTimeStep(sequence, t, batch, inputSize)

                Dim gateZ As Tensor = Gate(x, h, Wz, Uz, bz)
                Dim z As Tensor = Tensor.computeKernel.Sigmoid(gateZ)

                Dim gateR As Tensor = Gate(x, h, Wr, Ur, br)
                Dim r As Tensor = Tensor.computeKernel.Sigmoid(gateR)

                Dim resetHidden As Tensor = Tensor.computeKernel.Multiply(r, h)
                Dim gateH As Tensor = Gate(x, resetHidden, Wh, Uh, bh)
                Dim hTilde As Tensor = Tensor.computeKernel.Tanh(gateH)

                Dim oneMinusZ As Tensor = Tensor.computeKernel.AddScalar(
                    Tensor.computeKernel.Negate(z), 1.0)

                Dim newHidden As Tensor = Tensor.computeKernel.Add(
                    Tensor.computeKernel.Multiply(oneMinusZ, h),
                    Tensor.computeKernel.Multiply(z, hTilde))

                Call _steps.Add(New StepCache With {
                    .X = x,
                    .HPrev = h,
                    .Z = z,
                    .R = r,
                    .ResetHidden = resetHidden,
                    .HTilde = hTilde,
                    .OneMinusZ = oneMinusZ
                })

                Call CopyInto3D(newHidden, allHidden, t, batch, hiddenSize)
                h = newHidden
            Next

            Return (allHidden, h)
        End Function

        ''' <summary>
        ''' 沿时间反向传播（BPTT），累积全部门控参数的梯度。
        ''' </summary>
        ''' <param name="gradient">各时间步隐藏状态的梯度 <c>[T, B, hidden]</c></param>
        ''' <returns>输入序列的梯度 <c>[T, B, inputSize]</c></returns>
        Public Function BackwardSequence(gradient As Tensor) As Tensor
            If gradient Is Nothing Then Throw New ArgumentNullException(NameOf(gradient))
            If _steps.Count = 0 Then
                Throw New InvalidOperationException("必须先执行前向传播才能反向传播")
            End If

            Dim steps As Integer = gradient.Shape(0)
            Dim batch As Integer = gradient.Shape(1)

            If steps <> _steps.Count Then
                Throw New ArgumentException($"梯度时间步数 {steps} 与前向的 {_steps.Count} 不一致")
            End If

            Dim inputGradient As Tensor = New Tensor(steps, batch, inputSize)
            Dim dhNext As Tensor = New Tensor(batch, hiddenSize)

            For t As Integer = steps - 1 To 0 Step -1
                Dim step_ As StepCache = _steps(t)

                ' ---- 汇总本步输出梯度 ----
                Dim dh As Tensor = Tensor.computeKernel.Add(SliceTimeStep(gradient, t, batch, hiddenSize), dhNext)

                ' ---- h = (1-z)⊙hPrev + z⊙hTilde ----
                Dim dhPrev As Tensor = Tensor.computeKernel.Multiply(dh, step_.OneMinusZ)
                Dim dhTilde As Tensor = Tensor.computeKernel.Multiply(dh, step_.Z)

                ' ∂h/∂z = hTilde − hPrev
                Dim hTildeMinusPrev As Tensor = Tensor.computeKernel.Subtract(step_.HTilde, step_.HPrev)
                Dim dz As Tensor = Tensor.computeKernel.Multiply(dh, hTildeMinusPrev)

                ' ---- z = σ(az) ----
                Dim daz As Tensor = Tensor.computeKernel.Multiply(
                    Tensor.computeKernel.Multiply(dz, step_.Z), step_.OneMinusZ)

                ' ---- h̃ = tanh(ah) ----
                Dim tanhSquare As Tensor = Tensor.computeKernel.Square(step_.HTilde)
                Dim oneMinusTanh2 As Tensor = Tensor.computeKernel.AddScalar(
                    Tensor.computeKernel.Negate(tanhSquare), 1.0)
                Dim dah As Tensor = Tensor.computeKernel.Multiply(dhTilde, oneMinusTanh2)

                ' ---- ah = x·Whᵀ + (r⊙hPrev)·Uhᵀ + bh ----
                Dim dResetHidden As Tensor = TensorOps.BatchedMatMul(dah, Uh)
                Dim dPr As Tensor = Tensor.computeKernel.Multiply(dResetHidden, step_.HPrev)

                ' ---- r = σ(ar) ----
                Dim oneMinusR As Tensor = Tensor.computeKernel.AddScalar(
                    Tensor.computeKernel.Negate(step_.R), 1.0)
                Dim dar As Tensor = Tensor.computeKernel.Multiply(
                    Tensor.computeKernel.Multiply(dPr, step_.R), oneMinusR)

                ' ---- 隐藏状态梯度：三条门控路径 ----
                ' 更新门与重置门直接以 h 为输入；候选状态经由 resetH = r ⊙ h 间接依赖 h，
                ' 因此它对 h 的贡献是 dResetH ⊙ r（而不是 dResetH 本身）
                Call TensorOps.Accumulate(dhPrev, TensorOps.BatchedMatMul(daz, Uz))
                Call TensorOps.Accumulate(dhPrev, TensorOps.BatchedMatMul(dar, Ur))
                Call TensorOps.Accumulate(dhPrev, Tensor.computeKernel.Multiply(dResetHidden, step_.R))

                ' ---- 参数梯度 ----
                Call AccumulateGateGradients(daz, dah, dar, step_)

                ' ---- 输入梯度 ----
                Dim dx As Tensor = TensorOps.BatchedMatMul(daz, Wz)
                Call TensorOps.Accumulate(dx, TensorOps.BatchedMatMul(dar, Wr))
                Call TensorOps.Accumulate(dx, TensorOps.BatchedMatMul(dah, Wh))

                Call CopyInto3D(dx, inputGradient, t, batch, inputSize)

                dhNext = dhPrev
            Next

            Return inputGradient
        End Function

        ''' <summary>
        ''' 累积一步之内三组门控的权重梯度：<c>dW = dgateᵀ · x</c>，<c>dU = dgateᵀ · 输入</c>。
        ''' </summary>
        ''' <param name="daz">更新门预激活梯度</param>
        ''' <param name="dah">候选状态预激活梯度</param>
        ''' <param name="dar">重置门预激活梯度</param>
        ''' <param name="step_">该步的前向缓存</param>
        Private Sub AccumulateGateGradients(daz As Tensor, dah As Tensor, dar As Tensor, step_ As StepCache)
            ' dW = dgateᵀ · x（[hidden,B] × [B,input] = [hidden,input]）
            Call TensorOps.Accumulate(wzEntry.Gradient, TensorOps.BatchedMatMul(xTranspose(daz), step_.X))
            Call TensorOps.Accumulate(uzEntry.Gradient, TensorOps.BatchedMatMul(xTranspose(daz), step_.HPrev))
            Call TensorOps.Accumulate(bzEntry.Gradient, TensorOps.VecAddBackward(daz))

            Call TensorOps.Accumulate(whEntry.Gradient, TensorOps.BatchedMatMul(xTranspose(dah), step_.X))
            Call TensorOps.Accumulate(uhEntry.Gradient, TensorOps.BatchedMatMul(xTranspose(dah), step_.ResetHidden))
            Call TensorOps.Accumulate(bhEntry.Gradient, TensorOps.VecAddBackward(dah))

            Call TensorOps.Accumulate(wrEntry.Gradient, TensorOps.BatchedMatMul(xTranspose(dar), step_.X))
            Call TensorOps.Accumulate(urEntry.Gradient, TensorOps.BatchedMatMul(xTranspose(dar), step_.HPrev))
            Call TensorOps.Accumulate(brEntry.Gradient, TensorOps.VecAddBackward(dar))
        End Sub

        ''' <summary>
        ''' 门控线性变换：<c>x·Wᵀ + h·Uᵀ + b</c>。
        ''' </summary>
        Private Function Gate(x As Tensor, h As Tensor, W As Tensor, U As Tensor, b As Tensor) As Tensor
            Dim wx As Tensor = TensorOps.BatchedMatMul(x, TensorOps.TransposeLastTwo(W))
            Dim uh As Tensor = TensorOps.BatchedMatMul(h, TensorOps.TransposeLastTwo(U))

            Return TensorOps.VecAdd(Tensor.computeKernel.Add(wx, uh), b)
        End Function

        ''' <summary>把二维矩阵转置（用于 <c>dgateᵀ · input</c>）。</summary>
        Private Function xTranspose(m As Tensor) As Tensor
            Return TensorOps.TransposeLastTwo(m)
        End Function

        ''' <summary>从三维序列中取出第 <paramref name="t"/> 个时间步的二维切片。</summary>
        Private Shared Function SliceTimeStep(sequence As Tensor, t As Integer,
                                              batch As Integer, features As Integer) As Tensor
            Dim result As Tensor = New Tensor(batch, features)
            Dim src As Double() = sequence.Data
            Dim dst As Double() = result.Data
            Dim offset As Integer = t * batch * features

            Call System.Array.Copy(src, offset, dst, 0, batch * features)
            Call result.MarkHostModified()

            Return result
        End Function

        ''' <summary>把二维切片写回三维张量的第 <paramref name="t"/> 个时间步。</summary>
        Private Shared Sub CopyInto3D(source As Tensor, target As Tensor, t As Integer,
                                      batch As Integer, features As Integer)
            Dim offset As Integer = t * batch * features

            Call System.Array.Copy(source.Data, 0, target.Data, offset, batch * features)
            Call target.MarkHostModified()
        End Sub

        ''' <summary>按正态分布初始化权重矩阵（使用构造时注入的随机源）。</summary>
        Private Function RandomMatrix(rows As Integer, cols As Integer, stdDev As Double) As Tensor
            Dim t As Tensor = New Tensor(rows, cols)
            Dim data As Double() = t.Data

            ' Box-Muller 变换，与 Tensor.RandomNormal 同分布
            For i As Integer = 0 To data.Length - 1
                Dim u1 As Double = 1.0 - _rand.NextDouble()
                Dim u2 As Double = 1.0 - _rand.NextDouble()

                data(i) = stdDev * System.Math.Sqrt(-2.0 * System.Math.Log(u1)) * System.Math.Sin(2.0 * System.Math.PI * u2)
            Next

            Call t.MarkHostModified()

            Return t
        End Function

    End Class

End Namespace
