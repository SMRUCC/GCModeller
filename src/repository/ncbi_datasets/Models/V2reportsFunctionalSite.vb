' ============================================================================
' V2reportsFunctionalSite.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsFunctionalSite
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsFunctionalSite

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' type 属性
        ''' </summary>
        <Field("type")>
        Public Property Type As String

        ''' <summary>
        ''' specific 属性
        ''' </summary>
        <Field("specific")>
        Public Property Specific As Boolean?

        ''' <summary>
        ''' completeness 属性
        ''' </summary>
        <Field("completeness")>
        Public Property Completeness As Single?

        ''' <summary>
        ''' source_accession 属性
        ''' </summary>
        <Field("source_accession")>
        Public Property SourceAccession As String

        ''' <summary>
        ''' location 属性
        ''' </summary>
        <Field("location")>
        Public Property Location As List(Of Integer)

    End Class

End Namespace
