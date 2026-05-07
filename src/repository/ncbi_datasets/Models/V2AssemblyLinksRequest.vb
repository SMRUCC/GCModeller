' ============================================================================
' V2AssemblyLinksRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2AssemblyLinksRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2AssemblyLinksRequest

        ''' <summary>
        ''' accessions 属性
        ''' </summary>
        <Field("accessions")>
        Public Property Accessions As List(Of String)

        ''' <summary>
        ''' assembly_link_types 属性
        ''' </summary>
        <Field("assembly_link_types")>
        Public Property AssemblyLinkTypes As List(Of Object)

    End Class

End Namespace
