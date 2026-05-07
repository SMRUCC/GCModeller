' ============================================================================
' V2reportsAnnotationInfo.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsAnnotationInfo
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsAnnotationInfo

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <JsonProperty("name")>
        Public Property Name As String

        ''' <summary>
        ''' provider 属性
        ''' </summary>
        <JsonProperty("provider")>
        Public Property Provider As String

        ''' <summary>
        ''' release_date 属性
        ''' </summary>
        <JsonProperty("release_date")>
        Public Property ReleaseDate As String

        ''' <summary>
        ''' report_url 属性
        ''' </summary>
        <JsonProperty("report_url")>
        Public Property ReportUrl As String

        ''' <summary>
        ''' stats 属性
        ''' </summary>
        <JsonProperty("stats")>
        Public Property Stats As Object

        ''' <summary>
        ''' busco 属性
        ''' </summary>
        <JsonProperty("busco")>
        Public Property Busco As Object

        ''' <summary>
        ''' method 属性
        ''' </summary>
        <JsonProperty("method")>
        Public Property Method As String

        ''' <summary>
        ''' pipeline 属性
        ''' </summary>
        <JsonProperty("pipeline")>
        Public Property Pipeline As String

        ''' <summary>
        ''' software_version 属性
        ''' </summary>
        <JsonProperty("software_version")>
        Public Property SoftwareVersion As String

        ''' <summary>
        ''' status 属性
        ''' </summary>
        <JsonProperty("status")>
        Public Property Status As String

        ''' <summary>
        ''' release_version 属性
        ''' </summary>
        <JsonProperty("release_version")>
        Public Property ReleaseVersion As String

    End Class

End Namespace
