' ============================================================================
' V2reportsAssemblyRevision.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsAssemblyRevision
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsAssemblyRevision

        ''' <summary>
        ''' genbank_accession 属性
        ''' </summary>
        <JsonProperty("genbank_accession")>
        Public Property GenbankAccession As String

        ''' <summary>
        ''' refseq_accession 属性
        ''' </summary>
        <JsonProperty("refseq_accession")>
        Public Property RefseqAccession As String

        ''' <summary>
        ''' assembly_name 属性
        ''' </summary>
        <JsonProperty("assembly_name")>
        Public Property AssemblyName As String

        ''' <summary>
        ''' assembly_level 属性
        ''' </summary>
        <JsonProperty("assembly_level")>
        Public Property AssemblyLevel As Object

        ''' <summary>
        ''' release_date 属性
        ''' </summary>
        <JsonProperty("release_date")>
        Public Property ReleaseDate As String

        ''' <summary>
        ''' submission_date 属性
        ''' </summary>
        <JsonProperty("submission_date")>
        Public Property SubmissionDate As String

        ''' <summary>
        ''' sequencing_technology 属性
        ''' </summary>
        <JsonProperty("sequencing_technology")>
        Public Property SequencingTechnology As String

        ''' <summary>
        ''' identical 属性
        ''' </summary>
        <JsonProperty("identical")>
        Public Property Identical As Boolean?

    End Class

End Namespace
