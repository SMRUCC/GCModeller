' ============================================================================
' V2AssemblyLinksReplyAssemblyLink.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2AssemblyLinksReplyAssemblyLink
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2AssemblyLinksReplyAssemblyLink

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <Field("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' assembly_link_type 属性
        ''' </summary>
        <Field("assembly_link_type")>
        Public Property AssemblyLinkType As Object

        ''' <summary>
        ''' resource_link 属性
        ''' </summary>
        <Field("resource_link")>
        Public Property ResourceLink As String

        ''' <summary>
        ''' linked_identifiers 属性
        ''' </summary>
        <Field("linked_identifiers")>
        Public Property LinkedIdentifiers As List(Of String)

    End Class

End Namespace
