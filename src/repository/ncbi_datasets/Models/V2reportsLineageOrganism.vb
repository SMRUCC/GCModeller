' ============================================================================
' V2reportsLineageOrganism.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsLineageOrganism
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsLineageOrganism

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <JsonProperty("tax_id")>
        Public Property TaxId As Integer?

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <JsonProperty("name")>
        Public Property Name As String

    End Class

End Namespace
