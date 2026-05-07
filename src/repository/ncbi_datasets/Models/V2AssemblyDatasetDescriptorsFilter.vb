' ============================================================================
' V2AssemblyDatasetDescriptorsFilter.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2AssemblyDatasetDescriptorsFilter
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2AssemblyDatasetDescriptorsFilter

        ''' <summary>
        ''' reference_only 属性
        ''' </summary>
        <Field("reference_only")>
        Public Property ReferenceOnly As Boolean?

        ''' <summary>
        ''' assembly_source 属性
        ''' </summary>
        <Field("assembly_source")>
        Public Property AssemblySource As Object

        ''' <summary>
        ''' has_annotation 属性
        ''' </summary>
        <Field("has_annotation")>
        Public Property HasAnnotation As Boolean?

        ''' <summary>
        ''' exclude_paired_reports 属性
        ''' </summary>
        <Field("exclude_paired_reports")>
        Public Property ExcludePairedReports As Boolean?

        ''' <summary>
        ''' exclude_atypical 属性
        ''' </summary>
        <Field("exclude_atypical")>
        Public Property ExcludeAtypical As Boolean?

        ''' <summary>
        ''' assembly_version 属性
        ''' </summary>
        <Field("assembly_version")>
        Public Property AssemblyVersion As Object

        ''' <summary>
        ''' assembly_level 属性
        ''' </summary>
        <Field("assembly_level")>
        Public Property AssemblyLevel As List(Of Object)

        ''' <summary>
        ''' first_release_date 属性
        ''' </summary>
        <Field("first_release_date")>
        Public Property FirstReleaseDate As DateTime?

        ''' <summary>
        ''' last_release_date 属性
        ''' </summary>
        <Field("last_release_date")>
        Public Property LastReleaseDate As DateTime?

        ''' <summary>
        ''' search_text 属性
        ''' </summary>
        <Field("search_text")>
        Public Property SearchText As List(Of String)

        ''' <summary>
        ''' is_metagenome_derived 属性
        ''' </summary>
        <Field("is_metagenome_derived")>
        Public Property IsMetagenomeDerived As Object

        ''' <summary>
        ''' is_type_material 属性
        ''' </summary>
        <Field("is_type_material")>
        Public Property IsTypeMaterial As Boolean?

        ''' <summary>
        ''' is_ictv_exemplar 属性
        ''' </summary>
        <Field("is_ictv_exemplar")>
        Public Property IsIctvExemplar As Boolean?

        ''' <summary>
        ''' exclude_multi_isolate 属性
        ''' </summary>
        <Field("exclude_multi_isolate")>
        Public Property ExcludeMultiIsolate As Boolean?

        ''' <summary>
        ''' type_material_category 属性
        ''' </summary>
        <Field("type_material_category")>
        Public Property TypeMaterialCategory As Object

    End Class

End Namespace
