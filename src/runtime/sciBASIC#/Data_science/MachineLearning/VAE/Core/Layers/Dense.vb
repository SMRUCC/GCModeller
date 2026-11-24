' ============================================================================
' Dense.vb - 全连接层
'
' 与 GMVAE.vb L732-750 的三重标量循环版本的关键差别：
'   前向走 Tensor.MatMul，因此当 m·k·n >= MatrixDotThreshold(10000) 时
'   会命中 computeKernel 的 SIMD 并行内核（SIMDTensor.vb L343-378），
'   注册 CUDA 后端之后还能进一步下沉到 GPU。
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace Core.Layers

    ''' <summary>
    ''' 全连接层：y = activation(x · W + b)。
    ''' </summary>
    Public Class Dense
        Implements IModule

        Private ReadOnly _weights As Tensor
        Private ReadOnly _bias As Tensor
        Private ReadOnly _weightGrad As Tensor
        Private ReadOnly _biasGrad As Tensor
        Private ReadOnly _useRelu As Boolean
        Private ReadOnly _adam As Adam

        Private _input As Tensor
        Private _output As Tensor

        ''' <summary>
        ''' 创建全连接层。使用 ReLU 时采用 He 初始化，否则采用 Xavier 初始化。
        ''' </summary>
        Public Sub New(fanIn As Integer, fanOut As Integer,
                       Optional useRelu As Boolean = False,
                       Optional seed As Integer? = Nothing,
                       Optional learningRate As Double = 0.001)
            If fanIn <= 0 OrElse fanOut <= 0 Then
                Throw New ArgumentException($"全连接层维度必须为正：fanIn={fanIn}, fanOut={fanOut}")
            End If

            _useRelu = useRelu
            _weights = If(useRelu, Tensor.HeInit(fanIn, fanOut, seed), Tensor.XavierInit(fanIn, fanOut, seed))
            _bias = Tensor.Zeros(New Integer() {1, fanOut})
            _weightGrad = Tensor.Zeros(New Integer() {fanIn, fanOut})
            _biasGrad = Tensor.Zeros(New Integer() {1, fanOut})
            _adam = New Adam(_weights.Shape, learningRate)
        End Sub

        Public ReadOnly Property FanIn As Integer
            Get
                Return _weights.Shape(0)
            End Get
        End Property

        Public ReadOnly Property FanOut As Integer
            Get
                Return _weights.Shape(1)
            End Get
        End Property

        Public ReadOnly Property Weights As Tensor Implements IModule.Weights
            Get
                Return _weights
            End Get
        End Property

        Public ReadOnly Property Bias As Tensor
            Get
                Return _bias
            End Get
        End Property

        Public ReadOnly Property WeightGrad As Tensor Implements IModule.WeightGrad
            Get
                Return _weightGrad
            End Get
        End Property

        Public ReadOnly Property BiasGrad As Tensor
            Get
                Return _biasGrad
            End Get
        End Property

        ''' <summary>上一次前向传播的（激活后）输出，供反向传播使用。</summary>
        Public ReadOnly Property Output As Tensor
            Get
                Return _output
            End Get
        End Property

        ''' <summary>前向传播：y = activation(x·W + b)。</summary>
        Public Function Forward(input As Tensor) As Tensor Implements IModule.Forward
            If input Is Nothing Then
                Throw New ArgumentNullException(NameOf(input))
            End If

            If input.Rank <> 2 OrElse input.Shape(1) <> FanIn Then
                Throw New ArgumentException(
                    $"全连接层输入形状不符：期望 [batch, {FanIn}]，实际 [{String.Join(", ", input.Shape)}]")
            End If

            _input = input

            Dim z As Tensor = input.MatMul(_weights)
            Dim rows As Integer = z.Shape(0)
            Dim columns As Integer = z.Shape(1)
            Dim buffer As Double() = z.Data
            Dim offset As Double() = _bias.Data

            For i As Integer = 0 To rows - 1
                Dim baseIndex As Integer = i * columns

                For j As Integer = 0 To columns - 1
                    buffer(baseIndex + j) += offset(j)
                Next
            Next

            If _useRelu Then
                For i As Integer = 0 To buffer.Length - 1
                    If buffer(i) < 0 Then
                        buffer(i) = 0
                    End If
                Next
            End If

            z.MarkHostModified()

            _output = z

            Return z
        End Function

        ''' <summary>
        ''' 反向传播。返回对输入的梯度，并把权重 / 偏置梯度写入
        ''' <see cref="WeightGrad"/> 与 <see cref="BiasGrad"/>。
        ''' </summary>
        Public Function Backward(gradOutput As Tensor) As Tensor Implements IModule.Backward
            If _input Is Nothing Then
                Throw New InvalidOperationException("反向传播之前必须先调用 Forward")
            End If

            Dim grad As Tensor = gradOutput

            If _useRelu Then
                ' 用前向输出作为 ReLU 掩码：输出 > 0 等价于预激活 > 0
                Dim data As Double() = CType(gradOutput.Data.Clone(), Double())
                Dim mask As Double() = _output.Data

                For i As Integer = 0 To data.Length - 1
                    If mask(i) <= 0 Then
                        data(i) = 0
                    End If
                Next

                grad = New Tensor(data, gradOutput.Shape)
            End If

            Dim weightGradient As Tensor = _input.Transpose().MatMul(grad)
            Dim biasGradient As Tensor = grad.Sum(0)

            Array.Copy(weightGradient.Data, _weightGrad.Data, _weightGrad.Data.Length)
            Array.Copy(biasGradient.Data, _biasGrad.Data, _biasGrad.Data.Length)

            Return grad.MatMul(_weights.Transpose())
        End Function

        ''' <summary>用给定的 Adam 优化器更新权重与偏置。</summary>
        Public Sub Update(optimizer As Adam) Implements IModule.Update
            optimizer.Update(_weights, _weightGrad)
        End Sub

        ''' <summary>用内部 Adam 更新（学习率可覆盖）。</summary>
        Public Sub Update(Optional learningRate As Double = -1)
            _adam.Update(_weights, _weightGrad, learningRate)
            _adam.Update(_bias, _biasGrad, learningRate)
        End Sub

        ''' <summary>普通 SGD 更新（复现旧行为）。</summary>
        Public Sub UpdateSgd(learningRate As Double)
            Dim parameters As Double() = _weights.Data
            Dim gradients As Double() = _weightGrad.Data

            For i As Integer = 0 To parameters.Length - 1
                parameters(i) -= learningRate * gradients(i)
            Next

            Dim bias As Double() = _bias.Data
            Dim biasGradients As Double() = _biasGrad.Data

            For i As Integer = 0 To bias.Length - 1
                bias(i) -= learningRate * biasGradients(i)
            Next
        End Sub

        Public Overrides Function ToString() As String
            Return $"Dense({FanIn} -> {FanOut}, relu={_useRelu})"
        End Function

    End Class

End Namespace
