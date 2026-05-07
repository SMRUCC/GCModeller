' ============================================================================
' V2GeneDatasetReportsRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2GeneDatasetReportsRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2GeneDatasetReportsRequest

        ''' <summary>
        ''' returned_content 属性
        ''' </summary>
        <JsonProperty("returned_content")>
        Public Property ReturnedContent As Object

        ''' <summary>
        ''' gene_ids 属性
        ''' </summary>
        <JsonProperty("gene_ids")>
        Public Property GeneIds As List(Of Integer)

        ''' <summary>
        ''' accessions 属性
        ''' </summary>
        <JsonProperty("accessions")>
        Public Property Accessions As List(Of String)

        ''' <summary>
        ''' symbols_for_taxon 属性
        ''' </summary>
        <JsonProperty("symbols_for_taxon")>
        Public Property SymbolsForTaxon As Object

        ''' <summary>
        ''' taxon 属性
        ''' </summary>
        <JsonProperty("taxon")>
        Public Property Taxon As String

        ''' <summary>
        ''' locus_tags 属性
        ''' </summary>
        <JsonProperty("locus_tags")>
        Public Property LocusTags As List(Of String)

        ''' <summary>
        ''' table_fields 属性
        ''' </summary>
        <JsonProperty("table_fields")>
        Public Property TableFields As List(Of String)

        ''' <summary>
        ''' table_format 属性
        ''' </summary>
        <JsonProperty("table_format")>
        Public Property TableFormat As String

        ''' <summary>
        ''' include_tabular_header 属性
        ''' </summary>
        <JsonProperty("include_tabular_header")>
        Public Property IncludeTabularHeader As Object

        ''' <summary>
        ''' page_size 属性
        ''' </summary>
        <JsonProperty("page_size")>
        Public Property PageSize As Integer?

        ''' <summary>
        ''' page_token 属性
        ''' </summary>
        <JsonProperty("page_token")>
        Public Property PageToken As String

        ''' <summary>
        ''' query 属性
        ''' </summary>
        <JsonProperty("query")>
        Public Property Query As String

        ''' <summary>
        ''' types 属性
        ''' </summary>
        <JsonProperty("types")>
        Public Property Types As List(Of Object)

        ''' <summary>
        ''' accession_filter 属性
        ''' </summary>
        <JsonProperty("accession_filter")>
        Public Property AccessionFilter As List(Of String)

        ''' <summary>
        ''' tax_search_subtree 属性
        ''' </summary>
        <JsonProperty("tax_search_subtree")>
        Public Property TaxSearchSubtree As Boolean?

        ''' <summary>
        ''' sort 属性
        ''' </summary>
        <JsonProperty("sort")>
        Public Property Sort As List(Of Object)

    End Class

End Namespace
