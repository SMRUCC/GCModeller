' ============================================================================
' V2reportsBiocollectionsReportPage.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsBiocollectionsReportPage
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsBiocollectionsReportPage

        ''' <summary>
        ''' reports 属性
        ''' </summary>
        <Field("reports")>
        Public Property Reports As List(Of Object)

        ''' <summary>
        ''' messages 属性
        ''' </summary>
        <Field("messages")>
        Public Property Messages As List(Of Object)

        ''' <summary>
        ''' total_count 属性
        ''' </summary>
        <Field("total_count")>
        Public Property TotalCount As Integer?

        ''' <summary>
        ''' next_page_token 属性
        ''' </summary>
        <Field("next_page_token")>
        Public Property NextPageToken As String

    End Class

End Namespace
