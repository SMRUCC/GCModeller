' ---------------------------------------------------------------------------
' FeedForwardNetwork —— 位置前馈网络（两层全连接 + ReLU）
'
' 迁移到 TensorFlow\Tensor.vb 后不再有自动微分，因此前向阶段需要缓存
' 预激活值（ReLU 反向所需的掩码来源）与激活值（计算 W2 梯度所需），
' 反向阶段手工累加 W1/W2/b1/b2 的梯度。
' ---------------------------------------------------------------------------

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow

Namespace Transformer

    Public Class FeedForwardNetwork

        Private W1, W2 As Tensor
        Private b1, b2 As Tensor

        Private W1Optimizer, W2Optimizer, b1Optimizer, b2Optimizer As Optimizer

        ''' <summary>前向传播的中间量缓存，供反向传播使用。</summary>
        Public Class Cache
            ''' <summary>本层输入 G</summary>
            Public Input As Tensor
            ''' <summary>第一层的预激活值（ReLU 之前）</summary>
            Public PreActivation As Tensor
            ''' <summary>第一层的激活值（ReLU 之后）</summary>
            Public Activation As Tensor
        End Class

        Private _lastCache As Cache

        ''' <summary>最近一次 <see cref="FeedForward"/> 的中间量缓存。</summary>
        Public ReadOnly Property LastCache As Cache
            Get
                Return _lastCache
            End Get
        End Property

        Public Sub New(dff As Integer, embeddingSize As Integer)
            W1 = TensorOps.HeNormalInit(New Integer() {embeddingSize, dff})
            W2 = TensorOps.HeNormalInit(New Integer() {dff, embeddingSize})
            b1 = New Tensor(dff)
            b2 = New Tensor(embeddingSize)

            W1Optimizer = New Optimizer(W1)
            W2Optimizer = New Optimizer(W2)
            b1Optimizer = New Optimizer(b1)
            b2Optimizer = New Optimizer(b2)
        End Sub

        Public Function FeedForward(G As Tensor) As Tensor
            ' First layer
            Dim preActivation = TensorOps.VecAdd(TensorOps.BatchedMatMul(G, W1), b1)
            Dim activation = Tensor.computeKernel.Relu(preActivation)

            ' Second layer
            Dim FFN2 = TensorOps.VecAdd(TensorOps.BatchedMatMul(activation, W2), b2)

            _lastCache = New Cache With {
                .Input = G,
                .PreActivation = preActivation,
                .Activation = activation
            }

            Return FFN2
        End Function

        ''' <summary>
        ''' 反向传播：返回对本层输入 G 的梯度，并累加 W1/W2/b1/b2 的梯度。
        ''' </summary>
        ''' <param name="forwardCache">
        ''' 与本次回传相对应的前向缓存。解码器按词逐步前向时该层的
        ''' <see cref="LastCache"/> 会被后续步骤覆盖，因此必须显式传入当步的快照。
        ''' </param>
        ''' <param name="dOut">对 <see cref="FeedForward"/> 输出的梯度</param>
        Public Function Backward(forwardCache As Cache, dOut As Tensor) As Tensor
            Dim cache = forwardCache

            If cache Is Nothing Then Throw New InvalidOperationException("必须先执行前向传播才能反向传播")

            ' 偏置 b2
            Call TensorOps.Accumulate(b2Optimizer.Gradient, TensorOps.VecAddBackward(dOut))

            ' 第二层权重：FFN2 = activation · W2
            Dim dActivation As Tensor = Nothing, dW2 As Tensor = Nothing
            Call TensorOps.BatchedMatMulBackward(dOut, cache.Activation, W2, dActivation, dW2)
            Call TensorOps.Accumulate(W2Optimizer.Gradient, dW2)

            ' ReLU 反向：用预激活值的阶跃掩码门控上游梯度
            Dim dPreActivation = TensorOps.ElementwiseMultiply(dActivation, TensorOps.Heaviside(cache.PreActivation))

            ' 偏置 b1
            Call TensorOps.Accumulate(b1Optimizer.Gradient, TensorOps.VecAddBackward(dPreActivation))

            ' 第一层权重：preActivation = G · W1
            Dim dInput As Tensor = Nothing, dW1 As Tensor = Nothing
            Call TensorOps.BatchedMatMulBackward(dPreActivation, cache.Input, W1, dInput, dW1)
            Call TensorOps.Accumulate(W1Optimizer.Gradient, dW1)

            Return dInput
        End Function

        ''' <summary>清零本层所有参数的梯度累加器。</summary>
        Public Sub ZeroGradients()
            W1Optimizer.ZeroGrad()
            W2Optimizer.ZeroGrad()
            b1Optimizer.ZeroGrad()
            b2Optimizer.ZeroGrad()
        End Sub

        Public Sub MakeTrainingStep(learningRate As Double, [step] As Integer)
            W1Optimizer.MakeTrainingStep(learningRate, [step], W1)
            W2Optimizer.MakeTrainingStep(learningRate, [step], W2)
            b1Optimizer.MakeTrainingStep(learningRate, [step], b1)
            b2Optimizer.MakeTrainingStep(learningRate, [step], b2)
        End Sub

    End Class
End Namespace
