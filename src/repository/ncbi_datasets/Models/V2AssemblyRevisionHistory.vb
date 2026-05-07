' ============================================================================
' V2AssemblyRevisionHistory.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2AssemblyRevisionHistory
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2AssemblyRevisionHistory

        ''' <summary>
        ''' assembly_revisions 属性
        ''' </summary>
        <JsonProperty("assembly_revisions")>
        Public Property AssemblyRevisions As List(Of Object)

        ''' <summary>
        ''' total_count 属性
        ''' </summary>
        <JsonProperty("total_count")>
        Public Property TotalCount As Integer?

    End Class

End Namespace
