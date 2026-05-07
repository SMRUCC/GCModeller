' ============================================================================
' V2GeneDatasetRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2GeneDatasetRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2GeneDatasetRequest

        ''' <summary>
        ''' gene_ids 属性
        ''' </summary>
        <JsonProperty("gene_ids")>
        Public Property GeneIds As List(Of Integer)

        ''' <summary>
        ''' include_annotation_type 属性
        ''' </summary>
        <JsonProperty("include_annotation_type")>
        Public Property IncludeAnnotationType As List(Of Object)

        ''' <summary>
        ''' returned_content 属性
        ''' </summary>
        <JsonProperty("returned_content")>
        Public Property ReturnedContent As Object

        ''' <summary>
        ''' fasta_filter 属性
        ''' </summary>
        <JsonProperty("fasta_filter")>
        Public Property FastaFilter As List(Of String)

        ''' <summary>
        ''' accession_filter 属性
        ''' </summary>
        <JsonProperty("accession_filter")>
        Public Property AccessionFilter As List(Of String)

        ''' <summary>
        ''' aux_report 属性
        ''' </summary>
        <JsonProperty("aux_report")>
        Public Property AuxReport As List(Of Object)

        ''' <summary>
        ''' tabular_reports 属性
        ''' </summary>
        <JsonProperty("tabular_reports")>
        Public Property TabularReports As List(Of Object)

        ''' <summary>
        ''' table_fields 属性
        ''' </summary>
        <JsonProperty("table_fields")>
        Public Property TableFields As List(Of String)

        ''' <summary>
        ''' table_report_type 属性
        ''' </summary>
        <JsonProperty("table_report_type")>
        Public Property TableReportType As Object

    End Class

End Namespace
