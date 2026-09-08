' ============================================================================
' ResultModel.vb — 结构化路径搜索结果对象（JSON DTO，System.Text.Json）
' ============================================================================

Imports System.Text.Json.Serialization

Namespace Model

    ''' <summary>
    ''' 一次通路搜索的完整结果报告（JSON 输出的根对象）。
    ''' </summary>
    Public Class PathReport

        ''' <summary>程序名，恒为 "RetroPath"。</summary>
        <JsonPropertyName("program")>
        Public Property Program As String

        ''' <summary>程序版本号。</summary>
        <JsonPropertyName("version")>
        Public Property Version As String

        ''' <summary>本次搜索所使用的参数快照。</summary>
        <JsonPropertyName("parameters")>
        Public Property Parameters As SearchParameters

        ''' <summary>目标分子的 SMILES。</summary>
        <JsonPropertyName("target")>
        Public Property Target As String

        ''' <summary>搜索过程统计（尝试次数、耗时、找到的路径数等）。</summary>
        <JsonPropertyName("stats")>
        Public Property Stats As SearchStatsDto

        ''' <summary>按全局分降序排列的候选通路；未找到时为空列表。</summary>
        <JsonPropertyName("paths")>
        Public Property Paths As List(Of PathDto)

    End Class

    ''' <summary>
    ''' 搜索参数快照（随结果一同输出，便于复现）。
    ''' </summary>
    Public Class SearchParameters

        ''' <summary>搜索策略：beam 或 dfs。</summary>
        <JsonPropertyName("strategy")>
        Public Property Strategy As String

        ''' <summary>束宽。</summary>
        <JsonPropertyName("beam_width")>
        Public Property BeamWidth As Integer

        ''' <summary>最大搜索深度（步数上限）。</summary>
        <JsonPropertyName("max_depth")>
        Public Property MaxDepth As Integer

        ''' <summary>底盘汇集合中去重后的代谢物个数。</summary>
        <JsonPropertyName("sink_size")>
        Public Property SinkSize As Integer

        ''' <summary>参与搜索的广义规则条数。</summary>
        <JsonPropertyName("num_rules")>
        Public Property NumRules As Integer

        ''' <summary>评分权重，键为 thermo / enzyme / length。</summary>
        <JsonPropertyName("weights")>
        Public Property Weights As Dictionary(Of String, Double)

    End Class

    ''' <summary>
    ''' 搜索统计（JSON DTO）。
    ''' </summary>
    Public Class SearchStatsDto

        ''' <summary>尝试过的规则应用次数。</summary>
        <JsonPropertyName("applications_tried")>
        Public Property ApplicationsTried As Long

        ''' <summary>生成的搜索状态总数。</summary>
        <JsonPropertyName("states_generated")>
        Public Property StatesGenerated As Long

        ''' <summary>实际到达的最大深度。</summary>
        <JsonPropertyName("max_depth_reached")>
        Public Property MaxDepthReached As Integer

        ''' <summary>搜索耗时（毫秒）。</summary>
        <JsonPropertyName("elapsed_ms")>
        Public Property ElapsedMs As Long

        ''' <summary>找到并展示的完整通路条数。</summary>
        <JsonPropertyName("paths_found")>
        Public Property PathsFound As Integer

    End Class

End Namespace
