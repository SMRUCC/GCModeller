' ============================================================
' DifferentiationSystem.vb - 分化系统与生态位信号
' ============================================================
' 每个时间步在细胞内部物理与生命周期之后执行，分两步：
'
'   1. UpdateNiche —— 把局部微环境读数写进 CellularState.Niche：
'        radial_position      归一化径向位置（0 = 类器官核心，1 = 表层）
'        surface_proximity    1 − 径向位置
'        local_density        所在格点的拥挤度
'        neighbor_induction   邻域内「异质」细胞占比（诱导性接触）
'        neighbor_inhibition  邻域内「同质」细胞占比（Notch-Delta 式侧向抑制）
'        nutrient_access      局部营养可及性
'      这些读数既用于分化规则，也通过 CellaBlueprint.NicheGeneCoupling
'      调制基因表达的设定点 —— 这就是「空间极性 / 位置信号」。
'
'   2. Run —— 对每个细胞评估其可分化去向的概率：
'
'        p = 基础概率
'          × 生长因子节拍（RequiredSignals：任一通道为 0 则本步不可能）
'          × (0.25 + 诱导强度) × (1 − 0.75 · 侧向抑制强度)
'          × 径向位置偏好（高斯核 exp(−(Δr / tolerance)²)）
'          × (0.5 + 0.5 · 局部密度)
'
'      命中即换蓝图分化，并记录一条 FateSwitch 事件。
'
'   3. ApplyRadialSorting —— 终末分化细胞若径向位置偏离偏好带，以很低
'      的概率在邻域内做一次「位置校正」（细胞插入 / intercalation），
'      把肾单位各段排成从核心到表面的同心分带。这是形态发生，
'      不是自由迁移，因此速率很低且只在同命运的偏好带附近生效。
' ============================================================

Imports Microsoft.VisualBasic.Linq

''' <summary>本步发生的分化与重排事件</summary>
Public Class DifferentiationEvents

    Public Property Switches As Integer = 0
    Public Property Sortings As Integer = 0
    Public Property Records As New List(Of FateSwitchRecord)()
    Public Property SortingRecords As New List(Of SortingRecord)()

    Public Overrides Function ToString() As String
        Return $"switches={Switches}, sortings={Sortings}"
    End Function

End Class

''' <summary>一次命运转换</summary>
Public Class FateSwitchRecord

    Public Property time As Double
    Public Property cell_id As String
    Public Property from_fate As String
    Public Property to_fate As String
    Public Property generation As Integer
    Public Property radial_position As Double
    Public Property local_density As Double
    Public Property induction As Double
    Public Property inhibition As Double

End Class

