' ============================================================================
' V2reportsTranscript.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsTranscript
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsTranscript

        ''' <summary>
        ''' accession_version 属性
        ''' </summary>
        <JsonProperty("accession_version")>
        Public Property AccessionVersion As String

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <JsonProperty("name")>
        Public Property Name As String

        ''' <summary>
        ''' length 属性
        ''' </summary>
        <JsonProperty("length")>
        Public Property Length As Integer?

        ''' <summary>
        ''' cds 属性
        ''' </summary>
        <JsonProperty("cds")>
        Public Property Cds As Object

        ''' <summary>
        ''' genomic_locations 属性
        ''' </summary>
        <JsonProperty("genomic_locations")>
        Public Property GenomicLocations As List(Of Object)

        ''' <summary>
        ''' ensembl_transcript 属性
        ''' </summary>
        <JsonProperty("ensembl_transcript")>
        Public Property EnsemblTranscript As String

        ''' <summary>
        ''' protein 属性
        ''' </summary>
        <JsonProperty("protein")>
        Public Property Protein As Object

        ''' <summary>
        ''' type 属性
        ''' </summary>
        <JsonProperty("type")>
        Public Property Type As Object

        ''' <summary>
        ''' select_category 属性
        ''' </summary>
        <JsonProperty("select_category")>
        Public Property SelectCategory As Object

    End Class

End Namespace
