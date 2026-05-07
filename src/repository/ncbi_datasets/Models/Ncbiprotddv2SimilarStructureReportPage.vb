' ============================================================================
' Ncbiprotddv2SimilarStructureReportPage.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2SimilarStructureReportPage
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2SimilarStructureReportPage

        ''' <summary>
        ''' similar_structures 属性
        ''' </summary>
        <JsonProperty("similar_structures")>
        Public Property SimilarStructures As List(Of Object)

        ''' <summary>
        ''' next_page_token 属性
        ''' </summary>
        <JsonProperty("next_page_token")>
        Public Property NextPageToken As String

        ''' <summary>
        ''' total_count 属性
        ''' </summary>
        <JsonProperty("total_count")>
        Public Property TotalCount As Integer?

    End Class

End Namespace
