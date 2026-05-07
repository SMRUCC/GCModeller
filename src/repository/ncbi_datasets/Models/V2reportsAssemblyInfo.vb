' ============================================================================
' V2reportsAssemblyInfo.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsAssemblyInfo
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsAssemblyInfo

        ''' <summary>
        ''' assembly_level 属性
        ''' </summary>
        <Field("assembly_level")>
        Public Property AssemblyLevel As String

        ''' <summary>
        ''' assembly_status 属性
        ''' </summary>
        <Field("assembly_status")>
        Public Property AssemblyStatus As Object

        ''' <summary>
        ''' paired_assembly 属性
        ''' </summary>
        <Field("paired_assembly")>
        Public Property PairedAssembly As Object

        ''' <summary>
        ''' assembly_name 属性
        ''' </summary>
        <Field("assembly_name")>
        Public Property AssemblyName As String

        ''' <summary>
        ''' assembly_long_name 属性
        ''' </summary>
        <Field("assembly_long_name")>
        Public Property AssemblyLongName As String

        ''' <summary>
        ''' assembly_type 属性
        ''' </summary>
        <Field("assembly_type")>
        Public Property AssemblyType As String

        ''' <summary>
        ''' bioproject_lineage 属性
        ''' </summary>
        <Field("bioproject_lineage")>
        Public Property BioprojectLineage As List(Of Object)

        ''' <summary>
        ''' bioproject_accession 属性
        ''' </summary>
        <Field("bioproject_accession")>
        Public Property BioprojectAccession As String

        ''' <summary>
        ''' submission_date 属性
        ''' </summary>
        <Field("submission_date")>
        Public Property SubmissionDate As String

        ''' <summary>
        ''' release_date 属性
        ''' </summary>
        <Field("release_date")>
        Public Property ReleaseDate As String

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <Field("description")>
        Public Property Description As String

        ''' <summary>
        ''' submitter 属性
        ''' </summary>
        <Field("submitter")>
        Public Property Submitter As String

        ''' <summary>
        ''' refseq_category 属性
        ''' </summary>
        <Field("refseq_category")>
        Public Property RefseqCategory As String

        ''' <summary>
        ''' synonym 属性
        ''' </summary>
        <Field("synonym")>
        Public Property Synonym As String

        ''' <summary>
        ''' linked_assembly 属性
        ''' </summary>
        <Field("linked_assembly")>
        Public Property LinkedAssembly As String

        ''' <summary>
        ''' linked_assemblies 属性
        ''' </summary>
        <Field("linked_assemblies")>
        Public Property LinkedAssemblies As List(Of Object)

        ''' <summary>
        ''' atypical 属性
        ''' </summary>
        <Field("atypical")>
        Public Property Atypical As Object

        ''' <summary>
        ''' genome_notes 属性
        ''' </summary>
        <Field("genome_notes")>
        Public Property GenomeNotes As List(Of String)

        ''' <summary>
        ''' sequencing_tech 属性
        ''' </summary>
        <Field("sequencing_tech")>
        Public Property SequencingTech As String

        ''' <summary>
        ''' assembly_method 属性
        ''' </summary>
        <Field("assembly_method")>
        Public Property AssemblyMethod As String

        ''' <summary>
        ''' grouping_method 属性
        ''' </summary>
        <Field("grouping_method")>
        Public Property GroupingMethod As String

        ''' <summary>
        ''' biosample 属性
        ''' </summary>
        <Field("biosample")>
        Public Property Biosample As Object

        ''' <summary>
        ''' blast_url 属性
        ''' </summary>
        <Field("blast_url")>
        Public Property BlastUrl As String

        ''' <summary>
        ''' comments 属性
        ''' </summary>
        <Field("comments")>
        Public Property Comments As String

        ''' <summary>
        ''' suppression_reason 属性
        ''' </summary>
        <Field("suppression_reason")>
        Public Property SuppressionReason As String

        ''' <summary>
        ''' diploid_role 属性
        ''' </summary>
        <Field("diploid_role")>
        Public Property DiploidRole As Object

    End Class

End Namespace
