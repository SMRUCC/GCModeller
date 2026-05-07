' ============================================================================
' V2reportsAssemblyDataReportPage.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsAssemblyDataReportPage
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsAssemblyDataReportPage

        ''' <summary>
        ''' reports 属性
        ''' </summary>
        <JsonProperty("reports")>
        Public Property Reports As List(Of Object)

        ''' <summary>
        ''' content_type 属性
        ''' </summary>
        <JsonProperty("content_type")>
        Public Property ContentType As Object

        ''' <summary>
        ''' total_count 属性
        ''' </summary>
        <JsonProperty("total_count")>
        Public Property TotalCount As Integer?

        ''' <summary>
        ''' next_page_token 属性
        ''' </summary>
        <JsonProperty("next_page_token")>
        Public Property NextPageToken As String

        ''' <summary>
        ''' messages 属性
        ''' </summary>
        <JsonProperty("messages")>
        Public Property Messages As List(Of Object)

    End Class

End Namespace
