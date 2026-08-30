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

    ''' <summary>
    ''' json output of the blast result
    ''' </summary>
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

End Namespace
