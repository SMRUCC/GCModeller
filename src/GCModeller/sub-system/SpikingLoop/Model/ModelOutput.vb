' ============================================================================
' ModelOutput.vb — SNN-GRN 一次前向演化的完整输出
'
' 把"脉冲域"与"值域"的中间量一次性打包，供三类使用方各取所需：
'   · 训练器   需要 YHat（预测表达）与 SHistory/ULast（反向传播的接入点）
'   · 诊断/演示 需要 SHistory 画脉冲栅格、需要发放率判断是否静默/持续发放
'   · 扰动引擎  需要 YHat（作为下一决策步的输入估计）与 HLast（跨步保持膜电位）
' ============================================================================

Imports Microsoft.VisualBasic.DeepLearning.SpikingNeuralNetwork
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow

Namespace Model

    ''' <summary>SNN-GRN 单次前向演化的输出</summary>
    Public Class ModelOutput

        ''' <summary>各时间步的输出脉冲 S[t]（长度 = 仿真步数），用于脉冲栅格与发放率</summary>
        Public Property SHistory As List(Of Tensor)

        ''' <summary>末步触发前膜电位 U[T−1]（膜电位读取解码的输入）</summary>
        Public Property ULast As Tensor

        ''' <summary>末步复位后膜电位 H[T−1]（跨决策步保持的神经元状态）</summary>
        Public Property HLast As Tensor

        ''' <summary>喂给解码头的特征（依配置为膜电位 / 发放率 / 二者拼接）</summary>
        Public Property Decoded As Tensor

        ''' <summary>解码头输出 = 预测的表达值（归一化尺度，[batch, N]）</summary>
        Public Property YHat As Tensor

        ''' <summary>时间窗内各基因的脉冲计数 [batch, N]</summary>
        Public Property SpikeCounts As Tensor

        ''' <summary>时间窗内各基因的平均发放率 [batch, N]</summary>
        Public Property FiringRate As Tensor

        ''' <summary>仿真步数</summary>
        Public ReadOnly Property TimeSteps As Integer
            Get
                Return If(SHistory Is Nothing, 0, SHistory.Count)
            End Get
        End Property

        ''' <summary>
        ''' 对最近一次前向做健康检查（全静默 / 过度发放），空列表表示未见异常。
        ''' readme 六列出的两类典型失败模式，在 P2 阶段即可用本方法快速发现。
        ''' </summary>
        Public Function Diagnose() As List(Of String)
            Dim warns As New List(Of String)()
            If SHistory Is Nothing OrElse SHistory.Count = 0 Then Return warns

            Dim total = SpikeDecoders.TotalSpikeCount(SHistory)
            Dim units = SHistory(0).Shape(1)
            Dim batch = SHistory(0).Shape(0)
            Dim rate = total / (SHistory.Count * CDbl(batch) * units)

            If rate <= 0.0 Then
                warns.Add("最近一次仿真完全静默（发放率为 0）：请检查输入电流量级、阈值与先验权重尺度")
            ElseIf rate > 0.5 Then
                warns.Add($"最近一次仿真发放率 {rate:P1} 过高（疑似持续发放）：请检查权重归一化与复位模式")
            End If

            Dim active = SpikeDecoders.ActiveNeurons(SHistory)
            If active.Length > 0 AndAlso active.Length < units / 10 Then
                warns.Add($"仅 {active.Length}/{units} 个基因神经元曾被激活（存活性偏低）")
            End If

            Return warns
        End Function

        Public Overrides Function ToString() As String
            Dim shape = If(YHat Is Nothing, "-", String.Join(",", YHat.Shape))
            Return $"ModelOutput(T={TimeSteps}, yHat=[{shape}], spikes={SpikeDecoders.TotalSpikeCount(SHistory):F0})"
        End Function

    End Class

End Namespace
