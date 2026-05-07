' ============================================================================
' V2SciNameAndIdsSciNameAndId.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2SciNameAndIdsSciNameAndId
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2SciNameAndIdsSciNameAndId

        ''' <summary>
        ''' sci_name 属性
        ''' </summary>
        <JsonProperty("sci_name")>
        Public Property SciName As String

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <JsonProperty("tax_id")>
        Public Property TaxId As String

        ''' <summary>
        ''' common_name 属性
        ''' </summary>
        <JsonProperty("common_name")>
        Public Property CommonName As String

        ''' <summary>
        ''' matched_term 属性
        ''' </summary>
        <JsonProperty("matched_term")>
        Public Property MatchedTerm As String

        ''' <summary>
        ''' rank 属性
        ''' </summary>
        <JsonProperty("rank")>
        Public Property Rank As Object

        ''' <summary>
        ''' group_name 属性
        ''' </summary>
        <JsonProperty("group_name")>
        Public Property GroupName As String

    End Class

End Namespace
