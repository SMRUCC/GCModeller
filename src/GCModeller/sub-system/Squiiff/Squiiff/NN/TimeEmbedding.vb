Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace NN

    ''' <summary>
    ''' 时间步正弦位置编码 + 两层 MLP。
    '''
    ''' 扩散步 <c>t</c> 通过**正弦位置编码**注入网络，无需额外可学习参数即可表达尺度不变的时间信息：
    ''' <code>
    ''' 偶数维 i:  sin( pos / 10000^(i / d) )
    ''' 奇数维 i:  cos( pos / 10000^((i−1) / d) )
    ''' </code>
    ''' 编码表对 <c>[0, maxSteps]</c> 一次性预计算（避免每个训练步重复计算三角函数），
    ''' 随后经 <c>Linear → Act → Linear</c> 得到时间嵌入。
    '''
    ''' 说明：DeepLearning 中 <c>Transformer.Embedding.AddPositionalEncoding</c> 是 <c>Private</c>，
    ''' 无法直接复用，因此此处按其公式重新实现。
    ''' </summary>
    Public Class TimeEmbedding
        Implements IParameterized

        Private ReadOnly _name As String
        Private ReadOnly _dim As Integer
        Private ReadOnly _maxSteps As Integer
        Private ReadOnly _table As Double()
        Private ReadOnly _lin1 As Linear
        Private ReadOnly _lin2 As Linear
        Private ReadOnly _params As Parameter()
        Private ReadOnly _activation As ActivationKind

        Private _preActivation As Tensor

        Public Sub New(name As String, dim As Integer, Optional maxSteps As Integer = 1024,
                       Optional activation As ActivationKind = ActivationKind.SiLU)
            If dim <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(dim))
            If dim Mod 2 <> 0 Then
                ' 正弦编码按成对的 sin/cos 构造，维度必须为偶数
                Throw New ArgumentException("时间嵌入维度必须为偶数", NameOf(dim))
            End If

            Me._name = name
            Me._dim = dim
            Me._maxSteps = maxSteps
            Me._activation = activation
            Me._table = BuildSinusoidalTable(dim, maxSteps)
            Me._lin1 = New Linear($"{name}.l1", dim, dim)
            Me._lin2 = New Linear($"{name}.l2", dim, dim)
            Me._params = ParameterGroups.Flatten(Me._lin1, Me._lin2)
        End Sub

        ''' <summary>嵌入维度（偶数）。</summary>
        Public ReadOnly Property Dim As Integer
            Get
                Return _dim
            End Get
        End Property

        ''' <summary>预计算的最大时间步。</summary>
        Public ReadOnly Property MaxSteps As Integer
            Get
                Return _maxSteps
            End Get
        End Property

        Public ReadOnly Property Name As String
            Get
                Return _name
            End Get
        End Property

        Public ReadOnly Property Parameters As IEnumerable(Of Parameter) Implements IParameterized.Parameters
            Get
                Return _params
            End Get
        End Property

        Private Shared Function BuildSinusoidalTable(dim As Integer, maxSteps As Integer) As Double()
            Dim table((maxSteps + 1) * dim - 1) As Double
            Dim half = dim \ 2

            For pos As Integer = 0 To maxSteps
                Dim offset = pos * dim
                For i As Integer = 0 To half - 1
                    Dim freq = std.Exp(-std.Log(10000.0) * i / half)
                    table(offset + 2 * i) = std.Sin(pos * freq)
                    If 2 * i + 1 < dim Then
                        table(offset + 2 * i + 1) = std.Cos(pos * freq)
                    End If
                Next
            Next

            Return table
        End Function

        ''' <summary>
        ''' 前向：<paramref name="t"/> 为整型时间步数组（长度 = 批量）。
        ''' 返回 <c>[B, dim]</c> 的时间嵌入。
        ''' </summary>
        Public Function Forward(t As Integer(), training As Boolean) As Tensor
            Dim B = t.Length
            Dim emb = New Tensor(New Integer() {B, _dim})
            Dim data = emb.Data

            For b As Integer = 0 To B - 1
                Dim idx = t(b)
                If idx < 0 Then idx = 0
                If idx > _maxSteps Then idx = _maxSteps

                Dim src = idx * _dim
                Dim dst = b * _dim
                For i As Integer = 0 To _dim - 1
                    data(dst + i) = _table(src + i)
                Next
            Next

            Call emb.MarkHostModified()

            Dim h = _lin1.Forward(emb, training)
            Me._preActivation = h
            h = Activations.Forward(_activation, h)

            Return _lin2.Forward(h, training)
        End Function

        ''' <summary>
        ''' 反向：只有两层 Linear 含可训练参数，编码表本身不可训练，
        ''' 因此返回的"对时间索引的梯度"无意义，由调用方忽略。
        ''' </summary>
        Public Function Backward(dOut As Tensor) As Tensor
            Dim d = _lin2.Backward(dOut)
            d = Activations.Backward(_activation, _preActivation, d)
            Return _lin1.Backward(d)
        End Function
    End Class
End Namespace
