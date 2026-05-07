' ============================================================================
' V2TaxonomyFilteredSubtreeRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2TaxonomyFilteredSubtreeRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2TaxonomyFilteredSubtreeRequest

        ''' <summary>
        ''' taxons 属性
        ''' </summary>
        <Field("taxons")>
        Public Property Taxons As List(Of String)

        ''' <summary>
        ''' specified_limit 属性
        ''' </summary>
        <Field("specified_limit")>
        Public Property SpecifiedLimit As Boolean?

        ''' <summary>
        ''' exclude_extinct 属性
        ''' </summary>
        <Field("exclude_extinct")>
        Public Property ExcludeExtinct As Boolean?

        ''' <summary>
        ''' levels 属性
        ''' </summary>
        <Field("levels")>
        Public Property Levels As Integer?

        ''' <summary>
        ''' rank_limits 属性
        ''' </summary>
        <Field("rank_limits")>
        Public Property RankLimits As List(Of Object)

        ''' <summary>
        ''' include_incertae_sedis 属性
        ''' </summary>
        <Field("include_incertae_sedis")>
        Public Property IncludeIncertaeSedis As Boolean?

    End Class

End Namespace
