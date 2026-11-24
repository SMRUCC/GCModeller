' ============================================================================
' Adam.vb - 统一的 Adam 优化器
'
' 消除三处重复实现：
'   GraphVAE.vb L376-409  (LinearLayer.Update)
'   GraphVAE.vb L498-519  (GCNLayer.Update)
'   GMVAE 的裸 SGD      (GMVAE.vb L841-845 updateTensor)
' 数学与原实现完全等价（含偏差校正）。
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace Core.Layers

    ''' <summary>
    ''' Adam 优化器，一个实例对应一个参数张量。
    ''' </summary>
    Public Class Adam

        Private ReadOnly _m As Tensor
        Private ReadOnly _v As Tensor
        Private ReadOnly _learningRate As Double
        Private _step As Integer = 0

        ''' <summary>一阶矩衰减率。</summary>
        Public Property Beta1 As Double = 0.9

        ''' <summary>二阶矩衰减率。</summary>
        Public Property Beta2 As Double = 0.999

        ''' <summary>数值稳定项。</summary>
        Public Property Epsilon As Double = 0.00000001

        Public Sub New(shape As Integer(), Optional learningRate As Double = 0.001)
            _m = Tensor.Zeros(shape)
            _v = Tensor.Zeros(shape)
            _learningRate = learningRate
        End Sub

        ''' <summary>构造时指定的默认学习率。</summary>
        Public ReadOnly Property LearningRate As Double
            Get
                Return _learningRate
            End Get
        End Property

        ''' <summary>已经执行过的更新步数（用于偏差校正）。</summary>
        Public ReadOnly Property StepCount As Integer
            Get
                Return _step
            End Get
        End Property

        ''' <summary>
        ''' 执行一步更新：param -= lr · m̂ / (√v̂ + ε)。
        ''' </summary>
        ''' <param name="param">需要更新的参数（原地修改）</param>
        ''' <param name="grad">对应的梯度，必须与参数同形</param>
        ''' <param name="learningRate">本步学习率；为负时使用构造时的默认值</param>
        Public Sub Update(param As Tensor, grad As Tensor, Optional learningRate As Double = -1)
            If param Is Nothing OrElse grad Is Nothing Then
                Return
            End If

            If param.Length <> grad.Length Then
                Throw New ArgumentException(
                    $"Adam 更新要求参数与梯度同形：参数 {param.Length}，梯度 {grad.Length}")
            End If

            Dim lr As Double = If(learningRate < 0, _learningRate, learningRate)

            _step += 1

            Dim correction1 As Double = 1.0 - std.Pow(Beta1, _step)
            Dim correction2 As Double = 1.0 - std.Pow(Beta2, _step)

            Dim parameter As Double() = param.Data
            Dim gradient As Double() = grad.Data
            Dim firstMoment As Double() = _m.Data
            Dim secondMoment As Double() = _v.Data

            For i As Integer = 0 To parameter.Length - 1
                Dim g As Double = gradient(i)

                If Double.IsNaN(g) OrElse Double.IsInfinity(g) Then
                    Continue For
                End If

                firstMoment(i) = Beta1 * firstMoment(i) + (1.0 - Beta1) * g
                secondMoment(i) = Beta2 * secondMoment(i) + (1.0 - Beta2) * g * g

                Dim mHat As Double = firstMoment(i) / correction1
                Dim vHat As Double = secondMoment(i) / correction2

                parameter(i) -= lr * mHat / (std.Sqrt(vHat) + Epsilon)
            Next

            _m.MarkHostModified()
            _v.MarkHostModified()
            param.MarkHostModified()
        End Sub

        ''' <summary>清空矩估计与步数（重新训练前调用）。</summary>
        Public Sub Reset()
            _step = 0
            Array.Clear(_m.Data, 0, _m.Data.Length)
            Array.Clear(_v.Data, 0, _v.Data.Length)
        End Sub

        Public Overrides Function ToString() As String
            Return $"Adam(step={_step}, beta1={Beta1}, beta2={Beta2}, eps={Epsilon})"
        End Function

    End Class

End Namespace
