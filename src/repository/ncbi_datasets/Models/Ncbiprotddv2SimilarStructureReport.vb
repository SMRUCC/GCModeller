' ============================================================================
' Ncbiprotddv2SimilarStructureReport.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2SimilarStructureReport
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2SimilarStructureReport

        ''' <summary>
        ''' sdid 属性
        ''' </summary>
        <JsonProperty("sdid")>
        Public Property Sdid As Integer?

        ''' <summary>
        ''' structure_title 属性
        ''' </summary>
        <JsonProperty("structure_title")>
        Public Property StructureTitle As String

        ''' <summary>
        ''' protein_chain_name 属性
        ''' </summary>
        <JsonProperty("protein_chain_name")>
        Public Property ProteinChainName As String

        ''' <summary>
        ''' chain_id 属性
        ''' </summary>
        <JsonProperty("chain_id")>
        Public Property ChainId As String

        ''' <summary>
        ''' domain_number 属性
        ''' </summary>
        <JsonProperty("domain_number")>
        Public Property DomainNumber As Integer?

        ''' <summary>
        ''' mmdb_id 属性
        ''' </summary>
        <JsonProperty("mmdb_id")>
        Public Property MmdbId As Integer?

        ''' <summary>
        ''' pdb_id 属性
        ''' </summary>
        <JsonProperty("pdb_id")>
        Public Property PdbId As String

        ''' <summary>
        ''' vast_score 属性
        ''' </summary>
        <JsonProperty("vast_score")>
        Public Property VastScore As Object

        ''' <summary>
        ''' align_id 属性
        ''' </summary>
        <JsonProperty("align_id")>
        Public Property AlignId As Integer?

        ''' <summary>
        ''' superkingdom_id 属性
        ''' </summary>
        <JsonProperty("superkingdom_id")>
        Public Property SuperkingdomId As Integer?

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <JsonProperty("tax_id")>
        Public Property TaxId As Integer?

        ''' <summary>
        ''' footprints 属性
        ''' </summary>
        <JsonProperty("footprints")>
        Public Property Footprints As List(Of Object)

    End Class

End Namespace
