' ============================================================================
' RouteModel.vb — A → B 定向合成通路搜索的结果对象（JSON DTO，System.Text.Json）
' ----------------------------------------------------------------------------
' 与既有的 PathReport / PathDto 完全独立：SynthesisRoute 复用同一批规则、汇与
' 评分逻辑，但返回的是「带起点约束与经济性指标」的通路契约，既有 JSON 不受影响。
' ============================================================================

Imports System.Text.Json.Serialization
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.RetroPath.Chem

Namespace Model

    ''' <summary>
    ''' 起点化合物 A 在通路中允许扮演的角色。
    ''' </summary>
    Public Enum SourceRoles
        ''' <summary>A 必须是路径最上游的叶子原料（不能再被继续分解），语义最严格。</summary>
        Source = 0
        ''' <summary>A 出现在路径任意位置即可（如 B ← … ← A ← …），召回更高。</summary>
        Anywhere = 1
    End Enum

    ''' <summary>
    ''' 正向通路中一步反应的 JSON DTO（在 <see cref="ForwardStepDto"/> 基础上增加主链标注）。
    ''' </summary>
    Public Class RouteStepDto

        ''' <summary>该步使用的规则 ID。</summary>
        <JsonPropertyName("rule_id")>
        Public Property RuleId As String

        ''' <summary>该步使用的规则名称。</summary>
        <JsonPropertyName("rule_name")>
        Public Property RuleName As String

        ''' <summary>底物（前体 + 共底物）的 SMILES 列表。</summary>
        <JsonPropertyName("substrates")>
        Public Property Substrates As String()

        ''' <summary>产物的 SMILES 列表。</summary>
        <JsonPropertyName("products")>
        Public Property Products As String()

        ''' <summary>该步的 ΔG（kJ/mol）。</summary>
        <JsonPropertyName("delta_g")>
        Public Property DeltaG As Double

        ''' <summary>该步的酶可得性层级（1/2/3）。</summary>
        <JsonPropertyName("enzyme_tier")>
        Public Property EnzymeTier As EnzymeTiers

        ''' <summary>
        ''' 该步是否位于「A → B」主链上：即其某个底物可由 A 经前面的步骤得到。
        ''' </summary>
        <JsonPropertyName("from_source")>
        Public Property FromSource As Boolean

        ''' <summary>
        ''' 主链序号（从 A 起算，1 开始）；不在主链上时为 0。
        ''' </summary>
        <JsonPropertyName("stage")>
        Public Property Stage As Integer

        Public Overrides Function ToString() As String
            Return $"[{RuleId} - {RuleName}] {EnzymeTier.Description}, delta-G:{DeltaG}, " &
                   $"{Substrates.JoinBy(" + ")} => {Products.JoinBy(" + ")}" &
                   If(FromSource, "  [主链#" & Stage & "]", "")
        End Function

    End Class

    ''' <summary>
    ''' 一条「A → B」候选通路的 JSON DTO：原有评分明细 + 经济性指标。
    ''' </summary>
    Public Class RouteDto : Implements Enumeration(Of RouteStepDto)

        ''' <summary>通路编号，形如 "route_1"（按经济性排序编号）。</summary>
        <JsonPropertyName("id")>
        Public Property Id As String

        ''' <summary>经济性排名（1 为最经济）。</summary>
        <JsonPropertyName("economy_rank")>
        Public Property EconomyRank As Integer

        ''' <summary>加权全局得分（热力学 / 酶可得性 / 长度）。</summary>
        <JsonPropertyName("global_score")>
        Public Property GlobalScore As Double

        ''' <summary>热力学可行性得分 (0,1)。</summary>
        <JsonPropertyName("thermo_score")>
        Public Property ThermoScore As Double

        ''' <summary>酶可得性得分 (0,1]。</summary>
        <JsonPropertyName("enzyme_score")>
        Public Property EnzymeScore As Double

        ''' <summary>长度得分 = 1 / 步数。</summary>
        <JsonPropertyName("length_score")>
        Public Property LengthScore As Double

        ''' <summary>整条通路的 ΔG 合计（kJ/mol）。</summary>
        <JsonPropertyName("delta_g_total")>
        Public Property DeltaGTotal As Double

        ''' <summary>反应步数。</summary>
        <JsonPropertyName("num_steps")>
        Public Property NumSteps As Integer

        ''' <summary>
        ''' 除起点 A 之外还需要几个外源起始原料（越少越经济）。
        ''' </summary>
        <JsonPropertyName("external_count")>
        Public Property ExternalCount As Integer

        ''' <summary>这些外源起始原料的 SMILES（不含 A、不含货币分子）。</summary>
        <JsonPropertyName("external_precursors")>
        Public Property ExternalPrecursors As String()

        ''' <summary>起点 A 是否作为通路最上游的叶子原料出现。</summary>
        <JsonPropertyName("source_is_start")>
        Public Property SourceIsStart As Boolean

        ''' <summary>
        ''' A 在正向通路中的阶段：0 = 最上游起始原料；n &gt; 0 = 出现在第 n 步的底物中。
        ''' </summary>
        <JsonPropertyName("source_stage")>
        Public Property SourceStage As Integer

        ''' <summary>
        ''' 综合经济性得分（仅用于展示，排序以「外源数 → 步数 → 全局分」字典序为准）：
        ''' 0.5 × 1/(1+外源数) + 0.3 × 长度分 + 0.2 × 全局分。
        ''' </summary>
        <JsonPropertyName("economy_score")>
        Public Property EconomyScore As Double

        ''' <summary>正向生物合成顺序的步骤列表（A 等起始原料 → 目标 B）。</summary>
        <JsonPropertyName("steps")>
        Public Property Steps As RouteStepDto()

        Public Iterator Function GenericEnumerator() As IEnumerator(Of RouteStepDto) Implements Enumeration(Of RouteStepDto).GenericEnumerator
            For Each [step] As RouteStepDto In Steps
                Yield [step]
            Next
        End Function
    End Class

    ''' <summary>
    ''' A → B 定向搜索的统计信息（在 <see cref="SearchStatsDto"/> 基础上增加定向搜索相关计数）。
    ''' </summary>
    Public Class RouteStatsDto : Inherits SearchStatsDto

        ''' <summary>实际执行的搜索轮次（未命中时自动加大束宽/深度重试）。</summary>
        <JsonPropertyName("rounds")>
        Public Property Rounds As Integer

        ''' <summary>搜索得到的完整通路总数（未做起点约束过滤）。</summary>
        <JsonPropertyName("candidates_scanned")>
        Public Property CandidatesScanned As Integer

        ''' <summary>其中满足「含起点 A」约束的通路数。</summary>
        <JsonPropertyName("source_hits")>
        Public Property SourceHits As Integer

        ''' <summary>是否允许除 A 之外的底盘代谢物作为起点（False = strict 模式）。</summary>
        <JsonPropertyName("strict")>
        Public Property Strict As Boolean

    End Class

    ''' <summary>
    ''' 一次「A → B」定向通路搜索的完整结果报告（JSON 输出的根对象）。
    ''' </summary>
    Public Class RouteReport

        ''' <summary>程序名，恒为 "RetroPath"。</summary>
        <JsonPropertyName("program")>
        Public Property Program As String

        ''' <summary>程序版本号。</summary>
        <JsonPropertyName("version")>
        Public Property Version As String

        ''' <summary>起点化合物 A 的 SMILES。</summary>
        <JsonPropertyName("source")>
        Public Property Source As String

        ''' <summary>目标化合物 B 的 SMILES。</summary>
        <JsonPropertyName("target")>
        Public Property Target As String

        ''' <summary>起点角色：Source / Anywhere。</summary>
        <JsonPropertyName("role")>
        Public Property Role As String

        ''' <summary>本次搜索所使用的参数快照。</summary>
        <JsonPropertyName("parameters")>
        Public Property Parameters As SearchParameters

        ''' <summary>搜索过程统计。</summary>
        <JsonPropertyName("stats")>
        Public Property Stats As RouteStatsDto

        ''' <summary>
        ''' 最经济的一条通路（按「外源起始原料数 → 步数 → 全局分」排序的第一条）；
        ''' 未找到满足起点约束的通路时为 Nothing。
        ''' </summary>
        <JsonPropertyName("best")>
        Public Property Best As RouteDto

        ''' <summary>其余满足起点约束的候选通路（按经济性排序，不含 Best）。</summary>
        <JsonPropertyName("candidates")>
        Public Property Candidates As RouteDto()

        ''' <summary>是否找到了满足约束的通路。</summary>
        <JsonPropertyName("found")>
        Public Property Found As Boolean

    End Class

End Namespace
