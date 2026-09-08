Imports System.Text.Json.Serialization

Namespace Model

    ''' <summary>
    ''' 一条候选通路的 JSON DTO：评分明细 + 正向生物合成步骤。
    ''' </summary>
    Public Class PathDto

        ''' <summary>通路编号，形如 "path_1"（按全局分降序编号）。</summary>
        <JsonPropertyName("id")>
        Public Property Id As String

        ''' <summary>加权全局得分，用于排序。</summary>
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

        ''' <summary>正向生物合成顺序的步骤列表（汇前体 → 目标）。</summary>
        <JsonPropertyName("steps")>
        Public Property Steps As List(Of ForwardStepDto)

    End Class

    ''' <summary>
    ''' 正向通路中一步反应的 JSON DTO。
    ''' </summary>
    Public Class ForwardStepDto

        ''' <summary>该步使用的规则 ID。</summary>
        <JsonPropertyName("rule_id")>
        Public Property RuleId As String

        ''' <summary>该步使用的规则名称。</summary>
        <JsonPropertyName("rule_name")>
        Public Property RuleName As String

        ''' <summary>底物（前体 + 共底物）的 SMILES 列表。</summary>
        <JsonPropertyName("substrates")>
        Public Property Substrates As List(Of String)

        ''' <summary>产物的 SMILES 列表。</summary>
        <JsonPropertyName("products")>
        Public Property Products As List(Of String)

        ''' <summary>该步的 ΔG（kJ/mol）。</summary>
        <JsonPropertyName("delta_g")>
        Public Property DeltaG As Double

        ''' <summary>该步的酶可得性层级（1/2/3）。</summary>
        <JsonPropertyName("enzyme_tier")>
        Public Property EnzymeTier As Integer

    End Class

    ''' <summary>
    ''' 规则库的 JSON DTO（<c>enumerate-rules</c> 子命令的输出单元）。
    ''' </summary>
    Public Class RuleDto

        ''' <summary>规则 ID。</summary>
        <JsonPropertyName("id")>
        Public Property Id As String

        ''' <summary>规则名称。</summary>
        <JsonPropertyName("name")>
        Public Property Name As String

        ''' <summary>反应物侧模式（SMARTS 子集）。</summary>
        <JsonPropertyName("reactant_smarts")>
        Public Property ReactantSmarts As String

        ''' <summary>产物侧模式（SMARTS 子集）。</summary>
        <JsonPropertyName("product_smarts")>
        Public Property ProductSmarts As String

        ''' <summary>正向 ΔG（kJ/mol）。</summary>
        <JsonPropertyName("delta_g")>
        Public Property DeltaG As Double

        ''' <summary>酶可得性层级（1/2/3）。</summary>
        <JsonPropertyName("enzyme_tier")>
        Public Property EnzymeTier As Integer

        ''' <summary>该反应是否可逆。</summary>
        <JsonPropertyName("reversible")>
        Public Property Reversible As Boolean

    End Class

End Namespace