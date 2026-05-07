' ============================================================================
' V2DownloadSummary.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2DownloadSummary
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2DownloadSummary

        ''' <summary>
        ''' record_count 属性
        ''' </summary>
        <JsonProperty("record_count")>
        Public Property RecordCount As Integer?

        ''' <summary>
        ''' assembly_count 属性
        ''' </summary>
        <JsonProperty("assembly_count")>
        Public Property AssemblyCount As Integer?

        ''' <summary>
        ''' resource_updated_on 属性
        ''' </summary>
        <JsonProperty("resource_updated_on")>
        Public Property ResourceUpdatedOn As DateTime?

        ''' <summary>
        ''' hydrated 属性
        ''' </summary>
        <JsonProperty("hydrated")>
        Public Property Hydrated As Object

        ''' <summary>
        ''' dehydrated 属性
        ''' </summary>
        <JsonProperty("dehydrated")>
        Public Property Dehydrated As Object

        ''' <summary>
        ''' errors 属性
        ''' </summary>
        <JsonProperty("errors")>
        Public Property Errors As List(Of Object)

        ''' <summary>
        ''' messages 属性
        ''' </summary>
        <JsonProperty("messages")>
        Public Property Messages As List(Of Object)

        ''' <summary>
        ''' available_files 属性
        ''' </summary>
        <JsonProperty("available_files")>
        Public Property AvailableFiles As Object

    End Class

End Namespace
