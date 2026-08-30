Imports System.Text.Json.Serialization

Namespace Model

    Public Class BlastParameters

        <JsonPropertyName("word_size")>
        Public Property WordSize As Integer

        <JsonPropertyName("matrix")>
        Public Property Matrix As String

        <JsonPropertyName("reward")>
        Public Property Reward As Double

        <JsonPropertyName("penalty")>
        Public Property Penalty As Double

        <JsonPropertyName("threshold")>
        Public Property Threshold As Integer

        <JsonPropertyName("gap_open")>
        Public Property GapOpen As Double

        <JsonPropertyName("gap_extend")>
        Public Property GapExtend As Double

        <JsonPropertyName("evalue_cutoff")>
        Public Property EvalueCutoff As Double

        <JsonPropertyName("two_hit_window")>
        Public Property TwoHitWindow As Integer

        <JsonPropertyName("dust")>
        Public Property Dust As Boolean

        <JsonPropertyName("seg")>
        Public Property Seg As Boolean

        <JsonPropertyName("comp_based_stats")>
        Public Property CompBasedStats As Integer

        <JsonPropertyName("lambda")>
        Public Property Lambda As Double

        <JsonPropertyName("K")>
        Public Property K As Double

        <JsonPropertyName("H")>
        Public Property H As Double

        <JsonPropertyName("db_sequences")>
        Public Property DbSequences As Long

        <JsonPropertyName("db_residues")>
        Public Property DbResidues As Long

    End Class


End Namespace