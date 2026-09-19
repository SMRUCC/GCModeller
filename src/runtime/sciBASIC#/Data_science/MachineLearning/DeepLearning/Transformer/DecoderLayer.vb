' ---------------------------------------------------------------------------
' DecoderLayer —— 解码器单层：掩码自注意力 + 交叉注意力 + 前馈网络
'
' 三条子层均为「残差 + LayerNorm」。迁移要点：
'   * 每个子层的 AddNorm 统计量与前向输出都要缓存；
'   * 反向阶段逆序回传，交叉注意力会把梯度同时回传到 encoderOutput；
'   * 由于解码是按词逐步进行的，本层的前向缓存会被覆盖，
'     因此调用方（DecoderStack / TransformerModel）必须保存每一步的缓存副本。
' ---------------------------------------------------------------------------

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Namespace Transformer

    Public Class DecoderLayer

        Private embeddingSize As Integer

        Private mha As MultiHeadAttention
        Private mha_masked As MultiHeadAttention
        Public ff As FeedForwardNetwork

        Private dropoutMask1, dropoutMask2, dropoutMask3 As Boolean()
        Private dropoutRate As Double = 0

        ''' <summary>前向传播的中间量缓存，供反向传播使用。</summary>
        Public Class Cache
            Public Input As Tensor
            Public MaskedAttention As Tensor
            Public MaskedAttentionDropped As Tensor
            Public Normalized1 As Tensor
            Public Norm1Mean As Double()
            Public Norm1InvStd As Double()
            Public CrossAttention As Tensor
            Public CrossAttentionDropped As Tensor
            Public Normalized2 As Tensor
            Public Norm2Mean As Double()
            Public Norm2InvStd As Double()
            Public FeedForwardOutput As Tensor
            Public FeedForwardDropped As Tensor
            Public Norm3Mean As Double()
            Public Norm3InvStd As Double()
            Public DropoutApplied As Boolean
            ''' <summary>掩码自注意力子层的前向缓存快照</summary>
            Public MaskedCache As MultiHeadAttention.Cache
            ''' <summary>交叉注意力子层的前向缓存快照</summary>
            Public CrossCache As MultiHeadAttention.Cache
            ''' <summary>前馈子层的前向缓存快照</summary>
            Public FfCache As FeedForwardNetwork.Cache
        End Class

        Private _lastCache As Cache

        ''' <summary>最近一次 <see cref="Decode"/> 的中间量缓存。</summary>
        Public ReadOnly Property LastCache As Cache
            Get
                Return _lastCache
            End Get
        End Property

        Public Sub New(embeddingSize As Integer, dk As Integer, dv As Integer, h As Integer, dff As Integer)
            Me.embeddingSize = embeddingSize

            mha = New MultiHeadAttention(dk, dv, h, embeddingSize, False)
            mha_masked = New MultiHeadAttention(dk, dv, h, embeddingSize, True)
            ff = New FeedForwardNetwork(dff, embeddingSize)

            dropoutMask1 = New Boolean(embeddingSize - 1) {}
            dropoutMask2 = New Boolean(embeddingSize - 1) {}
            dropoutMask3 = New Boolean(embeddingSize - 1) {}
        End Sub

        Public Function Decode(encoderOutput As Tensor, decoderInput As Tensor, isTraining As Boolean) As Tensor
            Dim dropoutApplied = isTraining AndAlso dropoutRate > 0

            ' Masked multi headed attention
            Dim maskedAttentionFilteredData = mha_masked.Update(decoderInput)
            Dim maskedAttentionDropped = maskedAttentionFilteredData

            If dropoutApplied Then maskedAttentionDropped = TensorOps.DropoutMask(maskedAttentionDropped, dropoutMask1, dropoutRate)

            Dim mean1 As Double() = Nothing, invStd1 As Double() = Nothing
            Dim normalized1 = TensorOps.AddNormForward(decoderInput, maskedAttentionDropped, mean1, invStd1)

            ' Multi headed attention
            Dim attentionFilteredData = mha.Update(encoderOutput, normalized1)
            Dim attentionDropped = attentionFilteredData

            If dropoutApplied Then attentionDropped = TensorOps.DropoutMask(attentionDropped, dropoutMask2, dropoutRate)

            Dim mean2 As Double() = Nothing, invStd2 As Double() = Nothing
            Dim normalized2 = TensorOps.AddNormForward(normalized1, attentionDropped, mean2, invStd2)

            ' Feed forward neural network
            Dim feedForwardOutput = ff.FeedForward(normalized2)
            Dim feedForwardDropped = feedForwardOutput

            If dropoutApplied Then feedForwardDropped = TensorOps.DropoutMask(feedForwardDropped, dropoutMask3, dropoutRate)

            Dim mean3 As Double() = Nothing, invStd3 As Double() = Nothing
            Dim output = TensorOps.AddNormForward(normalized2, feedForwardDropped, mean3, invStd3)

            _lastCache = New Cache With {
                .Input = decoderInput,
                .MaskedAttention = maskedAttentionFilteredData,
                .MaskedAttentionDropped = maskedAttentionDropped,
                .Normalized1 = normalized1,
                .Norm1Mean = mean1,
                .Norm1InvStd = invStd1,
                .CrossAttention = attentionFilteredData,
                .CrossAttentionDropped = attentionDropped,
                .Normalized2 = normalized2,
                .Norm2Mean = mean2,
                .Norm2InvStd = invStd2,
                .FeedForwardOutput = feedForwardOutput,
                .FeedForwardDropped = feedForwardDropped,
                .Norm3Mean = mean3,
                .Norm3InvStd = invStd3,
                .DropoutApplied = dropoutApplied,
                .MaskedCache = mha_masked.LastCache,
                .CrossCache = mha.LastCache,
                .FfCache = ff.LastCache
            }

            Return output
        End Function

        ''' <summary>
        ''' 反向传播：返回对 <c>decoderInput</c> 的梯度，
        ''' 并通过 <paramref name="dEncoderOutput"/> 累加对 encoder 输出的梯度。
        ''' </summary>
        ''' <param name="forwardCache">与该解码步相对应的前向缓存快照</param>
        ''' <param name="dOut">对该步解码输出的梯度</param>
        ''' <param name="dEncoderOutput">对 encoder 输出的梯度累加器</param>
        Public Function Backward(forwardCache As Cache, dOut As Tensor, ByRef dEncoderOutput As Tensor) As Tensor
            Dim cache = forwardCache

            If cache Is Nothing Then Throw New InvalidOperationException("必须先执行前向传播才能反向传播")

            ' 第三个 AddNorm：output = AddNorm(normalized2, feedForwardDropped)
            Dim dx3 = TensorOps.AddNormBackward(dOut, cache.Normalized2, cache.FeedForwardDropped, cache.Norm3Mean, cache.Norm3InvStd)
            Dim dFeedForwardDropped = TensorOps.CloneTensor(dx3)
            Dim dNormalized2 = dx3

            Dim dFeedForward = dFeedForwardDropped

            If cache.DropoutApplied Then dFeedForward = TensorOps.DropoutMaskBackward(dFeedForward, dropoutMask3, dropoutRate)

            Call TensorOps.Accumulate(dNormalized2, ff.Backward(cache.FfCache, dFeedForward))

            ' 第二个 AddNorm：normalized2 = AddNorm(normalized1, attentionDropped)
            Dim dx2 = TensorOps.AddNormBackward(dNormalized2, cache.Normalized1, cache.CrossAttentionDropped, cache.Norm2Mean, cache.Norm2InvStd)
            Dim dAttentionDropped = TensorOps.CloneTensor(dx2)
            Dim dNormalized1 = dx2

            Dim dAttention = dAttentionDropped

            If cache.DropoutApplied Then dAttention = TensorOps.DropoutMaskBackward(dAttention, dropoutMask2, dropoutRate)

            Dim dFromEncoder As Tensor = Nothing

            Call TensorOps.Accumulate(dNormalized1, mha.Backward(cache.CrossCache, dAttention, dFromEncoder))

            If dFromEncoder IsNot Nothing Then
                If dEncoderOutput Is Nothing Then
                    dEncoderOutput = dFromEncoder
                Else
                    Call TensorOps.Accumulate(dEncoderOutput, dFromEncoder)
                End If
            End If

            ' 第一个 AddNorm：normalized1 = AddNorm(decoderInput, maskedAttentionDropped)
            Dim dx1 = TensorOps.AddNormBackward(dNormalized1, cache.Input, cache.MaskedAttentionDropped, cache.Norm1Mean, cache.Norm1InvStd)
            Dim dMaskedAttentionDropped = TensorOps.CloneTensor(dx1)
            Dim dDecoderInput = dx1

            Dim dMaskedAttention = dMaskedAttentionDropped

            If cache.DropoutApplied Then dMaskedAttention = TensorOps.DropoutMaskBackward(dMaskedAttention, dropoutMask1, dropoutRate)

            Dim unused As Tensor = Nothing

            Call TensorOps.Accumulate(dDecoderInput, mha_masked.Backward(cache.MaskedCache, dMaskedAttention, unused))

            Return dDecoderInput
        End Function

        Public Sub SetDropoutNodes(dropoutRate As Double)
            If dropoutRate < 0 OrElse dropoutRate >= 1 Then Throw New ArgumentException("Error: dropout rate must be >= 0 and < 1")

            Me.dropoutRate = dropoutRate

            For i = 0 To embeddingSize - 1
                dropoutMask1(i) = False
                If randf.NextDouble < dropoutRate Then dropoutMask1(i) = True

                dropoutMask2(i) = False
                If randf.NextDouble < dropoutRate Then dropoutMask2(i) = True

                dropoutMask3(i) = False
                If randf.NextDouble < dropoutRate Then dropoutMask3(i) = True
            Next
        End Sub

        ''' <summary>清零本层所有参数的梯度累加器。</summary>
        Public Sub ZeroGradients()
            mha_masked.ZeroGradients()
            mha.ZeroGradients()
            ff.ZeroGradients()
        End Sub

        Public Sub MakeTrainingStep(learningRate As Double, [step] As Integer)
            mha_masked.MakeTrainingStep(learningRate, [step])
            mha.MakeTrainingStep(learningRate, [step])
            ff.MakeTrainingStep(learningRate, [step])
        End Sub

    End Class
End Namespace
