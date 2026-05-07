' ============================================================================
' V2GeneCountsByTaxonReplyGeneTypeAndCount.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2GeneCountsByTaxonReplyGeneTypeAndCount
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2GeneCountsByTaxonReplyGeneTypeAndCount

        ''' <summary>
        ''' gene_type 属性
        ''' </summary>
        <JsonProperty("gene_type")>
        Public Property GeneType As String

        ''' <summary>
        ''' count 属性
        ''' </summary>
        <JsonProperty("count")>
        Public Property Count As Integer?

    End Class

End Namespace
