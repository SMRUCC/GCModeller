' ============================================================================
' V2RefGeneCatalogDatasetRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2RefGeneCatalogDatasetRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2RefGeneCatalogDatasetRequest

        ''' <summary>
        ''' opaque_solr_query 属性
        ''' </summary>
        <JsonProperty("opaque_solr_query")>
        Public Property OpaqueSolrQuery As String

        ''' <summary>
        ''' files 属性
        ''' </summary>
        <JsonProperty("files")>
        Public Property Files As List(Of Object)

        ''' <summary>
        ''' element_flank_config 属性
        ''' </summary>
        <JsonProperty("element_flank_config")>
        Public Property ElementFlankConfig As Object

    End Class

End Namespace
