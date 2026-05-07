' ============================================================================
' Ncbiprotddv2ParsedAbstractEpub.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2ParsedAbstractEpub
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2ParsedAbstractEpub

        ''' <summary>
        ''' journal 属性
        ''' </summary>
        <JsonProperty("journal")>
        Public Property Journal As String

        ''' <summary>
        ''' year 属性
        ''' </summary>
        <JsonProperty("year")>
        Public Property Year As Integer?

        ''' <summary>
        ''' volume 属性
        ''' </summary>
        <JsonProperty("volume")>
        Public Property Volume As Integer?

        ''' <summary>
        ''' pages 属性
        ''' </summary>
        <JsonProperty("pages")>
        Public Property Pages As String

    End Class

End Namespace
