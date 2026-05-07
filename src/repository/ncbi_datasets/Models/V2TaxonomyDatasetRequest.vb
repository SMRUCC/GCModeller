' ============================================================================
' V2TaxonomyDatasetRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2TaxonomyDatasetRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2TaxonomyDatasetRequest

        ''' <summary>
        ''' tax_ids 属性
        ''' </summary>
        <JsonProperty("tax_ids")>
        Public Property TaxIds As List(Of Integer)

        ''' <summary>
        ''' aux_reports 属性
        ''' </summary>
        <JsonProperty("aux_reports")>
        Public Property AuxReports As List(Of Object)

    End Class

End Namespace
