' ============================================================================
' V2reportsSeqRangeSet.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsSeqRangeSet
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsSeqRangeSet

        ''' <summary>
        ''' accession_version 属性
        ''' </summary>
        <JsonProperty("accession_version")>
        Public Property AccessionVersion As String

        ''' <summary>
        ''' range 属性
        ''' </summary>
        <JsonProperty("range")>
        Public Property Range As List(Of Object)

    End Class

End Namespace
