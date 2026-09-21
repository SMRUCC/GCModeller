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
    End Sub

    ''' <summary>
    ''' 导出细胞快照
    ''' </summary>
    Public Function Snapshot() As CellSnapshot
        Dim snap As New CellSnapshot With {
            .cell_id = Id,
            .taxonomy = If(taxonomy_info Is Nothing, Nothing, taxonomy_info.ToString()),
            .is_alive = True,
            .rna = State.AsDictionary(StatePool.mRNA),
            .protein = State.AsDictionary(StatePool.Protein),
            .metabolite = State.AsDictionary(StatePool.Metabolite)
        }

        If Spot IsNot Nothing Then
            snap.parent_id = $"{Spot.index.X},{Spot.index.Y},{Spot.index.Z}"
        End If

        Return snap
    End Function

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
