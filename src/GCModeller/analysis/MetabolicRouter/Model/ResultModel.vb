' ============================================================================
' ResultModel.vb — 结构化路径搜索结果对象（JSON DTO，System.Text.Json）
' ============================================================================

Imports System.Text.Json.Serialization

Namespace Model

    Public Class PathReport

        <JsonPropertyName("program")>
        Public Property Program As String

        <JsonPropertyName("version")>
        Public Property Version As String

        <JsonPropertyName("parameters")>
        Public Property Parameters As SearchParameters

        <JsonPropertyName("target")>
        Public Property Target As String

        <JsonPropertyName("stats")>
        Public Property Stats As SearchStatsDto

        <JsonPropertyName("paths")>
        Public Property Paths As List(Of PathDto)

    End Class

    Public Class SearchParameters

        <JsonPropertyName("strategy")>
        Public Property Strategy As String

        <JsonPropertyName("beam_width")>
        Public Property BeamWidth As Integer

        <JsonPropertyName("max_depth")>
        Public Property MaxDepth As Integer

        <JsonPropertyName("sink_size")>
        Public Property SinkSize As Integer

        <JsonPropertyName("num_rules")>
        Public Property NumRules As Integer

        <JsonPropertyName("weights")>
        Public Property Weights As Dictionary(Of String, Double)

    End Class

    Public Class SearchStatsDto

        <JsonPropertyName("applications_tried")>
        Public Property ApplicationsTried As Long

        <JsonPropertyName("states_generated")>
        Public Property StatesGenerated As Long

        <JsonPropertyName("max_depth_reached")>
        Public Property MaxDepthReached As Integer

        <JsonPropertyName("elapsed_ms")>
        Public Property ElapsedMs As Long

        <JsonPropertyName("paths_found")>
        Public Property PathsFound As Integer

    End Class

End Namespace
