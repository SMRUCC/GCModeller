Imports Microsoft.VisualBasic.MachineLearning.TensorFlow

Namespace NN

    ''' <summary>
    ''' 条件残差块（扩散去噪网络的基本构件）：
    ''' <code>
    ''' h  = CondBN₁(x, cond)
    ''' a  = Act(h)
    ''' u  = Linear₁(a)
    ''' h₂ = CondBN₂(u, cond)
    ''' a₂ = Act(h₂)
    ''' v  = Linear₂(a₂)
    ''' out = x + v            ' 残差连接
    ''' </code>
    ''' 残差连接在底层库中并不存在（<c>CNN.layers</c> 只有严格线性堆叠），因此在此显式实现：
    ''' 反向时上游梯度复制给"跳过连接"与"主干"两支后相加，与
    ''' DeepLearning 中 <c>TensorOps.AddNormBackward</c> 对残差分支的处理方式一致。
    ''' </summary>
    Public Class ResidualBlock
        Implements IParameterized

        Private ReadOnly _name As String
        Private ReadOnly _norm1 As ConditionalBatchNorm
        Private ReadOnly _lin1 As Linear
        Private ReadOnly _norm2 As ConditionalBatchNorm
        Private ReadOnly _lin2 As Linear
        Private ReadOnly _params As Parameter()
        Private ReadOnly _activation As ActivationKind

        Private _preActivation1 As Tensor
        Private _preActivation2 As Tensor

        Public Sub New(name As String, hidden As Integer, conditionDim As Integer,
                       Optional activation As ActivationKind = ActivationKind.SiLU)
            Me._name = name
            Me._activation = activation
            Me._norm1 = New ConditionalBatchNorm($"{name}.n1", hidden, conditionDim)
            Me._lin1 = New Linear($"{name}.l1", hidden, hidden)
            Me._norm2 = New ConditionalBatchNorm($"{name}.n2", hidden, conditionDim)
            Me._lin2 = New Linear($"{name}.l2", hidden, hidden)
            Me._params = ParameterGroups.Flatten(_norm1, _lin1, _norm2, _lin2)
        End Sub

        Public ReadOnly Property Name As String
            Get
                Return _name
            End Get
        End Property

        ''' <summary>本块直接持有的参数（含两个条件批归一化的投影层）。</summary>
        Public ReadOnly Property Parameters As IEnumerable(Of Parameter) Implements IParameterized.Parameters
            Get
                Return _params
            End Get
        End Property

        ''' <summary>本块包含的条件批归一化层（诊断 / 生成模式切换用）。</summary>
        Public ReadOnly Property Normalizations As IReadOnlyList(Of ConditionalBatchNorm)
            Get
                Return {_norm1, _norm2}
            End Get
        End Property

        ''' <summary>设置推理阶段是否使用批统计量（转发给内部两个条件批归一化）。</summary>
        Public Sub SetUseBatchStatsAtInference(value As Boolean)
            _norm1.UseBatchStatsAtInference = value
            _norm2.UseBatchStatsAtInference = value
        End Sub

        Public Function Forward(x As Tensor, cond As Tensor, training As Boolean) As Tensor
            Me._preActivation1 = _norm1.Forward(x, cond, training)
            Dim a1 = Activations.Forward(_activation, _preActivation1)
            Dim u = _lin1.Forward(a1, training)

            Me._preActivation2 = _norm2.Forward(u, cond, training)
            Dim a2 = Activations.Forward(_activation, _preActivation2)
            Dim v = _lin2.Forward(a2, training)

            Return x + v
        End Function

        ''' <summary>
        ''' 反向：返回对 <c>x</c> 的梯度，并把对条件向量的梯度写到 <paramref name="dCond"/>。
        ''' </summary>
        Public Function Backward(dOut As Tensor, ByRef dCond As Tensor) As Tensor
            ' 主干
            Dim d = _lin2.Backward(dOut)
            d = Activations.Backward(_activation, _preActivation2, d)

            Dim dCond2 As Tensor = Nothing
            d = _norm2.Backward(d, dCond2)

            d = _lin1.Backward(d)
            d = Activations.Backward(_activation, _preActivation1, d)

            Dim dCond1 As Tensor = Nothing
            d = _norm1.Backward(d, dCond1)

            dCond = dCond1 + dCond2

            ' 残差分支：梯度原样流回
            Return d + dOut
        End Function
    End Class
End Namespace
