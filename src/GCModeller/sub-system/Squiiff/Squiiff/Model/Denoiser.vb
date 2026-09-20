Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.Squiiff.Diffusion
Imports SMRUCC.genomics.Analysis.Squiiff.NN

Namespace Model

    ''' <summary>
    ''' 条件去噪网络 <c>ε_θ(x_t, t, z_sem)</c>。
    '''
    ''' 结构（对应 README 第三节"带残差连接与条件批归一化的 MLP"）：
    ''' <code>
    ''' tEmb = TimeEmbedding(t)                        ' 正弦位置编码 + 2 层 MLP
    ''' cond = concat(z_sem, tEmb)                    ' 条件向量
    ''' h    = Act(Linear_in(concat(x_t, tEmb)))
    ''' h    = ResidualBlock₁(h, cond)                ' CondBN 以 cond 为条件
    ''' ...
    ''' h    = ResidualBlockₙ(h, cond)
    ''' ε̂    = Linear_out(h)                          ' 回到基因维度
    ''' </code>
    ''' 语义条件 <c>z_sem</c> 通过**条件批归一化**作为显式输入注入
    ''' （与 classifier-free guidance 的"采样时混合条件与无条件得分"不同）。
    ''' </summary>
    Public Class Denoiser
        Implements IParameterized, INoisePredictor

        Private ReadOnly _config As SquiiffConfig
        Private ReadOnly _geneCount As Integer
        Private ReadOnly _semanticDim As Integer
        Private ReadOnly _timeDim As Integer
        Private ReadOnly _timeEmbed As TimeEmbedding
        Private ReadOnly _inProj As Linear
        Private ReadOnly _outProj As Linear
        Private ReadOnly _blocks As ResidualBlock()
        Private ReadOnly _params As Parameter()
        Private ReadOnly _activation As ActivationKind

        ' 前向缓存
        Private _zSem As Tensor
        Private _condition As Tensor
        Private _preActivation As Tensor

        Public Sub New(config As SquiiffConfig, geneCount As Integer, semanticDim As Integer)
            If config Is Nothing Then Throw New ArgumentNullException(NameOf(config))

            Me._config = config
            Me._geneCount = geneCount
            Me._semanticDim = semanticDim
            Me._timeDim = config.TimeEmbeddingDim
            Me._activation = config.Activation

            ' 时间嵌入表需覆盖 [0, T]，多留少量余量
            Me._timeEmbed = New TimeEmbedding("denoiser.time", Me._timeDim, config.DiffusionSteps + 8, config.Activation)
            Me._inProj = New Linear("denoiser.in", geneCount + Me._timeDim, config.DenoiserHiddenDim)

            ReDim Me._blocks(config.DenoiserBlocks - 1)
            For i As Integer = 0 To Me._blocks.Length - 1
                Me._blocks(i) = New ResidualBlock($"denoiser.block{i}", config.DenoiserHiddenDim, Me.ConditionDim, config.Activation)
            Next

            Me._outProj = New Linear("denoiser.out", config.DenoiserHiddenDim, geneCount)

            Dim items As New List(Of IParameterized) From {Me._timeEmbed, Me._inProj}
            items.AddRange(Me._blocks)
            items.Add(Me._outProj)

            Me._params = ParameterGroups.FlattenMany(items)

            Call ApplyInferenceNormalizationMode()
        End Sub

        ''' <summary>条件向量维度 = 语义维度 + 时间嵌入维度。</summary>
        Public ReadOnly Property ConditionDim As Integer
            Get
                Return _semanticDim + _timeDim
            End Get
        End Property

        ''' <summary>去噪网络的残差块个数。</summary>
        Public ReadOnly Property BlockCount As Integer
            Get
                Return _blocks.Length
            End Get
        End Property

        Public ReadOnly Property Parameters As IEnumerable(Of Parameter) Implements IParameterized.Parameters
            Get
                Return _params
            End Get
        End Property

        ''' <summary>全部残差块内的条件批归一化层（存档滑动统计量用，层名全局唯一）。</summary>
        Public ReadOnly Property Normalizations As IReadOnlyList(Of IRunningStatistics)
            Get
                Dim bag As New List(Of IRunningStatistics)
                For Each block In _blocks
                    For Each norm In block.Normalizations
                        bag.Add(norm)
                    Next
                Next
                Return bag
            End Get
        End Property

        ''' <summary>把 <see cref="SquiiffConfig.UseBatchStatsAtInference"/> 同步到所有条件批归一化层。</summary>
        Public Sub ApplyInferenceNormalizationMode()
            For Each block In _blocks
                Call block.SetUseBatchStatsAtInference(_config.UseBatchStatsAtInference)
            Next
        End Sub

        Public Function Predict(xt As Tensor, t As Integer(), zSem As Tensor, training As Boolean) As Tensor Implements INoisePredictor.Predict
            Me._zSem = zSem
            Dim timeEmb = Me._timeEmbed.Forward(t, training)                          ' [B,timeDim]
            Me._condition = TensorUtil.ConcatLast({zSem, timeEmb})                    ' [B,condDim]

            Dim hin = TensorUtil.ConcatLast({xt, timeEmb})                            ' [B, G+timeDim]
            Dim h = Me._inProj.Forward(hin, training)

            Me._preActivation = h
            h = Activations.Forward(_activation, h)

            For Each block In _blocks
                h = block.Forward(h, Me._condition, training)
            Next

            Return Me._outProj.Forward(h, training)
        End Function

        Public Function Backward(dEps As Tensor, ByRef dZsem As Tensor) As Tensor Implements INoisePredictor.Backward
            Dim d = Me._outProj.Backward(dEps)

            ' 条件向量被**所有**残差块共享，梯度需要跨块累加
            Dim dCondition As Tensor = Nothing
            For i As Integer = Me._blocks.Length - 1 To 0 Step -1
                Dim dCondBlock As Tensor = Nothing
                d = Me._blocks(i).Backward(d, dCondBlock)

                If dCondition Is Nothing Then
                    dCondition = dCondBlock
                Else
                    dCondition = dCondition + dCondBlock
                End If
            Next

            d = Activations.Backward(_activation, Me._preActivation, d)
            Dim dHin = Me._inProj.Backward(d)                                         ' [B, G+timeDim]

            ' 拆出 x_t 与时间嵌入两部分
            Dim dXt = TensorUtil.SliceLastDim(dHin, 0, Me._geneCount)
            Dim dTimeFromInput = TensorUtil.SliceLastDim(dHin, Me._geneCount, Me._timeDim)

            ' 条件中的时间嵌入部分同样要把梯度送回时间嵌入的两层线性
            Dim dTimeFromCondition = TensorUtil.SliceLastDim(dCondition, Me._semanticDim, Me._timeDim)
            Call Me._timeEmbed.Backward(dTimeFromInput + dTimeFromCondition)

            dZsem = TensorUtil.SliceLastDim(dCondition, 0, Me._semanticDim)
            Return dXt
        End Function
    End Class
End Namespace
