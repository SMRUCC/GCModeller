' ============================================================================
' V2reportsVirusAssemblySubmitterInfo.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsVirusAssemblySubmitterInfo
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsVirusAssemblySubmitterInfo

        ''' <summary>
        ''' names 属性
        ''' </summary>
        <JsonProperty("names")>
        Public Property Names As List(Of String)

        ''' <summary>
        ''' affiliation 属性
        ''' </summary>
        <JsonProperty("affiliation")>
        Public Property Affiliation As String

        ''' <summary>
        ''' country 属性
        ''' </summary>
        <JsonProperty("country")>
        Public Property Country As String

    End Class

End Namespace
