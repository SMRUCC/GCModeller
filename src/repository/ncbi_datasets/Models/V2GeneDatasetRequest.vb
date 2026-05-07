' ============================================================================
' V2GeneDatasetRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2GeneDatasetRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2GeneDatasetRequest

        ''' <summary>
        ''' gene_ids 属性
        ''' </summary>
        <Field("gene_ids")>
        Public Property GeneIds As List(Of Integer)

        ''' <summary>
        ''' include_annotation_type 属性
        ''' </summary>
        <Field("include_annotation_type")>
        Public Property IncludeAnnotationType As List(Of Object)

        ''' <summary>
        ''' returned_content 属性
        ''' </summary>
        <Field("returned_content")>
        Public Property ReturnedContent As Object

        ''' <summary>
        ''' fasta_filter 属性
        ''' </summary>
        <Field("fasta_filter")>
        Public Property FastaFilter As List(Of String)

        ''' <summary>
        ''' accession_filter 属性
        ''' </summary>
        <Field("accession_filter")>
        Public Property AccessionFilter As List(Of String)

        ''' <summary>
        ''' aux_report 属性
        ''' </summary>
        <Field("aux_report")>
        Public Property AuxReport As List(Of Object)

        ''' <summary>
        ''' tabular_reports 属性
        ''' </summary>
        <Field("tabular_reports")>
        Public Property TabularReports As List(Of Object)

        ''' <summary>
        ''' table_fields 属性
        ''' </summary>
        <Field("table_fields")>
        Public Property TableFields As List(Of String)

        ''' <summary>
        ''' table_report_type 属性
        ''' </summary>
        <Field("table_report_type")>
        Public Property TableReportType As Object

    End Class

End Namespace
