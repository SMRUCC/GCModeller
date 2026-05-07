' ============================================================================
' V2reportsVirusAssembly.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsVirusAssembly
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsVirusAssembly

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <JsonProperty("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' is_complete 属性
        ''' </summary>
        <JsonProperty("is_complete")>
        Public Property IsComplete As Boolean?

        ''' <summary>
        ''' is_annotated 属性
        ''' </summary>
        <JsonProperty("is_annotated")>
        Public Property IsAnnotated As Boolean?

        ''' <summary>
        ''' isolate 属性
        ''' </summary>
        <JsonProperty("isolate")>
        Public Property Isolate As Object

        ''' <summary>
        ''' source_database 属性
        ''' </summary>
        <JsonProperty("source_database")>
        Public Property SourceDatabase As String

        ''' <summary>
        ''' protein_count 属性
        ''' </summary>
        <JsonProperty("protein_count")>
        Public Property ProteinCount As Integer?

        ''' <summary>
        ''' host 属性
        ''' </summary>
        <JsonProperty("host")>
        Public Property Host As Object

        ''' <summary>
        ''' virus 属性
        ''' </summary>
        <JsonProperty("virus")>
        Public Property Virus As Object

        ''' <summary>
        ''' bioprojects 属性
        ''' </summary>
        <JsonProperty("bioprojects")>
        Public Property Bioprojects As List(Of String)

        ''' <summary>
        ''' location 属性
        ''' </summary>
        <JsonProperty("location")>
        Public Property Location As Object

        ''' <summary>
        ''' update_date 属性
        ''' </summary>
        <JsonProperty("update_date")>
        Public Property UpdateDate As String

        ''' <summary>
        ''' release_date 属性
        ''' </summary>
        <JsonProperty("release_date")>
        Public Property ReleaseDate As String

        ''' <summary>
        ''' nucleotide_completeness 属性
        ''' </summary>
        <JsonProperty("nucleotide_completeness")>
        Public Property NucleotideCompleteness As String

        ''' <summary>
        ''' completeness 属性
        ''' </summary>
        <JsonProperty("completeness")>
        Public Property Completeness As Object

        ''' <summary>
        ''' length 属性
        ''' </summary>
        <JsonProperty("length")>
        Public Property Length As Integer?

        ''' <summary>
        ''' gene_count 属性
        ''' </summary>
        <JsonProperty("gene_count")>
        Public Property GeneCount As Integer?

        ''' <summary>
        ''' mature_peptide_count 属性
        ''' </summary>
        <JsonProperty("mature_peptide_count")>
        Public Property MaturePeptideCount As Integer?

        ''' <summary>
        ''' biosample 属性
        ''' </summary>
        <JsonProperty("biosample")>
        Public Property Biosample As String

        ''' <summary>
        ''' mol_type 属性
        ''' </summary>
        <JsonProperty("mol_type")>
        Public Property MolType As String

        ''' <summary>
        ''' nucleotide 属性
        ''' </summary>
        <JsonProperty("nucleotide")>
        Public Property Nucleotide As Object

        ''' <summary>
        ''' purpose_of_sampling 属性
        ''' </summary>
        <JsonProperty("purpose_of_sampling")>
        Public Property PurposeOfSampling As Object

        ''' <summary>
        ''' sra_accessions 属性
        ''' </summary>
        <JsonProperty("sra_accessions")>
        Public Property SraAccessions As List(Of String)

        ''' <summary>
        ''' submitter 属性
        ''' </summary>
        <JsonProperty("submitter")>
        Public Property Submitter As Object

        ''' <summary>
        ''' lab_host 属性
        ''' </summary>
        <JsonProperty("lab_host")>
        Public Property LabHost As String

        ''' <summary>
        ''' is_lab_host 属性
        ''' </summary>
        <JsonProperty("is_lab_host")>
        Public Property IsLabHost As Boolean?

        ''' <summary>
        ''' is_vaccine_strain 属性
        ''' </summary>
        <JsonProperty("is_vaccine_strain")>
        Public Property IsVaccineStrain As Boolean?

        ''' <summary>
        ''' segment 属性
        ''' </summary>
        <JsonProperty("segment")>
        Public Property Segment As String

    End Class

End Namespace
