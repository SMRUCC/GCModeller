' ============================================================================
' V2TaxonomyTaxIdsPage.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2TaxonomyTaxIdsPage
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2TaxonomyTaxIdsPage

        ''' <summary>
        ''' tax_ids 属性
        ''' </summary>
        <Field("tax_ids")>
        Public Property TaxIds As List(Of Integer)

        ''' <summary>
        ''' next_page_token 属性
        ''' </summary>
        <Field("next_page_token")>
        Public Property NextPageToken As String

    End Class

End Namespace
