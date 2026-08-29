Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports GNN = Microsoft.VisualBasic.DeepLearning.GNN

Namespace Layers

    ''' <summary>
    ''' 全连接层（线性变换层），实现 y = x @ W + b
    ''' </summary>
    ''' <remarks>
    ''' GNN 模块自带的 <see cref="GNN.LinearLayer"/> 权重形状为 [inFeatures, outFeatures]，
    ''' 但其 Forward 使用 <c>input.MatMul(_weights.Transpose())</c>、Backward 使用
    ''' <c>gradient.MatMul(_weights)</c>，两者都要求 inFeatures 与 outFeatures 相等，
    ''' 否则会抛出「矩阵维度不匹配」。GEARS 的隐藏层维度需要自由配置（例如 34 → 32），
    ''' 因此这里提供一个维度语义正确的全连接实现（权重 [in, out]），
    ''' 不修改共享运行时代码以免影响 GNN 模块的既有调用方。
    '''
    ''' 本层的参数张量与梯度张量在整个生命周期内保持同一批实例，
    ''' Adam 优化器持有它们的引用并原地更新。
    ''' </remarks>
    Public Class DenseLayer
        Inherits GNN.Layer

        ''' <summary>权重矩阵 [inFeatures, outFeatures]</summary>
        ReadOnly weights As Tensor

        ''' <summary>偏置向量 [1, outFeatures]</summary>
        ReadOnly bias As Tensor

        ''' <summary>权重梯度 [inFeatures, outFeatures]</summary>
        ReadOnly weightGrad As Tensor

        ''' <summary>偏置梯度 [1, outFeatures]</summary>
        ReadOnly biasGrad As Tensor

        ''' <summary>上一次前向传播的输入，用于反向传播</summary>
        Dim lastInput As Tensor

        ''' <summary>输入特征维度</summary>
        ''' <returns>输入维度大小</returns>
        Public ReadOnly Property InFeatures As Integer

        ''' <summary>输出特征维度</summary>
        ''' <returns>输出维度大小</returns>
        Public ReadOnly Property OutFeatures As Integer

        ''' <summary>是否使用偏置项</summary>
        ''' <returns>使用偏置则返回 True</returns>
        Public ReadOnly Property UseBias As Boolean

        ''' <summary>是否跳过线性变换（恒等映射，用于把某一路分支固定为直通）</summary>
        ''' <returns>恒等映射则返回 True</returns>
        Public ReadOnly Property IsIdentity As Boolean

        ''' <summary>
        ''' 创建全连接层
        ''' </summary>
        ''' <param name="inFeatures">输入特征维度</param>
        ''' <param name="outFeatures">输出特征维度</param>
        ''' <param name="useBias">是否使用偏置项</param>
        ''' <param name="scale">权重初始化的缩放系数，默认 1.0（Xavier 初始化的标准差乘上该系数）</param>
        ''' <param name="identity">为 True 时构造一个恒等映射层（权重固定为单位矩阵、不参与训练）</param>
        ''' <param name="name">层名称</param>
        Public Sub New(inFeatures As Integer,
                       outFeatures As Integer,
                       Optional useBias As Boolean = True,
                       Optional scale As Double = 1.0,
                       Optional identity As Boolean = False,
                       Optional name As String = Nothing)

            Me.InFeatures = inFeatures
            Me.OutFeatures = outFeatures
            Me.UseBias = useBias
            Me.IsIdentity = identity
            MyBase.Name = If(name, $"Dense_{inFeatures}_{outFeatures}")

            If identity Then
                If inFeatures <> outFeatures Then
                    Throw New ArgumentException("恒等映射层要求输入输出维度相同")
                End If

                weights = Tensor.Identity(inFeatures)
                bias = New Tensor(1, outFeatures)
                weightGrad = New Tensor(inFeatures, outFeatures)
                biasGrad = New Tensor(1, outFeatures)
            Else
                weights = Tensor.XavierInit(inFeatures, outFeatures)

                If scale <> 1.0 Then
                    Dim wd As Double() = weights.Data

                    For i As Integer = 0 To wd.Length - 1
                        wd(i) *= scale
                    Next
                End If

                If useBias Then
                    bias = New Tensor(1, outFeatures)
                Else
                    bias = New Tensor(1, 0)
                End If

                weightGrad = New Tensor(inFeatures, outFeatures)
                biasGrad = If(useBias, New Tensor(1, outFeatures), New Tensor(1, 0))
            End If
        End Sub

        ''' <summary>
        ''' 前向传播：y = x @ W + b
        ''' </summary>
        ''' <param name="input">输入特征 [batch, inFeatures]</param>
        ''' <returns>输出特征 [batch, outFeatures]</returns>
        Public Overrides Function Forward(input As Tensor) As Tensor
            lastInput = input

            Dim output As Tensor = MatOps.Mul(input, weights)

            If UseBias Then
                Dim bd As Double() = bias.Data
                Dim od As Double() = output.Data
                Dim n As Integer = output.Shape(0)
                Dim d As Integer = output.Shape(1)

                For i As Integer = 0 To n - 1
                    Dim off As Integer = i * d

                    For j As Integer = 0 To d - 1
                        od(off + j) += bd(j)
                    Next
                Next
            End If

            Return output
        End Function

        ''' <summary>
        ''' 反向传播：累积 dW / db 并返回输入梯度 dX
        ''' </summary>
        ''' <param name="gradient">上游梯度 [batch, outFeatures]</param>
        ''' <returns>输入梯度 [batch, inFeatures]</returns>
        Public Overrides Function Backward(gradient As Tensor) As Tensor
            If IsIdentity Then
                ' 恒等映射不参与训练，梯度直接透传
                Return gradient
            End If

            ' dW = Xᵀ @ G  [in, out]
            Dim dW As Tensor = MatOps.MulAT(lastInput, gradient)
            ' db = Σ_rows G  [1, out]
            Dim dB As Tensor = New Tensor(1, gradient.Shape(1))
            ' dX = G @ Wᵀ  [batch, in]
            Dim dX As Tensor = MatOps.MulBT(gradient, weights)

            Call MatOps.ColSumInto(gradient, dB)
            Call MatOps.Accumulate(dW, weightGrad)

            If UseBias Then
                Call MatOps.Accumulate(dB, biasGrad)
            End If

            Return dX
        End Function

        ''' <summary>
        ''' 获取本层可训练参数（权重与偏置，恒等映射层返回空列表）
        ''' </summary>
        ''' <returns>参数张量列表</returns>
        Public Overrides Function GetParameters() As List(Of Tensor)
            If IsIdentity Then
                Return New List(Of Tensor)()
            End If

            Dim params As New List(Of Tensor) From {weights}

            If UseBias Then
                params.Add(bias)
            End If

            Return params
        End Function

        ''' <summary>
        ''' 获取本层参数梯度（顺序与 <see cref="GetParameters"/> 一致）
        ''' </summary>
        ''' <returns>梯度张量列表</returns>
        Public Overrides Function GetGradients() As List(Of Tensor)
            If IsIdentity Then
                Return New List(Of Tensor)()
            End If

            Dim grads As New List(Of Tensor) From {weightGrad}

            If UseBias Then
                grads.Add(biasGrad)
            End If

            Return grads
        End Function

        ''' <summary>
        ''' 获取权重矩阵（用于调试或模型检查）
        ''' </summary>
        ''' <returns>权重张量 [inFeatures, outFeatures]</returns>
        Public Function GetWeights() As Tensor
            Return weights
        End Function

        ''' <summary>
        ''' 获取偏置向量（用于调试或模型检查）
        ''' </summary>
        ''' <returns>偏置张量 [1, outFeatures]</returns>
        Public Function GetBias() As Tensor
            Return bias
        End Function
    End Class
End Namespace
