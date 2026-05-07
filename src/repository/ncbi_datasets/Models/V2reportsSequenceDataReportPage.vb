' ============================================================================
' V2reportsSequenceDataReportPage.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsSequenceDataReportPage
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsSequenceDataReportPage

        ''' <summary>
        ''' reports 属性
        ''' </summary>
        <JsonProperty("reports")>
        Public Property Reports As List(Of Object)

        ''' <summary>
        ''' total_count 属性
        ''' </summary>
        <JsonProperty("total_count")>
        Public Property TotalCount As Integer?

    End Class

End Namespace
