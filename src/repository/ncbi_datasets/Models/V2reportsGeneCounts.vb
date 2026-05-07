' ============================================================================
' V2reportsGeneCounts.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsGeneCounts
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsGeneCounts

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
        ''' non_coding 属性
        ''' </summary>
        <Field("non_coding")>
        Public Property NonCoding As Integer?

        ''' <summary>
        ''' pseudogene 属性
        ''' </summary>
        <Field("pseudogene")>
        Public Property Pseudogene As Integer?

        ''' <summary>
        ''' other 属性
        ''' </summary>
        <Field("other")>
        Public Property Other As Integer?

    End Class

End Namespace
