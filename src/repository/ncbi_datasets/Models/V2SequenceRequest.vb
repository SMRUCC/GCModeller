' ============================================================================
' V2SequenceRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2SequenceRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2SequenceRequest

        ''' <summary>
        ''' accessions 属性
        ''' </summary>
        <JsonProperty("accessions")>
        Public Property Accessions As List(Of String)

        ''' <summary>
        ''' include_all_versions 属性
        ''' </summary>
        <JsonProperty("include_all_versions")>
        Public Property IncludeAllVersions As Boolean?

    End Class

End Namespace
