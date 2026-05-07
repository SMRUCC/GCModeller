' ============================================================================
' V2reportsProductDescriptor.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsProductDescriptor
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsProductDescriptor

        ''' <summary>
        ''' gene_id 属性
        ''' </summary>
        <JsonProperty("gene_id")>
        Public Property GeneId As String

        ''' <summary>
        ''' symbol 属性
        ''' </summary>
        <JsonProperty("symbol")>
        Public Property Symbol As String

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <JsonProperty("description")>
        Public Property Description As String

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <JsonProperty("tax_id")>
        Public Property TaxId As String

        ''' <summary>
        ''' taxname 属性
        ''' </summary>
        <JsonProperty("taxname")>
        Public Property Taxname As String

        ''' <summary>
        ''' common_name 属性
        ''' </summary>
        <JsonProperty("common_name")>
        Public Property CommonName As String

        ''' <summary>
        ''' type 属性
        ''' </summary>
        <JsonProperty("type")>
        Public Property Type As Object

        ''' <summary>
        ''' rna_type 属性
        ''' </summary>
        <JsonProperty("rna_type")>
        Public Property RnaType As Object

        ''' <summary>
        ''' transcripts 属性
        ''' </summary>
        <JsonProperty("transcripts")>
        Public Property Transcripts As List(Of Object)

        ''' <summary>
        ''' transcript_count 属性
        ''' </summary>
        <JsonProperty("transcript_count")>
        Public Property TranscriptCount As Integer?

        ''' <summary>
        ''' protein_count 属性
        ''' </summary>
        <JsonProperty("protein_count")>
        Public Property ProteinCount As Integer?

        ''' <summary>
        ''' transcript_type_counts 属性
        ''' </summary>
        <JsonProperty("transcript_type_counts")>
        Public Property TranscriptTypeCounts As List(Of Object)

    End Class

End Namespace
