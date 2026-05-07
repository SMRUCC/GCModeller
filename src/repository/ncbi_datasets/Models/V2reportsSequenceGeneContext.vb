' ============================================================================
' V2reportsSequenceGeneContext.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsSequenceGeneContext
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsSequenceGeneContext

        ''' <summary>
        ''' gene_symbol 属性
        ''' </summary>
        <JsonProperty("gene_symbol")>
        Public Property GeneSymbol As String

        ''' <summary>
        ''' gene_id 属性
        ''' </summary>
        <JsonProperty("gene_id")>
        Public Property GeneId As String

        ''' <summary>
        ''' genomic_location 属性
        ''' </summary>
        <JsonProperty("genomic_location")>
        Public Property GenomicLocation As Object

        ''' <summary>
        ''' exons 属性
        ''' </summary>
        <JsonProperty("exons")>
        Public Property Exons As Object

        ''' <summary>
        ''' select_category 属性
        ''' </summary>
        <JsonProperty("select_category")>
        Public Property SelectCategory As String

        ''' <summary>
        ''' refseq_select_category 属性
        ''' </summary>
        <JsonProperty("refseq_select_category")>
        Public Property RefseqSelectCategory As String

    End Class

End Namespace
