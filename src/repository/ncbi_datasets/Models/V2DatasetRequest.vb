' ============================================================================
' V2DatasetRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2DatasetRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2DatasetRequest

        ''' <summary>
        ''' genome_v2 属性
        ''' </summary>
        <JsonProperty("genome_v2")>
        Public Property GenomeV2 As Object

        ''' <summary>
        ''' gene_v2 属性
        ''' </summary>
        <JsonProperty("gene_v2")>
        Public Property GeneV2 As Object

        ''' <summary>
        ''' virus_v2 属性
        ''' </summary>
        <JsonProperty("virus_v2")>
        Public Property VirusV2 As Object

    End Class

End Namespace
