' ============================================================================
' BlastResult.vb — 结构化比对结果对象（JSON 序列化模型）
' ----------------------------------------------------------------------------
' 输出结构：
'   report.program / task / version / parameters{...}
'   report.queries[] → hits[] → hsps[]（outfmt6 全字段 + 比对串）
' 序列化使用 System.Text.Json（BCL，无第三方依赖）。
' ============================================================================

Imports System.Text.Json.Serialization

Namespace Model

    Public Class BlastReport

        <JsonPropertyName("program")>
        Public Property Program As String

        <JsonPropertyName("task")>
        Public Property Task As String

        <JsonPropertyName("version")>
        Public Property Version As String

        <JsonPropertyName("parameters")>
        Public Property Parameters As BlastParameters

        <JsonPropertyName("queries")>
        Public Property Queries As List(Of QueryResult)

    End Class

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

    Public Class QueryResult

        <JsonPropertyName("id")>
        Public Property Id As String

        <JsonPropertyName("description")>
        Public Property Description As String

        <JsonPropertyName("length")>
        Public Property Length As Integer

        <JsonPropertyName("hits")>
        Public Property Hits As List(Of Hit)

    End Class

    Public Class Hit

        <JsonPropertyName("id")>
        Public Property Id As String

        <JsonPropertyName("description")>
        Public Property Description As String

        <JsonPropertyName("length")>
        Public Property Length As Integer

        <JsonPropertyName("hsps")>
        Public Property Hsps As List(Of Hsp)

    End Class

    Public Class Hsp

        <JsonPropertyName("score")>
        Public Property Score As Double

        <JsonPropertyName("bit_score")>
        Public Property BitScore As Double

        <JsonPropertyName("evalue")>
        Public Property Evalue As Double

        <JsonPropertyName("identities")>
        Public Property Identities As Integer

        <JsonPropertyName("positives")>
        Public Property Positives As Integer

        <JsonPropertyName("gaps")>
        Public Property Gaps As Integer

        <JsonPropertyName("query_from")>
        Public Property QueryFrom As Integer

        <JsonPropertyName("query_to")>
        Public Property QueryTo As Integer

        <JsonPropertyName("subject_from")>
        Public Property SubjectFrom As Integer

        <JsonPropertyName("subject_to")>
        Public Property SubjectTo As Integer

        <JsonPropertyName("query_frame")>
        Public Property QueryFrame As Integer

        <JsonPropertyName("query_seq")>
        Public Property QuerySeq As String

        <JsonPropertyName("midline")>
        Public Property Midline As String

        <JsonPropertyName("subject_seq")>
        Public Property SubjectSeq As String

    End Class

End Namespace
