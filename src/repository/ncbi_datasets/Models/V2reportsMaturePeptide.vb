' ============================================================================
' V2reportsMaturePeptide.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsMaturePeptide
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsMaturePeptide

        ''' <summary>
        ''' accession_version 属性
        ''' </summary>
        <JsonProperty("accession_version")>
        Public Property AccessionVersion As String

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <JsonProperty("name")>
        Public Property Name As String

        ''' <summary>
        ''' length 属性
        ''' </summary>
        <JsonProperty("length")>
        Public Property Length As Integer?

    End Class

End Namespace
