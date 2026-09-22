' ============================================================================
' SNNGRNModel.vb — SNN-GRN 模型本体（readme 二 · 模型定义模块）
'
' 组装关系（对应 readme 〇 的"SNN-GRN 模型核心"）：
'
'   基因 = LIF 神经元        →  RecurrentLIFLayer(N)，W[pre,post] 为 TF→Target 有向突触
'   TF-Target = 有向突触     →  Weight 由 PriorGraph.WInit 初始化
'   WGCNA = 结构先验         →  Mask 冻结先验之外的位置 + W₀ 参与结构正则
'   输入编码                 →  SpikeEncoders.DirectCurrentEncode（表达量 → T 步持续电流）
'   输出解码                 →  SpikeDecoders（膜电位 / 发放率 / 拼接）+ LinearReadout
'   训练                     →  代理梯度 + BPTT（RecurrentLIFLayer.BackwardTime）
'
' 一次前向（readme 二.1 的 FORWARD）：
'   I_ext = ENCODE(U_seq[t])                     归一化表达状态 → 持续输入电流
'   u0    = MembraneInitGain · U_seq[t]          表达式状态映射为初始膜电位
'   S, u  = LIF_FORWARD(I_ext, u0, W, A)         递归演化 SimulationSteps 步
'   y_hat = DECODE(S, u)                         解码头读出预测表达
'
' 一次反向（readme 三.2 的 Step 5 + 3.3）：
'   dDecoded = Readout.BackwardTime(dYHat)                 ← 解码头
'   依解码方式把 dDecoded 折算为 dU_last（膜电位路径）与/或 dS[t]（发放率路径）
'   grads = Layer.BackwardTime(dS, dNothing, dU_last)      ← 代理梯度 BPTT
'
' 解码方式对反向路径的影响（readme 二.4 与三.3 的关键差异）：
'   膜电位读取：梯度只经末步膜电位回传 —— 路径最短、最平滑，回归任务首选。
'   发放率解码：梯度均匀分摊到每个时间步的脉冲（dS[t] = dRate/T）——
'               脉冲稀疏时极易出现"整段梯度为 0"的死亡神经元。
'   拼接解码：  两条路径并存，兼顾瞬时状态与发放统计。
' ============================================================================

Imports Microsoft.VisualBasic.DeepLearning.SpikingNeuralNetwork
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.SpikingLoop.Graph
Imports std = System.Math