''' <summary>一次位置校正（细胞插入）</summary>
Public Class SortingRecord

    Public Property time As Double
    Public Property cell_id As String
    Public Property fate As String
    Public Property from_radius As Double
    Public Property to_radius As Double
    Public Property from_x As Integer
    Public Property from_y As Integer
    Public Property from_z As Integer
    Public Property to_x As Integer
    Public Property to_y As Integer
    Public Property to_z As Integer

End Class

''' <summary>
''' 分化系统：生态位信号更新 + 命运决定 + 位置校正
''' </summary>
Public Class DifferentiationSystem

    ''' <summary>标准生态位信号通道</summary>
    Public Shared ReadOnly Property StandardNicheChannels As String()
        Get
            Return {
                "radial_position",
                "surface_proximity",
                "local_density",
                "neighbor_induction",
                "neighbor_inhibition",
                "nutrient_access"
            }
        End Get
    End Property

    Public Property Catalog As FateCatalog
    Public Property Random As Random

    ''' <summary>是否启用位置校正（自组织分区）</summary>
    Public Property RadialSortingEnabled As Boolean = True

    ''' <summary>局部密度归一化用的每格点参考承载量</summary>
    Public Property DensityReference As Double = 5.0

    ''' <summary>营养可及性归一化的半饱和常数</summary>
    Public Property NutrientHalfSaturation As Double = 4.0

    ''' <summary>
    ''' 发育时钟：分化概率随仿真时间线性增强的速率。
    ''' 语义是「类器官越成熟，祖细胞池越倾向于退出增殖并分化」，
    ''' 乘子为 <c>1 + DevelopmentalClockRate · t</c>。
    ''' </summary>
    Public Property DevelopmentalClockRate As Double = 0.04

    Public ReadOnly Property LastEvents As DifferentiationEvents
        Get
            Return _lastEvents
        End Get
    End Property

    Private _lastEvents As New DifferentiationEvents()
    Private ReadOnly cumulative As New List(Of FateSwitchRecord)()
    Private ReadOnly sortingLog As New List(Of SortingRecord)()

    Public Sub New(catalog As FateCatalog, rand As Random)
        Me.Catalog = catalog
        Me.Random = rand
    End Sub

    ''' <summary>整场仿真的全部命运转换记录</summary>
    Public ReadOnly Property Switches As IEnumerable(Of FateSwitchRecord)
        Get
            Return cumulative
        End Get
    End Property

    ''' <summary>整场仿真的全部位置校正记录</summary>
    Public ReadOnly Property Sortings As IEnumerable(Of SortingRecord)
        Get
            Return sortingLog
        End Get
    End Property

    ''' <summary>某命运曾经的累计出现次数（含已分化走的细胞）</summary>
    Public Function SwitchCount(fromFate As String, toFate As String) As Integer
        Return cumulative _
            .Where(Function(r) String.Equals(r.from_fate, fromFate, StringComparison.OrdinalIgnoreCase) AndAlso
                                String.Equals(r.to_fate, toFate, StringComparison.OrdinalIgnoreCase)) _
            .Count()
    End Function

    ' ==================== 1. 生态位信号 ====================

    ''' <summary>
    ''' 刷新所有细胞的生态位信号读数
    ''' </summary>
    Public Sub UpdateNiche(env As Environment)
        If env Is Nothing Then
            Return
        End If

        For Each spot As Spot In env.GetAllSpots()
            If spot.cells.Count = 0 Then
                Continue For
            End If

            Dim neighbors As Spot() = env.GetNeighbors(spot)
            Dim region As New List(Of VirtualCella)(spot.cells)

            For Each n As Spot In neighbors
                region.AddRange(n.cells)
            Next

            Dim total As Integer = region.Count
            Dim density As Double = System.Math.Min(1.0, spot.cells.Count / System.Math.Max(1.0, DensityReference))

            ' 邻域里的命运计数，用于同质 / 异质接触强度
            Dim counts As Dictionary(Of String, Integer) = CountFates(region)

            For Each cella As VirtualCella In spot.cells
                Dim state As CellularState = cella.State
                Dim radial As Double = If(spot.NormalizedRadius >= 0, spot.NormalizedRadius, 0.5)
                Dim own As Integer = 0

                Call counts.TryGetValue(If(cella.Species, ""), own)

                Dim same As Double = If(total > 0, own / CDbl(total), 0.0)
                Dim hetero As Double = If(total > 0, (total - own) / CDbl(total), 0.0)

                Call SetNiche(state, "radial_position", radial)
                Call SetNiche(state, "surface_proximity", 1.0 - radial)
                Call SetNiche(state, "local_density", density)
                Call SetNiche(state, "neighbor_induction", hetero)
                Call SetNiche(state, "neighbor_inhibition", same)

                Dim nutrient As Double = spot.NutrientLevel(cella.Blueprint)

                Call SetNiche(state, "nutrient_access", nutrient / (nutrient + NutrientHalfSaturation))
            Next
        Next
    End Sub

    Private Shared Sub SetNiche(state As CellularState, name As String, value As Double)
        Dim idx As Integer = -1

        If state.NicheIndex.TryGetValue(name, idx) Then
            If Double.IsNaN(value) OrElse Double.IsInfinity(value) Then
                value = 0.0
            End If

            If value < 0 Then
                value = 0.0
            ElseIf value > 1.0 Then
                ' radial_position 允许略大于 1（球形边界），其余通道钳制到 1
                value = System.Math.Min(value, 2.0)
            End If

            state.Niche(idx) = value
        End If
    End Sub

    Private Shared Function CountFates(cells As IEnumerable(Of VirtualCella)) As Dictionary(Of String, Integer)
        Dim counts As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For Each cella As VirtualCella In cells.SafeQuery
            Dim key As String = If(cella.Species, "(unknown)")
            Dim n As Integer = 0

            Call counts.TryGetValue(key, n)
            counts(key) = n + 1
        Next

        Return counts
    End Function

    ' ==================== 2. 命运决定 ====================

    ''' <summary>
    ''' 推进一个时间步的分化过程
    ''' </summary>
    Public Function Run(env As Environment, dt As Double) As DifferentiationEvents
        Dim events As New DifferentiationEvents()

        _lastEvents = events

        If env Is Nothing OrElse Catalog Is Nothing OrElse dt <= 0 Then
            Return events
        End If

        Dim now_ As Double = env.CurrentTime

        For Each cella As VirtualCella In env.GetAllCells().ToArray()
            If Not cella.IsAlive OrElse cella.Spot Is Nothing Then
                Continue For
            End If

            Dim precursor As CellFateDefinition = Catalog.ById(cella.Species)

            If precursor Is Nothing Then
                Continue For
            End If

            Dim candidates As CellFateDefinition() = Catalog.CandidatesFrom(precursor.Id)

            If candidates.Length = 0 Then
                Continue For
            End If

            Dim target As CellFateDefinition = ChooseFate(cella, candidates, env)

            If target Is Nothing Then
                Continue For
            End If

            Dim spot As Spot = cella.Spot
            Dim radial As Double = If(spot.NormalizedRadius >= 0, spot.NormalizedRadius, 0.5)
            Dim record As New FateSwitchRecord With {
                .time = now_,
                .cell_id = cella.Id,
                .from_fate = precursor.Id,
                .to_fate = target.Id,
                .generation = cella.Generation,
                .radial_position = radial,
                .local_density = cella.State.Level(StatePool.Niche, "local_density"),
                .induction = cella.State.Level(StatePool.Niche, "neighbor_induction"),
                .inhibition = cella.State.Level(StatePool.Niche, "neighbor_inhibition")
            }

            Call CellaFactory.DifferentiateTo(cella, target, target.BiomassRetention)

            cumulative.Add(record)
            events.Records.Add(record)
            events.Switches += 1
        Next

        Return events
    End Function

    ''' <summary>
    ''' 按概率在所有可分化去向中挑选一个；都不满足则返回 Nothing
    ''' </summary>
    Private Function ChooseFate(cella As VirtualCella, candidates As CellFateDefinition(), env As Environment) As CellFateDefinition
        Dim spot As Spot = cella.Spot
        Dim radial As Double = If(spot.NormalizedRadius >= 0, spot.NormalizedRadius, 0.5)
        Dim density As Double = System.Math.Min(1.0, spot.cells.Count / System.Math.Max(1.0, DensityReference))

        ' 邻域命运计数（诱导 / 侧向抑制都要用）
        Dim region As New List(Of VirtualCella)(spot.cells)

        For Each n As Spot In env.GetNeighbors(spot)
            region.AddRange(n.cells)
        Next

        Dim counts As Dictionary(Of String, Integer) = CountFates(region)

        For Each target As CellFateDefinition In candidates
            Dim p As Double = Probability(cella, target, radial, density, counts)

            If p <= 0 Then
                Continue For
            End If

            If Random.NextDouble() < p Then
                Return target
            End If
        Next

        Return Nothing
    End Function

    Private Function Probability(cella As VirtualCella, target As CellFateDefinition,
                                 radial As Double, density As Double,
                                 counts As Dictionary(Of String, Integer)) As Double

        If cella.Generation < target.MinGeneration Then
            Return 0.0
        End If

        Dim p As Double = target.BaseProbability

        ' ---- 生长因子节拍 ----
        If Not target.RequiredSignals.IsNullOrEmpty Then
            For Each item In target.RequiredSignals
                Dim activity As Double = cella.State.Level(StatePool.Signal, item.Key)

                If activity <= 0.0 Then
                    Return 0.0
                End If

                If item.Value > 0 Then
                    p *= System.Math.Min(1.0, activity / item.Value)
                End If
            Next
        End If

        ' ---- 诱导（邻域里已有该前体 / 诱导者时更容易出现）----
        Dim induction As Double = 0.0

        If Not target.InducedBy.IsNullOrEmpty Then
            For Each item In target.InducedBy
                Dim n As Integer = 0

                Call counts.TryGetValue(item.Key, n)

                If item.Value > 0 Then
                    induction += System.Math.Min(1.0, n / item.Value)
                End If
            Next
        End If

        p *= 0.25 + System.Math.Min(1.0, induction)

        ' ---- 侧向抑制（Notch-Delta 式：邻域里已有太多同一命运则被压制）----
        Dim inhibition As Double = 0.0

        If Not target.InhibitedBy.IsNullOrEmpty Then
            For Each item In target.InhibitedBy
                Dim n As Integer = 0

                Call counts.TryGetValue(item.Key, n)

                If item.Value > 0 Then
                    inhibition += System.Math.Min(1.0, n / item.Value)
                End If
            Next
        End If

        p *= 1.0 - 0.75 * System.Math.Min(1.0, inhibition)

        ' ---- 空间极性：只有处在偏好径向带附近才容易分化 ----
        If target.Blueprint IsNot Nothing AndAlso target.Blueprint.PreferredRadius >= 0 Then
            Dim tolerance As Double = System.Math.Max(0.05, target.Blueprint.RadialTolerance)
            Dim d As Double = (radial - target.Blueprint.PreferredRadius) / tolerance

            p *= System.Math.Exp(-d * d)
        End If

        ' ---- 拥挤促进分化 ----
        p *= 0.5 + 0.5 * density

        ' ---- 发育时钟：越成熟的类器官越倾向于完成分化 ----
        If DevelopmentalClockRate > 0 Then
            p *= 1.0 + DevelopmentalClockRate * cella.Elapsed
        End If

        Return p
    End Function

    ' ==================== 3. 位置校正 ====================

    ''' <summary>
    ''' 终末分化细胞的位置校正：把各命运排到它偏好的径向带上
    ''' </summary>
    Public Sub ApplyRadialSorting(env As Environment, dt As Double)
        If Not RadialSortingEnabled OrElse env Is Nothing OrElse dt <= 0 Then
            Return
        End If

        Dim now_ As Double = env.CurrentTime

        For Each cella As VirtualCella In env.GetAllCells().ToArray()
            If Not cella.IsAlive OrElse cella.Spot Is Nothing Then
                Continue For
            End If

            Dim blueprint As CellaBlueprint = cella.Blueprint

            If blueprint Is Nothing OrElse blueprint.PreferredRadius < 0 OrElse blueprint.RadialSortingRate <= 0 Then
                Continue For
            End If

            Dim spot As Spot = cella.Spot

            If spot.NormalizedRadius < 0 Then
                Continue For
            End If

            Dim deviation As Double = System.Math.Abs(spot.NormalizedRadius - blueprint.PreferredRadius)

            If deviation <= blueprint.RadialTolerance Then
                Continue For
            End If

            If Random.NextDouble() > blueprint.RadialSortingRate * dt Then
                Continue For
            End If

            ' 在邻域里找一个径向位置更接近偏好带、且还有空位的格点
            Dim target As Spot = Nothing
            Dim best As Double = deviation

            For Each n As Spot In env.GetNeighbors(spot)
                If n.NormalizedRadius < 0 Then
                    Continue For
                End If
                If n.cells.Count >= blueprint.MaxCellsPerSpot Then
                    Continue For
                End If

                Dim d As Double = System.Math.Abs(n.NormalizedRadius - blueprint.PreferredRadius)

                If d < best Then
                    best = d
                    target = n
                End If
            Next

            If target Is Nothing Then
                Continue For
            End If

            Dim record As New SortingRecord With {
                .time = now_,
                .cell_id = cella.Id,
                .fate = cella.Species,
                .from_radius = spot.NormalizedRadius,
                .to_radius = target.NormalizedRadius,
                .from_x = spot.index.X,
                .from_y = spot.index.Y,
                .from_z = spot.index.Z,
                .to_x = target.index.X,
                .to_y = target.index.Y,
                .to_z = target.index.Z
            }

            Call spot.cells.Remove(cella)

            cella.Spot = target
            target.cells.Add(cella)

            sortingLog.Add(record)
            _lastEvents.SortingRecords.Add(record)
            _lastEvents.Sortings += 1
        Next
    End Sub

End Class
