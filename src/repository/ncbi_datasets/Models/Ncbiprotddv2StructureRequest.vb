' ============================================================================
' Ncbiprotddv2StructureRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2StructureRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2StructureRequest

        ''' <summary>
        ''' pdb_id 属性
        ''' </summary>
        <JsonProperty("pdb_id")>
        Public Property PdbId As String

        ''' <summary>
        ''' mmdb_id 属性
        ''' </summary>
        <JsonProperty("mmdb_id")>
        Public Property MmdbId As Integer?

    End Class

End Namespace