Namespace Model

    ''' <summary>基于 LIF 脉冲神经网络的基因表达调控网络模型</summary>
    Public Class SNNGRNModel

#Region "组成"

        Public ReadOnly Property Config As SpikingLoopConfig

        ''' <summary>先验调控图（决定突触拓扑、初始权重与可训练掩码）</summary>
        Public ReadOnly Property Graph As PriorGraph

        ''' <summary>基因神经元群：递归 LIF 层（可训练、BPTT）</summary>
        Public ReadOnly Property Layer As RecurrentLIFLayer

        ''' <summary>回归解码头：脉冲状态 → 连续表达值</summary>
        Public ReadOnly Property Readout As LinearReadout

        ''' <summary>基因数量 = 神经元数量</summary>
        Public ReadOnly Property NumGenes As Integer
            Get
                Return Graph.Units
            End Get
        End Property

#End Region

        Public Sub New(graph As PriorGraph, config As SpikingLoopConfig)
            If graph Is Nothing Then
                Throw New ArgumentNullException(NameOf(graph))
            End If
            If config Is Nothing Then
                Throw New ArgumentNullException(NameOf(config))
            End If

            Me.Config = config
            Me.Graph = graph

            ' He 初始化的种子固定，保证"先验图 + 配置"可完全复现；
            ' 权重随后被先验 W_init 覆盖，因此这里只是给 Tensor 一个合法的初值。
            Me.Layer = New RecurrentLIFLayer(
                "GRN",
                graph.Units,
                graph.WInit,
                graph.Mask,
                config.Beta,
                config.Threshold,
                config.ResetMode,
                config.Surrogate,
                config.Alpha,
                config.Seed)

            Me.Layer.Trainable = Not config.FrozenWeights

            Dim readoutIn = ReadoutInputSize(graph.Units, config.ReadoutSource)
            Dim initWeight = If(config.IdentityReadoutInit AndAlso readoutIn = graph.Units,
                                Tensor.Identity(graph.Units),
                                Nothing)

            Me.Readout = New LinearReadout("readout", readoutIn, graph.Units, initWeight, Nothing, config.Seed)
        End Sub

        Private Shared Function ReadoutInputSize(units As Integer, source As ReadoutSource) As Integer
            Select Case source
                Case ReadoutSource.Concat
                    Return units * 2
                Case Else
                    Return units
            End Select
        End Function

#Region "编码"

        ''' <summary>
        ''' 输入编码（readme 二.3，mode = "current"）：把表达状态编码为
        ''' <c>SimulationSteps</c> 个时间步的持续输入电流。
        ''' </summary>
        ''' <param name="expression">归一化表达状态 [batch, N]</param>
        Public Function EncodeCurrents(expression As Tensor) As List(Of Tensor)
            Return SpikeEncoders.DirectCurrentEncode(expression, Config.SimulationSteps, Config.CurrentGain)
        End Function

        ''' <summary>把表达状态映射为初始膜电位 u0 = gain · U_seq[t]（readme 三.2 的 MAP_TO_MEMBRANE）</summary>
        Public Function MapToMembrane(expression As Tensor) As Tensor
            If Config.MembraneInitGain = 1.0 Then Return expression
            Return expression * CSng(Config.MembraneInitGain)
        End Function

#End Region

#Region "前向"

        ''' <summary>
        ''' 前向演化：编码后的输入电流序列 → 递归 LIF → 解码头。
        ''' </summary>
        ''' <param name="currents">输入电流序列（通常来自 <see cref="EncodeCurrents"/>）</param>
        ''' <param name="u0">初始膜电位 [batch, N]；为 Nothing 时由调用方保证从零开始</param>
        ''' <param name="decode">是否执行解码（仅做脉冲诊断时可设为 False 以省去一次矩阵乘）</param>
        Public Function Forward(currents As List(Of Tensor),
                                Optional u0 As Tensor = Nothing,
                                Optional decode As Boolean = True) As ModelOutput

            Dim sHist = Layer.ForwardSequence(currents, u0)

            Dim output As New ModelOutput With {
                .SHistory = sHist,
                .ULast = Layer.ULast,
                .HLast = Layer.HLast,
                .SpikeCounts = SpikeDecoders.SpikeCounts(sHist),
                .FiringRate = SpikeDecoders.FiringRate(sHist)
            }

            If decode Then
                Dim features = BuildReadoutFeatures(output)
                output.Decoded = features
                output.YHat = Readout.Forward(features)
            End If

            Return output
        End Function

        ''' <summary>
        ''' 单步预测（readme 三.1 的自回归式单步预测）：用 t 时刻的表达状态预测 t+Δt 时刻的表达。
        ''' </summary>
        ''' <param name="expression">当前表达状态 [batch, N]（归一化尺度）</param>
        Public Function PredictNext(expression As Tensor) As Tensor
            Dim currents = EncodeCurrents(expression)
            Dim output = Forward(currents, MapToMembrane(expression))
            Return output.YHat
        End Function

        ''' <summary>按配置的解码方式构造解码头输入特征</summary>
        Private Function BuildReadoutFeatures(output As ModelOutput) As Tensor
            Select Case Config.ReadoutSource
                Case ReadoutSource.FiringRate
                    Return output.FiringRate
                Case ReadoutSource.Concat
                    Return SpikeDecoders.ConcatRateAndPotential(output.FiringRate, output.ULast)
                Case Else ' MembranePotential
                    Return output.ULast
            End Select
        End Function

#End Region

