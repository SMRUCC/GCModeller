' ============================================================================
' V2AssemblyDatasetReportsRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2AssemblyDatasetReportsRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2AssemblyDatasetReportsRequest

        ''' <summary>
        ''' taxons 属性
        ''' </summary>
        <JsonProperty("taxons")>
        Public Property Taxons As List(Of String)

        ''' <summary>
        ''' bioprojects 属性
        ''' </summary>
        <JsonProperty("bioprojects")>
        Public Property Bioprojects As List(Of String)

        ''' <summary>
        ''' biosample_ids 属性
        ''' </summary>
        <JsonProperty("biosample_ids")>
        Public Property BiosampleIds As List(Of String)

        ''' <summary>
        ''' assembly_names 属性
        ''' </summary>
        <JsonProperty("assembly_names")>
        Public Property AssemblyNames As List(Of String)

        ''' <summary>
        ''' wgs_accessions 属性
        ''' </summary>
        <JsonProperty("wgs_accessions")>
        Public Property WgsAccessions As List(Of String)

        ''' <summary>
        ''' accessions 属性
        ''' </summary>
        <JsonProperty("accessions")>
        Public Property Accessions As List(Of String)

        ''' <summary>
        ''' filters 属性
        ''' </summary>
        <JsonProperty("filters")>
        Public Property Filters As Object

        ''' <summary>
        ''' tax_exact_match 属性
        ''' </summary>
        <JsonProperty("tax_exact_match")>
        Public Property TaxExactMatch As Boolean?

        ''' <summary>
        ''' chromosomes 属性
        ''' </summary>
        <JsonProperty("chromosomes")>
        Public Property Chromosomes As List(Of String)

        ''' <summary>
        ''' table_fields 属性
        ''' </summary>
        <JsonProperty("table_fields")>
        Public Property TableFields As List(Of String)

        ''' <summary>
        ''' returned_content 属性
        ''' </summary>
        <JsonProperty("returned_content")>
        Public Property ReturnedContent As Object

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
        ''' sort 属性
        ''' </summary>
        <JsonProperty("sort")>
        Public Property Sort As List(Of Object)

        ''' <summary>
        ''' include_tabular_header 属性
        ''' </summary>
        <JsonProperty("include_tabular_header")>
        Public Property IncludeTabularHeader As Object

        ''' <summary>
        ''' table_format 属性
        ''' </summary>
        <JsonProperty("table_format")>
        Public Property TableFormat As String

    End Class

End Namespace
