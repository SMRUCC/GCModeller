' ============================================================================
' V2AssemblyDatasetRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2AssemblyDatasetRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2AssemblyDatasetRequest

        ''' <summary>
        ''' accessions 属性
        ''' </summary>
        <JsonProperty("accessions")>
        Public Property Accessions As List(Of String)

        ''' <summary>
        ''' chromosomes 属性
        ''' </summary>
        <JsonProperty("chromosomes")>
        Public Property Chromosomes As List(Of String)

        ''' <summary>
        ''' include_annotation_type 属性
        ''' </summary>
        <JsonProperty("include_annotation_type")>
        Public Property IncludeAnnotationType As List(Of Object)

        ''' <summary>
        ''' hydrated 属性
        ''' </summary>
        <JsonProperty("hydrated")>
        Public Property Hydrated As Object

        ''' <summary>
        ''' include_tsv 属性
        ''' </summary>
        <JsonProperty("include_tsv")>
        Public Property IncludeTsv As Boolean?

    End Class

End Namespace
