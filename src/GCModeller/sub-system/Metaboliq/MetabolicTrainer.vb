Imports Microsoft.VisualBasic.DeepLearning.LiquidNeuralNetwork
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' PINN 风格多目标训练的损失权重与优化超参数
''' </summary>
Public Class MetabolicTrainerConfig

    ''' <summary>λ_data：浓度轨迹拟合项权重</summary>
    Public Property LambdaData As Double = 1.0

    ''' <summary>λ1：质量守恒项 ‖S·v̂‖² 权重</summary>
    Public Property LambdaMass As Double = 1.0

    ''' <summary>λ2：热力学方向性项（不可逆反应通量不得为负）权重</summary>
    Public Property LambdaThermo As Double = 0.5

    ''' <summary>λ3：通量监督项 ‖v̂ − v_MFA‖² 权重（无真值通量时设为 0）</summary>
    Public Property LambdaFlux As Double = 0.1

    ''' <summary>学习率</summary>
    Public Property LearningRate As Double = 0.005

    ''' <summary>训练轮数</summary>
    Public Property Epochs As Integer = 300

    ''' <summary>学习率预热轮数（在前 N 轮线性放大学习率，稳定 ODE 训练初期）</summary>
    Public Property WarmupEpochs As Integer = 20

    ''' <summary>梯度裁剪阈值（按全部参数的全局 L2 范数）</summary>
    Public Property GradientClip As Double = 5.0

    ''' <summary>teacher forcing 初始概率（训练早期用真实浓度覆盖状态）</summary>
    Public Property TeacherForcingStart As Double = 0.9

    ''' <summary>teacher forcing 结束概率（后期切换到自由运行）</summary>
    Public Property TeacherForcingEnd As Double = 0.0

    ''' <summary>是否打印训练进度</summary>
    Public Property Verbose As Boolean = True

    ''' <summary>每隔多少轮打印一次</summary>
    Public Property LogEvery As Integer = 10

    ''' <summary>随机种子（teacher forcing 采样）</summary>
    Public Property Seed As Integer = 123

End Class

''' <summary>
''' 单轮训练的四路损失分解
''' </summary>
Public Class EpochLoss

    Public Property Epoch As Integer
    Public Property Data As Double
    Public Property Mass As Double
    Public Property Thermo As Double
    Public Property Flux As Double

    Public ReadOnly Property Total As Double
        Get
            Return Data + Mass + Thermo + Flux
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"epoch {Epoch,4}: total={Total:F6} (data={Data:F6} mass={Mass:F6} thermo={Thermo:F6} flux={Flux:F6})"
    End Function

End Class

''' <summary>
''' 轻量 Adam 优化器，用于更新不属于 <see cref="LiquidNeuralNetwork"/> 的自持参数
''' （本项目中是通量读取头）
''' </summary>
Public Class AdamOptimizer

    Private ReadOnly _m As New Dictionary(Of String, Tensor)()
    Private ReadOnly _v As New Dictionary(Of String, Tensor)()
    Private _t As Integer = 0

    Public Property LearningRate As Double = 0.005
    Public Property Beta1 As Double = 0.9
    Public Property Beta2 As Double = 0.999
    Public Property Epsilon As Double = 0.00000001

    ''' <summary>
    ''' 按梯度配对列表做一步 Adam 更新
    ''' </summary>
    ''' <param name="pairs">参数-梯度配对</param>
    ''' <param name="learningRateScale">学习率缩放（用于预热）</param>
    Public Sub [Step](pairs As IEnumerable(Of ParameterPair), Optional learningRateScale As Double = 1.0)
        _t += 1

        Dim b1 = std.Pow(Beta1, _t)
        Dim b2 = std.Pow(Beta2, _t)

        For Each pair In pairs
            Dim key = pair.Name

            If Not _m.ContainsKey(key) Then
                _m.Add(key, Tensor.Zeros(pair.Value.Shape))
                _v.Add(key, Tensor.Zeros(pair.Value.Shape))
            End If

            Dim m = _m(key)
            Dim v = _v(key)
            Dim p = pair.Value
            Dim g = pair.Gradient
            Dim lr = LearningRate * learningRateScale

            For i = 0 To p.Length - 1
                m(i) = Beta1 * m(i) + (1 - Beta1) * g(i)
                v(i) = Beta2 * v(i) + (1 - Beta2) * g(i) * g(i)

                Dim mHat = m(i) / (1 - b1)
                Dim vHat = v(i) / (1 - b2)

                p(i) -= lr * mHat / (std.Sqrt(vHat) + Epsilon)
            Next
        Next
    End Sub

    ''' <summary>计算全部梯度的全局 L2 范数</summary>
    Public Shared Function GlobalNorm(pairs As IEnumerable(Of ParameterPair)) As Double
        Dim sq As Double = 0.0

        For Each pair In pairs
            Dim g = pair.Gradient

            For i = 0 To g.Length - 1
                sq += g(i) * g(i)
            Next
        Next

        Return std.Sqrt(sq)
    End Function

    ''' <summary>按全局 L2 范数原地裁剪全部梯度</summary>
    Public Shared Sub ClipGlobal(pairs As IEnumerable(Of ParameterPair), threshold As Double)
        Dim norm = GlobalNorm(pairs)

        If norm <= threshold OrElse norm = 0.0 Then
            Return
        End If

        Dim scale = threshold / norm

        For Each pair In pairs
            Dim g = pair.Gradient

            For i = 0 To g.Length - 1
                g(i) = g(i) * scale
            Next
        Next
    End Sub

    ''' <summary>检测梯度中是否出现 NaN / Inf（ODE 训练中常见的数值故障）</summary>
    Public Shared Function HasNonFinite(pairs As IEnumerable(Of ParameterPair)) As Boolean
        For Each pair In pairs
            Dim g = pair.Gradient

            For i = 0 To g.Length - 1
                If Double.IsNaN(g(i)) OrElse Double.IsInfinity(g(i)) Then
                    Return True
                End If
            Next
        Next

        Return False
    End Function

End Class

''' <summary>
''' 代谢网络的 BPTT 训练器
''' </summary>
''' <remarks>
''' 损失函数（readme 第五节，PINN 风格）：
''' <code>
''' L = λ_data·‖ĉ − c‖²            (仅在观测点计算，天然支持不规则采样与缺失值)
'''   + λ1·‖S·v̂‖²                  (质量守恒 / 稳态软约束)
'''   + λ2·Σ_j max(0, −v_j)²        (热力学方向性：不可逆反应通量非负)
'''   + λ3·‖v̂ − v_MFA‖²             (通量监督，可选)
''' </code>
''' 反向传播分两条路径汇合到隐藏状态：
''' 浓度拟合项经 <c>Liquid.BackwardOutput</c>，通量相关项经 <c>FluxBackward</c>，
''' 两者相加后再交给 <c>Liquid.BackwardLiquid</c> 完成跨时间步的 BPTT。
''' </remarks>
Public Class MetabolicTrainer

