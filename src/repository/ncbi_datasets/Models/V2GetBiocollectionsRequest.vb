' ============================================================================
' V2GetBiocollectionsRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2GetBiocollectionsRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2GetBiocollectionsRequest

        ''' <summary>
        ''' collection_ids 属性
        ''' </summary>
        <JsonProperty("collection_ids")>
        Public Property CollectionIds As List(Of String)

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

    End Class

End Namespace
