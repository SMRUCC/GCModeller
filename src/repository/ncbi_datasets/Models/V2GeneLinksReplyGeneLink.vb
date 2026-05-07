' ============================================================================
' V2GeneLinksReplyGeneLink.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2GeneLinksReplyGeneLink
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2GeneLinksReplyGeneLink

        ''' <summary>
        ''' gene_id 属性
        ''' </summary>
        <Field("gene_id")>
        Public Property GeneId As Integer?

        ''' <summary>
        ''' gene_link_type 属性
        ''' </summary>
        <Field("gene_link_type")>
        Public Property GeneLinkType As Object

        ''' <summary>
        ''' resource_link 属性
        ''' </summary>
        <Field("resource_link")>
        Public Property ResourceLink As String

        ''' <summary>
        ''' resource_id 属性
        ''' </summary>
        <Field("resource_id")>
        Public Property ResourceId As String

    End Class

End Namespace
