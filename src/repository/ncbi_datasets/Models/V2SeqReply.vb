' ============================================================================
' V2SeqReply.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2SeqReply
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2SeqReply

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <JsonProperty("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' seq_length 属性
        ''' </summary>
        <JsonProperty("seq_length")>
        Public Property SeqLength As String

        ''' <summary>
        ''' mol_type 属性
        ''' </summary>
        <JsonProperty("mol_type")>
        Public Property MolType As Object

        ''' <summary>
        ''' defline 属性
        ''' </summary>
        <JsonProperty("defline")>
        Public Property Defline As String

        ''' <summary>
        ''' sequence 属性
        ''' </summary>
        <JsonProperty("sequence")>
        Public Property Sequence As String

    End Class

End Namespace
