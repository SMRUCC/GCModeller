' ============================================================
' VirtualCella.vb - 虚拟细胞
' ============================================================
' 一个虚拟细胞 = 一份状态中枢（CellularState）+ 六个子网络：
'
'   信号转导 → 转录调控 → 翻译 → 跨膜转运 → 代谢 → 周转
'
' 调度顺序即因果链：胞外信号先决定转录因子的活性，转录调控网络据此给出
' mRNA 丰度，翻译系统把它变成蛋白（酶 / 转运蛋白），转运系统把胞外营养
' 折算成代谢网络的边界驱动，代谢网络推进代谢物浓度，最后由周转系统回收
' 降解产物并重新投入代谢池。
'
' 两条构建入口：
'   * BuildFrom(blueprint) —— 轻量入口（推荐）
'   * FromModel(...)       —— GCMarkup 全基因组模型（内部翻译为蓝图）
' ============================================================

Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Definitions
Imports SMRUCC.genomics.Metagenomics

Public Class VirtualCella

    ''' <summary>细胞唯一标识</summary>
    Public Property Id As String

    Public Property taxonomy_info As Taxonomy

    ''' <summary>状态中枢：子网络之间唯一的通信媒介</summary>
    Public Property State As CellularState

    ''' <summary>所在的空间格点（用于读取胞外培养基）</summary>
    Public Property Spot As Spot

    ''' <summary>构建该细胞所用的蓝图</summary>
    Public Property Blueprint As CellaBlueprint

    ' ==================== 子网络 ====================

    Public Property signaling As SignalTransductionNetwork
    Public Property grn As GeneRegulatoryNetwork
    Public Property translation As TranslationSystem
    Public Property transportation As TransportSystem
    Public Property metabolic As MetabolicNetwork
    Public Property turnover As TurnoverSystem

    ''' <summary>该细胞累计推进的时间</summary>
    Public ReadOnly Property Elapsed As Double
        Get
            Return _elapsed
        End Get
    End Property

    Private _elapsed As Double = 0.0

    ' ==================== 生命周期元数据 ====================

    ''' <summary>细胞类型 / 物种标识（通常取自 <see cref="CellaBlueprint.SpeciesName"/>）</summary>
    Public Property Species As String

    ''' <summary>亲代细胞 id；初始接种的细胞为 Nothing</summary>
    Public Property ParentId As String

    ''' <summary>代数：初始接种的细胞为第 0 代</summary>
    Public Property Generation As Integer = 0

    ''' <summary>出生时间（环境时钟）</summary>
    Public Property BirthTime As Double = 0.0

    ''' <summary>死亡时间（环境时钟）；未死亡为 Nothing</summary>
    Public Property DeathTime As Double?
    ''' <summary>死亡原因；未死亡为 Nothing</summary>
    Public Property DeathCause As String

    ''' <summary>当前生物量；达到分裂阈值即二分裂</summary>
    Public Property Biomass As Double = 0.0

    ''' <summary>已存活时间</summary>
    Public Property Age As Double = 0.0

    ''' <summary>是否存活</summary>
    Public Property IsAlive As Boolean = True

    ''' <summary>连续处于饥饿状态的步数</summary>
    Public Property StarvedTicks As Integer = 0

    ''' <summary>该细胞产生的子代数量</summary>
    Public ReadOnly Property Offspring As Integer
        Get
            Return offspringCounter
        End Get
    End Property

    Friend offspringCounter As Integer = 0

    Sub New()
    End Sub

    ''' <summary>
    ''' 按因果链顺序推进一个时间步
    ''' </summary>
    Public Sub Tick(dt As Double)
        If State Is Nothing Then
            Return
        End If

        If signaling IsNot Nothing Then
            Call signaling.Tick(dt)
        End If
        If grn IsNot Nothing Then
            Call grn.Tick(dt)
        End If
        If translation IsNot Nothing Then
            Call translation.Tick(dt)
        End If
        If transportation IsNot Nothing Then
            Call transportation.Tick(dt)
        End If
        If metabolic IsNot Nothing Then
            Call metabolic.Tick(dt)
        End If
        If turnover IsNot Nothing Then
            Call turnover.Tick(dt)
        End If

        ' 数值安全兜底：任何一步产生 NaN / 负值都在这里被钳制
        Call State.Sanitize()

        _elapsed += dt
        Age += dt

        ' 生物量累积：以「平均比通量」作为能量代理，叠加「平均蛋白水平」作为结构代理。
        ' 分裂阈值与死亡判定都交给 Lifecycle 模块，这里只负责累积。
        Me.Biomass += GrowthRatePerTick() * dt
    End Sub

    ''' <summary>
    ''' 单位时间新增的生物量
    ''' </summary>
    Public Function GrowthRatePerTick() As Double
        If Blueprint Is Nothing Then
            Return 0.0
        End If

        Dim fluxTerm As Double = 0.0

        If metabolic IsNot Nothing AndAlso metabolic.Fluxes IsNot Nothing AndAlso metabolic.Fluxes.Length > 0 Then
            Dim sum As Double = 0.0

            For Each v As Double In metabolic.Fluxes
                sum += System.Math.Abs(v)
            Next

            fluxTerm = sum / metabolic.Fluxes.Length
        End If

        Dim proteinTerm As Double = 0.0

        If State IsNot Nothing AndAlso State.NGene > 0 Then
            Dim sum As Double = 0.0

            For Each p As Double In State.Protein
                sum += p
            Next

            proteinTerm = sum / State.NGene
        End If

        Return Blueprint.BiomassYieldPerFlux * fluxTerm + Blueprint.BiomassYieldPerProtein * proteinTerm
    End Function

    ''' <summary>
    ''' 是否满足二分裂条件（生物量阈值 + 最小年龄 + 格点承载上限）
    ''' </summary>
    ''' <summary>
    ''' 是否满足分裂的生物学条件（生物量、最小年龄、存活）。
    ''' 格点承载上限由 <see cref="CellLifecycle"/> 在挑选子代落位时处理，
    ''' 因为允许子代向外扩散时亲代格点即使已满也仍然可以分裂。
    ''' </summary>
    Public Function CanDivide() As Boolean
        If Not IsAlive OrElse Blueprint Is Nothing Then
            Return False
        End If

        If Biomass < Blueprint.DivisionBiomassThreshold Then
            Return False
        End If

        If Age < Blueprint.MinDivisionAge Then
            Return False
        End If

        Return True
    End Function

    ''' <summary>亲代所在格点是否还有空位</summary>
    Public ReadOnly Property HasRoom As Boolean
        Get
            If Spot Is Nothing OrElse Blueprint Is Nothing Then
                Return False
            End If

            Return Spot.cells.Count < Blueprint.MaxCellsPerSpot
        End Get
    End Property

    ''' <summary>
    ''' 导出细胞快照
    ''' </summary>
    Public Function Snapshot() As CellSnapshot
        Dim snap As New CellSnapshot With {
            .cell_id = Id,
            .parent_id = ParentId,
            .taxonomy = If(taxonomy_info Is Nothing, Nothing, taxonomy_info.ToString()),
            .species = Species,
            .generation = Generation,
            .biomass = Biomass,
            .age = Age,
            .is_alive = IsAlive,
            .rna = State.AsDictionary(StatePool.mRNA),
            .protein = State.AsDictionary(StatePool.Protein),
            .metabolite = State.AsDictionary(StatePool.Metabolite)
        }

        If Spot IsNot Nothing Then
            snap.x = Spot.index.X
            snap.y = Spot.index.Y
            snap.z = Spot.index.Z
        End If

        Return snap
    End Function

    ''' <summary>
    ''' 让所有子网络的内部积分器重新对齐 <see cref="State"/>
    ''' </summary>
    ''' <remarks>
    ''' 二分裂产生的子代直接继承了亲代的一半状态，如果不同步，子网络的内部
    ''' 积分器仍然停留在自己的历史状态上，会导致子代的第一步出现跳变。
    ''' </remarks>
    Public Sub ResyncSubNetworks()
        For Each net As SubNetwork In SubNetworks()
            If net IsNot Nothing Then
                Call net.Resync()
            End If
        Next
    End Sub

    ''' <summary>
    ''' 汇总所有子网络的统计量
    ''' </summary>
    Public Function GetStats() As Dictionary(Of String, Double)
        Dim stats As New Dictionary(Of String, Double)

        For Each net As SubNetwork In SubNetworks()
            If net Is Nothing Then
                Continue For
            End If

            For Each item In net.GetStats().SafeQuery
                stats($"{net.Name}.{item.Key}") = item.Value
            Next
        Next

        stats("time") = _elapsed
        stats("cycle_phase") = If(State Is Nothing, 0.0, State.CyclePhase)
        stats("biomass") = Biomass
        stats("age") = Age
        stats("generation") = Generation

        Return stats
    End Function

    ''' <summary>枚举所有子网络</summary>
    Public Function SubNetworks() As IEnumerable(Of SubNetwork)
        Return New SubNetwork() {signaling, grn, translation, transportation, metabolic, turnover}
    End Function

    Public Overrides Function ToString() As String
        Return $"[{Id}] {If(State Is Nothing, "empty", State.ToString())}"
    End Function

    ' ==================== 构建入口 ====================

    ''' <summary>
    ''' 轻量构建入口：由蓝图直接构建一个虚拟细胞
    ''' </summary>
    Public Shared Function BuildFrom(blueprint As CellaBlueprint,
                                     Optional taxonomy As Taxonomy = Nothing,
                                     Optional id As String = Nothing,
                                     Optional initialMetabolite As Double() = Nothing,
                                     Optional initialMrna As Double() = Nothing) As VirtualCella
        Return CellaFactory.BuildCell(blueprint, taxonomy, id, initialMetabolite, initialMrna)
    End Function

    ''' <summary>
    ''' 由 GCMarkup 全基因组模型构建虚拟细胞
    ''' </summary>
    ''' <remarks>
    ''' 该路径会把 GCMarkup 的转录调控网络展开为基因级先验网络、
    ''' 把代谢反应翻译为 <see cref="SMRUCC.genomics.MetabolicModel.MetabolicReaction"/>，
    ''' 再走与 <see cref="BuildFrom"/> 完全相同的装配逻辑。
    ''' GCMarkup 模型本身不含实测表达矩阵，因此基线表达量使用合成名义值。
    ''' </remarks>
    Public Shared Function FromModel(cell As VirtualCell, define As Definition, dynamics As FluxBaseline) As VirtualCella
        Dim blueprint As CellaBlueprint = CellaFactory.BlueprintFromModel(cell, define, dynamics)
        Dim cella As VirtualCella = CellaFactory.BuildCell(blueprint, cell.taxonomy)

        Return cella
    End Function

End Class
