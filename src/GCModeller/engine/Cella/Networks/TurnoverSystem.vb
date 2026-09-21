' ============================================================
' TurnoverSystem.vb - 物质周转 / 回收系统
' ============================================================
' 负责 mRNA 降解、以及降解产物的回收与再投入：
'
'     d(mRNA_i)/dt = −k_degR,i · mRNA_i                （解析求解）
'     d(recycle)/dt = Σ_i ( ρ_R · k_degR,i · mRNA_i + ρ_P · k_degP,i · P_i )
'                     − k_use · recycle                 （CVODE BDF）
'
' 性能考虑（重要）：mRNA 的降解是一阶线性衰减，有闭式解
'
'     mRNA_i(t+dt) = mRNA_i(t) · exp(−k_degR,i · dt)
'
' 因此 mRNA 走解析解，直接作用在状态池上；CVODE 只负责一维的回收池。
' 早期版本把 mRNA 放进 ODE 状态向量里，而转录调控网络每个时间步都会改写
' mRNA，导致每步都必须重新 Initialize 求解器（丢掉 BDF 的历史与阶数），
' 单细胞单步开销高达数十毫秒。改成解析衰减 + 一维回收池之后，
' 求解器可以跨步复用，开销下降两个数量级。
'
' 说明：蛋白质的降解由翻译系统的方程负责（避免两个求解器同时写同一个
' 状态变量），本系统只*读取*蛋白质水平来计算回收当量的输入项。
' 回收池被消耗的部分按质量守恒注入到指定的代谢物（蓝图
' RecycleTargetMetabolite），实现「降解 → 回收 → 重新进入代谢」的闭环。
' ============================================================

Imports Microsoft.VisualBasic.Math.Sundials.CVODE

''' <summary>
''' 物质回收的细胞周转系统，采用 CVODE(BDF) 刚性常微分方程系统来建模
''' </summary>
Public Class TurnoverSystem : Inherits OdeSubNetwork

    ReadOnly messengerDecay As Double()
    ReadOnly proteinDecay As Double()
    ReadOnly recycleYieldRNA As Double
    ReadOnly recycleYieldProtein As Double
    ReadOnly recycleUseRate As Double

    ''' <summary>回收池被消耗时的去向：状态池中的代谢物槽位（-1 表示不注入）</summary>
    ReadOnly recycleTargetSlot As Integer

    ''' <summary>本步内视为常量的蛋白水平（回收当量的输入项）</summary>
    Private proteinInput As Double()

    ''' <summary>本步内视为常量的回收当量产生速率</summary>
    Private productionRate As Double = 0.0

    ''' <summary>累计回收并重新投入代谢的物质当量（诊断用）</summary>
    Public ReadOnly Property RecycledTotal As Double
        Get
            Return _recycledTotal
        End Get
    End Property

    Private _recycledTotal As Double = 0.0

    ''' <summary>累计解析降解掉的 mRNA 总量（诊断用）</summary>
    Public ReadOnly Property DecayedTotal As Double
        Get
            Return _decayedTotal
        End Get
    End Property

    Private _decayedTotal As Double = 0.0

    Public Sub New(cell As VirtualCella, blueprint As CellaBlueprint)
        ' 状态向量只有一维：回收池
        Call MyBase.New(cell, 1, NameOf(TurnoverSystem))

        Dim genes As String() = cell.State.GeneNames

        messengerDecay = New Double(genes.Length - 1) {}
        proteinDecay = New Double(genes.Length - 1) {}
        proteinInput = New Double(genes.Length - 1) {}

        For i As Integer = 0 To genes.Length - 1
            messengerDecay(i) = blueprint.MessengerDegradationOf(genes(i))
            proteinDecay(i) = blueprint.ProteinDegradationOf(genes(i))
        Next

        recycleYieldRNA = blueprint.RecycleYieldRNA
        recycleYieldProtein = blueprint.RecycleYieldProtein
        recycleUseRate = blueprint.RecycleUseRate

        Dim target As String = blueprint.RecycleTargetMetabolite
        Dim slot As Integer = -1

        If target IsNot Nothing AndAlso cell.State.MetaboliteIndex.TryGetValue(target, slot) Then
            recycleTargetSlot = slot
        Else
            recycleTargetSlot = -1
        End If

        Call InitializeFrom({cell.State.RecyclePool})
    End Sub

    Protected Overrides Function VectorFromState() As Double()
        Return {cell.State.RecyclePool}
    End Function

    Protected Overrides Sub RHS(t As Double, y As NVector, ydot As NVector)
        ydot(0) = productionRate - recycleUseRate * y(0)
    End Sub

    Protected Overrides Sub Jacobian(t As Double, y As NVector, fy As NVector, J As DenseMatrix)
        J(0, 0) = -recycleUseRate
    End Sub

    Public Overrides Sub Tick(dt As Double)
        If dt <= 0 Then
            Return
        End If

        Dim state As CellularState = cell.State
        Dim nGenes As Integer = state.NGene

        Call Array.Copy(state.Protein, proteinInput, nGenes)

        ' ---- 1. mRNA 一阶衰减的闭式解 + 回收当量核算 ----
        Dim produced As Double = 0.0
        Dim decayed As Double = 0.0

        For i As Integer = 0 To nGenes - 1
            Dim level As Double = state.mRNA(i)

            If level <= 0 Then
                produced += recycleYieldProtein * proteinDecay(i) * proteinInput(i)
                Continue For
            End If

            Dim k As Double = messengerDecay(i)
            Dim remaining As Double = level * System.Math.Exp(-k * dt)
            Dim decayFlux As Double = level - remaining

            state.mRNA(i) = remaining
            decayed += decayFlux
            produced += recycleYieldRNA * decayFlux + recycleYieldProtein * proteinDecay(i) * proteinInput(i)
        Next

        _decayedTotal += decayed

        ' ---- 2. 回收池：一维 CVODE BDF，输入按步首的值冻结 ----
        productionRate = produced

        Dim poolBefore As Double = state.RecyclePool
        Dim rollback As Double() = CurrentState()

        If Not Advance(dt) Then
            Call ResetState(rollback)
            Return
        End If

        Dim pool As Double = CurrentState()(0)

        If Double.IsNaN(pool) OrElse pool < 0 Then
            pool = 0.0
        End If

        state.RecyclePool = pool

        ' ---- 3. 质量守恒：回收池被消耗的部分注入到目标代谢物 ----
        Dim consumed As Double = recycleUseRate * poolBefore * dt

        If consumed > 0 AndAlso recycleTargetSlot >= 0 Then
            state.Metabolite(recycleTargetSlot) += consumed
            _recycledTotal += consumed
        End If
    End Sub

    Public Overrides Function GetStats() As Dictionary(Of String, Double)
        Dim stats As Dictionary(Of String, Double) = MyBase.GetStats()

        stats("recycle_pool") = cell.State.RecyclePool
        stats("recycled_total") = _recycledTotal
        stats("mrna_decayed_total") = _decayedTotal

        Return stats
    End Function

End Class
