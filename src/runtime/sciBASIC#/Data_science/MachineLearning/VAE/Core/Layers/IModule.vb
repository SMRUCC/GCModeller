' ============================================================================
' IModule.vb - 可训练网络层的最小契约
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow

Namespace Core.Layers

    ''' <summary>
    ''' 带可训练参数的前馈层契约：单输入前向、反向梯度、参数更新。
    ''' </summary>
    Public Interface IModule

        ''' <summary>权重矩阵 [fanIn, fanOut]。</summary>
        ReadOnly Property Weights As Tensor

        ''' <summary>权重梯度，与 <see cref="Weights"/> 同形。</summary>
        ReadOnly Property WeightGrad As Tensor

        ''' <summary>前向传播。</summary>
        Function Forward(input As Tensor) As Tensor

        ''' <summary>反向传播，返回对输入的梯度。</summary>
        Function Backward(gradOutput As Tensor) As Tensor

        ''' <summary>用给定的 Adam 优化器更新自身的可训练参数。</summary>
        Sub Update(optimizer As Adam)

    End Interface

End Namespace
