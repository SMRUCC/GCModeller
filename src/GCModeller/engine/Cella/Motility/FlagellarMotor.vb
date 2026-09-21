' ============================================================
' FlagellarMotor.vb - 鞭毛运动（格点间游动）
' ============================================================
' 运动模型 = 随机游走 + 营养梯度偏置：
'
'   1. 运动能力 motility   = meanProtein(鞭毛基因) / (mean + reference) ∈ [0,1]
'      —— 鞭毛结构蛋白不足（例如敲除了 fliC/motA）的细胞几乎不动
'   2. 趋化能力 chemotaxis = meanProtein(趋化受体基因) / (mean + reference) ∈ [0,1]
'      —— 决定梯度偏置项的强度
'   3. 每步以 pMove = 基础迁移概率 × motility 决定是否尝试游动
'   4. 决定游动时，在 x/y/z 六个邻居中按权重抽样：
'        w_i = max(ε, 1 + 偏置强度 × chemotaxis × tanh(Δnutrient_i / 梯度尺度))
'      —— 营养更高的邻居被选中的概率更大，但随机项保证不会完全确定化
'
' 迁移只在 Spot.Tick 之外执行，避免在遍历 cells 的过程中修改集合；
' 同时尊重目标格点的承载上限。
' ============================================================

Imports Microsoft.VisualBasic.Linq

''' <summary>
''' 一次格点间迁移事件
''' </summary>
Public Class MigrationEvent

    Public Property time As Double
    Public Property cell_id As String
    Public Property species As String

    Public Property from_x As Integer
    Public Property from_y As Integer
    Public Property from_z As Integer

    Public Property to_x As Integer
    Public Property to_y As Integer
    Public Property to_z As Integer

    ''' <summary>运动能力（0~1）</summary>
    Public Property motility As Double
    ''' <summary>趋化能力（0~1）</summary>
    Public Property chemotaxis As Double

    Public Property nutrient_before As Double
    Public Property nutrient_after As Double

End Class

''' <summary>
''' 鞭毛运动马达
''' </summary>
Public Module FlagellarMotor

    ''' <summary>权重下界，避免梯度极负时概率被压到 0</summary>
    Private Const MinBias As Double = 0.05

    ''' <summary>
    ''' 推进一个时间步的细胞游动
    ''' </summary>
    ''' <returns>本步实际发生的迁移事件</returns>
    Public Function Swim(env As Environment, dt As Double, rand As Random) As List(Of MigrationEvent)
        Dim events As New List(Of MigrationEvent)()

        If env Is Nothing OrElse rand Is Nothing Then
            Return events
        End If

        For Each cella As VirtualCella In env.GetAllCells().ToArray()
            If Not cella.IsAlive OrElse cella.Spot Is Nothing OrElse cella.Blueprint Is Nothing Then
                Continue For
            End If

            Dim blueprint As CellaBlueprint = cella.Blueprint

            If blueprint.MotilityBaseProbability <= 0 OrElse blueprint.FlagellarGenes.IsNullOrEmpty Then
                Continue For
            End If

            Dim motility As Double = MotilityOf(cella)

            If motility <= 0 Then
                Continue For
            End If

            If rand.NextDouble() >= blueprint.MotilityBaseProbability * motility Then
                Continue For
            End If

            Dim current As Spot = cella.Spot
            Dim candidates As Spot() = env.GetNeighbors(current) _
                .Where(Function(s) s IsNot Nothing AndAlso s.cells.Count < blueprint.MaxCellsPerSpot) _
                .ToArray()

            If candidates.Length = 0 Then
                Continue For
            End If

            Dim chemotaxis As Double = ChemotaxisOf(cella)
            Dim nutrientBefore As Double = current.LigandLevel(blueprint)
            Dim scale As Double = System.Math.Max(blueprint.MotilityGradientScale, 0.000001)
            Dim weights As Double() = New Double(candidates.Length - 1) {}
            Dim total As Double = 0.0

            For i As Integer = 0 To candidates.Length - 1
                Dim delta As Double = (candidates(i).LigandLevel(blueprint) - nutrientBefore) / scale
                Dim bias As Double = 1.0 + blueprint.MotilityGradientBias * chemotaxis * System.Math.Tanh(delta)

                If bias < MinBias Then
                    bias = MinBias
                End If

                weights(i) = bias
                total += bias
            Next

            Dim pick As Spot = Sample(candidates, weights, total, rand)

            If pick Is Nothing OrElse ReferenceEquals(pick, current) Then
                Continue For
            End If

            Call current.cells.Remove(cella)
            pick.cells.Add(cella)
            cella.Spot = pick
            Call env.Lineage.UpdateLocation(cella)

            events.Add(New MigrationEvent With {
                .time = env.CurrentTime,
                .cell_id = cella.Id,
                .species = cella.Species,
                .from_x = current.index.X,
                .from_y = current.index.Y,
                .from_z = current.index.Z,
                .to_x = pick.index.X,
                .to_y = pick.index.Y,
                .to_z = pick.index.Z,
                .motility = motility,
                .chemotaxis = chemotaxis,
                .nutrient_before = nutrientBefore,
                .nutrient_after = pick.LigandLevel(blueprint)
            })
        Next

        Return events
    End Function

    ''' <summary>运动能力：鞭毛结构蛋白水平的饱和函数 ∈ [0,1]</summary>
    Public Function MotilityOf(cella As VirtualCella) As Double
        Dim blueprint As CellaBlueprint = cella.Blueprint

        If blueprint Is Nothing Then
            Return 0.0
        End If

        Return Saturation(CellaBlueprint.MeanProteinOf(cella.State, blueprint.FlagellarGenes),
                          blueprint.MotilityReference)
    End Function

    ''' <summary>趋化能力：趋化受体蛋白水平的饱和函数 ∈ [0,1]</summary>
    Public Function ChemotaxisOf(cella As VirtualCella) As Double
        Dim blueprint As CellaBlueprint = cella.Blueprint

        If blueprint Is Nothing Then
            Return 0.0
        End If

        If blueprint.ChemotaxisGenes.IsNullOrEmpty Then
            ' 未指定趋化受体时退回「无偏置」的纯随机游走
            Return 0.0
        End If

        Return Saturation(CellaBlueprint.MeanProteinOf(cella.State, blueprint.ChemotaxisGenes),
                          blueprint.MotilityReference)
    End Function

    Private Function Saturation(protein As Double, reference As Double) As Double
        If protein <= 0 OrElse Double.IsNaN(protein) Then
            Return 0.0
        End If

        Dim k As Double = System.Math.Max(reference, 0.000001)

        Return System.Math.Min(1.0, protein / (protein + k))
    End Function

    Private Function Sample(candidates As Spot(), weights As Double(), total As Double, rand As Random) As Spot
        If total <= 0 Then
            Return candidates(rand.Next(candidates.Length))
        End If

        Dim r As Double = rand.NextDouble() * total
        Dim acc As Double = 0.0

        For i As Integer = 0 To candidates.Length - 1
            acc += weights(i)

            If r <= acc Then
                Return candidates(i)
            End If
        Next

        Return candidates(candidates.Length - 1)
    End Function

End Module
