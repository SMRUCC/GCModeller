Imports System.Text.Json.Serialization

Namespace Model

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