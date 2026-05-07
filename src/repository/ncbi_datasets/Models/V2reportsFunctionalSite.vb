' ============================================================================
' V2reportsFunctionalSite.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsFunctionalSite
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsFunctionalSite

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <JsonProperty("name")>
        Public Property Name As String

        ''' <summary>
        ''' type 属性
        ''' </summary>
        <JsonProperty("type")>
        Public Property Type As String

        ''' <summary>
        ''' specific 属性
        ''' </summary>
        <JsonProperty("specific")>
        Public Property Specific As Boolean?

        ''' <summary>
        ''' completeness 属性
        ''' </summary>
        <JsonProperty("completeness")>
        Public Property Completeness As Single?

        ''' <summary>
        ''' source_accession 属性
        ''' </summary>
        <JsonProperty("source_accession")>
        Public Property SourceAccession As String

        ''' <summary>
        ''' location 属性
        ''' </summary>
        <JsonProperty("location")>
        Public Property Location As List(Of Integer)

    End Class

End Namespace
