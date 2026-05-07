' ============================================================================
' V2reportsBuscoStat.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsBuscoStat
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsBuscoStat

        ''' <summary>
        ''' busco_lineage 属性
        ''' </summary>
        <JsonProperty("busco_lineage")>
        Public Property BuscoLineage As String

        ''' <summary>
        ''' busco_ver 属性
        ''' </summary>
        <JsonProperty("busco_ver")>
        Public Property BuscoVer As String

        ''' <summary>
        ''' complete 属性
        ''' </summary>
        <JsonProperty("complete")>
        Public Property Complete As Single?

        ''' <summary>
        ''' single_copy 属性
        ''' </summary>
        <JsonProperty("single_copy")>
        Public Property SingleCopy As Single?

        ''' <summary>
        ''' duplicated 属性
        ''' </summary>
        <JsonProperty("duplicated")>
        Public Property Duplicated As Single?

        ''' <summary>
        ''' fragmented 属性
        ''' </summary>
        <JsonProperty("fragmented")>
        Public Property Fragmented As Single?

        ''' <summary>
        ''' missing 属性
        ''' </summary>
        <JsonProperty("missing")>
        Public Property Missing As Single?

        ''' <summary>
        ''' total_count 属性
        ''' </summary>
        <JsonProperty("total_count")>
        Public Property TotalCount As String

    End Class

End Namespace
