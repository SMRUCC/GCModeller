' ============================================================================
' V2reportsOrganelleGeneCounts.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsOrganelleGeneCounts
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsOrganelleGeneCounts

        ''' <summary>
        ''' total 属性
        ''' </summary>
        <Field("total")>
        Public Property Total As Integer?

        ''' <summary>
        ''' protein_coding 属性
        ''' </summary>
        <Field("protein_coding")>
        Public Property ProteinCoding As Integer?

        ''' <summary>
        ''' rrna 属性
        ''' </summary>
        <Field("rrna")>
        Public Property Rrna As Integer?

        ''' <summary>
        ''' trna 属性
        ''' </summary>
        <Field("trna")>
        Public Property Trna As Integer?

        ''' <summary>
        ''' lncrna 属性
        ''' </summary>
        <Field("lncrna")>
        Public Property Lncrna As Integer?

    End Class

End Namespace
