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
End Namespace
