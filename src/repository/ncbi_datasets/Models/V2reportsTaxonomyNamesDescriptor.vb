' ============================================================================
' V2reportsTaxonomyNamesDescriptor.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsTaxonomyNamesDescriptor
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsTaxonomyNamesDescriptor

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <Field("tax_id")>
        Public Property TaxId As String

        ''' <summary>
        ''' rank 属性
        ''' </summary>
        <Field("rank")>
        Public Property Rank As Object

        ''' <summary>
        ''' current_scientific_name 属性
        ''' </summary>
        <Field("current_scientific_name")>
        Public Property CurrentScientificName As Object

        ''' <summary>
        ''' group_name 属性
        ''' </summary>
        <Field("group_name")>
        Public Property GroupName As String

        ''' <summary>
        ''' curator_common_name 属性
        ''' </summary>
        <Field("curator_common_name")>
        Public Property CuratorCommonName As String

        ''' <summary>
        ''' other_common_names 属性
        ''' </summary>
        <Field("other_common_names")>
        Public Property OtherCommonNames As List(Of String)

        ''' <summary>
        ''' general_notes 属性
        ''' </summary>
        <Field("general_notes")>
        Public Property GeneralNotes As List(Of String)

        ''' <summary>
        ''' links_from_type 属性
        ''' </summary>
        <Field("links_from_type")>
        Public Property LinksFromType As String

        ''' <summary>
        ''' citations 属性
        ''' </summary>
        <Field("citations")>
        Public Property Citations As List(Of Object)

        ''' <summary>
        ''' current_scientific_name_is_formal 属性
        ''' </summary>
        <Field("current_scientific_name_is_formal")>
        Public Property CurrentScientificNameIsFormal As Boolean?

    End Class

End Namespace
