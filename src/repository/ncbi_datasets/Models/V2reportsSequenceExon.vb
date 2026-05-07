' ============================================================================
' V2reportsSequenceExon.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsSequenceExon
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsSequenceExon

        ''' <summary>
        ''' exon_number 属性
        ''' </summary>
        <JsonProperty("exon_number")>
        Public Property ExonNumber As Integer?

        ''' <summary>
        ''' begin 属性
        ''' </summary>
        <JsonProperty("begin")>
        Public Property Begin As String

        ''' <summary>
        ''' end 属性
        ''' </summary>
        <JsonProperty("end")>
        Public Property End As String

    End Class

End Namespace
