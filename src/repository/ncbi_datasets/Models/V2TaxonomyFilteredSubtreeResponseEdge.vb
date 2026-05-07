' ============================================================================
' V2TaxonomyFilteredSubtreeResponseEdge.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2TaxonomyFilteredSubtreeResponseEdge
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2TaxonomyFilteredSubtreeResponseEdge

        ''' <summary>
        ''' visible_children 属性
        ''' </summary>
        <JsonProperty("visible_children")>
        Public Property VisibleChildren As List(Of Integer)

        ''' <summary>
        ''' children_status 属性
        ''' </summary>
        <JsonProperty("children_status")>
        Public Property ChildrenStatus As Object

        ''' <summary>
        ''' rank 属性
        ''' </summary>
        <JsonProperty("rank")>
        Public Property Rank As Object

        ''' <summary>
        ''' scientific_name 属性
        ''' </summary>
        <JsonProperty("scientific_name")>
        Public Property ScientificName As String

        ''' <summary>
        ''' curator_common_name 属性
        ''' </summary>
        <JsonProperty("curator_common_name")>
        Public Property CuratorCommonName As String

        ''' <summary>
        ''' assembly_count 属性
        ''' </summary>
        <JsonProperty("assembly_count")>
        Public Property AssemblyCount As Integer?

    End Class

End Namespace
