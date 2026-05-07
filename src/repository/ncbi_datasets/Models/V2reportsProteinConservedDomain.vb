' ============================================================================
' V2reportsProteinConservedDomain.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsProteinConservedDomain
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsProteinConservedDomain

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <Field("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <Field("description")>
        Public Property Description As String

        ''' <summary>
        ''' start 属性
        ''' </summary>
        <Field("start")>
        Public Property Start As Integer?

        ''' <summary>
        ''' stop 属性
        ''' </summary>
        <Field("stop")>
        Public Property Stop As Integer?

        ''' <summary>
        ''' specific 属性
        ''' </summary>
        <Field("specific")>
        Public Property Specific As Boolean?

        ''' <summary>
        ''' partial 属性
        ''' </summary>
        <Field("partial")>
        Public Property Partial As Boolean?

        ''' <summary>
        ''' evalue 属性
        ''' </summary>
        <Field("evalue")>
        Public Property Evalue As Single?

        ''' <summary>
        ''' bit_score 属性
        ''' </summary>
        <Field("bit_score")>
        Public Property BitScore As Single?

    End Class

End Namespace
