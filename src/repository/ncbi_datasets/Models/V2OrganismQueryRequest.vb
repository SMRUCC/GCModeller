' ============================================================================
' V2OrganismQueryRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2OrganismQueryRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2OrganismQueryRequest

        ''' <summary>
        ''' organism_query 属性
        ''' </summary>
        <JsonProperty("organism_query")>
        Public Property OrganismQuery As String

        ''' <summary>
        ''' taxon_query 属性
        ''' </summary>
        <JsonProperty("taxon_query")>
        Public Property TaxonQuery As String

        ''' <summary>
        ''' tax_rank_filter 属性
        ''' </summary>
        <JsonProperty("tax_rank_filter")>
        Public Property TaxRankFilter As Object

        ''' <summary>
        ''' taxon_resource_filter 属性
        ''' </summary>
        <JsonProperty("taxon_resource_filter")>
        Public Property TaxonResourceFilter As Object

        ''' <summary>
        ''' exact_match 属性
        ''' </summary>
        <JsonProperty("exact_match")>
        Public Property ExactMatch As Boolean?

    End Class

End Namespace
