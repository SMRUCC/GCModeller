Imports Microsoft.VisualBasic.MachineLearning.TensorFlow

Namespace NN

    ''' <summary>
    ''' 语义编码器专用的残差块（可进 <see cref="Sequential"/> 的 <see cref="LayerModule"/> 版本）。
    '''
    ''' 与 <see cref="ResidualBlock"/>（去噪网络用的**条件**残差块）的区别：
    ''' <list type="bullet">
    ''' <item>本块实现 <see cref="LayerModule"/>，因此可以被 <c>Sequential</c> 顺序容器组装；
    '''       <see cref="ResidualBlock"/> 只实现 <see cref="IParameterized"/>，其 <c>Backward</c>
    '''       额外输出 <c>dCond</c>，无法塞进顺序容器。</item>
    ''' <item>归一化层默认为**非条件** <see cref="BatchNorm"/>。因为
    '''       <c>Δz = Enc(x_pert) − Enc(x_ctrl)</c> 的向量算术要求编码侧不泄漏扰动标签，
    '''       否则 Δz 会被标签信息污染而失去可解释性（见 <c>SquiiffConfig.EncoderConditioned</c> 注释）。</item>
    ''' </list>
    '''
    ''' 结构：
    ''' <code>
    ''' h₁ = Norm₁(x[, cond])
    ''' a₁ = Act(h₁)
    ''' u  = Linear₁(a₁)
    ''' h₂ = Norm₂(u[, cond])
    ''' a₂ = Act(h₂)
    ''' v  = Linear₂(a₂)
    ''' out = shortcut(x) + v       ' shortcut 为恒等，或在升维时走 1×1 线性投影
    ''' </code>
    ''' </summary>
    Public Class EncoderResidualBlock
        Inherits LayerModule

        Private ReadOnly _name As String
        Private ReadOnly _activation As ActivationKind
        Private ReadOnly _conditioned As Boolean
        Private ReadOnly _conditionDim As Integer

        Private ReadOnly _norm1 As BatchNorm
        Private ReadOnly _norm2 As BatchNorm
        Private ReadOnly _cnorm1 As ConditionalBatchNorm
        Private ReadOnly _cnorm2 As ConditionalBatchNorm

        Private ReadOnly _lin1 As Linear
        Private ReadOnly _lin2 As Linear

        ''' <summary>跳跃连接投影；当 <c>inFeatures = hidden</c> 时为 Nothing（直接恒等）。</summary>
        Private ReadOnly _skip As Linear

        Private ReadOnly _params As Parameter()

        Private _preActivation1 As Tensor
        Private _preActivation2 As Tensor

        ''' <param name="inFeatures">输入宽度。</param>
        ''' <param name="hidden">块内宽度。</param>
        ''' <param name="conditioned">True 使用条件批归一化（需传入条件向量），False 使用普通批归一化。</param>
        ''' <param name="conditionDim">条件向量维度；仅在 <paramref name="conditioned"/> 为 True 时有效。</param>
        Public Sub New(name As String, inFeatures As Integer, hidden As Integer,
                       Optional activation As ActivationKind = ActivationKind.SiLU,
                       Optional conditioned As Boolean = False,
                       Optional conditionDim As Integer = 0)
            If inFeatures <= 0 OrElse hidden <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(hidden), "输入/隐层宽度必须为正整数")
            End If
            If conditioned AndAlso conditionDim <= 0 Then
                Throw New ArgumentException("使用条件批归一化时 conditionDim 必须为正整数", NameOf(conditionDim))
            End If

            Me._name = name
            Me._activation = activation
            Me._conditioned = conditioned
            Me._conditionDim = conditionDim

            If conditioned Then
                Me._cnorm1 = New ConditionalBatchNorm($"{name}.n1", inFeatures, conditionDim)
                Me._cnorm2 = New ConditionalBatchNorm($"{name}.n2", hidden, conditionDim)
            Else
                Me._norm1 = New BatchNorm($"{name}.n1", inFeatures)
                Me._norm2 = New BatchNorm($"{name}.n2", hidden)
            End If

            Me._lin1 = New Linear($"{name}.l1", inFeatures, hidden)
            Me._lin2 = New Linear($"{name}.l2", hidden, hidden)

            ' 升维（inFeatures ≠ hidden）时残差支路无法直接相加，用 1×1 线性投影对齐维度
            If inFeatures <> hidden Then
                Me._skip = New Linear($"{name}.skip", inFeatures, hidden)
            End If

            Dim items As New List(Of IParameterized)
            If conditioned Then
                items.Add(Me._cnorm1)
            Else
                items.Add(Me._norm1)
            End If
            items.Add(Me._lin1)
            If conditioned Then
                items.Add(Me._cnorm2)
            Else
                items.Add(Me._norm2)
            End If
            items.Add(Me._lin2)
            If Me._skip IsNot Nothing Then items.Add(Me._skip)

            Me._params = ParameterGroups.FlattenMany(items)
        End Sub

        Public Overrides ReadOnly Property Name As String
            Get
                Return _name
            End Get
        End Property

        ''' <summary>是否使用条件批归一化。</summary>
        Public ReadOnly Property IsConditioned As Boolean
            Get
                Return _conditioned
            End Get
        End Property

        ''' <summary>条件向量维度（非条件时为 0）。</summary>
        Public ReadOnly Property ConditionDim As Integer
            Get
                Return _conditionDim
            End Get
        End Property

        Public Overrides ReadOnly Property Parameters As IEnumerable(Of Parameter)
            Get
                Return _params
            End Get
        End Property

        ''' <summary>本块包含的归一化层（诊断 / 推理模式切换 / 存档滑动统计量用）。</summary>
        Public ReadOnly Property Normalizations As IReadOnlyList(Of IRunningStatistics)
            Get
                If _conditioned Then Return New IRunningStatistics() {_cnorm1, _cnorm2}
                Return New IRunningStatistics() {_norm1, _norm2}
            End Get
        End Property

        ''' <summary>把推理阶段是否使用批统计量转发给内部归一化层。</summary>
        Public Sub SetUseBatchStatsAtInference(value As Boolean)
            If _conditioned Then
                _cnorm1.UseBatchStatsAtInference = value
                _cnorm2.UseBatchStatsAtInference = value
            Else
                _norm1.UseBatchStatsAtInference = value
                _norm2.UseBatchStatsAtInference = value
            End If
        End Sub

#Region "前向"

        ''' <summary>
        ''' 非条件前向（<see cref="LayerModule"/> 契约）。
        ''' 当本块被配置为条件批归一化时，条件向量取全零——配合条件投影的零初始化
        ''' （<c>Wγ=0,bγ=1</c>；<c>Wβ=0,bβ=0</c>），结果退化为"标准化 + γ=1,β=0"，
        ''' 与非条件路径在训练起点等价。
        ''' </summary>
        Public Overrides Function Forward(x As Tensor, training As Boolean) As Tensor
            Dim cond As Tensor = Nothing
            If _conditioned Then cond = New Tensor(New Integer() {x.Shape(0), _conditionDim})

            Return Forward(x, cond, training)
        End Function

        ''' <summary>带条件向量的前向。</summary>
        Public Overloads Function Forward(x As Tensor, cond As Tensor, training As Boolean) As Tensor
            Dim h1 = Normalize1(x, cond, training)
            Me._preActivation1 = h1

            Dim a1 = Activations.Forward(_activation, h1)
            Dim u = Me._lin1.Forward(a1, training)

            Dim h2 = Normalize2(u, cond, training)
            Me._preActivation2 = h2

            Dim a2 = Activations.Forward(_activation, h2)
            Dim v = Me._lin2.Forward(a2, training)

            Dim shortcut = If(Me._skip Is Nothing, x, Me._skip.Forward(x, training))

            Return shortcut + v
        End Function

