' ============================================================================
' V2reportsTaxonomyTypeMaterial.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsTaxonomyTypeMaterial
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsTaxonomyTypeMaterial

        ''' <summary>
        ''' type_strain_name 属性
        ''' </summary>
        <Field("type_strain_name")>
        Public Property TypeStrainName As String

        ''' <summary>
        ''' type_strain_id 属性
        ''' </summary>
        <Field("type_strain_id")>
        Public Property TypeStrainId As String

        ''' <summary>
        ''' bio_collection_id 属性
        ''' </summary>
        <Field("bio_collection_id")>
        Public Property BioCollectionId As String

        ''' <summary>
        ''' bio_collection_name 属性
        ''' </summary>
        <Field("bio_collection_name")>
        Public Property BioCollectionName As String

        ''' <summary>
        ''' collection_type 属性
        ''' </summary>
        <Field("collection_type")>
        Public Property CollectionType As List(Of Object)

        ''' <summary>
        ''' type_class 属性
        ''' </summary>
        <Field("type_class")>
        Public Property TypeClass As String

    End Class

End Namespace
