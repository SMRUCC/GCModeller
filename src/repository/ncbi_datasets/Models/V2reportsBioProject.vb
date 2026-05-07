' ============================================================================
' V2reportsBioProject.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsBioProject
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsBioProject

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <Field("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' title 属性
        ''' </summary>
        <Field("title")>
        Public Property Title As String

        ''' <summary>
        ''' parent_accession 属性
        ''' </summary>
        <Field("parent_accession")>
        Public Property ParentAccession As String

        ''' <summary>
        ''' parent_accessions 属性
        ''' </summary>
        <Field("parent_accessions")>
        Public Property ParentAccessions As List(Of String)

    End Class

End Namespace
