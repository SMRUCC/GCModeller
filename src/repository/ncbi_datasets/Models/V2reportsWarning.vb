' ============================================================================
' V2reportsWarning.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsWarning
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsWarning

        ''' <summary>
        ''' gene_warning_code 属性
        ''' </summary>
        <Field("gene_warning_code")>
        Public Property GeneWarningCode As Object

        ''' <summary>
        ''' reason 属性
        ''' </summary>
        <Field("reason")>
        Public Property Reason As String

        ''' <summary>
        ''' message 属性
        ''' </summary>
        <Field("message")>
        Public Property Message As String

        ''' <summary>
        ''' replaced_id 属性
        ''' </summary>
        <Field("replaced_id")>
        Public Property ReplacedId As Object

        ''' <summary>
        ''' unrecognized_identifier 属性
        ''' </summary>
        <Field("unrecognized_identifier")>
        Public Property UnrecognizedIdentifier As String

    End Class

End Namespace
