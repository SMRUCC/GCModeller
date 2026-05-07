' ============================================================================
' V2reportsWGSInfo.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsWGSInfo
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsWGSInfo

        ''' <summary>
        ''' wgs_project_accession 属性
        ''' </summary>
        <JsonProperty("wgs_project_accession")>
        Public Property WgsProjectAccession As String

        ''' <summary>
        ''' master_wgs_url 属性
        ''' </summary>
        <JsonProperty("master_wgs_url")>
        Public Property MasterWgsUrl As String

        ''' <summary>
        ''' wgs_contigs_url 属性
        ''' </summary>
        <JsonProperty("wgs_contigs_url")>
        Public Property WgsContigsUrl As String

    End Class

End Namespace
