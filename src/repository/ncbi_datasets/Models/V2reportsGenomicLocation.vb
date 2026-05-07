' ============================================================================
' V2reportsGenomicLocation.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsGenomicLocation
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsGenomicLocation

        ''' <summary>
        ''' genomic_accession_version 属性
        ''' </summary>
        <Field("genomic_accession_version")>
        Public Property GenomicAccessionVersion As String

        ''' <summary>
        ''' sequence_name 属性
        ''' </summary>
        <Field("sequence_name")>
        Public Property SequenceName As String

        ''' <summary>
        ''' genomic_range 属性
        ''' </summary>
        <Field("genomic_range")>
        Public Property GenomicRange As Object

        ''' <summary>
        ''' exons 属性
        ''' </summary>
        <Field("exons")>
        Public Property Exons As List(Of Object)

    End Class

End Namespace
