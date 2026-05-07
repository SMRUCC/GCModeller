' ============================================================================
' V2reportsVirusAssemblyCollectionLocation.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsVirusAssemblyCollectionLocation
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsVirusAssemblyCollectionLocation

        ''' <summary>
        ''' geographic_location 属性
        ''' </summary>
        <JsonProperty("geographic_location")>
        Public Property GeographicLocation As String

        ''' <summary>
        ''' geographic_region 属性
        ''' </summary>
        <JsonProperty("geographic_region")>
        Public Property GeographicRegion As String

        ''' <summary>
        ''' usa_state 属性
        ''' </summary>
        <JsonProperty("usa_state")>
        Public Property UsaState As String

    End Class

End Namespace
