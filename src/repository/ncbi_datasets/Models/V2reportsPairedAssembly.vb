' ============================================================================
' V2reportsPairedAssembly.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsPairedAssembly
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsPairedAssembly

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <Field("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' status 属性
        ''' </summary>
        <Field("status")>
        Public Property Status As Object

        ''' <summary>
        ''' annotation_name 属性
        ''' </summary>
        <Field("annotation_name")>
        Public Property AnnotationName As String

        ''' <summary>
        ''' only_genbank 属性
        ''' </summary>
        <Field("only_genbank")>
        Public Property OnlyGenbank As String

        ''' <summary>
        ''' only_refseq 属性
        ''' </summary>
        <Field("only_refseq")>
        Public Property OnlyRefseq As String

        ''' <summary>
        ''' changed 属性
        ''' </summary>
        <Field("changed")>
        Public Property Changed As String

        ''' <summary>
        ''' manual_diff 属性
        ''' </summary>
        <Field("manual_diff")>
        Public Property ManualDiff As String

        ''' <summary>
        ''' refseq_genbank_are_different 属性
        ''' </summary>
        <Field("refseq_genbank_are_different")>
        Public Property RefseqGenbankAreDifferent As Boolean?

        ''' <summary>
        ''' differences 属性
        ''' </summary>
        <Field("differences")>
        Public Property Differences As String

    End Class

End Namespace
