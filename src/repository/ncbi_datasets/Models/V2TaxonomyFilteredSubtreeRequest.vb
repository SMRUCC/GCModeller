' ============================================================================
' V2TaxonomyFilteredSubtreeRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2TaxonomyFilteredSubtreeRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2TaxonomyFilteredSubtreeRequest

        ''' <summary>
        ''' taxons 属性
        ''' </summary>
        <JsonProperty("taxons")>
        Public Property Taxons As List(Of String)

        ''' <summary>
        ''' specified_limit 属性
        ''' </summary>
        <JsonProperty("specified_limit")>
        Public Property SpecifiedLimit As Boolean?

        ''' <summary>
        ''' exclude_extinct 属性
        ''' </summary>
        <JsonProperty("exclude_extinct")>
        Public Property ExcludeExtinct As Boolean?

        ''' <summary>
        ''' levels 属性
        ''' </summary>
        <JsonProperty("levels")>
        Public Property Levels As Integer?

        ''' <summary>
        ''' rank_limits 属性
        ''' </summary>
        <JsonProperty("rank_limits")>
        Public Property RankLimits As List(Of Object)

        ''' <summary>
        ''' include_incertae_sedis 属性
        ''' </summary>
        <JsonProperty("include_incertae_sedis")>
        Public Property IncludeIncertaeSedis As Boolean?

    End Class

End Namespace
