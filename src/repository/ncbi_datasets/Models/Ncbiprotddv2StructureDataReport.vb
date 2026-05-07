' ============================================================================
' Ncbiprotddv2StructureDataReport.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2StructureDataReport
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2StructureDataReport

        ''' <summary>
        ''' pdb_id 属性
        ''' </summary>
        <Field("pdb_id")>
        Public Property PdbId As String

        ''' <summary>
        ''' mmdb_id 属性
        ''' </summary>
        <Field("mmdb_id")>
        Public Property MmdbId As Integer?

        ''' <summary>
        ''' is_obsolete 属性
        ''' </summary>
        <Field("is_obsolete")>
        Public Property IsObsolete As Boolean?

        ''' <summary>
        ''' publication_pmid 属性
        ''' </summary>
        <Field("publication_pmid")>
        Public Property PublicationPmid As List(Of Integer)

        ''' <summary>
        ''' deposition_date 属性
        ''' </summary>
        <Field("deposition_date")>
        Public Property DepositionDate As String

        ''' <summary>
        ''' update_date 属性
        ''' </summary>
        <Field("update_date")>
        Public Property UpdateDate As String

        ''' <summary>
        ''' experiment 属性
        ''' </summary>
        <Field("experiment")>
        Public Property Experiment As Object

        ''' <summary>
        ''' chains 属性
        ''' </summary>
        <Field("chains")>
        Public Property Chains As List(Of Object)

        ''' <summary>
        ''' ligand_chains 属性
        ''' </summary>
        <Field("ligand_chains")>
        Public Property LigandChains As List(Of Object)

        ''' <summary>
        ''' asymmetric_chains 属性
        ''' </summary>
        <Field("asymmetric_chains")>
        Public Property AsymmetricChains As List(Of Object)

        ''' <summary>
        ''' asymmetric_ligands 属性
        ''' </summary>
        <Field("asymmetric_ligands")>
        Public Property AsymmetricLigands As List(Of Object)

        ''' <summary>
        ''' title 属性
        ''' </summary>
        <Field("title")>
        Public Property Title As String

    End Class

End Namespace
