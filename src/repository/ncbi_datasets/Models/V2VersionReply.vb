' ============================================================================
' V2VersionReply.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2VersionReply
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2VersionReply

        ''' <summary>
        ''' version 属性
        ''' </summary>
        <JsonProperty("version")>
        Public Property Version As String

        ''' <summary>
        ''' major_ver 属性
        ''' </summary>
        <JsonProperty("major_ver")>
        Public Property MajorVer As Integer?

        ''' <summary>
        ''' minor_ver 属性
        ''' </summary>
        <JsonProperty("minor_ver")>
        Public Property MinorVer As Integer?

        ''' <summary>
        ''' patch_ver 属性
        ''' </summary>
        <JsonProperty("patch_ver")>
        Public Property PatchVer As Integer?

    End Class

End Namespace
