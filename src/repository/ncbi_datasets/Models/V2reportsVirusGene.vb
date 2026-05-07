' ============================================================================
' V2reportsVirusGene.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsVirusGene
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsVirusGene

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <JsonProperty("name")>
        Public Property Name As String

        ''' <summary>
        ''' gene_id 属性
        ''' </summary>
        <JsonProperty("gene_id")>
        Public Property GeneId As Integer?

        ''' <summary>
        ''' nucleotide 属性
        ''' </summary>
        <JsonProperty("nucleotide")>
        Public Property Nucleotide As Object

        ''' <summary>
        ''' cds 属性
        ''' </summary>
        <JsonProperty("cds")>
        Public Property Cds As List(Of Object)

    End Class

End Namespace
