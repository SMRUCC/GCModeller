' ============================================================================
' V2TaxonomyMatch.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2TaxonomyMatch
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2TaxonomyMatch

        ''' <summary>
        ''' warnings 属性
        ''' </summary>
        <JsonProperty("warnings")>
        Public Property Warnings As List(Of Object)

        ''' <summary>
        ''' errors 属性
        ''' </summary>
        <JsonProperty("errors")>
        Public Property Errors As List(Of Object)

        ''' <summary>
        ''' query 属性
        ''' </summary>
        <JsonProperty("query")>
        Public Property Query As List(Of String)

        ''' <summary>
        ''' taxonomy 属性
        ''' </summary>
        <JsonProperty("taxonomy")>
        Public Property Taxonomy As Object

    End Class

End Namespace
