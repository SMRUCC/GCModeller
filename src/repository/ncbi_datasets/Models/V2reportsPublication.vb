' ============================================================================
' V2reportsPublication.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsPublication
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsPublication

        ''' <summary>
        ''' pmid 属性
        ''' </summary>
        <JsonProperty("pmid")>
        Public Property Pmid As Integer?

        ''' <summary>
        ''' title 属性
        ''' </summary>
        <JsonProperty("title")>
        Public Property Title As String

        ''' <summary>
        ''' date 属性
        ''' </summary>
        <JsonProperty("date")>
        Public Property Date As String

        ''' <summary>
        ''' authors 属性
        ''' </summary>
        <JsonProperty("authors")>
        Public Property Authors As List(Of Object)

    End Class

End Namespace
