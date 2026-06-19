' ============================================================================
' IMMO Framework - Neural Network Layers
' ============================================================================
' 基于IMMO算法框架原理实现的神经网络层
' 包含: Parameter, AdamOptimizer, DenseLayer, BatchNormLayer, 
'       DropoutLayer, SwishActivation, LinearActivation
' 依赖: Tensor类 (用户提供)
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace IMMO

    ' ========================================================================
    ' Tensor辅助函数模块
    ' 提供Tensor类未内置的常用矩阵操作
    ' ========================================================================
    Public Module TensorHelpers

        ''' <summary>
        ''' 将[1, out]的偏置加到[batch, out]的矩阵上（逐行广播）
        ''' </summary>
        Public Function AddBias(matrix As Tensor, bias As Tensor) As Tensor
            Dim rows = matrix.Shape(0)
            Dim cols = matrix.Shape(1)
            Dim result = New Tensor(rows, cols)
            For i = 0 To rows - 1
                For j = 0 To cols - 1
                    result(i, j) = matrix(i, j) + bias(0, j)
                Next
            Next
            Return result
        End Function

        ''' <summary>
        ''' 提取2D张量的列切片 [batch, startCol:endCol]
        ''' </summary>
        Public Function ColumnSlice(tensor As Tensor, startCol As Integer, numCols As Integer) As Tensor
            Dim rows = tensor.Shape(0)
            Dim result = New Tensor(rows, numCols)
            For i = 0 To rows - 1
                For j = 0 To numCols - 1
                    result(i, j) = tensor(i, startCol + j)
                Next
            Next
            Return result
        End Function

        ''' <summary>
        ''' 沿列方向拼接多个2D张量
        ''' </summary>
        Public Function ConcatColumns(ParamArray tensors As Tensor()) As Tensor
            Dim rows = tensors(0).Shape(0)
            Dim totalCols = 0
            For Each t In tensors
                totalCols += t.Shape(1)
            Next
            Dim result = New Tensor(rows, totalCols)
            Dim offset = 0
            For Each t In tensors
                For i = 0 To rows - 1
                    For j = 0 To t.Shape(1) - 1
                        result(i, offset + j) = t(i, j)
                    Next
                Next
                offset += t.Shape(1)
            Next
            Return result
        End Function

        ''' <summary>
        ''' 将[batch, 1]的列向量扩展为[batch, targetCols]（每列相同值）
        ''' </summary>
        Public Function ExpandColumn(col As Tensor, targetCols As Integer) As Tensor
            Dim rows = col.Shape(0)
            Dim result = New Tensor(rows, targetCols)
            For i = 0 To rows - 1
                Dim val = col(i, 0)
                For j = 0 To targetCols - 1
                    result(i, j) = val
                Next
            Next
            Return result
        End Function

        ''' <summary>
        ''' [batch, dim] 逐元素乘以 [batch, 1]（广播）
        ''' </summary>
        Public Function MultiplyByColumn(matrix As Tensor, col As Tensor) As Tensor
            Dim rows = matrix.Shape(0)
            Dim cols = matrix.Shape(1)
            Dim result = New Tensor(rows, cols)
            For i = 0 To rows - 1
                Dim val = col(i, 0)
                For j = 0 To cols - 1
                    result(i, j) = matrix(i, j) * val
                Next
            Next
            Return result
        End Function

        ''' <summary>
        ''' [batch, dim] 逐元素除以 [batch, 1]（广播）
        ''' </summary>
        Public Function DivideByColumn(matrix As Tensor, col As Tensor) As Tensor
            Dim rows = matrix.Shape(0)
            Dim cols = matrix.Shape(1)
            Dim result = New Tensor(rows, cols)
            For i = 0 To rows - 1
                Dim val = col(i, 0)
                For j = 0 To cols - 1
                    result(i, j) = matrix(i, j) / (val + 0.000000000001)
                Next
            Next
            Return result
        End Function

        ''' <summary>
        ''' 沿axis=0求均值，返回[1, features]
        ''' </summary>
        Public Function MeanAxis0(tensor As Tensor) As Tensor
            Dim rows = tensor.Shape(0)
            Dim cols = tensor.Shape(1)
            Dim result = New Tensor(1, cols)
            For j = 0 To cols - 1
                Dim sum = 0.0
                For i = 0 To rows - 1
                    sum += tensor(i, j)
                Next
                result(0, j) = sum / rows
            Next
            Return result
        End Function

        ''' <summary>
        ''' 沿axis=1求均值（每行求均值），返回[batch, 1]
        ''' 用于计算每个样本的掩码覆盖率
        ''' </summary>
        Public Function MeanAxis1(tensor As Tensor) As Tensor
            Dim rows = tensor.Shape(0)
            Dim cols = tensor.Shape(1)
            Dim result = New Tensor(rows, 1)
            For i = 0 To rows - 1
                Dim sum = 0.0
                For j = 0 To cols - 1
                    sum += tensor(i, j)
                Next
                result(i, 0) = sum / cols
            Next
            Return result
        End Function

        ''' <summary>
        ''' 沿axis=0求方差（有偏估计），返回[1, features]
        ''' </summary>
        Public Function VarAxis0(tensor As Tensor, mean As Tensor) As Tensor
            Dim rows = tensor.Shape(0)
            Dim cols = tensor.Shape(1)
            Dim result = New Tensor(1, cols)
            For j = 0 To cols - 1
                Dim m = mean(0, j)
                Dim sumSq = 0.0
                For i = 0 To rows - 1
                    Dim diff = tensor(i, j) - m
                    sumSq += diff * diff
                Next
                result(0, j) = sumSq / rows
            Next
            Return result
        End Function

        ''' <summary>
        ''' [batch, features] 减去 [1, features]（广播）
        ''' </summary>
        Public Function SubtractMean(tensor As Tensor, mean As Tensor) As Tensor
            Dim rows = tensor.Shape(0)
            Dim cols = tensor.Shape(1)
            Dim result = New Tensor(rows, cols)
            For i = 0 To rows - 1
                For j = 0 To cols - 1
                    result(i, j) = tensor(i, j) - mean(0, j)
                Next
            Next
            Return result
        End Function

        ''' <summary>
        ''' 逐元素加法: [batch, features] + [1, features]（广播）
        ''' </summary>
        Public Function AddRowVector(tensor As Tensor, rowVec As Tensor) As Tensor
            Dim rows = tensor.Shape(0)
            Dim cols = tensor.Shape(1)
            Dim result = New Tensor(rows, cols)
            For i = 0 To rows - 1
                For j = 0 To cols - 1
                    result(i, j) = tensor(i, j) + rowVec(0, j)
                Next
            Next
            Return result
        End Function

        ''' <summary>
        ''' 逐元素乘法: [batch, features] * [1, features]（广播）
        ''' </summary>
        Public Function MultiplyRowVector(tensor As Tensor, rowVec As Tensor) As Tensor
            Dim rows = tensor.Shape(0)
            Dim cols = tensor.Shape(1)
            Dim result = New Tensor(rows, cols)
            For i = 0 To rows - 1
                For j = 0 To cols - 1
                    result(i, j) = tensor(i, j) * rowVec(0, j)
                Next
            Next
            Return result
        End Function

        ''' <summary>
        ''' 逐元素除法: [batch, features] / [1, features]（广播）
        ''' </summary>
        Public Function DivideRowVector(tensor As Tensor, rowVec As Tensor) As Tensor
            Dim rows = tensor.Shape(0)
            Dim cols = tensor.Shape(1)
            Dim result = New Tensor(rows, cols)
            For i = 0 To rows - 1
                For j = 0 To cols - 1
                    result(i, j) = tensor(i, j) / (rowVec(0, j) + 0.000000000001)
                Next
            Next
            Return result
        End Function

        ''' <summary>
        ''' 计算所有元素的和（Double精度）
        ''' </summary>
        Public Function TotalSumDouble(tensor As Tensor) As Double
            Dim sum = 0.0
            For i = 0 To tensor.Length - 1
                sum += tensor(i)
            Next
            Return sum
        End Function

        ''' <summary>
        ''' 用Double值填充张量
        ''' </summary>
        Public Sub FillTensor(tensor As Tensor, value As Double)
            For i = 0 To tensor.Length - 1
                tensor(i) = value
            Next
        End Sub

        ''' <summary>
        ''' Sigmoid函数
        ''' </summary>
        Public Function Sigmoid(x As Double) As Double
            If x >= 0 Then
                Dim z = std.Exp(-x)
                Return 1.0 / (1.0 + z)
            Else
                Dim z = std.Exp(x)
                Return z / (1.0 + z)
            End If
        End Function

    End Module

    ' ========================================================================
    ' 可训练参数封装类
    ' 包含参数值、梯度、Adam优化器的一阶/二阶矩估计
    ' ========================================================================
    Public Class Parameter

        ''' <summary>参数值张量</summary>
        Public ReadOnly Value As Tensor

        ''' <summary>梯度（与Value同形状）</summary>
        Public ReadOnly Gradient As Tensor

        ''' <summary>Adam一阶矩估计</summary>
        Public ReadOnly M As Tensor

        ''' <summary>Adam二阶矩估计</summary>
        Public ReadOnly V As Tensor

        Public Sub New(value As Tensor)
            Me.Value = value
            Me.Gradient = New Tensor(value.Shape)
            Me.M = New Tensor(value.Shape)
            Me.V = New Tensor(value.Shape)
        End Sub

        ''' <summary>清零梯度</summary>
        Public Sub ZeroGrad()
            For i = 0 To Gradient.Length - 1
                Gradient(i) = 0.0
            Next
        End Sub

        ''' <summary>累加梯度</summary>
        Public Sub AccumulateGradient(grad As Tensor)
            For i = 0 To Gradient.Length - 1
                Gradient(i) += grad(i)
            Next
        End Sub

    End Class

    ' ========================================================================
    ' Adam优化器
    ' 自适应学习率 + 梯度裁剪
    ' ========================================================================
    Public Class AdamOptimizer

        Private _lr As Double
        Private _beta1 As Double
        Private _beta2 As Double
        Private _epsilon As Double
        Private _t As Integer = 0

        Public ReadOnly Property LearningRate As Double
            Get
                Return _lr
            End Get
        End Property

        Public Sub New(learningRate As Double,
                       Optional beta1 As Double = 0.9,
                       Optional beta2 As Double = 0.999,
                       Optional epsilon As Double = 0.00000001)
            _lr = learningRate
            _beta1 = beta1
            _beta2 = beta2
            _epsilon = epsilon
        End Sub

        ''' <summary>
        ''' 学习率衰减
        ''' </summary>
        Public Sub DecayLearningRate(decayRate As Double)
            _lr *= decayRate
        End Sub

        ''' <summary>
        ''' 更新所有参数
        ''' </summary>
        ''' <param name="parameters">所有可训练参数列表</param>
        ''' <param name="gradClipNorm">梯度裁剪范数（0表示不裁剪）</param>
        Public Sub Update(parameters As List(Of Parameter), Optional gradClipNorm As Double = 0.0)
            _t += 1

            ' 计算偏置校正后的有效学习率
            Dim biasCorrection1 = 1.0 - std.Pow(_beta1, _t)
            Dim biasCorrection2 = 1.0 - std.Pow(_beta2, _t)
            Dim correctedLr = _lr * std.Sqrt(biasCorrection2) / biasCorrection1

            ' 全局梯度裁剪（计算所有参数梯度的全局范数）
            If gradClipNorm > 0 Then
                Dim globalNormSq = 0.0
                For Each param In parameters
                    For i = 0 To param.Gradient.Length - 1
                        globalNormSq += param.Gradient(i) * param.Gradient(i)
                    Next
                Next
                Dim globalNorm = std.Sqrt(globalNormSq)
                If globalNorm > gradClipNorm Then
                    Dim scale = gradClipNorm / (globalNorm + 0.00000001)
                    For Each param In parameters
                        For i = 0 To param.Gradient.Length - 1
                            param.Gradient(i) *= scale
                        Next
                    Next
                End If
            End If

            ' Adam参数更新
            For Each param In parameters
                For i = 0 To param.Value.Length - 1
                    Dim g = param.Gradient(i)
                    ' 更新一阶矩
                    param.M(i) = _beta1 * param.M(i) + (1.0 - _beta1) * g
                    ' 更新二阶矩
                    param.V(i) = _beta2 * param.V(i) + (1.0 - _beta2) * g * g
                    ' 偏置校正
                    Dim mHat = param.M(i) / biasCorrection1
                    Dim vHat = param.V(i) / biasCorrection2
                    ' 参数更新
                    param.Value(i) -= correctedLr * mHat / (std.Sqrt(vHat) + _epsilon)
                Next
            Next
        End Sub

    End Class

    ' ========================================================================
    ' 神经网络层基类
    ' ========================================================================
    Public MustInherit Class Layer

        ''' <summary>前向传播</summary>
        ''' <param name="input">输入张量</param>
        ''' <param name="training">是否为训练模式（影响Dropout和BatchNorm行为）</param>
        ''' <returns>输出张量</returns>
        Public MustOverride Function Forward(input As Tensor, training As Boolean) As Tensor

        ''' <summary>反向传播</summary>
        ''' <param name="gradOutput">损失对输出的梯度</param>
        ''' <returns>损失对输入的梯度</returns>
        Public MustOverride Function Backward(gradOutput As Tensor) As Tensor

        ''' <summary>获取所有可训练参数</summary>
        Public MustOverride Function GetParameters() As List(Of Parameter)

        ''' <summary>清零所有参数梯度</summary>
        Public Overridable Sub ZeroGrad()
            For Each param In GetParameters()
                param.ZeroGrad()
            Next
        End Sub

    End Class

    ' ========================================================================
    ''' <summary>
    ''' 全连接层（Dense Layer）
    ''' 前向: Y = X · W + b
    ''' 使用He初始化（适合Swish激活函数）
    ''' </summary>
    Public Class DenseLayer
        Inherits Layer

        Private _weight As Parameter  ' [in_dim, out_dim]
        Private _bias As Parameter    ' [1, out_dim]
        Private _inputCache As Tensor ' 缓存前向传播的输入，用于反向传播

        Public ReadOnly Property InputDim As Integer
        Public ReadOnly Property OutputDim As Integer

        Public Sub New(inputDim As Integer, outputDim As Integer, Optional seed As Integer? = Nothing)
            Me.InputDim = inputDim
            Me.OutputDim = outputDim
            ' He初始化权重
            Dim w = Tensor.HeInit(inputDim, outputDim, seed)
            _weight = New Parameter(w)
            ' 偏置初始化为0
            _bias = New Parameter(New Tensor(1, outputDim))
        End Sub

        Public Overrides Function Forward(input As Tensor, training As Boolean) As Tensor
            _inputCache = input
            ' Y = X · W
            Dim output = input.MatMul(_weight.Value)
            ' Y = Y + b（广播偏置）
            output = TensorHelpers.AddBias(output, _bias.Value)
            Return output
        End Function

        Public Overrides Function Backward(gradOutput As Tensor) As Tensor
            Dim batch = gradOutput.Shape(0)
            Dim outDim = gradOutput.Shape(1)
            Dim inDim = _inputCache.Shape(1)

            ' gradW = X^T · gradY  [in_dim, out_dim]
            Dim inputT = _inputCache.Transpose()
            Dim gradW = inputT.MatMul(gradOutput)
            _weight.AccumulateGradient(gradW)

            ' gradb = sum(gradY, axis=0)  [1, out_dim]
            Dim gradB = New Tensor(1, outDim)
            For j = 0 To outDim - 1
                Dim sum = 0.0
                For i = 0 To batch - 1
                    sum += gradOutput(i, j)
                Next
                gradB(0, j) = sum
            Next
            _bias.AccumulateGradient(gradB)

            ' gradX = gradY · W^T  [batch, in_dim]
            Dim wT = _weight.Value.Transpose()
            Dim gradInput = gradOutput.MatMul(wT)
            Return gradInput
        End Function

        Public Overrides Function GetParameters() As List(Of Parameter)
            Return New List(Of Parameter) From {_weight, _bias}
        End Function

    End Class

    ' ========================================================================
    ''' <summary>
    ''' 批归一化层（Batch Normalization, 1D）
    ''' 前向(训练): Y = gamma * (X - mu_B) / sqrt(var_B + eps) + beta
    ''' 前向(推理): Y = gamma * (X - running_mean) / sqrt(running_var + eps) + beta
    ''' </summary>
    Public Class BatchNormLayer
        Inherits Layer

        Private _gamma As Parameter       ' [1, features]
        Private _beta As Parameter        ' [1, features]
        Private _runningMean As Tensor    ' [1, features] 推理时使用
        Private _runningVar As Tensor     ' [1, features]
        Private _momentum As Double
        Private _epsilon As Double

        ' 反向传播缓存
        Private _inputCache As Tensor
        Private _meanCache As Tensor
        Private _varCache As Tensor
        Private _xHatCache As Tensor
        Private _stdCache As Tensor

        Public ReadOnly Property NumFeatures As Integer

        Public Sub New(numFeatures As Integer,
                       Optional momentum As Double = 0.9,
                       Optional epsilon As Double = 0.00001)
            Me.NumFeatures = numFeatures
            _momentum = momentum
            _epsilon = epsilon
            ' gamma初始化为1, beta初始化为0
            _gamma = New Parameter(Tensor.Ones({1, numFeatures}))
            _beta = New Parameter(New Tensor(1, numFeatures))
            ' 运行均值/方差初始化
            _runningMean = New Tensor(1, numFeatures)
            _runningVar = Tensor.Ones({1, numFeatures})
        End Sub

        Public Overrides Function Forward(input As Tensor, training As Boolean) As Tensor
            _inputCache = input
            Dim batch = input.Shape(0)
            Dim features = input.Shape(1)

            Dim mean, var, xHat As Tensor

            If training Then
                ' 训练模式：使用batch统计量
                mean = TensorHelpers.MeanAxis0(input)
                var = TensorHelpers.VarAxis0(input, mean)

                ' 更新运行统计量
                For j = 0 To features - 1
                    _runningMean(0, j) = _momentum * _runningMean(0, j) + (1.0 - _momentum) * mean(0, j)
                    _runningVar(0, j) = _momentum * _runningVar(0, j) + (1.0 - _momentum) * var(0, j)
                Next

                ' x_hat = (X - mu) / sqrt(var + eps)
                Dim centered = TensorHelpers.SubtractMean(input, mean)
                ' 计算stdTensor = sqrt(var + eps)
                Dim stdTensor = New Tensor(1, features)
                For j = 0 To features - 1
                    stdTensor(0, j) = std.Sqrt(var(0, j) + _epsilon)
                Next
                xHat = TensorHelpers.DivideRowVector(centered, stdTensor)

                ' 缓存用于反向传播
                _meanCache = mean
                _varCache = var
                _xHatCache = xHat
                _stdCache = stdTensor
            Else
                ' 推理模式：使用运行统计量
                Dim centered = TensorHelpers.SubtractMean(input, _runningMean)
                Dim stdTensor = New Tensor(1, features)
                For j = 0 To features - 1
                    stdTensor(0, j) = std.Sqrt(_runningVar(0, j) + _epsilon)
                Next
                xHat = TensorHelpers.DivideRowVector(centered, stdTensor)
            End If

            ' Y = gamma * x_hat + beta
            Dim scaled = TensorHelpers.MultiplyRowVector(xHat, _gamma.Value)
            Dim output = TensorHelpers.AddRowVector(scaled, _beta.Value)
            Return output
        End Function

        Public Overrides Function Backward(gradOutput As Tensor) As Tensor
            Dim batch = gradOutput.Shape(0)
            Dim features = gradOutput.Shape(1)

            ' grad_gamma = sum(gradY * x_hat, axis=0)
            Dim gradGamma = New Tensor(1, features)
            ' grad_beta = sum(gradY, axis=0)
            Dim gradBeta = New Tensor(1, features)

            For j = 0 To features - 1
                Dim sumGamma = 0.0
                Dim sumBeta = 0.0
                For i = 0 To batch - 1
                    sumGamma += gradOutput(i, j) * _xHatCache(i, j)
                    sumBeta += gradOutput(i, j)
                Next
                gradGamma(0, j) = sumGamma
                gradBeta(0, j) = sumBeta
            Next
            _gamma.AccumulateGradient(gradGamma)
            _beta.AccumulateGradient(gradBeta)

            ' grad_x_hat = gradY * gamma
            Dim gradXHat = TensorHelpers.MultiplyRowVector(gradOutput, _gamma.Value)

            ' 简化的BatchNorm反向传播公式:
            ' grad_X = (1 / (N * std)) * (N * grad_x_hat - sum(grad_x_hat) - x_hat * sum(grad_x_hat * x_hat))
            Dim gradInput = New Tensor(batch, features)

            For j = 0 To features - 1
                Dim stdVal = _stdCache(0, j)
                Dim sumGradXHat = 0.0
                Dim sumGradXHatXHat = 0.0
                For i = 0 To batch - 1
                    sumGradXHat += gradXHat(i, j)
                    sumGradXHatXHat += gradXHat(i, j) * _xHatCache(i, j)
                Next
                For i = 0 To batch - 1
                    gradInput(i, j) = (gradXHat(i, j) - sumGradXHat / batch - _xHatCache(i, j) * sumGradXHatXHat / batch) / stdVal
                Next
            Next

            Return gradInput
        End Function

        Public Overrides Function GetParameters() As List(Of Parameter)
            Return New List(Of Parameter) From {_gamma, _beta}
        End Function

    End Class

    ' ========================================================================
    ''' <summary>
    ''' Dropout层
    ''' 训练时以概率(1-p)随机置零，并缩放保留的值
    ''' 推理时不做任何操作
    ''' </summary>
    Public Class DropoutLayer
        Inherits Layer

        Private _rate As Double          ' 丢弃概率
        Private _mask As Tensor          ' 缓存的Dropout掩码
        Private _rng As Random

        Public Sub New(rate As Double, Optional seed As Integer? = Nothing)
            _rate = rate
            _rng = If(seed.HasValue, New Random(seed.Value), New Random())
        End Sub

        Public Overrides Function Forward(input As Tensor, training As Boolean) As Tensor
            If Not training OrElse _rate <= 0 Then
                Return input
            End If

            Dim keepProb = 1.0 - _rate
            Dim result = New Tensor(input.Shape)
            _mask = New Tensor(input.Shape)

            For i = 0 To input.Length - 1
                If _rng.NextDouble() < keepProb Then
                    _mask(i) = 1.0 / keepProb  ' 逆Dropout缩放
                    result(i) = input(i) * _mask(i)
                Else
                    _mask(i) = 0.0
                    result(i) = 0.0
                End If
            Next
            Return result
        End Function

        Public Overrides Function Backward(gradOutput As Tensor) As Tensor
            If _mask Is Nothing Then
                Return gradOutput
            End If
            Dim gradInput = New Tensor(gradOutput.Shape)
            For i = 0 To gradOutput.Length - 1
                gradInput(i) = gradOutput(i) * _mask(i)
            Next
            Return gradInput
        End Function

        Public Overrides Function GetParameters() As List(Of Parameter)
            Return New List(Of Parameter)()
        End Function

    End Class

    ' ========================================================================
    ''' <summary>
    ''' Swish激活函数
    ''' Swish(x) = x * sigmoid(x) = x / (1 + e^(-x))
    ''' 导数: Swish'(x) = sigmoid(x) + x * sigmoid(x) * (1 - sigmoid(x))
    '''                = sigmoid(x) * (1 + x * (1 - sigmoid(x)))
    ''' </summary>
    Public Class SwishActivation
        Inherits Layer

        Private _inputCache As Tensor

        Public Overrides Function Forward(input As Tensor, training As Boolean) As Tensor
            _inputCache = input
            Dim output = New Tensor(input.Shape)
            For i = 0 To input.Length - 1
                Dim x = input(i)
                Dim sig = TensorHelpers.Sigmoid(x)
                output(i) = x * sig
            Next
            Return output
        End Function

        Public Overrides Function Backward(gradOutput As Tensor) As Tensor
            Dim gradInput = New Tensor(gradOutput.Shape)
            For i = 0 To gradOutput.Length - 1
                Dim x = _inputCache(i)
                Dim sig = TensorHelpers.Sigmoid(x)
                Dim deriv = sig * (1.0 + x * (1.0 - sig))
                gradInput(i) = gradOutput(i) * deriv
            Next
            Return gradInput
        End Function

        Public Overrides Function GetParameters() As List(Of Parameter)
            Return New List(Of Parameter)()
        End Function

    End Class

    ' ========================================================================
    ''' <summary>
    ''' 线性激活函数（恒等映射）
    ''' 用于解码器最后一层
    ''' </summary>
    Public Class LinearActivation
        Inherits Layer

        Public Overrides Function Forward(input As Tensor, training As Boolean) As Tensor
            Return input
        End Function

        Public Overrides Function Backward(gradOutput As Tensor) As Tensor
            Return gradOutput
        End Function

        Public Overrides Function GetParameters() As List(Of Parameter)
            Return New List(Of Parameter)()
        End Function

    End Class

    ' ========================================================================
    ''' <summary>
    ''' 顺序容器 - 按顺序执行多个层
    ''' </summary>
    Public Class Sequential
        Inherits Layer

        Private _layers As New List(Of Layer)

        Public ReadOnly Property Layers As List(Of Layer)
            Get
                Return _layers
            End Get
        End Property

        Public Sub Add(layer As Layer)
            _layers.Add(layer)
        End Sub

        Public Overrides Function Forward(input As Tensor, training As Boolean) As Tensor
            Dim current = input
            For Each layer In _layers
                current = layer.Forward(current, training)
            Next
            Return current
        End Function

        Public Overrides Function Backward(gradOutput As Tensor) As Tensor
            Dim current = gradOutput
            ' 反向传播：从最后一层到第一层
            For i = _layers.Count - 1 To 0 Step -1
                current = _layers(i).Backward(current)
            Next
            Return current
        End Function

        Public Overrides Function GetParameters() As List(Of Parameter)
            Dim params As New List(Of Parameter)
            For Each layer In _layers
                params.AddRange(layer.GetParameters())
            Next
            Return params
        End Function

        Public Overrides Sub ZeroGrad()
            For Each layer In _layers
                layer.ZeroGrad()
            Next
        End Sub

    End Class

End Namespace