#Region "字段与属性"

    Public ReadOnly Property Model As MetabolicLiquidNetwork
    Public ReadOnly Property Config As MetabolicTrainerConfig

    ''' <summary>LNN 侧参数（τ / W / U / b / 门控 / 输出层）的优化器</summary>
    Private ReadOnly _lnnTrainer As LNNTrainer

    ''' <summary>通量读取头参数的优化器</summary>
    Private ReadOnly _adam As AdamOptimizer

    Private ReadOnly _rng As Random
    Private ReadOnly _m As Integer
    Private ReadOnly _r As Integer
    Private ReadOnly _nAll As Integer

#End Region

    Public Sub New(model As MetabolicLiquidNetwork, Optional config As MetabolicTrainerConfig = Nothing)
        Me.Model = model
        Me.Config = If(config, New MetabolicTrainerConfig())

        ' 梯度裁剪统一在本训练器里按"全部参数"做，避免两处裁剪尺度不一致
        _lnnTrainer = New LNNTrainer(model.Liquid, Me.Config.LearningRate) With {
            .OptimizerType = "adam",
            .Verbose = False,
            .UseGradientClipping = False
        }
        _adam = New AdamOptimizer With {.LearningRate = Me.Config.LearningRate}
        _rng = New Random(Me.Config.Seed)

        _m = model.MetaboliteCount
        _r = model.ReactionCount
        _nAll = model.Graph.MetaboliteIds.Length
    End Sub

