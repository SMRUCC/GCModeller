' ============================================================================
' V2DownloadSummary.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2DownloadSummary
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2DownloadSummary

        ''' <summary>
        ''' record_count 属性
        ''' </summary>
        <Field("record_count")>
        Public Property RecordCount As Integer?

        ''' <summary>
        ''' assembly_count 属性
        ''' </summary>
        <Field("assembly_count")>
        Public Property AssemblyCount As Integer?

        ''' <summary>
        ''' resource_updated_on 属性
        ''' </summary>
        <Field("resource_updated_on")>
        Public Property ResourceUpdatedOn As DateTime?

        ''' <summary>
        ''' hydrated 属性
        ''' </summary>
        <Field("hydrated")>
        Public Property Hydrated As Object

        ''' <summary>
        ''' dehydrated 属性
        ''' </summary>
        <Field("dehydrated")>
        Public Property Dehydrated As Object

        ''' <summary>
        ''' errors 属性
        ''' </summary>
        <Field("errors")>
        Public Property Errors As List(Of Object)

        ''' <summary>
        ''' messages 属性
        ''' </summary>
        <Field("messages")>
        Public Property Messages As List(Of Object)

        ''' <summary>
        ''' available_files 属性
        ''' </summary>
        <Field("available_files")>
        Public Property AvailableFiles As Object

    End Class

End Namespace
