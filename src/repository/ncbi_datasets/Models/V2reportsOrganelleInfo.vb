' ============================================================================
' V2reportsOrganelleInfo.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsOrganelleInfo
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsOrganelleInfo

        ''' <summary>
        ''' assembly_name 属性
        ''' </summary>
        <JsonProperty("assembly_name")>
        Public Property AssemblyName As String

        ''' <summary>
        ''' infraspecific_name 属性
        ''' </summary>
        <JsonProperty("infraspecific_name")>
        Public Property InfraspecificName As String

        ''' <summary>
        ''' bioproject 属性
        ''' </summary>
        <JsonProperty("bioproject")>
        Public Property Bioproject As List(Of String)

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <JsonProperty("description")>
        Public Property Description As String

        ''' <summary>
        ''' total_seq_length 属性
        ''' </summary>
        <JsonProperty("total_seq_length")>
        Public Property TotalSeqLength As String

        ''' <summary>
        ''' submitter 属性
        ''' </summary>
        <JsonProperty("submitter")>
        Public Property Submitter As String

    End Class

End Namespace
