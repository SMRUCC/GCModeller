' ============================================================================
' V2reportsProductDescriptor.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsProductDescriptor
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsProductDescriptor

        ''' <summary>
        ''' gene_id 属性
        ''' </summary>
        <Field("gene_id")>
        Public Property GeneId As String

        ''' <summary>
        ''' symbol 属性
        ''' </summary>
        <Field("symbol")>
        Public Property Symbol As String

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <Field("description")>
        Public Property Description As String

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <Field("tax_id")>
        Public Property TaxId As String

        ''' <summary>
        ''' taxname 属性
        ''' </summary>
        <Field("taxname")>
        Public Property Taxname As String

        ''' <summary>
        ''' common_name 属性
        ''' </summary>
        <Field("common_name")>
        Public Property CommonName As String

        ''' <summary>
        ''' type 属性
        ''' </summary>
        <Field("type")>
        Public Property Type As Object

        ''' <summary>
        ''' rna_type 属性
        ''' </summary>
        <Field("rna_type")>
        Public Property RnaType As Object

        ''' <summary>
        ''' transcripts 属性
        ''' </summary>
        <Field("transcripts")>
        Public Property Transcripts As List(Of Object)

        ''' <summary>
        ''' transcript_count 属性
        ''' </summary>
        <Field("transcript_count")>
        Public Property TranscriptCount As Integer?

        ''' <summary>
        ''' protein_count 属性
        ''' </summary>
        <Field("protein_count")>
        Public Property ProteinCount As Integer?

        ''' <summary>
        ''' transcript_type_counts 属性
        ''' </summary>
        <Field("transcript_type_counts")>
        Public Property TranscriptTypeCounts As List(Of Object)

    End Class

End Namespace
