' ============================================================================
' V2reportsANIMatch.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsANIMatch
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsANIMatch

        ''' <summary>
        ''' assembly 属性
        ''' </summary>
        <JsonProperty("assembly")>
        Public Property Assembly As String

        ''' <summary>
        ''' organism_name 属性
        ''' </summary>
        <JsonProperty("organism_name")>
        Public Property OrganismName As String

        ''' <summary>
        ''' category 属性
        ''' </summary>
        <JsonProperty("category")>
        Public Property Category As Object

        ''' <summary>
        ''' ani 属性
        ''' </summary>
        <JsonProperty("ani")>
        Public Property Ani As Single?

        ''' <summary>
        ''' assembly_coverage 属性
        ''' </summary>
        <JsonProperty("assembly_coverage")>
        Public Property AssemblyCoverage As Single?

        ''' <summary>
        ''' type_assembly_coverage 属性
        ''' </summary>
        <JsonProperty("type_assembly_coverage")>
        Public Property TypeAssemblyCoverage As Single?

    End Class

End Namespace
