' ============================================================================
' V2reportsVirusPeptide.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsVirusPeptide
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsVirusPeptide

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <JsonProperty("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <JsonProperty("name")>
        Public Property Name As String

        ''' <summary>
        ''' other_names 属性
        ''' </summary>
        <JsonProperty("other_names")>
        Public Property OtherNames As List(Of String)

        ''' <summary>
        ''' nucleotide 属性
        ''' </summary>
        <JsonProperty("nucleotide")>
        Public Property Nucleotide As Object

        ''' <summary>
        ''' protein 属性
        ''' </summary>
        <JsonProperty("protein")>
        Public Property Protein As Object

        ''' <summary>
        ''' pdb_ids 属性
        ''' </summary>
        <JsonProperty("pdb_ids")>
        Public Property PdbIds As List(Of String)

        ''' <summary>
        ''' cdd 属性
        ''' </summary>
        <JsonProperty("cdd")>
        Public Property Cdd As List(Of Object)

        ''' <summary>
        ''' uni_prot_kb 属性
        ''' </summary>
        <JsonProperty("uni_prot_kb")>
        Public Property UniProtKb As Object

        ''' <summary>
        ''' mature_peptide 属性
        ''' </summary>
        <JsonProperty("mature_peptide")>
        Public Property MaturePeptide As List(Of Object)

        ''' <summary>
        ''' protein_completeness 属性
        ''' </summary>
        <JsonProperty("protein_completeness")>
        Public Property ProteinCompleteness As Object

    End Class

End Namespace
