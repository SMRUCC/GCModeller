' ============================================================================
' V2reportsPublication.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsPublication
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsPublication

        ''' <summary>
        ''' pmid 属性
        ''' </summary>
        <Field("pmid")>
        Public Property Pmid As Integer?

        ''' <summary>
        ''' title 属性
        ''' </summary>
        <Field("title")>
        Public Property Title As String

        ''' <summary>
        ''' date 属性
        ''' </summary>
        <Field("date")>
        Public Property Date As String

        ''' <summary>
        ''' authors 属性
        ''' </summary>
        <Field("authors")>
        Public Property Authors As List(Of Object)

    End Class

End Namespace
