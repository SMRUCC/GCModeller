' ============================================================================
' V2GenomeAnnotationRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2GenomeAnnotationRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2GenomeAnnotationRequest

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <Field("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' annotation_ids 属性
        ''' </summary>
        <Field("annotation_ids")>
        Public Property AnnotationIds As List(Of String)

        ''' <summary>
        ''' symbols 属性
        ''' </summary>
        <Field("symbols")>
        Public Property Symbols As List(Of String)

        ''' <summary>
        ''' locations 属性
        ''' </summary>
        <Field("locations")>
        Public Property Locations As List(Of String)

        ''' <summary>
        ''' gene_types 属性
        ''' </summary>
        <Field("gene_types")>
        Public Property GeneTypes As List(Of String)

        ''' <summary>
        ''' search_text 属性
        ''' </summary>
        <Field("search_text")>
        Public Property SearchText As List(Of String)

        ''' <summary>
        ''' sort 属性
        ''' </summary>
        <Field("sort")>
        Public Property Sort As List(Of Object)

        ''' <summary>
        ''' include_annotation_type 属性
        ''' </summary>
        <Field("include_annotation_type")>
        Public Property IncludeAnnotationType As List(Of Object)

        ''' <summary>
        ''' page_size 属性
        ''' </summary>
        <Field("page_size")>
        Public Property PageSize As Integer?

        ''' <summary>
        ''' table_fields 属性
        ''' </summary>
        <Field("table_fields")>
        Public Property TableFields As List(Of String)

        ''' <summary>
        ''' table_format 属性
        ''' </summary>
        <Field("table_format")>
        Public Property TableFormat As Object

        ''' <summary>
        ''' include_tabular_header 属性
        ''' </summary>
        <Field("include_tabular_header")>
        Public Property IncludeTabularHeader As Object

        ''' <summary>
        ''' page_token 属性
        ''' </summary>
        <Field("page_token")>
        Public Property PageToken As String

    End Class

End Namespace
