' ============================================================================
' V2reportsAssemblyDataReport.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsAssemblyDataReport
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsAssemblyDataReport

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <JsonProperty("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' current_accession 属性
        ''' </summary>
        <JsonProperty("current_accession")>
        Public Property CurrentAccession As String

        ''' <summary>
        ''' paired_accession 属性
        ''' </summary>
        <JsonProperty("paired_accession")>
        Public Property PairedAccession As String

        ''' <summary>
        ''' source_database 属性
        ''' </summary>
        <JsonProperty("source_database")>
        Public Property SourceDatabase As Object

        ''' <summary>
        ''' organism 属性
        ''' </summary>
        <JsonProperty("organism")>
        Public Property Organism As Object

        ''' <summary>
        ''' assembly_info 属性
        ''' </summary>
        <JsonProperty("assembly_info")>
        Public Property AssemblyInfo As Object

        ''' <summary>
        ''' assembly_stats 属性
        ''' </summary>
        <JsonProperty("assembly_stats")>
        Public Property AssemblyStats As Object

        ''' <summary>
        ''' organelle_info 属性
        ''' </summary>
        <JsonProperty("organelle_info")>
        Public Property OrganelleInfo As List(Of Object)

        ''' <summary>
        ''' additional_submitters 属性
        ''' </summary>
        <JsonProperty("additional_submitters")>
        Public Property AdditionalSubmitters As List(Of Object)

        ''' <summary>
        ''' annotation_info 属性
        ''' </summary>
        <JsonProperty("annotation_info")>
        Public Property AnnotationInfo As Object

        ''' <summary>
        ''' wgs_info 属性
        ''' </summary>
        <JsonProperty("wgs_info")>
        Public Property WgsInfo As Object

        ''' <summary>
        ''' type_material 属性
        ''' </summary>
        <JsonProperty("type_material")>
        Public Property TypeMaterial As Object

        ''' <summary>
        ''' checkm_info 属性
        ''' </summary>
        <JsonProperty("checkm_info")>
        Public Property CheckmInfo As Object

        ''' <summary>
        ''' average_nucleotide_identity 属性
        ''' </summary>
        <JsonProperty("average_nucleotide_identity")>
        Public Property AverageNucleotideIdentity As Object

    End Class

End Namespace
