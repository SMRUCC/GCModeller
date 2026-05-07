' ============================================================================
' V2TaxonomyFilteredSubtreeResponse.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2TaxonomyFilteredSubtreeResponse
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2TaxonomyFilteredSubtreeResponse

        ''' <summary>
        ''' root_nodes 属性
        ''' </summary>
        <Field("root_nodes")>
        Public Property RootNodes As List(Of Integer)

        ''' <summary>
        ''' edges 属性
        ''' </summary>
        <Field("edges")>
        Public Property Edges As Object

        ''' <summary>
        ''' warnings 属性
        ''' </summary>
        <Field("warnings")>
        Public Property Warnings As List(Of Object)

        ''' <summary>
        ''' errors 属性
        ''' </summary>
        <Field("errors")>
        Public Property Errors As List(Of Object)

    End Class

End Namespace
