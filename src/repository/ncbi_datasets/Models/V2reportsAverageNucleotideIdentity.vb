' ============================================================================
' V2reportsAverageNucleotideIdentity.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsAverageNucleotideIdentity
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsAverageNucleotideIdentity

        ''' <summary>
        ''' taxonomy_check_status 属性
        ''' </summary>
        <Field("taxonomy_check_status")>
        Public Property TaxonomyCheckStatus As Object

        ''' <summary>
        ''' match_status 属性
        ''' </summary>
        <Field("match_status")>
        Public Property MatchStatus As Object

        ''' <summary>
        ''' submitted_organism 属性
        ''' </summary>
        <Field("submitted_organism")>
        Public Property SubmittedOrganism As String

        ''' <summary>
        ''' submitted_species 属性
        ''' </summary>
        <Field("submitted_species")>
        Public Property SubmittedSpecies As String

        ''' <summary>
        ''' category 属性
        ''' </summary>
        <Field("category")>
        Public Property Category As Object

        ''' <summary>
        ''' submitted_ani_match 属性
        ''' </summary>
        <Field("submitted_ani_match")>
        Public Property SubmittedAniMatch As Object

        ''' <summary>
        ''' best_ani_match 属性
        ''' </summary>
        <Field("best_ani_match")>
        Public Property BestAniMatch As Object

        ''' <summary>
        ''' comment 属性
        ''' </summary>
        <Field("comment")>
        Public Property Comment As String

    End Class

End Namespace
