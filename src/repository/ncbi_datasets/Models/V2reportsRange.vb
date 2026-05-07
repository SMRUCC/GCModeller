' ============================================================================
' V2reportsRange.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsRange
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsRange

        ''' <summary>
        ''' begin 属性
        ''' </summary>
        <JsonProperty("begin")>
        Public Property Begin As String

        ''' <summary>
        ''' end 属性
        ''' </summary>
        <JsonProperty("end")>
        Public Property End As String

        ''' <summary>
        ''' orientation 属性
        ''' </summary>
        <JsonProperty("orientation")>
        Public Property Orientation As Object

        ''' <summary>
        ''' order 属性
        ''' </summary>
        <JsonProperty("order")>
        Public Property Order As Integer?

        ''' <summary>
        ''' ribosomal_slippage 属性
        ''' </summary>
        <JsonProperty("ribosomal_slippage")>
        Public Property RibosomalSlippage As Integer?

    End Class

End Namespace
