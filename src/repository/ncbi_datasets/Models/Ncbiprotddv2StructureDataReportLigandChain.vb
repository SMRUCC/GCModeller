' ============================================================================
' Ncbiprotddv2StructureDataReportLigandChain.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2StructureDataReportLigandChain
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2StructureDataReportLigandChain

        ''' <summary>
        ''' chain_id 属性
        ''' </summary>
        <JsonProperty("chain_id")>
        Public Property ChainId As String

        ''' <summary>
        ''' kind 属性
        ''' </summary>
        <JsonProperty("kind")>
        Public Property Kind As Object

        ''' <summary>
        ''' sid 属性
        ''' </summary>
        <JsonProperty("sid")>
        Public Property Sid As Integer?

        ''' <summary>
        ''' sdid 属性
        ''' </summary>
        <JsonProperty("sdid")>
        Public Property Sdid As Integer?

    End Class

End Namespace
