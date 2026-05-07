' ============================================================================
' V2HttpBody.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2HttpBody
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2HttpBody

        ''' <summary>
        ''' content_type 属性
        ''' </summary>
        <JsonProperty("content_type")>
        Public Property ContentType As String

        ''' <summary>
        ''' data 属性
        ''' </summary>
        <JsonProperty("data")>
        Public Property Data As Byte()

    End Class

End Namespace
