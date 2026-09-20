Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.Squiiff.NN
Imports std = System.Math

''' <summary>
''' 有限差分梯度校验。
'''
''' 底层张量库没有自动微分，条件批归一化 / 残差块 / 激活函数的反向传播全部是手写的，
''' 因此必须用**中心差分数值梯度**独立验证解析反向的正确性——这是扩散模型能否训练收敛的
''' 先决条件（若反向有误，损失曲线会毫无意义地停滞或发散）。
'''
''' 校验方式：固定随机权重 <c>w</c>，取标量目标 <c>L = Σ(w ⊙ out)</c>，于是 <c>dL/dOut = w</c>。
''' 用各模块的 <c>Backward(w)</c> 得到解析梯度，再对参数/输入的采样元素做
''' <c>(L(p+h) − L(p−h)) / 2h</c> 与解析值比对。
'''
''' 注意：<c>w</c> 必须在"解析反向"与"数值差分"之间**保持同一份张量**，
''' 否则两侧对应的是不同的损失函数，校验必然失败。
''' </summary>
Module GradientCheck

    ''' <summary>
    ''' 运行全部梯度校验。
    ''' </summary>
    ''' <param name="tolerance">判定通过的最大相对误差阈值。</param>
    ''' <returns>全部通过返回 True。</returns>
    Public Function Run(Optional seed As Integer = 20240920, Optional tolerance As Double = 0.00001) As Boolean
        Dim rng As New Random(seed)

        Console.WriteLine()
        Console.WriteLine(New String("="c, 78))
        Console.WriteLine("  有限差分梯度校验（解析反向 vs 中心差分数值梯度）")
        Console.WriteLine($"  张量计算后端 = {Tensor.computeKernel.Name}    容差 = {tolerance:E1}")
        Console.WriteLine(New String("="c, 78))

        Dim pass As Boolean = True

        pass = CheckLayer("Linear", New Linear("gc.linear", 5, 4), {6, 5}, rng, tolerance) And pass
        pass = CheckLayer("Activation(ReLU)", New ActivationLayer("gc.relu", ActivationKind.ReLU), {6, 5}, rng, tolerance) And pass
        pass = CheckLayer("Activation(SiLU)", New ActivationLayer("gc.silu", ActivationKind.SiLU), {6, 5}, rng, tolerance) And pass
        pass = CheckLayer("Activation(Tanh)", New ActivationLayer("gc.tanh", ActivationKind.Tanh), {6, 5}, rng, tolerance) And pass
        pass = CheckLayer("Activation(Sigmoid)", New ActivationLayer("gc.sigmoid", ActivationKind.Sigmoid), {6, 5}, rng, tolerance) And pass
        pass = CheckLayer("BatchNorm", New BatchNorm("gc.bn", 5), {6, 5}, rng, tolerance) And pass

        Dim seq As New Sequential("gc.seq")
        Call seq.Add(New Linear("gc.seq.fc1", 5, 7))
        Call seq.Add(New ActivationLayer("gc.seq.act", ActivationKind.SiLU))
        Call seq.Add(New BatchNorm("gc.seq.bn", 7))
        Call seq.Add(New Linear("gc.seq.fc2", 7, 4))
        pass = CheckLayer("Sequential(Linear-SiLU-BN-Linear)", seq, {6, 5}, rng, tolerance) And pass

        pass = CheckConditioned("ConditionalBatchNorm", New ConditionalBatchNorm("gc.cbn", 5, 3), {6, 5}, {6, 3}, rng, tolerance) And pass
        pass = CheckResidual("ResidualBlock", New ResidualBlock("gc.res", 5, 3), {6, 5}, {6, 3}, rng, tolerance) And pass

        Console.WriteLine(New String("-"c, 78))
        Console.WriteLine($"  梯度校验总结果: {(If(pass, "全部通过", "存在失败项"))}")
        Console.WriteLine(New String("="c, 78))

        Return pass
    End Function

#Region "校验入口"

    ''' <summary>对 <see cref="LayerModule"/> 做梯度校验（输入与参数）。</summary>
    Private Function CheckLayer(name As String, layer As LayerModule, inputShape As Integer(),
                                rng As Random, tolerance As Double) As Boolean
        Dim x = TensorUtil.StandardNormal(inputShape, rng)

        layer.ZeroGrad()
        Dim out = layer.Forward(x, training:=True)
        Dim w = TensorUtil.StandardNormal(out.Shape, rng)      ' dL/dOut，同时也定义损失 L = Σ(w ⊙ out)
        Dim dx = layer.Backward(w)

        Return NumericCheck(name, layer.Parameters, Function() layer.Forward(x, training:=True),
                            {x}, {dx}, w, rng, tolerance)
    End Function

    ''' <summary>对 <see cref="ConditionalBatchNorm"/> 做梯度校验（输入、条件向量与参数）。</summary>
    Private Function CheckConditioned(name As String, norm As ConditionalBatchNorm,
                                      inputShape As Integer(), condShape As Integer(),
                                      rng As Random, tolerance As Double) As Boolean
        Dim x = TensorUtil.StandardNormal(inputShape, rng)
        Dim cond = TensorUtil.StandardNormal(condShape, rng)

        Dim out = norm.Forward(x, cond, training:=True)
        Dim w = TensorUtil.StandardNormal(out.Shape, rng)

        Dim dCond As Tensor = Nothing
        Dim dx = norm.Backward(w, dCond)

        Return NumericCheck(name, norm.Parameters, Function() norm.Forward(x, cond, training:=True),
                            {x, cond}, {dx, dCond}, w, rng, tolerance)
    End Function

    ''' <summary>对 <see cref="ResidualBlock"/> 做梯度校验（输入、条件向量与参数）。</summary>
    Private Function CheckResidual(name As String, block As ResidualBlock,
                                   inputShape As Integer(), condShape As Integer(),
                                   rng As Random, tolerance As Double) As Boolean
        Dim x = TensorUtil.StandardNormal(inputShape, rng)
        Dim cond = TensorUtil.StandardNormal(condShape, rng)

        Dim out = block.Forward(x, cond, training:=True)
        Dim w = TensorUtil.StandardNormal(out.Shape, rng)

        Dim dCond As Tensor = Nothing
        Dim dx = block.Backward(w, dCond)

        Return NumericCheck(name, block.Parameters, Function() block.Forward(x, cond, training:=True),
                            {x, cond}, {dx, dCond}, w, rng, tolerance)
    End Function

