' ============================================================================
' V2DownloadSummaryAvailableFiles.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2DownloadSummaryAvailableFiles
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2DownloadSummaryAvailableFiles

        ''' <summary>
        ''' all_genomic_fasta 属性
        ''' </summary>
        <JsonProperty("all_genomic_fasta")>
        Public Property AllGenomicFasta As Object

        ''' <summary>
        ''' genome_gff 属性
        ''' </summary>
        <JsonProperty("genome_gff")>
        Public Property GenomeGff As Object

        ''' <summary>
        ''' genome_gbff 属性
        ''' </summary>
        <JsonProperty("genome_gbff")>
        Public Property GenomeGbff As Object

        ''' <summary>
        ''' rna_fasta 属性
        ''' </summary>
        <JsonProperty("rna_fasta")>
        Public Property RnaFasta As Object

        ''' <summary>
        ''' prot_fasta 属性
        ''' </summary>
        <JsonProperty("prot_fasta")>
        Public Property ProtFasta As Object

        ''' <summary>
        ''' genome_gtf 属性
        ''' </summary>
        <JsonProperty("genome_gtf")>
        Public Property GenomeGtf As Object

        ''' <summary>
        ''' cds_fasta 属性
        ''' </summary>
        <JsonProperty("cds_fasta")>
        Public Property CdsFasta As Object

        ''' <summary>
        ''' sequence_report 属性
        ''' </summary>
        <JsonProperty("sequence_report")>
        Public Property SequenceReport As Object

        ''' <summary>
        ''' annotation_report 属性
        ''' </summary>
        <JsonProperty("annotation_report")>
        Public Property AnnotationReport As Object

    End Class

End Namespace
