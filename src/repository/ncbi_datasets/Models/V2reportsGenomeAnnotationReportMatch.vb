' ============================================================================
' V2reportsGenomeAnnotationReportMatch.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsGenomeAnnotationReportMatch
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsGenomeAnnotationReportMatch

        ''' <summary>
        ''' annotation 属性
        ''' </summary>
        <Field("annotation")>
        Public Property Annotation As Object

        ''' <summary>
        ''' query 属性
        ''' </summary>
        <Field("query")>
        Public Property Query As List(Of String)

        ''' <summary>
        ''' warning 属性
        ''' </summary>
        <Field("warning")>
        Public Property Warning As Object

        ''' <summary>
        ''' errors 属性
        ''' </summary>
        <Field("errors")>
        Public Property Errors As List(Of Object)

        ''' <summary>
        ''' row_id 属性
        ''' </summary>
        <Field("row_id")>
        Public Property RowId As String

    End Class

End Namespace
