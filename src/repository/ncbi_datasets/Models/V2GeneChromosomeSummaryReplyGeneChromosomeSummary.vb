' ============================================================================
' V2GeneChromosomeSummaryReplyGeneChromosomeSummary.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2GeneChromosomeSummaryReplyGeneChromosomeSummary
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2GeneChromosomeSummaryReplyGeneChromosomeSummary

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <JsonProperty("name")>
        Public Property Name As String

        ''' <summary>
        ''' count 属性
        ''' </summary>
        <JsonProperty("count")>
        Public Property Count As Integer?

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <JsonProperty("accession")>
        Public Property Accession As String

    End Class

End Namespace
