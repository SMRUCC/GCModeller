Imports Microsoft.VisualBasic.MachineLearning.TensorFlow

Namespace NN

    ''' <summary>
    ''' 手写前向 / 反向传播的层契约。
    '''
    ''' 底层张量库（<c>Microsoft.VisualBasic.MachineLearning.TensorFlow</c>）**不提供自动微分**，
    ''' 因此每一层都必须显式实现 <see cref="Forward"/> 与 <see cref="Backward"/>：
    ''' <list type="bullet">
    ''' <item><see cref="Forward"/> 缓存反向所需的中间量（输入 / 归一化统计量等）；</item>
    ''' <item><see cref="Backward"/> 接收上游梯度，把本层参数的梯度**原地累加**进
    '''       <see cref="Parameter.Gradient"/>，并返回对本层输入的梯度。</item>
    ''' </list>
    ''' </summary>
    Public MustInherit Class LayerModule
        Implements IParameterized

        ''' <summary>层名称（诊断输出用）。</summary>
        Public MustOverride ReadOnly Property Name As String

        ''' <summary>本层直接持有的可训练参数（默认无参数）。</summary>
        Public Overridable ReadOnly Property Parameters As IEnumerable(Of Parameter) Implements IParameterized.Parameters
            Get
                Return Enumerable.Empty(Of Parameter)()
            End Get
        End Property

        ''' <summary>
        ''' 前向传播。
        ''' </summary>
        ''' <param name="x">输入张量（约定第 0 维为批量维）。</param>
        ''' <param name="training">
        ''' True 表示训练模式：批归一化使用当前批统计量并更新滑动统计量；
        ''' False 表示推理/生成模式：批归一化使用滑动统计量（当层配置为推理时使用滑动统计量）。
        ''' </param>
        Public MustOverride Function Forward(x As Tensor, training As Boolean) As Tensor

        ''' <summary>
        ''' 反向传播。返回对输入的梯度；参数梯度已原地累加。
        ''' </summary>
        Public MustOverride Function Backward(dOut As Tensor) As Tensor

        ''' <summary>清空本层全部参数的梯度槽。</summary>
        Public Sub ZeroGrad()
            For Each p In Me.Parameters
                If p IsNot Nothing Then Call p.ZeroGrad()
            Next
        End Sub

        ''' <summary>本层参数元素总数（诊断用）。</summary>
        Public ReadOnly Property ParameterSize As Integer
            Get
                Dim n As Integer = 0
                For Each p In Me.Parameters
                    n += p.Size
                Next
                Return n
            End Get
        End Property
    End Class

    ''' <summary>
    ''' 带滑动统计量（running mean / running variance）的归一化层。
    '''
    ''' 批归一化系列层除了可训练参数 γ / β，还持有**不参与梯度下降**的滑动统计量。
    ''' 推理阶段默认使用滑动统计量，因此模型存档必须把它们一并落盘，
    ''' 否则 Save → Load 之后同一份输入会得到不同的输出。
    ''' </summary>
    Public Interface IRunningStatistics

        ''' <summary>层名（存档时作为主键，必须全局唯一）。</summary>
        ReadOnly Property Name As String

        ''' <summary>滑动均值 <c>[1,H]</c>。</summary>
        ReadOnly Property RunningMean As Tensor

        ''' <summary>滑动方差 <c>[1,H]</c>。</summary>
        ReadOnly Property RunningVariance As Tensor
    End Interface
End Namespace
