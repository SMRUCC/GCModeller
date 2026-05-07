' ============================================================================
' V2GeneLinksRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2GeneLinksRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2GeneLinksRequest

        ''' <summary>
        ''' gene_ids 属性
        ''' </summary>
        <JsonProperty("gene_ids")>
        Public Property GeneIds As List(Of Integer)

    End Class

End Namespace