#End Region

#Region "反向"

        ''' <summary>非条件反向（<see cref="LayerModule"/> 契约）；条件梯度被丢弃。</summary>
        Public Overrides Function Backward(dOut As Tensor) As Tensor
            Dim discarded As Tensor = Nothing
            Return Backward(dOut, discarded)
        End Function

        ''' <summary>
        ''' 反向：返回对 <c>x</c> 的梯度；条件批归一化启用时把对条件向量的梯度写到
        ''' <paramref name="dCond"/>（否则置 Nothing）。
        ''' </summary>
        Public Overloads Function Backward(dOut As Tensor, ByRef dCond As Tensor) As Tensor
            Dim d = Me._lin2.Backward(dOut)
            d = Activations.Backward(_activation, Me._preActivation2, d)

            Dim dCond2 As Tensor = Nothing
            d = Denormalize2(d, dCond2)

            d = Me._lin1.Backward(d)
            d = Activations.Backward(_activation, Me._preActivation1, d)

            Dim dCond1 As Tensor = Nothing
            d = Denormalize1(d, dCond1)

            dCond = If(_conditioned, dCond1 + dCond2, Nothing)

            ' 残差支路：梯度经跳跃连接（或 1×1 投影）原样流回
            Dim dShortcut = If(Me._skip Is Nothing, dOut, Me._skip.Backward(dOut))

            Return d + dShortcut
        End Function

#End Region

        Private Function Normalize1(x As Tensor, cond As Tensor, training As Boolean) As Tensor
            If _conditioned Then Return _cnorm1.Forward(x, cond, training)
            Return _norm1.Forward(x, training)
        End Function

        Private Function Normalize2(x As Tensor, cond As Tensor, training As Boolean) As Tensor
            If _conditioned Then Return _cnorm2.Forward(x, cond, training)
            Return _norm2.Forward(x, training)
        End Function

        Private Function Denormalize1(dOut As Tensor, ByRef dCond As Tensor) As Tensor
            If _conditioned Then Return _cnorm1.Backward(dOut, dCond)
            Return _norm1.Backward(dOut)
        End Function

        Private Function Denormalize2(dOut As Tensor, ByRef dCond As Tensor) As Tensor
            If _conditioned Then Return _cnorm2.Backward(dOut, dCond)
            Return _norm2.Backward(dOut)
        End Function
    End Class
End Namespace
