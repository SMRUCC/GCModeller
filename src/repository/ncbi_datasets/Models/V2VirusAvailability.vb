' ============================================================================
' V2VirusAvailability.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2VirusAvailability
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2VirusAvailability

        ''' <summary>
        ''' valid_accessions 属性
        ''' </summary>
        <JsonProperty("valid_accessions")>
        Public Property ValidAccessions As List(Of String)

        ''' <summary>
        ''' invalid_accessions 属性
        ''' </summary>
        <JsonProperty("invalid_accessions")>
        Public Property InvalidAccessions As List(Of String)

        ''' <summary>
        ''' message 属性
        ''' </summary>
        <JsonProperty("message")>
        Public Property Message As String

    End Class

End Namespace
