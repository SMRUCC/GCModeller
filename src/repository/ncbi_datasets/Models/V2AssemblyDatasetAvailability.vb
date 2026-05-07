' ============================================================================
' V2AssemblyDatasetAvailability.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2AssemblyDatasetAvailability
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2AssemblyDatasetAvailability

        ''' <summary>
        ''' valid_assemblies 属性
        ''' </summary>
        <JsonProperty("valid_assemblies")>
        Public Property ValidAssemblies As List(Of String)

        ''' <summary>
        ''' invalid_assemblies 属性
        ''' </summary>
        <JsonProperty("invalid_assemblies")>
        Public Property InvalidAssemblies As List(Of String)

        ''' <summary>
        ''' reason 属性
        ''' </summary>
        <JsonProperty("reason")>
        Public Property Reason As String

    End Class

End Namespace
