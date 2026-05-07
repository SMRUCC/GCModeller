' ============================================================================
' V2reportsGenomeAnnotation.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsGenomeAnnotation
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsGenomeAnnotation

        ''' <summary>
        ''' gene_id 属性
        ''' </summary>
        <JsonProperty("gene_id")>
        Public Property GeneId As String

        ''' <summary>
        ''' symbol 属性
        ''' </summary>
        <JsonProperty("symbol")>
        Public Property Symbol As String

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <JsonProperty("description")>
        Public Property Description As String

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <JsonProperty("name")>
        Public Property Name As String

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <JsonProperty("tax_id")>
        Public Property TaxId As String

        ''' <summary>
        ''' taxname 属性
        ''' </summary>
        <JsonProperty("taxname")>
        Public Property Taxname As String

        ''' <summary>
        ''' common_name 属性
        ''' </summary>
        <JsonProperty("common_name")>
        Public Property CommonName As String

        ''' <summary>
        ''' type 属性
        ''' </summary>
        <JsonProperty("type")>
        Public Property Type As Object

        ''' <summary>
        ''' gene_type 属性
        ''' </summary>
        <JsonProperty("gene_type")>
        Public Property GeneType As String

        ''' <summary>
        ''' rna_type 属性
        ''' </summary>
        <JsonProperty("rna_type")>
        Public Property RnaType As Object

        ''' <summary>
        ''' orientation 属性
        ''' </summary>
        <JsonProperty("orientation")>
        Public Property Orientation As Object

        ''' <summary>
        ''' locus_tag 属性
        ''' </summary>
        <JsonProperty("locus_tag")>
        Public Property LocusTag As String

        ''' <summary>
        ''' reference_standards 属性
        ''' </summary>
        <JsonProperty("reference_standards")>
        Public Property ReferenceStandards As List(Of Object)

        ''' <summary>
        ''' genomic_regions 属性
        ''' </summary>
        <JsonProperty("genomic_regions")>
        Public Property GenomicRegions As List(Of Object)

        ''' <summary>
        ''' transcripts 属性
        ''' </summary>
        <JsonProperty("transcripts")>
        Public Property Transcripts As List(Of Object)

        ''' <summary>
        ''' proteins 属性
        ''' </summary>
        <JsonProperty("proteins")>
        Public Property Proteins As List(Of Object)

        ''' <summary>
        ''' chromosomes 属性
        ''' </summary>
        <JsonProperty("chromosomes")>
        Public Property Chromosomes As List(Of String)

        ''' <summary>
        ''' swiss_prot_accessions 属性
        ''' </summary>
        <JsonProperty("swiss_prot_accessions")>
        Public Property SwissProtAccessions As List(Of String)

        ''' <summary>
        ''' ensembl_gene_ids 属性
        ''' </summary>
        <JsonProperty("ensembl_gene_ids")>
        Public Property EnsemblGeneIds As List(Of String)

        ''' <summary>
        ''' omim_ids 属性
        ''' </summary>
        <JsonProperty("omim_ids")>
        Public Property OmimIds As List(Of String)

        ''' <summary>
        ''' synonyms 属性
        ''' </summary>
        <JsonProperty("synonyms")>
        Public Property Synonyms As List(Of String)

        ''' <summary>
        ''' annotations 属性
        ''' </summary>
        <JsonProperty("annotations")>
        Public Property Annotations As List(Of Object)

    End Class

End Namespace
