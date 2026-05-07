' ============================================================================
' V2reportsBioSampleDescription.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsBioSampleDescription
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsBioSampleDescription

        ''' <summary>
        ''' title 属性
        ''' </summary>
        <JsonProperty("title")>
        Public Property Title As String

        ''' <summary>
        ''' organism 属性
        ''' </summary>
        <JsonProperty("organism")>
        Public Property Organism As Object

        ''' <summary>
        ''' comment 属性
        ''' </summary>
        <JsonProperty("comment")>
        Public Property Comment As String

    End Class

End Namespace
