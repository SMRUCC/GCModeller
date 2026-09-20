Imports Microsoft.VisualBasic.MachineLearning.TensorFlow

Namespace NN

    ''' <summary>
    ''' 可训练参数：权重张量（<see cref="Value"/>）加上同形的梯度槽（<see cref="Gradient"/>）。
    '''
    ''' 约定：
    ''' 1. 反向传播阶段各层把本层参数的梯度**原地累加**进 <see cref="Gradient"/>；
    ''' 2. 优化器在每个更新步消费梯度并调用 <see cref="ZeroGrad"/> 清零。
    '''
    ''' 该约定与 <c>Transformer.Optimizer</c>（DeepLearning 工程）完全一致，
    ''' 因为底层张量库不提供自动微分，梯度累加与清空必须由调用方显式管理。
    ''' </summary>
    Public NotInheritable Class Parameter

        ''' <summary>参数名（诊断输出用，例如 <c>encoder.body.l1.W</c>）。</summary>
        Public ReadOnly Property Name As String

        ''' <summary>参数值张量（行优先的 Double 张量）。</summary>
        Public ReadOnly Property Value As Tensor

        ''' <summary>与 <see cref="Value"/> 同形状的梯度累加槽。</summary>
        Public ReadOnly Property Gradient As Tensor

        Public Sub New(name As String, value As Tensor)
            Me.Name = name
            Me.Value = value
            Me.Gradient = New Tensor(value.Shape)
        End Sub

        ''' <summary>参数元素个数。</summary>
        Public ReadOnly Property Size As Integer
            Get
                Return Me.Value.Length
            End Get
        End Property

        ''' <summary>把梯度槽清零（原地，绕过索引器直写底层数组后声明主机已修改）。</summary>
        Public Sub ZeroGrad()
            Dim grad = Me.Gradient
            Array.Clear(grad.Data, 0, grad.Data.Length)
            Call grad.MarkHostModified()
        End Sub

        Public Overrides Function ToString() As String
            Return $"{Me.Name} [{String.Join(", ", Me.Value.Shape)}]"
        End Function
    End Class

    ''' <summary>持有可训练参数集合的对象（层 / 模块 / 模型）。</summary>
    Public Interface IParameterized

        ''' <summary>本对象直接持有的可训练参数（不含嵌套子对象）。</summary>
        ReadOnly Property Parameters As IEnumerable(Of Parameter)
    End Interface

    ''' <summary>参数集合的扁平化工具（层 -> 模型 -> 优化器）。</summary>
    Public Module ParameterGroups

        ''' <summary>
        ''' 把若干 <see cref="IParameterized"/> 直接持有的参数按序拼接为数组。
        ''' </summary>
        Public Function Flatten(ParamArray items As IParameterized()) As Parameter()
            If items Is Nothing OrElse items.Length = 0 Then Return New Parameter() {}

            Dim bag As New List(Of Parameter)
            For Each item In items
                If item Is Nothing Then Continue For
                For Each p In item.Parameters
                    If p IsNot Nothing Then bag.Add(p)
                Next
            Next

            Return bag.ToArray()
        End Function
    End Module
End Namespace
