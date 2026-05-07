' ============================================================================
' V2reportsCheckM.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsCheckM
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsCheckM

        ''' <summary>
        ''' checkm_marker_set 属性
        ''' </summary>
        <Field("checkm_marker_set")>
        Public Property CheckmMarkerSet As String

        ''' <summary>
        ''' checkm_species_tax_id 属性
        ''' </summary>
        <Field("checkm_species_tax_id")>
        Public Property CheckmSpeciesTaxId As Integer?

        ''' <summary>
        ''' checkm_marker_set_rank 属性
        ''' </summary>
        <Field("checkm_marker_set_rank")>
        Public Property CheckmMarkerSetRank As String

        ''' <summary>
        ''' checkm_version 属性
        ''' </summary>
        <Field("checkm_version")>
        Public Property CheckmVersion As String

        ''' <summary>
        ''' completeness 属性
        ''' </summary>
        <Field("completeness")>
        Public Property Completeness As Single?

        ''' <summary>
        ''' contamination 属性
        ''' </summary>
        <Field("contamination")>
        Public Property Contamination As Single?

        ''' <summary>
        ''' completeness_percentile 属性
        ''' </summary>
        <Field("completeness_percentile")>
        Public Property CompletenessPercentile As Single?

    End Class

End Namespace
