' ============================================================================
' V2reportsTaxonomyNamesDescriptorCitation.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsTaxonomyNamesDescriptorCitation
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsTaxonomyNamesDescriptorCitation

        ''' <summary>
        ''' full_citation 属性
        ''' </summary>
        <JsonProperty("full_citation")>
        Public Property FullCitation As String

        ''' <summary>
        ''' short_citation 属性
        ''' </summary>
        <JsonProperty("short_citation")>
        Public Property ShortCitation As String

        ''' <summary>
        ''' pmid 属性
        ''' </summary>
        <JsonProperty("pmid")>
        Public Property Pmid As String

    End Class

End Namespace
