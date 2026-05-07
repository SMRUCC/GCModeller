' ============================================================================
' Ncbiprotddv2VastScore.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2VastScore
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2VastScore

        ''' <summary>
        ''' vast_score 属性
        ''' </summary>
        <Field("vast_score")>
        Public Property VastScore As Single?

        ''' <summary>
        ''' align_length 属性
        ''' </summary>
        <Field("align_length")>
        Public Property AlignLength As Integer?

        ''' <summary>
        ''' pct_identity 属性
        ''' </summary>
        <Field("pct_identity")>
        Public Property PctIdentity As Single?

        ''' <summary>
        ''' rmsd 属性
        ''' </summary>
        <Field("rmsd")>
        Public Property Rmsd As Single?

        ''' <summary>
        ''' p_value 属性
        ''' </summary>
        <Field("p_value")>
        Public Property PValue As Single?

    End Class

End Namespace
