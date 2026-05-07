' ============================================================================
' V2DownloadSummaryHydrated.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2DownloadSummaryHydrated
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2DownloadSummaryHydrated

        ''' <summary>
        ''' estimated_file_size_mb 属性
        ''' </summary>
        <JsonProperty("estimated_file_size_mb")>
        Public Property EstimatedFileSizeMb As Integer?

        ''' <summary>
        ''' url 属性
        ''' </summary>
        <JsonProperty("url")>
        Public Property Url As String

        ''' <summary>
        ''' cli_download_command_line 属性
        ''' </summary>
        <JsonProperty("cli_download_command_line")>
        Public Property CliDownloadCommandLine As String

    End Class

End Namespace
