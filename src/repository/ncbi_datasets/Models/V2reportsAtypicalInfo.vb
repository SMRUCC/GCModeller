' ============================================================================
' V2reportsAtypicalInfo.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsAtypicalInfo
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsAtypicalInfo

        ''' <summary>
        ''' is_atypical 属性
        ''' </summary>
        <JsonProperty("is_atypical")>
        Public Property IsAtypical As Boolean?

        ''' <summary>
        ''' warnings 属性
        ''' </summary>
        <JsonProperty("warnings")>
        Public Property Warnings As List(Of String)

    End Class

End Namespace
