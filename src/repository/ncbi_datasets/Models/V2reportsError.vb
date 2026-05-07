' ============================================================================
' V2reportsError.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsError
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsError

        ''' <summary>
        ''' assembly_error_code 属性
        ''' </summary>
        <Field("assembly_error_code")>
        Public Property AssemblyErrorCode As Object

        ''' <summary>
        ''' gene_error_code 属性
        ''' </summary>
        <Field("gene_error_code")>
        Public Property GeneErrorCode As Object

        ''' <summary>
        ''' organelle_error_code 属性
        ''' </summary>
        <Field("organelle_error_code")>
        Public Property OrganelleErrorCode As Object

        ''' <summary>
        ''' virus_error_code 属性
        ''' </summary>
        <Field("virus_error_code")>
        Public Property VirusErrorCode As Object

        ''' <summary>
        ''' taxonomy_error_code 属性
        ''' </summary>
        <Field("taxonomy_error_code")>
        Public Property TaxonomyErrorCode As Object

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
        ''' invalid_identifiers 属性
        ''' </summary>
        <Field("invalid_identifiers")>
        Public Property InvalidIdentifiers As List(Of String)

    End Class

End Namespace
