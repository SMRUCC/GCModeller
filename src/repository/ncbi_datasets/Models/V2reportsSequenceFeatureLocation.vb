' ============================================================================
' V2reportsSequenceFeatureLocation.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsSequenceFeatureLocation
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsSequenceFeatureLocation

        ''' <summary>
        ''' begin 属性
        ''' </summary>
        <JsonProperty("begin")>
        Public Property Begin As String

        ''' <summary>
        ''' end 属性
        ''' </summary>
        <JsonProperty("end")>
        Public Property End As String

        ''' <summary>
        ''' position 属性
        ''' </summary>
        <JsonProperty("position")>
        Public Property Position As String

        ''' <summary>
        ''' orientation 属性
        ''' </summary>
        <JsonProperty("orientation")>
        Public Property Orientation As String

    End Class

End Namespace
