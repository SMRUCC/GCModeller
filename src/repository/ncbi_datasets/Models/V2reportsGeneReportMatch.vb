' ============================================================================
' V2reportsGeneReportMatch.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsGeneReportMatch
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsGeneReportMatch

        ''' <summary>
        ''' gene 属性
        ''' </summary>
        <JsonProperty("gene")>
        Public Property Gene As Object

        ''' <summary>
        ''' product 属性
        ''' </summary>
        <JsonProperty("product")>
        Public Property Product As Object

        ''' <summary>
        ''' query 属性
        ''' </summary>
        <JsonProperty("query")>
        Public Property Query As List(Of String)

        ''' <summary>
        ''' warnings 属性
        ''' </summary>
        <JsonProperty("warnings")>
        Public Property Warnings As List(Of Object)

        ''' <summary>
        ''' warning 属性
        ''' </summary>
        <JsonProperty("warning")>
        Public Property Warning As Object

        ''' <summary>
        ''' errors 属性
        ''' </summary>
        <JsonProperty("errors")>
        Public Property Errors As List(Of Object)

    End Class

End Namespace
