' ---------------------------------------------------------------------------
' OutputLayer —— 输出层：把解码器输出投影到词表维度并做 softmax
'
' 迁移要点：前向需要缓存「压平后的输入」（用于计算 Wo 的梯度）与输入原始形状
' （反向时把梯度还原回 [batch, seq, emb]）。softmax 的反向被交叉熵梯度吸收
' （d(logits) = softmax − onehot），因此 Backward 直接接受对 logits 的梯度。
' ---------------------------------------------------------------------------

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow

Namespace Transformer

    ''' <summary>
    ''' Produce a flat array with the same dimension as the number of words in the dictionary
    ''' </summary>
    Public Class OutputLayer

        Public Wo As Tensor

        Private WoOptimizer As Optimizer

        ''' <summary>前向传播的中间量缓存，供反向传播使用。</summary>
        Public Class Cache
            Public FlatInput As Tensor
            Public InputShape As Integer()
            Public Logits As Tensor
        End Class

        Private _lastCache As Cache

        ''' <summary>最近一次 <see cref="Output"/> 的中间量缓存。</summary>
        Public ReadOnly Property LastCache As Cache
            Get
                Return _lastCache
            End Get
        End Property

        Public Sub New(sequenceLength As Integer, embeddingSize As Integer, dictionarySize As Integer)
            Wo = TensorOps.HeNormalInit(New Integer() {embeddingSize * sequenceLength, dictionarySize})

            WoOptimizer = New Optimizer(Wo)
        End Sub

        Public Function Output(input As Tensor) As Tensor
            Dim flatInput = TensorOps.FlattenLastTwo(input)
            Dim filteredOutput = TensorOps.BatchedMatMul(flatInput, Wo)
            Dim softmaxOutput = TensorOps.SoftmaxLastDim(filteredOutput)

            _lastCache = New Cache With {
                .FlatInput = flatInput,
                .InputShape = CType(input.Shape.Clone(), Integer()),
                .Logits = filteredOutput
            }

            Return softmaxOutput
        End Function

        ''' <summary>
        ''' 反向传播：接受对 logits（softmax 之前）的梯度，返回对解码器输出的梯度。
        ''' </summary>
        Public Function Backward(dLogits As Tensor) As Tensor
            Dim cache = _lastCache

            If cache Is Nothing Then Throw New InvalidOperationException("必须先执行前向传播才能反向传播")

            Dim dFlat As Tensor = Nothing, dWo As Tensor = Nothing
            Call TensorOps.BatchedMatMulBackward(dLogits, cache.FlatInput, Wo, dFlat, dWo)
            Call TensorOps.Accumulate(WoOptimizer.Gradient, dWo)

            Return TensorOps.UnflattenLastTwo(dFlat, cache.InputShape)
        End Function

        ''' <summary>清零输出层的梯度累加器。</summary>
        Public Sub ZeroGradients()
            WoOptimizer.ZeroGrad()
        End Sub

        Public Sub MakeTrainingStep(learningRate As Double, [step] As Integer)
            WoOptimizer.MakeTrainingStep(learningRate, [step], Wo)
        End Sub

    End Class
End Namespace
