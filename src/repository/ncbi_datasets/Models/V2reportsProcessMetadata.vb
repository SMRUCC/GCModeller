' ============================================================================
' V2reportsProcessMetadata.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsProcessMetadata
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsProcessMetadata

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' go_id 属性
        ''' </summary>
        <Field("go_id")>
        Public Property GoId As String

        ''' <summary>
        ''' evidence_code 属性
        ''' </summary>
        <Field("evidence_code")>
        Public Property EvidenceCode As String

        ''' <summary>
        ''' qualifier 属性
        ''' </summary>
        <Field("qualifier")>
        Public Property Qualifier As String

        ''' <summary>
        ''' reference 属性
        ''' </summary>
        <Field("reference")>
        Public Property Reference As Object

    End Class

End Namespace
