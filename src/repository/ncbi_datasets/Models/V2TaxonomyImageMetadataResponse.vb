' ============================================================================
' V2TaxonomyImageMetadataResponse.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2TaxonomyImageMetadataResponse
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2TaxonomyImageMetadataResponse

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <JsonProperty("tax_id")>
        Public Property TaxId As String

        ''' <summary>
        ''' src 属性
        ''' </summary>
        <JsonProperty("src")>
        Public Property Src As String

        ''' <summary>
        ''' license 属性
        ''' </summary>
        <JsonProperty("license")>
        Public Property License As String

        ''' <summary>
        ''' attribution 属性
        ''' </summary>
        <JsonProperty("attribution")>
        Public Property Attribution As String

        ''' <summary>
        ''' source 属性
        ''' </summary>
        <JsonProperty("source")>
        Public Property Source As String

        ''' <summary>
        ''' image_sizes 属性
        ''' </summary>
        <JsonProperty("image_sizes")>
        Public Property ImageSizes As List(Of Object)

        ''' <summary>
        ''' format 属性
        ''' </summary>
        <JsonProperty("format")>
        Public Property Format As String

        ''' <summary>
        ''' license_url 属性
        ''' </summary>
        <JsonProperty("license_url")>
        Public Property LicenseUrl As String

    End Class

End Namespace
