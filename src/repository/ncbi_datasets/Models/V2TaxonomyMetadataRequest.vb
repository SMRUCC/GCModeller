' ============================================================================
' V2TaxonomyMetadataRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2TaxonomyMetadataRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2TaxonomyMetadataRequest

        ''' <summary>
        ''' taxons 属性
        ''' </summary>
        <JsonProperty("taxons")>
        Public Property Taxons As List(Of String)

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
        ''' include_tabular_header 属性
        ''' </summary>
        <JsonProperty("include_tabular_header")>
        Public Property IncludeTabularHeader As Object

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
        ''' children 属性
        ''' </summary>
        <JsonProperty("children")>
        Public Property Children As Boolean?

        ''' <summary>
        ''' ranks 属性
        ''' </summary>
        <JsonProperty("ranks")>
        Public Property Ranks As List(Of Object)

    End Class

End Namespace
