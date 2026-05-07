' ============================================================================
' V2TaxonomyLinksResponse.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2TaxonomyLinksResponse
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2TaxonomyLinksResponse

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <JsonProperty("tax_id")>
        Public Property TaxId As String

        ''' <summary>
        ''' encyclopedia_of_life 属性
        ''' </summary>
        <JsonProperty("encyclopedia_of_life")>
        Public Property EncyclopediaOfLife As String

        ''' <summary>
        ''' global_biodiversity_information_facility 属性
        ''' </summary>
        <JsonProperty("global_biodiversity_information_facility")>
        Public Property GlobalBiodiversityInformationFacility As String

        ''' <summary>
        ''' inaturalist 属性
        ''' </summary>
        <JsonProperty("inaturalist")>
        Public Property Inaturalist As String

        ''' <summary>
        ''' viralzone 属性
        ''' </summary>
        <JsonProperty("viralzone")>
        Public Property Viralzone As String

        ''' <summary>
        ''' wikipedia 属性
        ''' </summary>
        <JsonProperty("wikipedia")>
        Public Property Wikipedia As String

        ''' <summary>
        ''' generic_links 属性
        ''' </summary>
        <JsonProperty("generic_links")>
        Public Property GenericLinks As List(Of Object)

    End Class

End Namespace
