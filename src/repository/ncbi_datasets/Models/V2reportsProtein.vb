' ============================================================================
' V2reportsProtein.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsProtein
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsProtein

        ''' <summary>
        ''' accession_version 属性
        ''' </summary>
        <Field("accession_version")>
        Public Property AccessionVersion As String

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' length 属性
        ''' </summary>
        <Field("length")>
        Public Property Length As Integer?

        ''' <summary>
        ''' isoform_name 属性
        ''' </summary>
        <Field("isoform_name")>
        Public Property IsoformName As String

        ''' <summary>
        ''' ensembl_protein 属性
        ''' </summary>
        <Field("ensembl_protein")>
        Public Property EnsemblProtein As String

        ''' <summary>
        ''' mature_peptides 属性
        ''' </summary>
        <Field("mature_peptides")>
        Public Property MaturePeptides As List(Of Object)

    End Class

End Namespace
