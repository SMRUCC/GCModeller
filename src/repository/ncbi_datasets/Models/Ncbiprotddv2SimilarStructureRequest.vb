' ============================================================================
' Ncbiprotddv2SimilarStructureRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2SimilarStructureRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2SimilarStructureRequest

        ''' <summary>
        ''' sdid 属性
        ''' </summary>
        <Field("sdid")>
        Public Property Sdid As String

        ''' <summary>
        ''' page_token 属性
        ''' </summary>
        <Field("page_token")>
        Public Property PageToken As String

        ''' <summary>
        ''' redundancy_level 属性
        ''' </summary>
        <Field("redundancy_level")>
        Public Property RedundancyLevel As Object

        ''' <summary>
        ''' sort_by 属性
        ''' </summary>
        <Field("sort_by")>
        Public Property SortBy As Object

        ''' <summary>
        ''' hits_per_page 属性
        ''' </summary>
        <Field("hits_per_page")>
        Public Property HitsPerPage As Integer?

    End Class

End Namespace
