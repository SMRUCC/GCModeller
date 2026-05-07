' ============================================================================
' V2reportsProcessMetadata.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsProcessMetadata
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsProcessMetadata

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <JsonProperty("name")>
        Public Property Name As String

        ''' <summary>
        ''' go_id 属性
        ''' </summary>
        <JsonProperty("go_id")>
        Public Property GoId As String

        ''' <summary>
        ''' evidence_code 属性
        ''' </summary>
        <JsonProperty("evidence_code")>
        Public Property EvidenceCode As String

        ''' <summary>
        ''' qualifier 属性
        ''' </summary>
        <JsonProperty("qualifier")>
        Public Property Qualifier As String

        ''' <summary>
        ''' reference 属性
        ''' </summary>
        <JsonProperty("reference")>
        Public Property Reference As Object

    End Class

End Namespace
