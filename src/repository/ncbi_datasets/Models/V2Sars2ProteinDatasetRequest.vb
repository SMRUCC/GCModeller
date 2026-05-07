' ============================================================================
' V2Sars2ProteinDatasetRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2Sars2ProteinDatasetRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2Sars2ProteinDatasetRequest

        ''' <summary>
        ''' proteins 属性
        ''' </summary>
        <JsonProperty("proteins")>
        Public Property Proteins As List(Of String)

        ''' <summary>
        ''' refseq_only 属性
        ''' </summary>
        <JsonProperty("refseq_only")>
        Public Property RefseqOnly As Boolean?

        ''' <summary>
        ''' annotated_only 属性
        ''' </summary>
        <JsonProperty("annotated_only")>
        Public Property AnnotatedOnly As Boolean?

        ''' <summary>
        ''' released_since 属性
        ''' </summary>
        <JsonProperty("released_since")>
        Public Property ReleasedSince As DateTime?

        ''' <summary>
        ''' updated_since 属性
        ''' </summary>
        <JsonProperty("updated_since")>
        Public Property UpdatedSince As DateTime?

        ''' <summary>
        ''' host 属性
        ''' </summary>
        <JsonProperty("host")>
        Public Property Host As String

        ''' <summary>
        ''' pangolin_classification 属性
        ''' </summary>
        <JsonProperty("pangolin_classification")>
        Public Property PangolinClassification As String

        ''' <summary>
        ''' geo_location 属性
        ''' </summary>
        <JsonProperty("geo_location")>
        Public Property GeoLocation As String

        ''' <summary>
        ''' usa_state 属性
        ''' </summary>
        <JsonProperty("usa_state")>
        Public Property UsaState As String

        ''' <summary>
        ''' complete_only 属性
        ''' </summary>
        <JsonProperty("complete_only")>
        Public Property CompleteOnly As Boolean?

        ''' <summary>
        ''' table_fields 属性
        ''' </summary>
        <JsonProperty("table_fields")>
        Public Property TableFields As List(Of Object)

        ''' <summary>
        ''' include_sequence 属性
        ''' </summary>
        <JsonProperty("include_sequence")>
        Public Property IncludeSequence As List(Of Object)

        ''' <summary>
        ''' aux_report 属性
        ''' </summary>
        <JsonProperty("aux_report")>
        Public Property AuxReport As List(Of Object)

        ''' <summary>
        ''' format 属性
        ''' </summary>
        <JsonProperty("format")>
        Public Property Format As Object

    End Class

End Namespace
