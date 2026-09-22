Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports tfMath = Microsoft.VisualBasic.MachineLearning.TensorFlow.Math
Imports tfnn = Microsoft.VisualBasic.MachineLearning.TensorFlow.nn

Namespace NN

    ''' <summary>激活函数种类（扩散去噪网络默认使用 SiLU，与主流扩散模型一致）。</summary>
    Public Enum ActivationKind
        ReLU = 0
        SiLU = 1
        Tanh = 2
        Sigmoid = 3
    End Enum

    ''' <summary>
    ''' 激活函数的前向与**解析反向**。
    '''
    ''' 底层张量库只提供 <c>Heaviside</c>（ReLU 反向掩码）等少数反向所需算子，
    ''' 其余激活的导数必须由本模块组合张量算子给出：
    ''' <list type="bullet">
    ''' <item>ReLU: <c>f' = (x &gt; 0)</c>，直接用 <c>computeKernel.Heaviside</c>；</item>
    ''' <item>SiLU: <c>f = x·σ(x)</c>，<c>f' = σ(x) + f·(1 − σ(x))</c>；</item>
    ''' <item>Tanh: <c>f = tanh(x)</c>，<c>f' = 1 − f²</c>；</item>
    ''' <item>Sigmoid: <c>f = σ(x)</c>，<c>f' = f·(1 − f)</c>。</item>
    ''' </list>
    ''' </summary>
    Public Module Activations

        Public Function Forward(kind As ActivationKind, x As Tensor) As Tensor
            Select Case kind
                Case ActivationKind.ReLU
                    Return tfnn.relu(x)
                Case ActivationKind.SiLU
                    Return x.ElementwiseMultiply(tfnn.sigmoid(x))
                Case ActivationKind.Tanh
                    Return tfnn.tanh(x)
                Case ActivationKind.Sigmoid
                    Return tfnn.sigmoid(x)
                Case Else
                    Throw New NotSupportedException($"未实现的激活函数: {kind}")
            End Select
        End Function

        ''' <summary>
        ''' 反向：给定前向的**输入** <paramref name="x"/>（激活前）与上游梯度 <paramref name="dOut"/>，
        ''' 返回对 <paramref name="x"/> 的梯度。
        ''' </summary>
        Public Function Backward(kind As ActivationKind, x As Tensor, dOut As Tensor) As Tensor
            Select Case kind
                Case ActivationKind.ReLU
                    Return dOut.ElementwiseMultiply(Tensor.computeKernel.Heaviside(x))

                Case ActivationKind.SiLU
                    Dim s = tfnn.sigmoid(x)
                    Dim f = x.ElementwiseMultiply(s)
                    ' f' = σ + f·(1 − σ)
                    Dim oneMinusS = tfMath.add_scalar(tfMath.negative(s), 1.0)
                    Dim df = s + f.ElementwiseMultiply(oneMinusS)
                    Return dOut.ElementwiseMultiply(df)

                Case ActivationKind.Tanh
                    Dim f = tfnn.tanh(x)
                    ' f' = 1 − f²
                    Dim df = tfMath.add_scalar(tfMath.negative(f.ElementwiseMultiply(f)), 1.0)
                    Return dOut.ElementwiseMultiply(df)

                Case ActivationKind.Sigmoid
                    Dim f = tfnn.sigmoid(x)
                    Dim df = f.ElementwiseMultiply(tfMath.add_scalar(tfMath.negative(f), 1.0))
                    Return dOut.ElementwiseMultiply(df)

                Case Else
                    Throw New NotSupportedException($"未实现的激活函数: {kind}")
            End Select
        End Function
    End Module

    ''' <summary>把 <see cref="Activations"/> 包装为可按 <see cref="LayerModule"/> 组装的层。</summary>
    Public Class ActivationLayer
        Inherits LayerModule

        Private ReadOnly _name As String
        Private ReadOnly _kind As ActivationKind
        Private _preActivation As Tensor

        Public Sub New(name As String, kind As ActivationKind)
            Me._name = name
            Me._kind = kind
        End Sub

        Public ReadOnly Property Kind As ActivationKind
            Get
                Return _kind
            End Get
        End Property

        Public Overrides ReadOnly Property Name As String
            Get
                Return _name
            End Get
        End Property

        Public Overrides Function Forward(x As Tensor, training As Boolean) As Tensor
            Me._preActivation = x
            Return Activations.Forward(_kind, x)
        End Function

        Public Overrides Function Backward(dOut As Tensor) As Tensor
            Return Activations.Backward(_kind, _preActivation, dOut)
        End Function
    End Class
End Namespace
