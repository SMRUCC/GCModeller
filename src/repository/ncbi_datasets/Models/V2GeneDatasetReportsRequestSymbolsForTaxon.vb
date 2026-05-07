' ============================================================================
' V2GeneDatasetReportsRequestSymbolsForTaxon.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2GeneDatasetReportsRequestSymbolsForTaxon
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2GeneDatasetReportsRequestSymbolsForTaxon

        ''' <summary>
        ''' symbols 属性
        ''' </summary>
        <JsonProperty("symbols")>
        Public Property Symbols As List(Of String)

        ''' <summary>
        ''' taxon 属性
        ''' </summary>
        <JsonProperty("taxon")>
        Public Property Taxon As String

    End Class

End Namespace
