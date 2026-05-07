' ============================================================================
' V2reportsBioSampleOwner.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsBioSampleOwner
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsBioSampleOwner

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <JsonProperty("name")>
        Public Property Name As String

        ''' <summary>
        ''' contacts 属性
        ''' </summary>
        <JsonProperty("contacts")>
        Public Property Contacts As List(Of Object)

    End Class

End Namespace
