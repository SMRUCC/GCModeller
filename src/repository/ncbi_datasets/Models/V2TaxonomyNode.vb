' ============================================================================
' V2TaxonomyNode.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2TaxonomyNode
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2TaxonomyNode

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <JsonProperty("tax_id")>
        Public Property TaxId As Integer?

        ''' <summary>
        ''' organism_name 属性
        ''' </summary>
        <JsonProperty("organism_name")>
        Public Property OrganismName As String

        ''' <summary>
        ''' common_name 属性
        ''' </summary>
        <JsonProperty("common_name")>
        Public Property CommonName As String

        ''' <summary>
        ''' genbank_common_name 属性
        ''' </summary>
        <JsonProperty("genbank_common_name")>
        Public Property GenbankCommonName As String

        ''' <summary>
        ''' acronyms 属性
        ''' </summary>
        <JsonProperty("acronyms")>
        Public Property Acronyms As List(Of String)

        ''' <summary>
        ''' genbank_acronym 属性
        ''' </summary>
        <JsonProperty("genbank_acronym")>
        Public Property GenbankAcronym As String

        ''' <summary>
        ''' blast_name 属性
        ''' </summary>
        <JsonProperty("blast_name")>
        Public Property BlastName As String

        ''' <summary>
        ''' lineage 属性
        ''' </summary>
        <JsonProperty("lineage")>
        Public Property Lineage As List(Of Integer)

        ''' <summary>
        ''' children 属性
        ''' </summary>
        <JsonProperty("children")>
        Public Property Children As List(Of Integer)

        ''' <summary>
        ''' descendent_with_described_species_names_count 属性
        ''' </summary>
        <JsonProperty("descendent_with_described_species_names_count")>
        Public Property DescendentWithDescribedSpeciesNamesCount As Integer?

        ''' <summary>
        ''' rank 属性
        ''' </summary>
        <JsonProperty("rank")>
        Public Property Rank As Object

        ''' <summary>
        ''' has_described_species_name 属性
        ''' </summary>
        <JsonProperty("has_described_species_name")>
        Public Property HasDescribedSpeciesName As Boolean?

        ''' <summary>
        ''' counts 属性
        ''' </summary>
        <JsonProperty("counts")>
        Public Property Counts As List(Of Object)

        ''' <summary>
        ''' min_ord 属性
        ''' </summary>
        <JsonProperty("min_ord")>
        Public Property MinOrd As Integer?

        ''' <summary>
        ''' max_ord 属性
        ''' </summary>
        <JsonProperty("max_ord")>
        Public Property MaxOrd As Integer?

        ''' <summary>
        ''' extinct 属性
        ''' </summary>
        <JsonProperty("extinct")>
        Public Property Extinct As Boolean?

        ''' <summary>
        ''' genomic_moltype 属性
        ''' </summary>
        <JsonProperty("genomic_moltype")>
        Public Property GenomicMoltype As String

    End Class

End Namespace
