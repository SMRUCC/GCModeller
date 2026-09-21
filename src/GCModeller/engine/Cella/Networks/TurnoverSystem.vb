' ============================================================
' TurnoverSystem.vb - 物质周转 / 回收系统
' ============================================================
' 负责 mRNA 降解、以及降解产物的回收与再投入：
'
'     d(mRNA_i)/dt = −k_degR,i · mRNA_i
'     d(recycle)/dt = Σ_i ( ρ_R · k_degR,i · mRNA_i + ρ_P · k_degP,i · P_i )
'                     − k_use · recycle
'
' 状态向量 = [mRNA_0 … mRNA_{n-1}, recycle]，共 n+1 维。
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

    Private proteinInput As Double()

    ''' <summary>累计回收并重新投入代谢的物质当量（诊断用）</summary>
    Public ReadOnly Property RecycledTotal As Double
        Get
            Return _recycledTotal
        End Get
    End Property

    Private _recycledTotal As Double = 0.0

    Public Sub New(cell As VirtualCella, blueprint As CellaBlueprint)
        ' 状态 = mRNA(n) + 回收池(1)
        Call MyBase.New(cell, cell.State.NGene + 1, NameOf(TurnoverSystem))

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

        Dim initial As Double() = New Double(n - 1) {}

        Call Array.Copy(cell.State.mRNA, initial, cell.State.NGene)

        initial(n - 1) = cell.State.RecyclePool

        Call InitializeFrom(initial)
    End Sub

    Protected Overrides Function VectorFromState() As Double()
        Dim v As Double() = New Double(n - 1) {}

        Call Array.Copy(cell.State.mRNA, v, cell.State.NGene)

        v(n - 1) = cell.State.RecyclePool

        Return v
    End Function

    Protected Overrides Sub RHS(t As Double, y As NVector, ydot As NVector)
        Dim produced As Double = 0.0

        For i As Integer = 0 To n - 2
            Dim decayFlux As Double = messengerDecay(i) * y(i)

            ydot(i) = -decayFlux
            produced += recycleYieldRNA * decayFlux + recycleYieldProtein * proteinDecay(i) * proteinInput(i)
        Next

        ydot(n - 1) = produced - recycleUseRate * y(n - 1)
    End Sub

    Protected Overrides Sub Jacobian(t As Double, y As NVector, fy As NVector, J As DenseMatrix)
        For i As Integer = 0 To n - 1
            For col As Integer = 0 To n - 1
                J(i, col) = 0.0
            Next
        Next

        For i As Integer = 0 To n - 2
            J(i, i) = -messengerDecay(i)
            ' 回收池对 mRNA 的偏导
            J(n - 1, i) = recycleYieldRNA * messengerDecay(i)
        Next

        J(n - 1, n - 1) = -recycleUseRate
    End Sub

    Public Overrides Sub Tick(dt As Double)
        Dim state As CellularState = cell.State

        ' 蛋白质水平（只读，用于回收当量）
        Call Array.Copy(state.Protein, proteinInput, state.NGene)

        ' mRNA 会被转录调控网络修改，先把最新的 mRNA 同步进求解器
        Dim sync As Double() = CurrentState()

        Call Array.Copy(state.mRNA, sync, state.NGene)
        sync(n - 1) = state.RecyclePool
        Call ResetState(sync)

        Dim recycleBefore As Double = state.RecyclePool

        If Not Advance(dt) Then
            Call ResetState(sync)
            Return
        End If

        Dim result As Double() = CurrentState()

        For i As Integer = 0 To state.NGene - 1
            state.mRNA(i) = If(result(i) < 0.0, 0.0, result(i))
        Next

        Dim pool As Double = If(result(n - 1) < 0.0, 0.0, result(n - 1))

        state.RecyclePool = pool

        ' 质量守恒：回收池被消耗的部分注入到目标代谢物
        Dim consumed As Double = recycleUseRate * recycleBefore * dt

        If consumed > 0 AndAlso recycleTargetSlot >= 0 Then
            state.Metabolite(recycleTargetSlot) += consumed
            _recycledTotal += consumed
        End If
    End Sub

    Public Overrides Function GetStats() As Dictionary(Of String, Double)
        Dim stats As Dictionary(Of String, Double) = MyBase.GetStats()

        stats("recycle_pool") = cell.State.RecyclePool
        stats("recycled_total") = _recycledTotal

        Dim mrna As Double() = cell.State.mRNA
        Dim sum As Double = 0.0

        For i As Integer = 0 To mrna.Length - 1
            sum += mrna(i)
        Next

        stats("mrna_decayed_basal") = sum

        Return stats
    End Function

End Class
