' ============================================================================
' V2TaxonomyRelatedIdRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2TaxonomyRelatedIdRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2TaxonomyRelatedIdRequest

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <JsonProperty("tax_id")>
        Public Property TaxId As Integer?

        ''' <summary>
        ''' include_lineage 属性
        ''' </summary>
        <JsonProperty("include_lineage")>
        Public Property IncludeLineage As Boolean?

        ''' <summary>
        ''' include_subtree 属性
        ''' </summary>
        <JsonProperty("include_subtree")>
        Public Property IncludeSubtree As Boolean?

        ''' <summary>
        ''' ranks 属性
        ''' </summary>
        <JsonProperty("ranks")>
        Public Property Ranks As List(Of Object)

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

    End Class

End Namespace
