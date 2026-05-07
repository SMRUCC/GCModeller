' ============================================================================
' V2GeneChromosomeSummaryRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2GeneChromosomeSummaryRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2GeneChromosomeSummaryRequest

        ''' <summary>
        ''' taxon 属性
        ''' </summary>
        <JsonProperty("taxon")>
        Public Property Taxon As String

        ''' <summary>
        ''' annotation_name 属性
        ''' </summary>
        <JsonProperty("annotation_name")>
        Public Property AnnotationName As String

    End Class

End Namespace
