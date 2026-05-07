' ============================================================================
' Ncbiprotddv2StructureDataReportBiounitChain.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2StructureDataReportBiounitChain
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2StructureDataReportBiounitChain

        ''' <summary>
        ''' chain_id 属性
        ''' </summary>
        <JsonProperty("chain_id")>
        Public Property ChainId As String

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <JsonProperty("tax_id")>
        Public Property TaxId As Integer?

        ''' <summary>
        ''' kind 属性
        ''' </summary>
        <JsonProperty("kind")>
        Public Property Kind As Object

        ''' <summary>
        ''' molecule_group 属性
        ''' </summary>
        <JsonProperty("molecule_group")>
        Public Property MoleculeGroup As Integer?

        ''' <summary>
        ''' sdid 属性
        ''' </summary>
        <JsonProperty("sdid")>
        Public Property Sdid As Integer?

    End Class

End Namespace
