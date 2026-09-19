' ---------------------------------------------------------------------------
' AdamW —— 带解耦权重衰减的 Adam 优化器
'
' 与既有 Transformer\Optimizer.vb（纯 Adam）的唯一差别是权重衰减的施加方式：
'
'   Adam + L2      :  g ← g + wd·θ  再走 Adam 的自适应缩放
'                    → 衰减项被 1/sqrt(v) 缩放，对大梯度维度反而衰减得少，
'                      与"权重越大惩罚越强"的初衷相悖；
'   AdamW（解耦）  :  先按 Adam 更新，再独立地做  θ ← θ − lr·wd·θ
'                    → 衰减量与梯度统计无关，超参之间不再互相纠缠。
'
' 这是 LLM 预训练的事实标准（GPT-3 / Llama / DeepSeek 均用 AdamW）。
'
' 状态组织沿用本仓库的既有约定：每个参数张量配对一份 <see cref="AdamW"/> 实例，
' 实例内部持有与其同形的一阶矩 m、二阶矩 v 与梯度累加器 g；反向阶段只往 g 里
' 原地 +=，训练步结束时统一更新参数并清零 g。
' ---------------------------------------------------------------------------

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace LLM

    ''' <summary>
    ''' AdamW：解耦权重衰减的 Adam 优化器。
    ''' </summary>
    Public Class AdamW

        Private Const Beta1 As Double = 0.9
        Private Const Beta2 As Double = 0.999
        Private Const Eps As Double = 0.00000001

        ''' <summary>一阶矩估计（与参数同形）</summary>
        Private ReadOnly _m As Tensor

        ''' <summary>二阶矩估计（与参数同形）</summary>
        Private ReadOnly _v As Tensor

        ''' <summary>梯度累加器（与参数同形）</summary>
        Private ReadOnly _gradient As Tensor

        ''' <summary>
        ''' 与该参数关联的权重衰减系数。0 表示退化为纯 Adam。
        ''' </summary>
        ''' <remarks>
        ''' 惯例上只对权重矩阵（含嵌入矩阵）施加衰减，不对 RMSNorm 的 γ 与偏置施加，
        ''' 因此这里把它做成"逐参数可配"的。
        ''' </remarks>
        Public Property WeightDecay As Double = 0.0

        ''' <summary>与参数同形的梯度累加器；反向阶段由调用方原地累加。</summary>
        Public ReadOnly Property Gradient As Tensor
            Get
                Return _gradient
            End Get
        End Property

        ''' <summary>按参数张量的形状创建优化器状态。</summary>
        ''' <param name="param">待优化的参数张量（本类只读它的形状）</param>
        ''' <param name="weightDecay">解耦权重衰减系数</param>
        Public Sub New(param As Tensor, Optional weightDecay As Double = 0.0)
            _m = New Tensor(param.Shape)
            _v = New Tensor(param.Shape)
            _gradient = New Tensor(param.Shape)
            Me.WeightDecay = weightDecay
        End Sub

        ''' <summary>把梯度累加器清零。</summary>
        Public Sub ZeroGrad()
            Call LLMTensorOps.ZeroInPlace(_gradient)
        End Sub

        ''' <summary>当前梯度累加器的 L2 范数（用于逐参数的观测诊断）。</summary>
        Public Function GradientNorm() As Double
            Return Tensor.computeKernel.L2Norm(_gradient)
        End Function

        ''' <summary>
        ''' 按 AdamW 规则原地更新参数，并在更新完成后清零梯度累加器。
        ''' </summary>
        ''' <param name="learningRate">学习率</param>
        ''' <param name="step">训练步序号（从 1 开始，用于偏差校正）</param>
        ''' <param name="param">待更新的参数张量</param>
        Public Sub MakeTrainingStep(learningRate As Double, step As Integer, param As Tensor)
            Dim p = param.Data
            Dim g = _gradient.Data
            Dim m = _m.Data
            Dim v = _v.Data

            Dim bc1 = 1.0 - std.Pow(Beta1, step)
            Dim bc2 = 1.0 - std.Pow(Beta2, step)
            Dim wd = WeightDecay

            For i As Integer = 0 To p.Length - 1
                Dim gi = g(i)
                Dim mi = Beta1 * m(i) + (1.0 - Beta1) * gi
                Dim vi = Beta2 * v(i) + (1.0 - Beta2) * gi * gi

                m(i) = mi
                v(i) = vi

                Dim mHat = mi / bc1
                Dim vHat = vi / bc2

                ' Adam 自适应项
                Dim delta = learningRate * mHat / (std.Sqrt(vHat) + Eps)

                ' 解耦权重衰减：与梯度统计完全无关的一项
                If wd > 0 Then delta += learningRate * wd * p(i)

                p(i) -= delta
            Next

            Call param.MarkHostModified()
            Call ZeroGrad()
        End Sub

    End Class

End Namespace
