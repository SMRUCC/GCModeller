' ============================================================================
' V2TaxonomyImageMetadataResponse.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2TaxonomyImageMetadataResponse
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2TaxonomyImageMetadataResponse

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <Field("tax_id")>
        Public Property TaxId As String

        ''' <summary>
        ''' src 属性
        ''' </summary>
        <Field("src")>
        Public Property Src As String

        ''' <summary>
        ''' license 属性
        ''' </summary>
        <Field("license")>
        Public Property License As String

        ''' <summary>
        ''' attribution 属性
        ''' </summary>
        <Field("attribution")>
        Public Property Attribution As String

        ''' <summary>
        ''' source 属性
        ''' </summary>
        <Field("source")>
        Public Property Source As String

        ''' <summary>
        ''' image_sizes 属性
        ''' </summary>
        <Field("image_sizes")>
        Public Property ImageSizes As List(Of Object)

        ''' <summary>
        ''' format 属性
        ''' </summary>
        <Field("format")>
        Public Property Format As String

        ''' <summary>
        ''' license_url 属性
        ''' </summary>
        <Field("license_url")>
        Public Property LicenseUrl As String

    End Class

End Namespace
