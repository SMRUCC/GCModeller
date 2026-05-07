' ============================================================================
' V2reportsSubmission.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsSubmission
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsSubmission

        ''' <summary>
        ''' date 属性
        ''' </summary>
        <Field("date")>
        Public Property Date As String

        ''' <summary>
        ''' institution 属性
        ''' </summary>
        <Field("institution")>
        Public Property Institution As String

        ''' <summary>
        ''' address 属性
        ''' </summary>
        <Field("address")>
        Public Property Address As String

        ''' <summary>
        ''' names 属性
        ''' </summary>
        <Field("names")>
        Public Property Names As List(Of String)

    End Class

End Namespace
