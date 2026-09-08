' ============================================================================
' ResultModel.vb — 结构化路径搜索结果对象（JSON DTO，System.Text.Json）
' ============================================================================

Imports System.Text.Json.Serialization

Namespace RetroPath.Model

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

    Public Class PathDto

        <JsonPropertyName("id")>
        Public Property Id As String

        <JsonPropertyName("global_score")>
        Public Property GlobalScore As Double

        <JsonPropertyName("thermo_score")>
        Public Property ThermoScore As Double

        <JsonPropertyName("enzyme_score")>
        Public Property EnzymeScore As Double

        <JsonPropertyName("length_score")>
        Public Property LengthScore As Double

        <JsonPropertyName("delta_g_total")>
        Public Property DeltaGTotal As Double

        <JsonPropertyName("num_steps")>
        Public Property NumSteps As Integer

        <JsonPropertyName("steps")>
        Public Property Steps As List(Of ForwardStepDto)

    End Class

    Public Class ForwardStepDto

        <JsonPropertyName("rule_id")>
        Public Property RuleId As String

        <JsonPropertyName("rule_name")>
        Public Property RuleName As String

        <JsonPropertyName("substrates")>
        Public Property Substrates As List(Of String)

        <JsonPropertyName("products")>
        Public Property Products As List(Of String)

        <JsonPropertyName("delta_g")>
        Public Property DeltaG As Double

        <JsonPropertyName("enzyme_tier")>
        Public Property EnzymeTier As Integer

    End Class

    Public Class RuleDto

        <JsonPropertyName("id")>
        Public Property Id As String

        <JsonPropertyName("name")>
        Public Property Name As String

        <JsonPropertyName("reactant_smarts")>
        Public Property ReactantSmarts As String

        <JsonPropertyName("product_smarts")>
        Public Property ProductSmarts As String

        <JsonPropertyName("delta_g")>
        Public Property DeltaG As Double

        <JsonPropertyName("enzyme_tier")>
        Public Property EnzymeTier As Integer

        <JsonPropertyName("reversible")>
        Public Property Reversible As Boolean

    End Class

End Namespace
