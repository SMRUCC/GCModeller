' ============================================================
' SignalTransductionNetwork.vb - 信号转导与细胞周期
' ============================================================
' 双组分系统（two-component system）式的磷酸化级联：
'
'   胞外刺激 u_i ──(自磷酸化)──▶ 传感器激酶 Sp_i ──(磷酸转移)──▶ 响应调节因子 Rp_i
'
'     d(Sp_i)/dt = k_a · u_i · (1 − Sp_i) − k_d · Sp_i
'     d(Rp_i)/dt = k_t · Sp_i · (1 − Rp_i) − k_p · Rp_i
'
' Rp_i 就是转录因子 i 的活化水平，直接作为转录调控网络（GEARS）的扰动
' 信号来源 —— 这也是本模块与 GRN 的唯一耦合点。
'
' 细胞周期部分用一个松弛振荡器给出周期相位：
'
'     d(Cyc)/dt  = k_syn · mean(Rp) − k_deg · Cyc
'     d(Phase)/dt = ω · Cyc
'
' 状态向量 = [Sp_0 … Sp_{k-1}, Rp_0 … Rp_{k-1}, Cyc, Phase]，共 2k+2 维。
' ============================================================

Imports Microsoft.VisualBasic.Math.Sundials.CVODE

''' <summary>
''' 胞外信号 → 磷酸化级联 → 转录因子活性 与 细胞周期相位
''' </summary>
Public Class SignalTransductionNetwork : Inherits OdeSubNetwork

    ReadOnly kAuto As Double
    ReadOnly kDephos As Double
    ReadOnly kTransfer As Double
    ReadOnly kRegulatorDephos As Double
    ReadOnly kCyclinSyn As Double
    ReadOnly kCyclinDeg As Double
    ReadOnly omega As Double

    ''' <summary>信号通道数</summary>
    ReadOnly channels As Integer

    ''' <summary>信号通道 → 环境代谢物（刺激物）名称</summary>
    ReadOnly stimuliNames As String()

    ''' <summary>本步内视为常量的胞外刺激强度</summary>
    Private stimuli As Double()

    ''' <summary>信号通道 → 状态池 Signal 的槽位</summary>
    ReadOnly signalSlot As Integer()

    Public Sub New(cell As VirtualCella, blueprint As CellaBlueprint)
        ' 状态 = Sp(k) + Rp(k) + Cyc + Phase
        Call MyBase.New(cell, cell.State.NSignal * 2 + 2, NameOf(SignalTransductionNetwork))

        channels = cell.State.NSignal

        kAuto = blueprint.KinaseAutophosphorylation
        kDephos = blueprint.KinaseDephosphorylation
        kTransfer = blueprint.PhosphoTransferRate
        kRegulatorDephos = blueprint.RegulatorDephosphorylation
        kCyclinSyn = blueprint.CyclinSynthesis
        kCyclinDeg = blueprint.CyclinDegradation
        omega = blueprint.CycleAngularVelocity

        Dim names As String() = cell.State.SignalNames

        stimuliNames = New String(channels - 1) {}
        signalSlot = New Integer(channels - 1) {}
        stimuli = New Double(channels - 1) {}

        For i As Integer = 0 To channels - 1
            Dim stim As String = Nothing

            If blueprint.ExternalStimuli IsNot Nothing AndAlso
                blueprint.ExternalStimuli.TryGetValue(names(i), stim) Then

                stimuliNames(i) = stim
            Else
                stimuliNames(i) = Nothing
            End If

            signalSlot(i) = i
        Next

        Dim initial As Double() = New Double(n - 1) {}

        ' 传感器激酶与响应调节因子都从零开始，相位从零开始
        Call InitializeFrom(initial)
    End Sub

    ''' <summary>
    ''' 从所在格点的胞外环境读取刺激强度
    ''' </summary>
    Private Sub RefreshStimuli()
        Dim medium As Dictionary(Of String, Double) = If(cell.Spot Is Nothing, Nothing, cell.Spot.Medium)

        For i As Integer = 0 To channels - 1
            Dim name As String = stimuliNames(i)

            If name Is Nothing OrElse medium Is Nothing Then
                stimuli(i) = 0.5
                Continue For
            End If

            Dim level As Double = 0.0

            If medium.TryGetValue(name, level) Then
                ' 环境浓度 → [0,1] 的刺激强度
                stimuli(i) = 1.0 / (1.0 + System.Math.Exp(-level))
            Else
                stimuli(i) = 0.5
            End If
        Next
    End Sub

    Protected Overrides Sub RHS(t As Double, y As NVector, ydot As NVector)
        Dim rpSum As Double = 0.0

        For i As Integer = 0 To channels - 1
            Dim sp As Double = y(i)
            Dim rp As Double = y(channels + i)
            Dim u As Double = stimuli(i)

            ydot(i) = kAuto * u * (1.0 - sp) - kDephos * sp
            ydot(channels + i) = kTransfer * sp * (1.0 - rp) - kRegulatorDephos * rp

            rpSum += rp
        Next

        Dim cyc As Double = y(2 * channels)

        ydot(2 * channels) = kCyclinSyn * (rpSum / System.Math.Max(1, channels)) - kCyclinDeg * cyc
        ydot(2 * channels + 1) = omega * cyc
    End Sub

    Protected Overrides Sub Jacobian(t As Double, y As NVector, fy As NVector, J As DenseMatrix)
        For i As Integer = 0 To n - 1
            For j As Integer = 0 To n - 1
                J(i, j) = 0.0
            Next
        Next

        For i As Integer = 0 To channels - 1
            Dim sp As Double = y(i)
            Dim rp As Double = y(channels + i)
            Dim u As Double = stimuli(i)

            ' Sp 方程
            J(i, i) = -kAuto * u - kDephos
            ' Rp 方程
            J(channels + i, i) = kTransfer * (1.0 - rp)
            J(channels + i, channels + i) = -kTransfer * sp - kRegulatorDephos
            ' 周期蛋白方程对 Rp 的偏导
            J(2 * channels, channels + i) = kCyclinSyn / System.Math.Max(1, channels)
        Next

        J(2 * channels, 2 * channels) = -kCyclinDeg
        J(2 * channels + 1, 2 * channels) = omega
    End Sub

    Public Overrides Sub Tick(dt As Double)
        Call RefreshStimuli()

        Dim rollback As Double() = CurrentState()

        If Not Advance(dt) Then
            Call ResetState(rollback)
            Return
        End If

        Dim result As Double() = CurrentState()
        Dim state As CellularState = cell.State

        For i As Integer = 0 To channels - 1
            Dim activity As Double = result(channels + i)

            If Double.IsNaN(activity) Then
                activity = 0.0
            End If

            If activity < 0.0 Then
                activity = 0.0
            ElseIf activity > 1.0 Then
                activity = 1.0
            End If

            state.Signal(signalSlot(i)) = activity
        Next

        Dim phase As Double = result(2 * channels + 1)
        Dim twoPi As Double = 2.0 * System.Math.PI

        If Double.IsNaN(phase) Then
            phase = 0.0
        End If

        phase = phase Mod twoPi

        If phase < 0 Then
            phase += twoPi
        End If

        state.CyclePhase = phase
    End Sub

    Public Overrides Function GetStats() As Dictionary(Of String, Double)
        Dim stats As Dictionary(Of String, Double) = MyBase.GetStats()
        Dim signal As Double() = cell.State.Signal
        Dim sum As Double = 0.0
        Dim max As Double = 0.0

        For i As Integer = 0 To signal.Length - 1
            sum += signal(i)

            If signal(i) > max Then
                max = signal(i)
            End If
        Next

        Dim k As Integer = System.Math.Max(1, signal.Length)

        stats("tf_activity_mean") = sum / k
        stats("tf_activity_max") = max
        stats("cycle_phase") = cell.State.CyclePhase

        Return stats
    End Function

End Class
