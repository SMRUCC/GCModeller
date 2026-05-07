' ============================================================================
' V2reportsOrganelleInfo.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsOrganelleInfo
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsOrganelleInfo

        ''' <summary>
        ''' assembly_name 属性
        ''' </summary>
        <Field("assembly_name")>
        Public Property AssemblyName As String

        ''' <summary>
        ''' infraspecific_name 属性
        ''' </summary>
        <Field("infraspecific_name")>
        Public Property InfraspecificName As String

        ''' <summary>
        ''' bioproject 属性
        ''' </summary>
        <Field("bioproject")>
        Public Property Bioproject As List(Of String)

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <Field("description")>
        Public Property Description As String

        ''' <summary>
        ''' total_seq_length 属性
        ''' </summary>
        <Field("total_seq_length")>
        Public Property TotalSeqLength As String

        ''' <summary>
        ''' submitter 属性
        ''' </summary>
        <Field("submitter")>
        Public Property Submitter As String

    End Class

End Namespace
