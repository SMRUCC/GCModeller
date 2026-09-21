' ============================================================
' MetabolicNetwork.vb - 代谢网络
' ============================================================
' 采用 Metaboliq 的结构化液态神经网络（LTC）建模：
'
'   * 隐藏状态 h ∈ R^m  —— 每一个**内部代谢物**对应一个液态神经元，
'     状态即该代谢物的浓度（log+z-score 归一化空间）。
'   * 外部输入 u ∈ R^(r+nB) —— 各反应的酶水平 + 边界（胞外）代谢物浓度。
'   * 连接由生化拓扑强制：循环权重 W 被代谢物邻接掩码 A_adj 约束，
'     输入权重 U 被参与掩码约束 ——「无生化关联即不可连接」。
'   * 反应通量由独立读取头给出：v = e ⊙ σ(Wv·[h;u] + bv)。
'
' 每个细胞持有独立的液态网络实例（隐藏状态是实例私有的），但参数从
' 预训练模板复制，避免「N 个细胞 × N 次训练」的开销。
' ============================================================

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DeepLearning.LiquidNeuralNetwork
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.Metaboliq

''' <summary>
''' 采用液态神经网络（LTC）所表示的代谢网络模型
''' </summary>
Public Class MetabolicNetwork : Inherits SubNetwork

    ''' <summary>代谢网络拓扑（自动派生 S / A_adj / P 掩码）</summary>
    Public ReadOnly Property Graph As MetabolicNetworkGraph

    ''' <summary>结构化液态神经网络</summary>
    Public ReadOnly Property Model As MetabolicLiquidNetwork

    ''' <summary>最近一次推进后的反应通量 [reaction]</summary>
    Public ReadOnly Property Fluxes As Double()
        Get
            Return _fluxes
        End Get
    End Property

    ''' <summary>最近一次推进后的代谢物浓度读出（归一化空间）[metabolite]</summary>
    Public ReadOnly Property Concentrations As Double()
        Get
            Return _concentrations
        End Get
    End Property

    ''' <summary>最近一次推进后的液态时间常数 τ^sys [metabolite]</summary>
    Public ReadOnly Property SystemTau As Double()
        Get
            Return _systemTau
        End Get
    End Property

    ReadOnly _fluxes As Double()
    ReadOnly _concentrations As Double()
    ReadOnly _systemTau As Double()

    ''' <summary>反应 id → 催化基因在基因向量中的索引（-1 表示该反应无对应酶基因）</summary>
    ReadOnly enzymeSource As Integer()

    ''' <summary>内部代谢物 id → 状态池索引</summary>
    ReadOnly metaboliteSlot As Integer()
    ''' <summary>边界代谢物 id → 状态池索引</summary>
    ReadOnly boundarySlot As Integer()

    ReadOnly enzymeReference As Double
    ReadOnly genes As String()

    ''' <summary>最近一次推进实际执行的 ODE 子步数</summary>
    Public ReadOnly Property SubStepsTaken As Integer
        Get
            Return lastSubSteps
        End Get
    End Property

    Private ReadOnly enzymeBuffer As Tensor
    Private ReadOnly boundaryBuffer As Tensor
    Private lastU As Tensor

    Sub New(network As MetabolicNetworkGraph,
            cell As VirtualCella,
            blueprint As CellaBlueprint,
            Optional template As MetabolicLiquidNetwork = Nothing,
            Optional initialState As Double() = Nothing)

        Call MyBase.New(cell, NameOf(MetabolicNetwork))

        Me.Graph = network
        Me.enzymeReference = System.Math.Max(blueprint.EnzymeReference, 0.000001)

        Dim liquid As New MetabolicLiquidNetwork(
            graph:=network,
            mode:=blueprint.MetabolicMode,
            solver:=blueprint.MetabolicSolver
        )

        Call liquid.SetTauBounds(blueprint.TauMin, blueprint.TauMax)

        liquid.MaxSubStep = blueprint.MaxSubStep
        Me.Model = liquid

        If template IsNot Nothing Then
            Call CopyWeights(template, liquid)
        End If

        Dim m As Integer = network.MetaboliteCount
        Dim r As Integer = network.ReactionCount
        Dim nB As Integer = network.BoundaryCount

        _fluxes = New Double(r - 1) {}
        _concentrations = New Double(m - 1) {}
        _systemTau = New Double(m - 1) {}

        enzymeBuffer = New Tensor(r)
        boundaryBuffer = New Tensor(nB)

        ' ---- 反应 → 催化基因 ----
        genes = cell.State.GeneNames
        enzymeSource = New Integer(r - 1) {}

        For j As Integer = 0 To r - 1
            Dim gene As String = blueprint.GeneOfReaction(network.ReactionIds(j))
            Dim idx As Integer = -1

            If gene IsNot Nothing AndAlso cell.State.GeneIndex.TryGetValue(gene, idx) Then
                enzymeSource(j) = idx
            Else
                enzymeSource(j) = -1
            End If
        Next

        ' ---- 代谢物 → 状态池槽位 ----
        metaboliteSlot = New Integer(m - 1) {}
        boundarySlot = New Integer(nB - 1) {}

        For i As Integer = 0 To m - 1
            Dim idx As Integer = -1

            If cell.State.MetaboliteIndex.TryGetValue(network.InternalIds(i), idx) Then
                metaboliteSlot(i) = idx
            Else
                metaboliteSlot(i) = -1
            End If
        Next

        For i As Integer = 0 To nB - 1
            Dim idx As Integer = -1

            If cell.State.BoundaryIndex.TryGetValue(network.BoundaryIds(i), idx) Then
                boundarySlot(i) = idx
            Else
                boundarySlot(i) = -1
            End If
        Next

        ' ---- 初始状态 ----
        Dim h0 As Double() = If(initialState, cell.State.Metabolite)
        Dim h As Tensor = New Tensor(m)

        For i As Integer = 0 To m - 1
            h(i) = If(i < h0.Length, h0(i), 0.0)
        Next

        Call liquid.Liquid.ResetState()
        Call liquid.Liquid.LiquidLayer.Cells(0).SetState(h)

        lastU = liquid.BuildInput(enzymeBuffer, boundaryBuffer)

        Call Refresh()
    End Sub

    ' ==================== 参数复制 ====================

    ''' <summary>
    ''' 把预训练模板的参数复制到新的液态网络实例（按参数名匹配）
    ''' </summary>
    Public Shared Sub CopyWeights(template As MetabolicLiquidNetwork, target As MetabolicLiquidNetwork)
        Dim dst As New Dictionary(Of String, Tensor)(StringComparer.OrdinalIgnoreCase)

        For Each p As ParameterPair In target.Liquid.GetParameterPairs()
            dst(p.Name) = p.Value
        Next

        For Each p As ParameterPair In template.Liquid.GetParameterPairs()
            Dim t As Tensor = Nothing

            If dst.TryGetValue(p.Name, t) AndAlso t IsNot Nothing AndAlso t.Length = p.Value.Length Then
                Call Array.Copy(p.Value.Data, t.Data, p.Value.Length)
            End If
        Next

        If template.FluxWeight IsNot Nothing AndAlso target.FluxWeight IsNot Nothing AndAlso
            template.FluxWeight.Length = target.FluxWeight.Length Then

            Call Array.Copy(template.FluxWeight.Data, target.FluxWeight.Data, template.FluxWeight.Length)
        End If

        If template.FluxBias IsNot Nothing AndAlso target.FluxBias IsNot Nothing AndAlso
            template.FluxBias.Length = target.FluxBias.Length Then

            Call Array.Copy(template.FluxBias.Data, target.FluxBias.Data, template.FluxBias.Length)
        End If

        If template.Liquid.OutputWeight IsNot Nothing AndAlso target.Liquid.OutputWeight IsNot Nothing AndAlso
            template.Liquid.OutputWeight.Length = target.Liquid.OutputWeight.Length Then

            Call Array.Copy(template.Liquid.OutputWeight.Data, target.Liquid.OutputWeight.Data, template.Liquid.OutputWeight.Length)
        End If

        If template.Liquid.OutputBias IsNot Nothing AndAlso target.Liquid.OutputBias IsNot Nothing AndAlso
            template.Liquid.OutputBias.Length = target.Liquid.OutputBias.Length Then

            Call Array.Copy(template.Liquid.OutputBias.Data, target.Liquid.OutputBias.Data, template.Liquid.OutputBias.Length)
        End If

        Call target.ApplyStructuralMasks()
    End Sub

    ' ==================== 推进 ====================

    Public Overrides Sub Tick(dt As Double)
        If dt <= 0 Then
            Return
        End If

        Dim state As CellularState = cell.State
        Dim r As Integer = Graph.ReactionCount
        Dim nB As Integer = Graph.BoundaryCount

        ' ---- 1. 酶水平：蛋白质水平 → [0,1] ----
        Dim protein As Double() = state.Protein

        For j As Integer = 0 To r - 1
            Dim slot As Integer = enzymeSource(j)

            If slot >= 0 AndAlso slot < protein.Length Then
                Dim p As Double = protein(slot)

                enzymeBuffer(j) = p / (p + enzymeReference)
            Else
                enzymeBuffer(j) = 1.0
            End If
        Next

        ' ---- 2. 边界代谢物：由跨膜转运系统写入状态池 ----
        Dim boundary As Double() = state.Boundary

        For k As Integer = 0 To nB - 1
            Dim slot As Integer = boundarySlot(k)

            boundaryBuffer(k) = If(slot >= 0 AndAlso slot < boundary.Length, boundary(slot), 0.0)
        Next

        ' ---- 3. 组装输入并推进一个时间步 ----
        lastU = Model.BuildInput(enzymeBuffer, boundaryBuffer)

        Try
            lastSubSteps = Model.StepInterval(lastU, dt)
        Catch ex As Exception
            Call CountFailure()
            Return
        End Try

        Call Refresh()
    End Sub

    ''' <summary>把液态网络的当前状态读回状态池与统计量</summary>
    Private Sub Refresh()
        Dim liquidCell As LiquidCell = Model.Liquid.LiquidLayer.Cells(0)
        Dim h As Tensor = liquidCell.State
        Dim state As CellularState = cell.State

        ' 浓度必须经读出层 ĉ = W_out·h + b_out 才是「浓度空间」的量
        Dim c As Tensor = Model.Liquid.ComputeOutputFrom(h)
        Dim v As Tensor = Model.ComputeFlux(h, lastU)
        Dim tau As Tensor = liquidCell.GetSystemTau(h, lastU)

        For i As Integer = 0 To _concentrations.Length - 1
            Dim value As Double = c(i)

            If Double.IsNaN(value) OrElse Double.IsInfinity(value) Then
                value = 0.0
                Call CountFailure()
            End If

            _concentrations(i) = value

            Dim slot As Integer = metaboliteSlot(i)

            If slot >= 0 AndAlso slot < state.Metabolite.Length Then
                state.Metabolite(slot) = value
            End If
        Next

        For j As Integer = 0 To _fluxes.Length - 1
            Dim value As Double = v(j)

            If Double.IsNaN(value) OrElse Double.IsInfinity(value) Then
                value = 0.0
            End If

            _fluxes(j) = value
        Next

        For i As Integer = 0 To _systemTau.Length - 1
            Dim value As Double = tau(i)

            If Double.IsNaN(value) OrElse Double.IsInfinity(value) Then
                value = 0.0
            End If

            _systemTau(i) = value
        Next
    End Sub

    ''' <summary>设置某个反应的酶水平（0 = 敲除，1 = 野生型）</summary>
    Public Sub KnockOut(reactionId As String)
        Call Model.KnockOut(reactionId)
    End Sub

    Public Sub SetEnzymeLevel(reactionId As String, level As Double)
        Call Model.SetEnzymeLevel(reactionId, level)
    End Sub

    Public Overrides Function GetStats() As Dictionary(Of String, Double)
        Dim cSum As Double = 0.0
        Dim vSum As Double = 0.0
        Dim tauSum As Double = 0.0

        For i As Integer = 0 To _concentrations.Length - 1
            cSum += _concentrations(i)
            tauSum += _systemTau(i)
        Next

        For j As Integer = 0 To _fluxes.Length - 1
            vSum += System.Math.Abs(_fluxes(j))
        Next

        Dim m As Integer = System.Math.Max(1, _concentrations.Length)
        Dim r As Integer = System.Math.Max(1, _fluxes.Length)

        Return New Dictionary(Of String, Double) From {
            {"metabolites", _concentrations.Length},
            {"reactions", _fluxes.Length},
            {"boundary", Graph.BoundaryCount},
            {"conc_mean", cSum / m},
            {"flux_abs_mean", vSum / r},
            {"tau_mean", tauSum / m},
            {"ode_substeps", lastSubSteps},
            {"masked_ratio", Model.MaskedRatio()}
        }
    End Function

End Class
