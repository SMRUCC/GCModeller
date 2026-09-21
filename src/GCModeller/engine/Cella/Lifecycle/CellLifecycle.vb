' ============================================================
' CellLifecycle.vb - 细胞分裂与死亡
' ============================================================
' 每个时间步在细胞完成内部代谢推进之后执行，顺序为：
'
'   1. 饥饿计数：所在 Spot 的营养指标低于阈值即累加，恢复供料则清零
'   2. 死亡判定：连续饥饿超过上限 → 饥饿死亡；年龄超过最大寿命 → 老化死亡
'   3. 二分裂：生物量达到阈值且满足最小年龄 / 格点承载上限 → 分裂一次
'
' 分裂实现为二分裂：子代继承亲代一半的物质型状态池（mRNA / 蛋白 / 代谢物 /
' 回收池），强度型状态池（边界浓度 / 信号活性 / 周期相位）直接复制；亲代与
' 子代的生物量各得一半。
' ============================================================

Imports Microsoft.VisualBasic.Linq

''' <summary>
''' 一个时间步内发生的生命周期事件统计
''' </summary>
Public Class LifecycleEvents

    ''' <summary>本步发生的分裂次数</summary>
    Public Property Divisions As Integer = 0

    ''' <summary>本步发生的死亡次数</summary>
    Public Property Deaths As Integer = 0

    ''' <summary>本步新生细胞（分裂产生的子代）</summary>
    Public Property Births As New List(Of VirtualCella)()

    ''' <summary>本步死亡细胞</summary>
    Public Property Casualties As New List(Of VirtualCella)()

    Public Overrides Function ToString() As String
        Return $"divisions={Divisions}, deaths={Deaths}"
    End Function

End Class

''' <summary>
''' 细胞生命周期规则：生物量累积 → 二分裂 / 饥饿死亡 / 老化死亡
''' </summary>
Public Module CellLifecycle

    Public Const CauseStarvation As String = "starvation"
    Public Const CauseSenescence As String = "senescence"

    ''' <summary>
    ''' 推进一个时间步的分裂与死亡规则
    ''' </summary>
    Public Function Run(env As Environment, dt As Double) As LifecycleEvents
        Dim events As New LifecycleEvents()

        If dt <= 0 OrElse env Is Nothing Then
            Return events
        End If

        Dim now_ As Double = env.CurrentTime
        Dim snapshot As VirtualCella() = env.GetAllCells().ToArray()

        ' ---- 1. 饥饿计数与死亡判定 ----
        For Each cella As VirtualCella In snapshot
            If Not cella.IsAlive Then
                Continue For
            End If

            If cella.Spot IsNot Nothing Then
                Call env.Lineage.UpdateLocation(cella)
            End If

            Dim blueprint As CellaBlueprint = cella.Blueprint

            If blueprint Is Nothing Then
                Continue For
            End If

            Dim nutrient As Double = If(cella.Spot Is Nothing, 0.0, cella.Spot.NutrientLevel(blueprint))

            If nutrient <= blueprint.StarvationThreshold Then
                cella.StarvedTicks += 1
            Else
                cella.StarvedTicks = 0
            End If

            Dim cause As String = Nothing

            If cella.StarvedTicks >= blueprint.StarvationDeathTicks Then
                cause = CauseStarvation
            ElseIf cella.Age > blueprint.MaxCellAge Then
                cause = CauseSenescence
            End If

            If cause IsNot Nothing Then
                ' 必须把谱系登记簿传进去，否则死亡时间不会被回填，
                ' 谱系树里所有个体都会显示为「存活」
                Call Kill(cella, cause, now_, env.Lineage)
                events.Deaths += 1
                events.Casualties.Add(cella)
            End If
        Next

        ' ---- 2. 二分裂 ----
        ' 重新取一次存活细胞：上一步可能已经移除了一些细胞，
        ' 且分裂会改变 Spot 的承载计数，必须一个一个串行处理
        For Each cella As VirtualCella In env.GetAllCells().ToArray()
            If Not cella.IsAlive OrElse Not cella.CanDivide() Then
                Continue For
            End If

            Dim child As VirtualCella = CellaFactory.DivideCell(cella, now_)

            If child Is Nothing Then
                Continue For
            End If

            Call env.Lineage.RecordDivision(cella, child, now_)
            events.Divisions += 1
            events.Births.Add(child)
        Next

        Return events
    End Function

    ''' <summary>
    ''' 杀死一个细胞：从所在格点移除、标记死亡、回填谱系
    ''' </summary>
    Public Sub Kill(cella As VirtualCella, cause As String, time As Double,
                    Optional lineage As CellLineage = Nothing)

        If cella Is Nothing OrElse Not cella.IsAlive Then
            Return
        End If

        cella.IsAlive = False
        cella.DeathTime = time
        cella.DeathCause = cause

        If cella.Spot IsNot Nothing Then
            Call cella.Spot.cells.Remove(cella)
        End If

        If lineage IsNot Nothing Then
            Call lineage.RecordDeath(cella, cause, time)
        End If
    End Sub

End Module
