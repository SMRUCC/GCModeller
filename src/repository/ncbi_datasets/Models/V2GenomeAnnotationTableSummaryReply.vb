' ============================================================================
' V2GenomeAnnotationTableSummaryReply.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2GenomeAnnotationTableSummaryReply
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2GenomeAnnotationTableSummaryReply

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <JsonProperty("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' chromosomes 属性
        ''' </summary>
        <JsonProperty("chromosomes")>
        Public Property Chromosomes As List(Of String)

        ''' <summary>
        ''' gene_types 属性
        ''' </summary>
        <JsonProperty("gene_types")>
        Public Property GeneTypes As List(Of String)

        ''' <summary>
        ''' empty_columns 属性
        ''' </summary>
        <JsonProperty("empty_columns")>
        Public Property EmptyColumns As List(Of String)

    End Class

End Namespace
