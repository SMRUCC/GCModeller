' ============================================================================
' V2reportsBioSampleDescriptor.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsBioSampleDescriptor
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsBioSampleDescriptor

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <JsonProperty("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' last_updated 属性
        ''' </summary>
        <JsonProperty("last_updated")>
        Public Property LastUpdated As String

        ''' <summary>
        ''' publication_date 属性
        ''' </summary>
        <JsonProperty("publication_date")>
        Public Property PublicationDate As String

        ''' <summary>
        ''' submission_date 属性
        ''' </summary>
        <JsonProperty("submission_date")>
        Public Property SubmissionDate As String

        ''' <summary>
        ''' sample_ids 属性
        ''' </summary>
        <JsonProperty("sample_ids")>
        Public Property SampleIds As List(Of Object)

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <JsonProperty("description")>
        Public Property Description As Object

        ''' <summary>
        ''' owner 属性
        ''' </summary>
        <JsonProperty("owner")>
        Public Property Owner As Object

        ''' <summary>
        ''' models 属性
        ''' </summary>
        <JsonProperty("models")>
        Public Property Models As List(Of String)

        ''' <summary>
        ''' bioprojects 属性
        ''' </summary>
        <JsonProperty("bioprojects")>
        Public Property Bioprojects As List(Of Object)

        ''' <summary>
        ''' package 属性
        ''' </summary>
        <JsonProperty("package")>
        Public Property Package As String

        ''' <summary>
        ''' attributes 属性
        ''' </summary>
        <JsonProperty("attributes")>
        Public Property Attributes As List(Of Object)

        ''' <summary>
        ''' status 属性
        ''' </summary>
        <JsonProperty("status")>
        Public Property Status As Object

        ''' <summary>
        ''' age 属性
        ''' </summary>
        <JsonProperty("age")>
        Public Property Age As String

        ''' <summary>
        ''' biomaterial_provider 属性
        ''' </summary>
        <JsonProperty("biomaterial_provider")>
        Public Property BiomaterialProvider As String

        ''' <summary>
        ''' breed 属性
        ''' </summary>
        <JsonProperty("breed")>
        Public Property Breed As String

        ''' <summary>
        ''' collected_by 属性
        ''' </summary>
        <JsonProperty("collected_by")>
        Public Property CollectedBy As String

        ''' <summary>
        ''' collection_date 属性
        ''' </summary>
        <JsonProperty("collection_date")>
        Public Property CollectionDate As String

        ''' <summary>
        ''' cultivar 属性
        ''' </summary>
        <JsonProperty("cultivar")>
        Public Property Cultivar As String

        ''' <summary>
        ''' dev_stage 属性
        ''' </summary>
        <JsonProperty("dev_stage")>
        Public Property DevStage As String

        ''' <summary>
        ''' ecotype 属性
        ''' </summary>
        <JsonProperty("ecotype")>
        Public Property Ecotype As String

        ''' <summary>
        ''' geo_loc_name 属性
        ''' </summary>
        <JsonProperty("geo_loc_name")>
        Public Property GeoLocName As String

        ''' <summary>
        ''' host 属性
        ''' </summary>
        <JsonProperty("host")>
        Public Property Host As String

        ''' <summary>
        ''' host_disease 属性
        ''' </summary>
        <JsonProperty("host_disease")>
        Public Property HostDisease As String

        ''' <summary>
        ''' identified_by 属性
        ''' </summary>
        <JsonProperty("identified_by")>
        Public Property IdentifiedBy As String

        ''' <summary>
        ''' ifsac_category 属性
        ''' </summary>
        <JsonProperty("ifsac_category")>
        Public Property IfsacCategory As String

        ''' <summary>
        ''' isolate 属性
        ''' </summary>
        <JsonProperty("isolate")>
        Public Property Isolate As String

        ''' <summary>
        ''' isolate_name_alias 属性
        ''' </summary>
        <JsonProperty("isolate_name_alias")>
        Public Property IsolateNameAlias As String

        ''' <summary>
        ''' isolation_source 属性
        ''' </summary>
        <JsonProperty("isolation_source")>
        Public Property IsolationSource As String

        ''' <summary>
        ''' lat_lon 属性
        ''' </summary>
        <JsonProperty("lat_lon")>
        Public Property LatLon As String

        ''' <summary>
        ''' project_name 属性
        ''' </summary>
        <JsonProperty("project_name")>
        Public Property ProjectName As String

        ''' <summary>
        ''' sample_name 属性
        ''' </summary>
        <JsonProperty("sample_name")>
        Public Property SampleName As String

        ''' <summary>
        ''' serovar 属性
        ''' </summary>
        <JsonProperty("serovar")>
        Public Property Serovar As String

        ''' <summary>
        ''' sex 属性
        ''' </summary>
        <JsonProperty("sex")>
        Public Property Sex As String

        ''' <summary>
        ''' source_type 属性
        ''' </summary>
        <JsonProperty("source_type")>
        Public Property SourceType As String

        ''' <summary>
        ''' strain 属性
        ''' </summary>
        <JsonProperty("strain")>
        Public Property Strain As String

        ''' <summary>
        ''' sub_species 属性
        ''' </summary>
        <JsonProperty("sub_species")>
        Public Property SubSpecies As String

        ''' <summary>
        ''' tissue 属性
        ''' </summary>
        <JsonProperty("tissue")>
        Public Property Tissue As String

        ''' <summary>
        ''' serotype 属性
        ''' </summary>
        <JsonProperty("serotype")>
        Public Property Serotype As String

    End Class

End Namespace
