' ============================================================================
' V2reportsVirusAnnotationReport.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsVirusAnnotationReport
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsVirusAnnotationReport

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <Field("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' isolate_name 属性
        ''' </summary>
        <Field("isolate_name")>
        Public Property IsolateName As String

        ''' <summary>
        ''' genes 属性
        ''' </summary>
        <Field("genes")>
        Public Property Genes As List(Of Object)

    End Class

End Namespace
