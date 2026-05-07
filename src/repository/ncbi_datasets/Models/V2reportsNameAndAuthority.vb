' ============================================================================
' V2reportsNameAndAuthority.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsNameAndAuthority
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsNameAndAuthority

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' authority 属性
        ''' </summary>
        <Field("authority")>
        Public Property Authority As String

        ''' <summary>
        ''' type_strains 属性
        ''' </summary>
        <Field("type_strains")>
        Public Property TypeStrains As List(Of Object)

        ''' <summary>
        ''' curator_synonym 属性
        ''' </summary>
        <Field("curator_synonym")>
        Public Property CuratorSynonym As String

        ''' <summary>
        ''' homotypic_synonyms 属性
        ''' </summary>
        <Field("homotypic_synonyms")>
        Public Property HomotypicSynonyms As List(Of Object)

        ''' <summary>
        ''' heterotypic_synonyms 属性
        ''' </summary>
        <Field("heterotypic_synonyms")>
        Public Property HeterotypicSynonyms As List(Of Object)

        ''' <summary>
        ''' other_synonyms 属性
        ''' </summary>
        <Field("other_synonyms")>
        Public Property OtherSynonyms As List(Of Object)

        ''' <summary>
        ''' informal_names 属性
        ''' </summary>
        <Field("informal_names")>
        Public Property InformalNames As List(Of String)

        ''' <summary>
        ''' basionym 属性
        ''' </summary>
        <Field("basionym")>
        Public Property Basionym As Object

        ''' <summary>
        ''' publications 属性
        ''' </summary>
        <Field("publications")>
        Public Property Publications As List(Of Object)

        ''' <summary>
        ''' notes 属性
        ''' </summary>
        <Field("notes")>
        Public Property Notes As List(Of Object)

        ''' <summary>
        ''' formal 属性
        ''' </summary>
        <Field("formal")>
        Public Property Formal As Boolean?

    End Class

End Namespace
