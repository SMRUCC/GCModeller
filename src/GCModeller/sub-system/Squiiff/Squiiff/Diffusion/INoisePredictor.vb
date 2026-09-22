Imports Microsoft.VisualBasic.MachineLearning.TensorFlow

Namespace Diffusion

    ''' <summary>
    ''' 去噪网络契约 <c>ε_θ(x_t, t, z_sem)</c>。
    '''
    ''' 与标准 DDPM 相同，网络学习预测**被注入的噪声**；不同之处在于它以语义隐变量
    ''' <c>z_sem</c> 为显式条件（经条件批归一化注入），因此反向传播必须同时回传
    ''' 两条梯度：对 <c>x_t</c> 的梯度（用于 DDIM 轨迹）与对 <c>z_sem</c> 的梯度
    ''' （用于训练语义编码器）。
    ''' </summary>
    Public Interface INoisePredictor

        ''' <summary>
        ''' 前向：返回对噪声的预测，形状与 <paramref name="xt"/> 相同；内部缓存反向所需的中间量。
        ''' </summary>
        ''' <param name="xt">加噪后的表达谱 <c>[B,G]</c>。</param>
        ''' <param name="t">逐样本时间步 <c>[B]</c>。</param>
        ''' <param name="zSem">语义条件 <c>[B,dz]</c>。</param>
        ''' <param name="training">True 为训练模式（批归一化使用批统计量），False 为生成模式。</param>
        Function Predict(xt As Tensor, t As Integer(), zSem As Tensor, training As Boolean) As Tensor

        ''' <summary>
        ''' 反向：输入 <c>dL/dε̂</c>，返回 <c>dL/dx_t</c>；
        ''' 并通过 <paramref name="dZsem"/> 输出 <c>dL/dz_sem</c>。
        ''' 网络参数梯度原地累加到各 <see cref="NN.Parameter.Gradient"/>。
        ''' </summary>
        Function Backward(dEps As Tensor, ByRef dZsem As Tensor) As Tensor
    End Interface
End Namespace
