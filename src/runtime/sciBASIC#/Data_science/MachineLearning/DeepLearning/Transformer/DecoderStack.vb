' ---------------------------------------------------------------------------
' DecoderStack —— 解码器堆叠（Nx 层）
'
' 解码按词逐步推进，每一步都会覆盖各层的前向缓存，因此 <see cref="Decode"/> 会把
' 当步每一层的缓存快照保存到 <see cref="LastCaches"/>，供随后的反向传播使用。
' ---------------------------------------------------------------------------

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow

Namespace Transformer

    Public Class DecoderStack

        Private Nx As Integer
        Private decoderLayers As List(Of DecoderLayer) = New List(Of DecoderLayer)()

        Private _lastCaches As List(Of DecoderLayer.Cache)

        ''' <summary>最近一次 <see cref="Decode"/> 每一步各层的前向缓存快照。</summary>
        Public ReadOnly Property LastCaches As List(Of DecoderLayer.Cache)
            Get
                Return _lastCaches
            End Get
        End Property

        Public Sub New(Nx As Integer, embeddingSize As Integer, dk As Integer, dv As Integer, h As Integer, dff As Integer)
            Me.Nx = Nx

            For i = 0 To Nx - 1
                decoderLayers.Add(New DecoderLayer(embeddingSize, dk, dv, h, dff))
            Next
        End Sub

        Public Function Decode(encoderOutput As Tensor, word_embeddings As Tensor, isTraining As Boolean) As Tensor
            _lastCaches = New List(Of DecoderLayer.Cache)()

            Dim decoderOutput = decoderLayers(0).Decode(encoderOutput, word_embeddings, isTraining)
            _lastCaches.Add(decoderLayers(0).LastCache)

            For i = 1 To Nx - 1
                decoderOutput = decoderLayers(i).Decode(encoderOutput, decoderOutput, isTraining)
                _lastCaches.Add(decoderLayers(i).LastCache)
            Next

            Return decoderOutput
        End Function

        ''' <summary>
        ''' 反向传播：返回对解码器输入（目标语言词嵌入）的梯度，
        ''' 并通过 <paramref name="dEncoderOutput"/> 累加对 encoder 输出的梯度。
        ''' </summary>
        ''' <param name="caches">与该解码步对应的前向缓存快照</param>
        ''' <param name="dOut">对该步解码输出的梯度</param>
        ''' <param name="dEncoderOutput">对 encoder 输出的梯度累加器（跨解码步共享）</param>
        Public Function Backward(caches As List(Of DecoderLayer.Cache), dOut As Tensor, ByRef dEncoderOutput As Tensor) As Tensor
            Dim d = dOut

            For i = caches.Count - 1 To 0 Step -1
                d = decoderLayers(i).Backward(caches(i), d, dEncoderOutput)
            Next

            Return d
        End Function

        Public Sub SetDropoutNodes(dropout As Double)
            For i = 0 To Nx - 1
                decoderLayers(i).SetDropoutNodes(dropout)
            Next
        End Sub

        Public Sub ZeroGradients()
            For i = 0 To Nx - 1
                decoderLayers(i).ZeroGradients()
            Next
        End Sub

        Public Sub MakeTrainingStep(learningRate As Double, [step] As Integer)
            For i = 0 To Nx - 1
                decoderLayers(i).MakeTrainingStep(learningRate, [step])
            Next
        End Sub

    End Class
End Namespace
