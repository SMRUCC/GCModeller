' ============================================================================
' Ncbiprotddv2ParsedAbstract.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2ParsedAbstract
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2ParsedAbstract

        ''' <summary>
        ''' pmid 属性
        ''' </summary>
        <JsonProperty("pmid")>
        Public Property Pmid As Integer?

        ''' <summary>
        ''' title 属性
        ''' </summary>
        <JsonProperty("title")>
        Public Property Title As String

        ''' <summary>
        ''' authors 属性
        ''' </summary>
        <JsonProperty("authors")>
        Public Property Authors As List(Of Object)

        ''' <summary>
        ''' epub 属性
        ''' </summary>
        <JsonProperty("epub")>
        Public Property Epub As Object

        ''' <summary>
        ''' abstract_text 属性
        ''' </summary>
        <JsonProperty("abstract_text")>
        Public Property AbstractText As String

    End Class

End Namespace
