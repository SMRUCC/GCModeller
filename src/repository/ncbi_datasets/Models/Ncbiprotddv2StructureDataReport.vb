' ============================================================================
' Ncbiprotddv2StructureDataReport.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2StructureDataReport
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2StructureDataReport

        ''' <summary>
        ''' pdb_id 属性
        ''' </summary>
        <JsonProperty("pdb_id")>
        Public Property PdbId As String

        ''' <summary>
        ''' mmdb_id 属性
        ''' </summary>
        <JsonProperty("mmdb_id")>
        Public Property MmdbId As Integer?

        ''' <summary>
        ''' is_obsolete 属性
        ''' </summary>
        <JsonProperty("is_obsolete")>
        Public Property IsObsolete As Boolean?

        ''' <summary>
        ''' publication_pmid 属性
        ''' </summary>
        <JsonProperty("publication_pmid")>
        Public Property PublicationPmid As List(Of Integer)

        ''' <summary>
        ''' deposition_date 属性
        ''' </summary>
        <JsonProperty("deposition_date")>
        Public Property DepositionDate As String

        ''' <summary>
        ''' update_date 属性
        ''' </summary>
        <JsonProperty("update_date")>
        Public Property UpdateDate As String

        ''' <summary>
        ''' experiment 属性
        ''' </summary>
        <JsonProperty("experiment")>
        Public Property Experiment As Object

        ''' <summary>
        ''' chains 属性
        ''' </summary>
        <JsonProperty("chains")>
        Public Property Chains As List(Of Object)

        ''' <summary>
        ''' ligand_chains 属性
        ''' </summary>
        <JsonProperty("ligand_chains")>
        Public Property LigandChains As List(Of Object)

        ''' <summary>
        ''' asymmetric_chains 属性
        ''' </summary>
        <JsonProperty("asymmetric_chains")>
        Public Property AsymmetricChains As List(Of Object)

        ''' <summary>
        ''' asymmetric_ligands 属性
        ''' </summary>
        <JsonProperty("asymmetric_ligands")>
        Public Property AsymmetricLigands As List(Of Object)

        ''' <summary>
        ''' title 属性
        ''' </summary>
        <JsonProperty("title")>
        Public Property Title As String

    End Class

End Namespace
