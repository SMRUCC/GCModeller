' ============================================================================
' V2VirusAvailability.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2VirusAvailability
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2VirusAvailability

        ''' <summary>
        ''' valid_accessions 属性
        ''' </summary>
        <Field("valid_accessions")>
        Public Property ValidAccessions As List(Of String)

        ''' <summary>
        ''' invalid_accessions 属性
        ''' </summary>
        <Field("invalid_accessions")>
        Public Property InvalidAccessions As List(Of String)

        ''' <summary>
        ''' message 属性
        ''' </summary>
        <Field("message")>
        Public Property Message As String

    End Class

End Namespace
