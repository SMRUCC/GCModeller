' ============================================================================
' V2reportsSequenceFeature.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsSequenceFeature
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsSequenceFeature

        ''' <summary>
        ''' type 属性
        ''' </summary>
        <JsonProperty("type")>
        Public Property Type As String

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <JsonProperty("name")>
        Public Property Name As String

        ''' <summary>
        ''' locus_tag 属性
        ''' </summary>
        <JsonProperty("locus_tag")>
        Public Property LocusTag As String

        ''' <summary>
        ''' gene_id 属性
        ''' </summary>
        <JsonProperty("gene_id")>
        Public Property GeneId As String

        ''' <summary>
        ''' location 属性
        ''' </summary>
        <JsonProperty("location")>
        Public Property Location As Object

        ''' <summary>
        ''' other_names 属性
        ''' </summary>
        <JsonProperty("other_names")>
        Public Property OtherNames As List(Of String)

        ''' <summary>
        ''' ec_number 属性
        ''' </summary>
        <JsonProperty("ec_number")>
        Public Property EcNumber As List(Of String)

        ''' <summary>
        ''' coded_protein_info 属性
        ''' </summary>
        <JsonProperty("coded_protein_info")>
        Public Property CodedProteinInfo As Object

        ''' <summary>
        ''' prediction_source 属性
        ''' </summary>
        <JsonProperty("prediction_source")>
        Public Property PredictionSource As Object

        ''' <summary>
        ''' data_provenance 属性
        ''' </summary>
        <JsonProperty("data_provenance")>
        Public Property DataProvenance As Object

        ''' <summary>
        ''' signal_sequence 属性
        ''' </summary>
        <JsonProperty("signal_sequence")>
        Public Property SignalSequence As String

        ''' <summary>
        ''' nested_features 属性
        ''' </summary>
        <JsonProperty("nested_features")>
        Public Property NestedFeatures As List(Of Object)

    End Class

End Namespace
