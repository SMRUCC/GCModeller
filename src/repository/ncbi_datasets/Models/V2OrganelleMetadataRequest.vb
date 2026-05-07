' ============================================================================
' V2OrganelleMetadataRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2OrganelleMetadataRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2OrganelleMetadataRequest

        ''' <summary>
        ''' taxons 属性
        ''' </summary>
        <JsonProperty("taxons")>
        Public Property Taxons As List(Of String)

        ''' <summary>
        ''' accessions 属性
        ''' </summary>
        <JsonProperty("accessions")>
        Public Property Accessions As List(Of String)

        ''' <summary>
        ''' organelle_types 属性
        ''' </summary>
        <JsonProperty("organelle_types")>
        Public Property OrganelleTypes As List(Of Object)

        ''' <summary>
        ''' first_release_date 属性
        ''' </summary>
        <JsonProperty("first_release_date")>
        Public Property FirstReleaseDate As DateTime?

        ''' <summary>
        ''' last_release_date 属性
        ''' </summary>
        <JsonProperty("last_release_date")>
        Public Property LastReleaseDate As DateTime?

        ''' <summary>
        ''' tax_exact_match 属性
        ''' </summary>
        <JsonProperty("tax_exact_match")>
        Public Property TaxExactMatch As Boolean?

        ''' <summary>
        ''' sort 属性
        ''' </summary>
        <JsonProperty("sort")>
        Public Property Sort As List(Of Object)

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
        ''' table_format 属性
        ''' </summary>
        <JsonProperty("table_format")>
        Public Property TableFormat As Object

        ''' <summary>
        ''' include_tabular_header 属性
        ''' </summary>
        <JsonProperty("include_tabular_header")>
        Public Property IncludeTabularHeader As Object

    End Class

End Namespace
