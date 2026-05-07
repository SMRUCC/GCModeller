' ============================================================================
' V2reportsGeneDescriptor.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsGeneDescriptor
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsGeneDescriptor

        ''' <summary>
        ''' gene_id 属性
        ''' </summary>
        <Field("gene_id")>
        Public Property GeneId As String

        ''' <summary>
        ''' symbol 属性
        ''' </summary>
        <Field("symbol")>
        Public Property Symbol As String

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <Field("description")>
        Public Property Description As String

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <Field("tax_id")>
        Public Property TaxId As String

        ''' <summary>
        ''' taxname 属性
        ''' </summary>
        <Field("taxname")>
        Public Property Taxname As String

        ''' <summary>
        ''' common_name 属性
        ''' </summary>
        <Field("common_name")>
        Public Property CommonName As String

        ''' <summary>
        ''' type 属性
        ''' </summary>
        <Field("type")>
        Public Property Type As Object

        ''' <summary>
        ''' rna_type 属性
        ''' </summary>
        <Field("rna_type")>
        Public Property RnaType As Object

        ''' <summary>
        ''' orientation 属性
        ''' </summary>
        <Field("orientation")>
        Public Property Orientation As Object

        ''' <summary>
        ''' reference_standards 属性
        ''' </summary>
        <Field("reference_standards")>
        Public Property ReferenceStandards As List(Of Object)

        ''' <summary>
        ''' genomic_regions 属性
        ''' </summary>
        <Field("genomic_regions")>
        Public Property GenomicRegions As List(Of Object)

        ''' <summary>
        ''' chromosomes 属性
        ''' </summary>
        <Field("chromosomes")>
        Public Property Chromosomes As List(Of String)

        ''' <summary>
        ''' nomenclature_authority 属性
        ''' </summary>
        <Field("nomenclature_authority")>
        Public Property NomenclatureAuthority As Object

        ''' <summary>
        ''' swiss_prot_accessions 属性
        ''' </summary>
        <Field("swiss_prot_accessions")>
        Public Property SwissProtAccessions As List(Of String)

        ''' <summary>
        ''' ensembl_gene_ids 属性
        ''' </summary>
        <Field("ensembl_gene_ids")>
        Public Property EnsemblGeneIds As List(Of String)

        ''' <summary>
        ''' omim_ids 属性
        ''' </summary>
        <Field("omim_ids")>
        Public Property OmimIds As List(Of String)

        ''' <summary>
        ''' synonyms 属性
        ''' </summary>
        <Field("synonyms")>
        Public Property Synonyms As List(Of String)

        ''' <summary>
        ''' alternate_names 属性
        ''' </summary>
        <Field("alternate_names")>
        Public Property AlternateNames As List(Of String)

        ''' <summary>
        ''' replaced_gene_id 属性
        ''' </summary>
        <Field("replaced_gene_id")>
        Public Property ReplacedGeneId As String

        ''' <summary>
        ''' annotations 属性
        ''' </summary>
        <Field("annotations")>
        Public Property Annotations As List(Of Object)

        ''' <summary>
        ''' transcript_count 属性
        ''' </summary>
        <Field("transcript_count")>
        Public Property TranscriptCount As Integer?

        ''' <summary>
        ''' protein_count 属性
        ''' </summary>
        <Field("protein_count")>
        Public Property ProteinCount As Integer?

        ''' <summary>
        ''' transcript_type_counts 属性
        ''' </summary>
        <Field("transcript_type_counts")>
        Public Property TranscriptTypeCounts As List(Of Object)

        ''' <summary>
        ''' gene_groups 属性
        ''' </summary>
        <Field("gene_groups")>
        Public Property GeneGroups As List(Of Object)

        ''' <summary>
        ''' summary 属性
        ''' </summary>
        <Field("summary")>
        Public Property Summary As List(Of Object)

        ''' <summary>
        ''' gene_ontology 属性
        ''' </summary>
        <Field("gene_ontology")>
        Public Property GeneOntology As Object

        ''' <summary>
        ''' locus_tag 属性
        ''' </summary>
        <Field("locus_tag")>
        Public Property LocusTag As String

        ''' <summary>
        ''' map_locations 属性
        ''' </summary>
        <Field("map_locations")>
        Public Property MapLocations As List(Of Object)

    End Class

End Namespace
