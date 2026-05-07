' ============================================================================
' V2AssemblyCheckMHistogramReply.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2AssemblyCheckMHistogramReply
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2AssemblyCheckMHistogramReply

        ''' <summary>
        ''' species_taxid 属性
        ''' </summary>
        <Field("species_taxid")>
        Public Property SpeciesTaxid As Integer?

        ''' <summary>
        ''' histogram_intervals 属性
        ''' </summary>
        <Field("histogram_intervals")>
        Public Property HistogramIntervals As List(Of Object)

    End Class

End Namespace
