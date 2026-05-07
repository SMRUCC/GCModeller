' ============================================================================
' V2reportsSequenceFeature.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsSequenceFeature
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsSequenceFeature

        ''' <summary>
        ''' type 属性
        ''' </summary>
        <Field("type")>
        Public Property Type As String

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' locus_tag 属性
        ''' </summary>
        <Field("locus_tag")>
        Public Property LocusTag As String

        ''' <summary>
        ''' gene_id 属性
        ''' </summary>
        <Field("gene_id")>
        Public Property GeneId As String

        ''' <summary>
        ''' location 属性
        ''' </summary>
        <Field("location")>
        Public Property Location As Object

        ''' <summary>
        ''' other_names 属性
        ''' </summary>
        <Field("other_names")>
        Public Property OtherNames As List(Of String)

        ''' <summary>
        ''' ec_number 属性
        ''' </summary>
        <Field("ec_number")>
        Public Property EcNumber As List(Of String)

        ''' <summary>
        ''' coded_protein_info 属性
        ''' </summary>
        <Field("coded_protein_info")>
        Public Property CodedProteinInfo As Object

        ''' <summary>
        ''' prediction_source 属性
        ''' </summary>
        <Field("prediction_source")>
        Public Property PredictionSource As Object

        ''' <summary>
        ''' data_provenance 属性
        ''' </summary>
        <Field("data_provenance")>
        Public Property DataProvenance As Object

        ''' <summary>
        ''' signal_sequence 属性
        ''' </summary>
        <Field("signal_sequence")>
        Public Property SignalSequence As String

        ''' <summary>
        ''' nested_features 属性
        ''' </summary>
        <Field("nested_features")>
        Public Property NestedFeatures As List(Of Object)

    End Class

End Namespace
