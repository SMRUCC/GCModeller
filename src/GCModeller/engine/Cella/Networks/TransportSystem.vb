' ============================================================
' TransportSystem.vb - 跨膜转运系统
' ============================================================
' 跨膜转运**不单独建一套 ODE**，而是并入 Metaboliq 代谢网络的边界驱动：
'
'   胞外浓度（Spot.Medium）× 转运蛋白丰度 ──▶ 有效边界浓度 ──▶ Metaboliq 的 u 输入
'
' 这么做的理由：转运与代谢共享同一批边界代谢物，若各自建一套方程，同一
' 物质会被重复记账、质量不守恒。改为由液态网络统一积分，天然享受
' Metaboliq 自带的质量守恒软约束（λ_mass·‖S·v‖²）。
'
' 本系统负责两件事：
'   1. 折算有效边界浓度：boundary_b = 胞外浓度_b × 转运蛋白可用性_b
'   2. 从所在格点的培养基中扣除被摄取的量（环境侧的消耗）
' ============================================================

''' <summary>
''' 跨膜转运系统（把胞外营养折算为代谢网络的边界驱动）
''' </summary>
Public Class TransportSystem : Inherits SubNetwork

    ''' <summary>边界代谢物 → 状态池槽位</summary>
    ReadOnly boundarySlot As Integer()
    ''' <summary>边界代谢物 → 转运蛋白基因在状态池中的槽位（-1 表示无对应转运蛋白）</summary>
    ReadOnly transporterSlot As Integer()
    ''' <summary>边界代谢物名称</summary>
    ReadOnly boundaryNames As String()

    ReadOnly capacityReference As Double
    ReadOnly vmax As Double
    ReadOnly km As Double

    ''' <summary>最近一次推进中各边界代谢物的摄取速率（正数 = 摄入细胞）</summary>
    Public ReadOnly Property UptakeRates As Double()
        Get
            Return uptake
        End Get
    End Property

    ReadOnly uptake As Double()

    ''' <summary>累计从环境中摄取的物质量（诊断用）</summary>
    Public ReadOnly Property ConsumedTotal As Double
        Get
            Return _consumedTotal
        End Get
    End Property

    Private _consumedTotal As Double = 0.0

    Sub New(cell As VirtualCella, blueprint As CellaBlueprint)
        Call MyBase.New(cell, NameOf(TransportSystem))

        Dim names As String() = cell.State.BoundaryNames

        boundaryNames = names
        boundarySlot = New Integer(names.Length - 1) {}
        transporterSlot = New Integer(names.Length - 1) {}
        uptake = New Double(names.Length - 1) {}
        capacityReference = System.Math.Max(blueprint.EnzymeReference, 0.000001)
        vmax = blueprint.TransportVmax
        km = System.Math.Max(blueprint.TransportKm, 0.000001)

        For i As Integer = 0 To names.Length - 1
            boundarySlot(i) = i

            Dim gene As String = blueprint.TransporterOf(names(i))
            Dim slot As Integer = -1

            If gene IsNot Nothing AndAlso cell.State.GeneIndex.TryGetValue(gene, slot) Then
                transporterSlot(i) = slot
            Else
                transporterSlot(i) = -1
            End If
        Next
    End Sub

    Public Overrides Sub Tick(dt As Double)
        Dim state As CellularState = cell.State
        Dim medium As Dictionary(Of String, Double) = If(cell.Spot Is Nothing, Nothing, cell.Spot.Medium)
        Dim protein As Double() = state.Protein

        For i As Integer = 0 To boundaryNames.Length - 1
            Dim external As Double = 0.0

            If medium IsNot Nothing Then
                Call medium.TryGetValue(boundaryNames(i), external)
            End If

            If Double.IsNaN(external) OrElse external < 0.0 Then
                external = 0.0
            End If

            ' 转运蛋白丰度 → 可用性 ∈ [0,1]
            Dim capacity As Double = 1.0
            Dim slot As Integer = transporterSlot(i)

            If slot >= 0 AndAlso slot < protein.Length Then
                Dim p As Double = protein(slot)

                capacity = p / (p + capacityReference)
            End If

            ' 有效边界浓度：转运蛋白被敲除时该物质无法进入代谢网络
            state.Boundary(boundarySlot(i)) = external * capacity

            ' 环境侧消耗：米氏方程形式的摄取速率
            Dim rate As Double = vmax * capacity * external / (km + external)

            uptake(i) = rate

            If medium IsNot Nothing Then
                Dim remain As Double = external - rate * dt

                If remain < 0.0 Then
                    remain = 0.0
                End If

                medium(boundaryNames(i)) = remain
                _consumedTotal += (external - remain)
            End If
        Next
    End Sub

    Public Overrides Function GetStats() As Dictionary(Of String, Double)
        Dim sum As Double = 0.0

        For i As Integer = 0 To uptake.Length - 1
            sum += uptake(i)
        Next

        Return New Dictionary(Of String, Double) From {
            {"boundary_metabolites", uptake.Length},
            {"uptake_total", sum},
            {"consumed_total", _consumedTotal}
        }
    End Function

End Class
