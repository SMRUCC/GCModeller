' ============================================================================
' ProtobufAny.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: protobufAny
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class ProtobufAny

        ''' <summary>
        ''' type_url 属性
        ''' </summary>
        <JsonProperty("type_url")>
        Public Property TypeUrl As String

        ''' <summary>
        ''' value 属性
        ''' </summary>
        <JsonProperty("value")>
        Public Property Value As Byte()

    End Class

End Namespace
