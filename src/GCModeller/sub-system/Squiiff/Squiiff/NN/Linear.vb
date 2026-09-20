Imports Microsoft.VisualBasic.MachineLearning.TensorFlow

Namespace NN

    ''' <summary>
    ''' 全连接层：<c>y = x · W + b</c>。
    '''
    ''' 形状约定：<c>x:[B,in]</c>、<c>W:[in,out]</c>、<c>b:[1,out]</c>、<c>y:[B,out]</c>。
    ''' 偏置刻意保持二维 <c>[1,out]</c>（而不是一维），这样广播加走
    ''' <see cref="TensorUtil.BroadcastRow"/> 的矩阵乘路径，且偏置梯度 <c>dOut.Sum(0)</c>
    ''' 天然是 <c>[1,out]</c>，与参数形状一致。
    ''' </summary>
    Public Class Linear
        Inherits LayerModule

        Private ReadOnly _name As String
        Private ReadOnly _params As Parameter()

        ''' <summary>权重 <c>[in,out]</c>（He 初始化）。</summary>
        Public ReadOnly Property Weight As Parameter

        ''' <summary>偏置 <c>[1,out]</c>（零初始化）。</summary>
        Public ReadOnly Property Bias As Parameter

        Private _x As Tensor

        Public Sub New(name As String, inFeatures As Integer, outFeatures As Integer)
            If inFeatures <= 0 OrElse outFeatures <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(outFeatures), "输入/输出维度必须为正整数")
            End If

            Me._name = name
            Me.Weight = New Parameter($"{name}.W", TensorUtil.HeNormal(New Integer() {inFeatures, outFeatures}))
            Me.Bias = New Parameter($"{name}.b", New Tensor(New Integer() {1, outFeatures}))
            Me._params = {Me.Weight, Me.Bias}
        End Sub

        Public Overrides ReadOnly Property Name As String
            Get
                Return _name
            End Get
        End Property

        Public Overrides ReadOnly Property Parameters As IEnumerable(Of Parameter)
            Get
                Return _params
            End Get
        End Property

        Public Overrides Function Forward(x As Tensor, training As Boolean) As Tensor
            Me._x = x
            ' y = x·W + b（b 由 [1,out] 广播到 [B,out]）
            Return TensorUtil.BroadcastAdd(x.MatMul(Me.Weight.Value), Me.Bias.Value)
        End Function

        Public Overrides Function Backward(dOut As Tensor) As Tensor
            ' db = Σ_batch dOut -> [1,out]
            TensorUtil.Accumulate(Me.Bias.Gradient, dOut.Sum(axis:=0))

            ' dW = xᵀ · dOut -> [in,out]
            TensorUtil.Accumulate(Me.Weight.Gradient, _x.Transpose().MatMul(dOut))

            ' dx = dOut · Wᵀ -> [B,in]
            Return dOut.MatMul(Me.Weight.Value.Transpose())
        End Function
    End Class
End Namespace