#Region "训练"

    ''' <summary>
    ''' 在单条件时序数据上训练
    ''' </summary>
    ''' <param name="times">时间网格（可不规则）</param>
    ''' <param name="observed">观测浓度，形状 (T × m)；NaN 表示该点缺失，会被跳过</param>
    ''' <param name="enzymeSeries">酶表达序列，形状 (T × r)</param>
    ''' <param name="boundarySeries">边界代谢物浓度序列，形状 (T × 边界数)</param>
    ''' <param name="observedFlux">真值通量（可选），形状 (T × r)；传入则启用 λ3 通量监督</param>
    ''' <returns>逐轮损失分解</returns>
    Public Function Fit(times As Double(), observed As Tensor, enzymeSeries As Tensor,
                        boundarySeries As Tensor, Optional observedFlux As Tensor = Nothing) As List(Of EpochLoss)
        Dim T = times.Length
        Dim history As New List(Of EpochLoss)()

        If observed.Shape(0) <> T OrElse observed.Shape(1) <> _m Then
            Throw New ArgumentException($"观测浓度形状应为 ({T}, {_m})，实际为 ({observed.Shape(0)}, {observed.Shape(1)})")
        End If

        Dim h0 = Row(observed, 0)

        For epoch = 1 To Config.Epochs
            Dim loss = TrainEpoch(times, observed, enzymeSeries, boundarySeries, observedFlux, h0, epoch)
            history.Add(loss)

            If Config.Verbose AndAlso (epoch Mod Config.LogEvery = 0 OrElse epoch = 1) Then
                Console.WriteLine(loss.ToString())
            End If
        Next

        Return history
    End Function

    Private Function TrainEpoch(times As Double(), observed As Tensor, enzymeSeries As Tensor,
                                boundarySeries As Tensor, observedFlux As Tensor,
                                h0 As Tensor, epoch As Integer) As EpochLoss
        Dim T = times.Length
        Dim liquid = Model.Liquid
        Dim cell = liquid.LiquidLayer.Cells(0)
        Dim m = _m
        Dim r = _r

        ' teacher forcing 概率线性衰减
        Dim progress = std.Min(1.0, (epoch - 1) / std.Max(1.0, Config.Epochs - 1))
        Dim tfProb = Config.TeacherForcingStart + (Config.TeacherForcingEnd - Config.TeacherForcingStart) * progress

        ' ---------- 前向 ----------
        liquid.ResetState()
        liquid.Training = True
        cell.SetState(h0)

        Dim hTrace(T - 1) As Tensor
        Dim outTrace(T - 1) As Tensor
        Dim uTrace(T - 1) As Tensor
        Dim vTrace(T - 1) As Tensor
        Dim missCount As Integer = 0

        Dim lData As Double = 0.0, lMass As Double = 0.0, lThermo As Double = 0.0, lFlux As Double = 0.0

        For t = 0 To T - 1
            Dim u = Model.BuildInput(Row(enzymeSeries, t), Row(boundarySeries, t))
            Dim h = CType(cell.State.Clone(), Tensor)
            Dim out = liquid.ComputeOutputFrom(h)
            Dim v = Model.ComputeFlux(h, u)

            uTrace(t) = u
            hTrace(t) = h
            outTrace(t) = out
            vTrace(t) = v

            ' ---- 浓度拟合项 ----
            Dim obs = Row(observed, t)
            Dim cnt As Integer = 0
            Dim sq As Double = 0.0

            For i = 0 To m - 1
                If Double.IsNaN(obs(i)) Then
                    missCount += 1
                    Continue For
                End If
                Dim d = out(i) - obs(i)
                sq += d * d
                cnt += 1
            Next

            lData += sq / std.Max(1, cnt)

            ' ---- 质量守恒项 ----
            Dim residual = Model.Graph.SteadyStateResidual(v)
            Dim rsq As Double = 0.0
            For i = 0 To _nAll - 1
                rsq += residual(i) * residual(i)
            Next
            lMass += Config.LambdaMass * rsq / _nAll

            ' ---- 热力学方向性 + 通量监督 ----
            Dim fluxObs As Tensor = Nothing
            If observedFlux IsNot Nothing Then
                fluxObs = Row(observedFlux, t)
            End If

            For j = 0 To r - 1
                If Not Model.Graph.Reversible(j) AndAlso v(j) < 0 Then
                    lThermo += Config.LambdaThermo * v(j) * v(j) / r
                End If
                If fluxObs IsNot Nothing AndAlso Not Double.IsNaN(fluxObs(j)) Then
                    Dim df = v(j) - fluxObs(j)
                    lFlux += Config.LambdaFlux * df * df / r
                End If
            Next

            ' ---- teacher forcing：用真实浓度覆盖状态，提升长程稳定性 ----
            If t < T - 1 Then
                If tfProb > 0 AndAlso _rng.NextDouble() < tfProb Then
                    cell.SetState(Row(observed, t))
                End If
                Call liquid.Forward(u, times(t + 1) - times(t))
            End If
        Next

        ' ---------- 反向（逆时间序 BPTT） ----------
        For t = T - 1 To 0 Step -1
            Dim obs = Row(observed, t)

            ' (1) 浓度拟合项对输出的梯度
            Dim dOut = New Tensor(m)
            Dim cnt As Integer = 0
            For i = 0 To m - 1
                If Double.IsNaN(obs(i)) Then Continue For
                cnt += 1
            Next
            Dim scale = Config.LambdaData * 2.0 / (std.Max(1, cnt) * T)

            For i = 0 To m - 1
                If Double.IsNaN(obs(i)) Then
                    dOut(i) = 0.0
                Else
                    dOut(i) = scale * (outTrace(t)(i) - obs(i))
                End If
            Next

            Dim adjH = liquid.BackwardOutput(dOut, hTrace(t), outTrace(t))

            ' (2) 通量相关项对通量的梯度
            Dim adjV = New Tensor(r)
            Dim v = vTrace(t)

            If Config.LambdaMass > 0 Then
                Dim residual = Model.Graph.SteadyStateResidual(v)
                Dim toFlux = Model.Graph.ResidualGradientToFlux(residual)
                Dim ms = Config.LambdaMass * 2.0 / (_nAll * T)

                For j = 0 To r - 1
                    adjV(j) += ms * toFlux(j)
                Next
            End If

            Dim ts = Config.LambdaThermo * 2.0 / (r * T)
            For j = 0 To r - 1
                If Not Model.Graph.Reversible(j) AndAlso v(j) < 0 Then
                    adjV(j) += ts * v(j)
                End If
            Next

            If observedFlux IsNot Nothing AndAlso Config.LambdaFlux > 0 Then
                Dim fluxObs = Row(observedFlux, t)
                Dim fs = Config.LambdaFlux * 2.0 / (r * T)

                For j = 0 To r - 1
                    If Not Double.IsNaN(fluxObs(j)) Then
                        adjV(j) += fs * (v(j) - fluxObs(j))
                    End If
                Next
            End If

            Dim adjHFlux = Model.FluxBackward(adjV, hTrace(t), uTrace(t))

            For i = 0 To m - 1
                adjH(i) += adjHFlux(i)
            Next

            ' (3) 回传液态层（t=0 没有对应的前向步记录）
            If t >= 1 Then
                Call liquid.BackwardLiquid(adjH)
            End If
        Next

        liquid.Training = False

        ' ---------- 更新 ----------
        Return ApplyGradients(epoch, lData / T, lMass / T, lThermo / T, lFlux / T)
    End Function

    ''' <summary>
    ''' 全局梯度裁剪 → Adam 更新 LNN 与通量头 → 清零梯度 → 重新施加结构化掩码
    ''' </summary>
    Private Function ApplyGradients(epoch As Integer, data As Double, mass As Double,
                                    thermo As Double, flux As Double) As EpochLoss
        Dim headPairs = Model.GetFluxHeadPairs()
        Dim lnnPairs = Model.Liquid.GetParameterPairs()
        Dim allPairs = lnnPairs.Concat(headPairs).ToList()

        If AdamOptimizer.HasNonFinite(allPairs) Then
            ' 数值故障：丢弃本轮梯度，避免把参数推到不可恢复的区域
            If Config.Verbose Then
                Console.WriteLine($"epoch {epoch}: 检测到非有限梯度，跳过本轮更新")
            End If

            Model.Liquid.ZeroGradients()
            Model.ZeroFluxGradients()

            Return New EpochLoss With {.Epoch = epoch, .Data = data, .Mass = mass, .Thermo = thermo, .Flux = flux}
        End If

        Call AdamOptimizer.ClipGlobal(allPairs, Config.GradientClip)

        ' 学习率预热
        Dim warmup = std.Min(1.0, epoch / std.Max(1.0, Config.WarmupEpochs))

        _lnnTrainer.LearningRate = Config.LearningRate * warmup
        Call _lnnTrainer.Step()

        _adam.LearningRate = Config.LearningRate
        Call _adam.Step(headPairs, warmup)

        Call Model.ZeroFluxGradients()
        Call Model.ApplyStructuralMasks()

        Return New EpochLoss With {.Epoch = epoch, .Data = data, .Mass = mass, .Thermo = thermo, .Flux = flux}
    End Function

#End Region

#Region "推理"

    ''' <summary>
    ''' 用训练好的模型做自由运行模拟（不做 teacher forcing，不更新参数）
    ''' </summary>
    Public Function Predict(h0 As Tensor, times As Double(), enzymeSeries As Tensor,
                            boundarySeries As Tensor) As MetabolicTrajectory
        Return Model.Simulate(h0, enzymeSeries, boundarySeries, times)
    End Function

#End Region

    Private Function Row(mat As Tensor, r As Integer) As Tensor
        Dim width = mat.Shape(1)
        Dim v = New Tensor(width)

        For j = 0 To width - 1
            v(j) = mat(r, j)
        Next

        Return v
    End Function

End Class
