' ============================================================================
' RpcStatus.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: rpcStatus
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class RpcStatus

        ''' <summary>
        ''' code 属性
        ''' </summary>
        <JsonProperty("code")>
        Public Property Code As Integer?

        ''' <summary>
        ''' message 属性
        ''' </summary>
        <JsonProperty("message")>
        Public Property Message As String

        ''' <summary>
        ''' details 属性
        ''' </summary>
        <JsonProperty("details")>
        Public Property Details As List(Of Object)

    End Class

End Namespace
