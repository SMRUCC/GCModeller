' ---------------------------------------------------------------------------
' EncoderLayer —— 编码器单层：多头自注意力 + 前馈网络（均带残差 + LayerNorm）
'
' 迁移要点：前向阶段缓存两个 AddNorm 的统计量（均值 / 逆标准差）与两条子层
' 的输出；反向阶段按「第二个 AddNorm → 前馈 → 第一个 AddNorm → 自注意力」
' 的逆序回传。AddNorm 对 A/B 两个分支的梯度相同，因此需要各自持有独立副本。
' ---------------------------------------------------------------------------

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Namespace Transformer

    Public Class EncoderLayer

        Private embeddingSize As Integer

        Private mha As MultiHeadAttention
        Private ff As FeedForwardNetwork

        Private dropoutMask1, dropoutMask2 As Boolean()
        Private dropoutRate As Double = 0

        ''' <summary>前向传播的中间量缓存，供反向传播使用。</summary>
        Public Class Cache
            Public Input As Tensor
            Public AttentionOutput As Tensor
            Public AttentionDropped As Tensor
            Public Normalized1 As Tensor
            Public Norm1Mean As Double()
            Public Norm1InvStd As Double()
            Public FeedForwardOutput As Tensor
            Public FeedForwardDropped As Tensor
            Public Norm2Mean As Double()
            Public Norm2InvStd As Double()
            Public DropoutApplied As Boolean
            ''' <summary>自注意力子层的前向缓存快照</summary>
            Public MhaCache As MultiHeadAttention.Cache
            ''' <summary>前馈子层的前向缓存快照</summary>
            Public FfCache As FeedForwardNetwork.Cache
        End Class

        Private _lastCache As Cache

        ''' <summary>最近一次 <see cref="Encode"/> 的中间量缓存。</summary>
        Public ReadOnly Property LastCache As Cache
            Get
                Return _lastCache
            End Get
        End Property

        Public Sub New(embeddingSize As Integer, dk As Integer, dv As Integer, h As Integer, dff As Integer)
            Me.embeddingSize = embeddingSize

            mha = New MultiHeadAttention(dk, dv, h, embeddingSize, False)
            ff = New FeedForwardNetwork(dff, embeddingSize)

            dropoutMask1 = New Boolean(embeddingSize - 1) {}
            dropoutMask2 = New Boolean(embeddingSize - 1) {}
        End Sub

        Public Function Encode(encoderInput As Tensor, isTraining As Boolean) As Tensor
            Dim dropoutApplied = isTraining AndAlso dropoutRate > 0

            ' Multi headed attention
            Dim attentionFilteredData = mha.Update(encoderInput)
            Dim attentionDropped = attentionFilteredData

            If dropoutApplied Then attentionDropped = TensorOps.DropoutMask(attentionDropped, dropoutMask1, dropoutRate)

            Dim mean1 As Double() = Nothing, invStd1 As Double() = Nothing
            Dim normalized1 = TensorOps.AddNormForward(encoderInput, attentionDropped, mean1, invStd1)

            ' Feed forward neural network
            Dim feedForwardOutput = ff.FeedForward(normalized1)
            Dim feedForwardDropped = feedForwardOutput

            If dropoutApplied Then feedForwardDropped = TensorOps.DropoutMask(feedForwardDropped, dropoutMask2, dropoutRate)

            Dim mean2 As Double() = Nothing, invStd2 As Double() = Nothing
            Dim output = TensorOps.AddNormForward(normalized1, feedForwardDropped, mean2, invStd2)

            _lastCache = New Cache With {
                .Input = encoderInput,
                .AttentionOutput = attentionFilteredData,
                .AttentionDropped = attentionDropped,
                .Normalized1 = normalized1,
                .Norm1Mean = mean1,
                .Norm1InvStd = invStd1,
                .FeedForwardOutput = feedForwardOutput,
                .FeedForwardDropped = feedForwardDropped,
                .Norm2Mean = mean2,
                .Norm2InvStd = invStd2,
                .DropoutApplied = dropoutApplied,
                .MhaCache = mha.LastCache,
                .FfCache = ff.LastCache
            }

            Return output
        End Function

        ''' <summary>反向传播：返回对本层输入 <c>encoderInput</c> 的梯度。</summary>
        ''' <param name="dOut">对本层输出的梯度</param>
        ''' <param name="forwardCache">与本次回传相对应的前向缓存快照</param>
        Public Function Backward(dOut As Tensor, forwardCache As Cache) As Tensor
            Dim cache = forwardCache

            If cache Is Nothing Then Throw New InvalidOperationException("必须先执行前向传播才能反向传播")

            ' 第二个 AddNorm：output = AddNorm(normalized1, feedForwardDropped)
            Dim dx2 = TensorOps.AddNormBackward(dOut, cache.Normalized1, cache.FeedForwardDropped, cache.Norm2Mean, cache.Norm2InvStd)
            Dim dFeedForwardDropped = TensorOps.CloneTensor(dx2)
            Dim dNormalized1 = dx2

            Dim dFeedForward = dFeedForwardDropped

            If cache.DropoutApplied Then dFeedForward = TensorOps.DropoutMaskBackward(dFeedForward, dropoutMask2, dropoutRate)

            Call TensorOps.Accumulate(dNormalized1, ff.Backward(cache.FfCache, dFeedForward))

            ' 第一个 AddNorm：normalized1 = AddNorm(encoderInput, attentionDropped)
            Dim dx1 = TensorOps.AddNormBackward(dNormalized1, cache.Input, cache.AttentionDropped, cache.Norm1Mean, cache.Norm1InvStd)
            Dim dAttentionDropped = TensorOps.CloneTensor(dx1)
            Dim dInput = dx1

            Dim dAttention = dAttentionDropped

            If cache.DropoutApplied Then dAttention = TensorOps.DropoutMaskBackward(dAttention, dropoutMask1, dropoutRate)

            Dim unused As Tensor = Nothing

            Call TensorOps.Accumulate(dInput, mha.Backward(cache.MhaCache, dAttention, unused))

            Return dInput
        End Function

        Public Sub SetDropoutNodes(dropoutRate As Double)
            If dropoutRate < 0 OrElse dropoutRate >= 1 Then Throw New ArgumentException("Error: dropout rate must be >= 0 and < 1")

            Me.dropoutRate = dropoutRate

            For i = 0 To embeddingSize - 1
                dropoutMask1(i) = False
                If randf.NextDouble < dropoutRate Then dropoutMask1(i) = True

                dropoutMask2(i) = False
                If randf.NextDouble < dropoutRate Then dropoutMask2(i) = True
            Next
        End Sub

        ''' <summary>清零本层所有参数的梯度累加器。</summary>
        Public Sub ZeroGradients()
            mha.ZeroGradients()
            ff.ZeroGradients()
        End Sub

        Public Sub MakeTrainingStep(learningRate As Double, [step] As Integer)
            mha.MakeTrainingStep(learningRate, [step])
            ff.MakeTrainingStep(learningRate, [step])
        End Sub

    End Class
End Namespace
