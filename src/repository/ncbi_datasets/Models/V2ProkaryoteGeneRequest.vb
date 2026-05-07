' ============================================================================
' V2ProkaryoteGeneRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2ProkaryoteGeneRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2ProkaryoteGeneRequest

        ''' <summary>
        ''' accessions 属性
        ''' </summary>
        <JsonProperty("accessions")>
        Public Property Accessions As List(Of String)

        ''' <summary>
        ''' include_annotation_type 属性
        ''' </summary>
        <JsonProperty("include_annotation_type")>
        Public Property IncludeAnnotationType As List(Of Object)

        ''' <summary>
        ''' gene_flank_config 属性
        ''' </summary>
        <JsonProperty("gene_flank_config")>
        Public Property GeneFlankConfig As Object

        ''' <summary>
        ''' taxon 属性
        ''' </summary>
        <JsonProperty("taxon")>
        Public Property Taxon As String

    End Class

End Namespace
