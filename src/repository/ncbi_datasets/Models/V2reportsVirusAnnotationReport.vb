' ============================================================================
' V2reportsVirusAnnotationReport.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsVirusAnnotationReport
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsVirusAnnotationReport

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <JsonProperty("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' isolate_name 属性
        ''' </summary>
        <JsonProperty("isolate_name")>
        Public Property IsolateName As String

        ''' <summary>
        ''' genes 属性
        ''' </summary>
        <JsonProperty("genes")>
        Public Property Genes As List(Of Object)

    End Class

End Namespace
