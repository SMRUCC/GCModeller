' ============================================================================
' V2reportsAssemblyStats.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsAssemblyStats
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsAssemblyStats

        ''' <summary>
        ''' total_number_of_chromosomes 属性
        ''' </summary>
        <Field("total_number_of_chromosomes")>
        Public Property TotalNumberOfChromosomes As Integer?

        ''' <summary>
        ''' total_sequence_length 属性
        ''' </summary>
        <Field("total_sequence_length")>
        Public Property TotalSequenceLength As String

        ''' <summary>
        ''' total_ungapped_length 属性
        ''' </summary>
        <Field("total_ungapped_length")>
        Public Property TotalUngappedLength As String

        ''' <summary>
        ''' number_of_contigs 属性
        ''' </summary>
        <Field("number_of_contigs")>
        Public Property NumberOfContigs As Integer?

        ''' <summary>
        ''' contig_n50 属性
        ''' </summary>
        <Field("contig_n50")>
        Public Property ContigN50 As Integer?

        ''' <summary>
        ''' contig_l50 属性
        ''' </summary>
        <Field("contig_l50")>
        Public Property ContigL50 As Integer?

        ''' <summary>
        ''' number_of_scaffolds 属性
        ''' </summary>
        <Field("number_of_scaffolds")>
        Public Property NumberOfScaffolds As Integer?

        ''' <summary>
        ''' scaffold_n50 属性
        ''' </summary>
        <Field("scaffold_n50")>
        Public Property ScaffoldN50 As Integer?

        ''' <summary>
        ''' scaffold_l50 属性
        ''' </summary>
        <Field("scaffold_l50")>
        Public Property ScaffoldL50 As Integer?

        ''' <summary>
        ''' gaps_between_scaffolds_count 属性
        ''' </summary>
        <Field("gaps_between_scaffolds_count")>
        Public Property GapsBetweenScaffoldsCount As Integer?

        ''' <summary>
        ''' number_of_component_sequences 属性
        ''' </summary>
        <Field("number_of_component_sequences")>
        Public Property NumberOfComponentSequences As Integer?

        ''' <summary>
        ''' atgc_count 属性
        ''' </summary>
        <Field("atgc_count")>
        Public Property AtgcCount As String

        ''' <summary>
        ''' gc_count 属性
        ''' </summary>
        <Field("gc_count")>
        Public Property GcCount As String

        ''' <summary>
        ''' gc_percent 属性
        ''' </summary>
        <Field("gc_percent")>
        Public Property GcPercent As Single?

        ''' <summary>
        ''' genome_coverage 属性
        ''' </summary>
        <Field("genome_coverage")>
        Public Property GenomeCoverage As String

        ''' <summary>
        ''' number_of_organelles 属性
        ''' </summary>
        <Field("number_of_organelles")>
        Public Property NumberOfOrganelles As Integer?

    End Class

End Namespace