#Region "反向"

        ''' <summary>
        ''' 反向传播：给定对预测表达的梯度 dL/dYHat，回传至突触权重（累积到 <see cref="Layer"/> 与
        ''' <see cref="Readout"/> 的 Grad 属性中，不更新参数）。
        ''' </summary>
        ''' <param name="dYHat">上游梯度 [batch, N]</param>
        ''' <returns>递归 LIF 层的 BPTT 梯度结果（含未乘掩码的 dW）</returns>
        Public Function Backward(dYHat As Tensor) As RecurrentGradients
            If Layer.TimeSteps = 0 Then
                Throw New InvalidOperationException("尚未执行前向演化，无法反向传播")
            End If

            ' ---- 解码头：dL/d(decoded features) ----
            Dim dFeatures = Readout.BackwardTime(dYHat)

            ' ---- 依解码方式把 dFeatures 折算回膜电位 / 脉冲梯度 ----
            Dim steps = Layer.TimeSteps
            Dim dS As List(Of Tensor) = Nothing
            Dim dUlast As Tensor = Nothing

            Select Case Config.ReadoutSource
                Case ReadoutSource.FiringRate
                    dS = DistributeRateGradient(dFeatures, steps)

                Case ReadoutSource.Concat
                    Dim split = SplitConcatGradient(dFeatures, NumGenes)
                    dS = DistributeRateGradient(split.rate, steps)
                    dUlast = split.potential

                Case Else ' MembranePotential
                    dUlast = dFeatures
            End Select

            If dS Is Nothing Then
                dS = Layer.EmptySpikeGradients(steps)
            End If

            Return Layer.BackwardTime(dS, Nothing, dUlast)
        End Function

        ''' <summary>
        ''' 发放率解码的梯度分摊：rate = (Σ_t S[t]) / T  →  dS[t] = dRate / T。
        ''' </summary>
        Private Shared Function DistributeRateGradient(dRate As Tensor, steps As Integer) As List(Of Tensor)
            Dim scale = CSng(1.0 / steps)
            Dim list As New List(Of Tensor)()
            For t = 1 To steps
                list.Add(dRate * scale)
            Next
            Return list
        End Function

        ''' <summary>把拼接解码的梯度沿特征维切回（发放率梯度, 膜电位梯度）</summary>
        Private Shared Function SplitConcatGradient(d As Tensor, rateDim As Integer) As (rate As Tensor, potential As Tensor)
            Dim batch = d.Shape(0)
            Dim total = d.Shape(1)
            Dim potDim = total - rateDim

            If potDim <= 0 Then
                Throw New ArgumentException(
                    $"拼接解码的梯度特征维({total})应大于发放率维度({rateDim})")
            End If

            Dim dRate As New Tensor(batch, rateDim)
            Dim dPot As New Tensor(batch, potDim)
            Dim dd = d.Data

            For b = 0 To batch - 1
                Array.Copy(dd, b * total, dRate.Data, b * rateDim, rateDim)
                Array.Copy(dd, b * total + rateDim, dPot.Data, b * potDim, potDim)
            Next

            ' 绕过索引器就地写入：声明主机数据已修改
            dRate.MarkHostModified()
            dPot.MarkHostModified()

            Return (dRate, dPot)
        End Function

#End Region

#Region "参数更新辅助"

        ''' <summary>训练后的突触权重（[N, N]，先验之外的位置恒为 0）</summary>
        Public ReadOnly Property LearnedWeights As Tensor
            Get
                Return Layer.Weight
            End Get
        End Property

        ''' <summary>已乘结构掩码的突触权重梯度（先验之外的位置梯度归零）</summary>
        Public Function MaskedWeightGrad() As Tensor
            Return Layer.MaskedWeightGrad()
        End Function

        ''' <summary>清空所有权重梯度（每个训练步开始前调用）</summary>
        Public Sub ZeroGrad()
            Layer.WeightGrad = New Tensor(NumGenes, NumGenes)
            Readout.ZeroGrad()
        End Sub

        ''' <summary>把突触权重裁剪回结构掩码约束</summary>
        Public Sub ApplyMask()
            Layer.ApplyMaskToWeight()
        End Sub

        ''' <summary>
        ''' 自适应替代梯度形状（readme 3.3）：按倍率调整解码头之外的替代梯度陡度 α。
        ''' </summary>
        ''' <param name="growth">每轮乘性增长因子（1.0 = 不变）</param>
        Public Sub AnnealAlpha(growth As Double)
            If growth = 1.0 Then Return
            Layer.Alpha = std.Min(Config.AlphaMax, Layer.Alpha * growth)
        End Sub

        ''' <summary>当前替代梯度陡度 α</summary>
        Public ReadOnly Property Alpha As Double
            Get
                Return Layer.Alpha
            End Get
        End Property

#End Region

        Public Overrides Function ToString() As String
            Return $"SNNGRNModel(N={NumGenes}, readout={Config.ReadoutSource}, " &
                   $"T_sim={Config.SimulationSteps}, β={Config.Beta}, θ={Config.Threshold}, " &
                   $"synapses={Graph.NumSynapses})"
        End Function

    End Class

End Namespace
