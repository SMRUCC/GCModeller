' ============================================================================
' V2reportsSeqRangeSetFasta.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsSeqRangeSetFasta
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsSeqRangeSetFasta

        ''' <summary>
        ''' seq_id 属性
        ''' </summary>
        <Field("seq_id")>
        Public Property SeqId As String

        ''' <summary>
        ''' accession_version 属性
        ''' </summary>
        <Field("accession_version")>
        Public Property AccessionVersion As String

        ''' <summary>
        ''' title 属性
        ''' </summary>
        <Field("title")>
        Public Property Title As String

        ''' <summary>
        ''' sequence_hash 属性
        ''' </summary>
        <Field("sequence_hash")>
        Public Property SequenceHash As String

        ''' <summary>
        ''' range 属性
        ''' </summary>
        <Field("range")>
        Public Property Range As List(Of Object)

    End Class

End Namespace