#End Region

#Region "校验引擎"

    ''' <summary>
    ''' 数值梯度校验引擎：对固定损失 <c>L = Σ(w ⊙ out)</c> 校验参数梯度与输入梯度。
    ''' </summary>
    ''' <param name="parameters">待校验的可训练参数。</param>
    ''' <param name="forward">按当前参数/输入值重新计算输出的函数。</param>
    ''' <param name="inputs">参与校验的输入张量（第 0 个通常为主输入）。</param>
    ''' <param name="analyticInputGrads">各输入的解析梯度（与 <paramref name="inputs"/> 一一对应）。</param>
    ''' <param name="dOut">上游梯度，必须与解析反向时传入的是同一张量。</param>
    Private Function NumericCheck(name As String, parameters As IEnumerable(Of Parameter),
                                  forward As Func(Of Tensor),
                                  inputs As Tensor(), analyticInputGrads As Tensor(),
                                  dOut As Tensor, rng As Random, tolerance As Double) As Boolean
        Dim lossFunc As Func(Of Double) = Function() WeightedSum(forward(), dOut)

        ' 1) 先固化解析梯度（后续数值扰动会重新前向，避免读到被覆盖的状态）
        Dim analyticParams As New List(Of Parameter)
        Dim analyticValues As New List(Of Double())
        For Each p In parameters
            If p Is Nothing Then Continue For

            analyticParams.Add(p)
            analyticValues.Add(DirectCast(p.Gradient.Data.Clone(), Double()))
        Next

        Dim maxError As Double = 0.0
        Dim ok As Boolean = True

        ' 2) 参数梯度
        For i As Integer = 0 To analyticParams.Count - 1
            Dim p = analyticParams(i)
            Dim analytic = analyticValues(i)

            For Each index In SampleIndices(p.Value.Length, 8, rng)
                Dim numeric = NumericGradient(p.Value, index, lossFunc)
                Dim error1 = RelativeError(analytic(index), numeric)
                maxError = std.Max(maxError, error1)

                If error1 > tolerance Then
                    ok = False
                    Console.WriteLine($"    [FAIL] {name} · {p.Name}[{index}]  解析={analytic(index):E3}  数值={numeric:E3}  误差={error1:E2}")
                End If
            Next
        Next

        ' 3) 输入梯度
        For t As Integer = 0 To std.Min(inputs.Length, analyticInputGrads.Length) - 1
            Dim inputTensor = inputs(t)
            Dim analytic = analyticInputGrads(t)
            If analytic Is Nothing Then Continue For

            For Each index In SampleIndices(inputTensor.Length, 8, rng)
                Dim numeric = NumericGradient(inputTensor, index, lossFunc)
                Dim error1 = RelativeError(analytic.Data(index), numeric)
                maxError = std.Max(maxError, error1)

                If error1 > tolerance Then
                    ok = False
                    Console.WriteLine($"    [FAIL] {name} · dInput{t}[{index}]  解析={analytic.Data(index):E3}  数值={numeric:E3}  误差={error1:E2}")
                End If
            Next
        Next

        Console.WriteLine($"  [{(If(ok, "PASS", "FAIL"))}] {name,-40} 最大相对误差 = {maxError:E2}")
        Return ok
    End Function

    ''' <summary>标量损失 <c>L = Σ(w ⊙ out)</c>。</summary>
    Private Function WeightedSum(out As Tensor, w As Tensor) As Double
        Return Tensor.computeKernel.SumAll(out.ElementwiseMultiply(w))
    End Function

    ''' <summary>对张量某元素做中心差分数值梯度。</summary>
    Private Function NumericGradient(tensor As Tensor, index As Integer, loss As Func(Of Double)) As Double
        Dim data = tensor.Data
        Dim original = data(index)
        Dim h = 0.00001 * std.Max(1.0, std.Abs(original))

        Try
            data(index) = original + h
            Call tensor.MarkHostModified()
            Dim plus = loss()

            data(index) = original - h
            Call tensor.MarkHostModified()
            Dim minus = loss()

            Return (plus - minus) / (2.0 * h)
        Finally
            data(index) = original
            Call tensor.MarkHostModified()
        End Try
    End Function

    ''' <summary>从 [0, size) 中采样若干互不相同的下标（避免全量校验过慢）。</summary>
    Private Function SampleIndices(size As Integer, count As Integer, rng As Random) As Integer()
        If size <= 0 Then Return New Integer() {}

        Dim n = std.Min(size, count)
        Dim pool As New List(Of Integer)(size)
        For i As Integer = 0 To size - 1
            pool.Add(i)
        Next

        ' Fisher-Yates 部分洗牌
        For i As Integer = 0 To n - 1
            Dim j = rng.Next(i, pool.Count)
            Dim tmp = pool(i)
            pool(i) = pool(j)
            pool(j) = tmp
        Next

        Dim result(n - 1) As Integer
        For i As Integer = 0 To n - 1
            result(i) = pool(i)
        Next
        Return result
    End Function

    ''' <summary>相对误差（分母取 1 与两值绝对值的较大者，避免梯度接近 0 时误判）。</summary>
    Private Function RelativeError(analytic As Double, numeric As Double) As Double
        Dim denom = std.Max(1.0, std.Max(std.Abs(analytic), std.Abs(numeric)))
        Return std.Abs(analytic - numeric) / denom
    End Function

#End Region
End Module
