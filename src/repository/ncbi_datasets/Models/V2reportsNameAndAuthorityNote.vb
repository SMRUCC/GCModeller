' ============================================================================
' V2reportsNameAndAuthorityNote.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsNameAndAuthorityNote
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsNameAndAuthorityNote

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <JsonProperty("name")>
        Public Property Name As String

        ''' <summary>
        ''' note 属性
        ''' </summary>
        <JsonProperty("note")>
        Public Property Note As String

        ''' <summary>
        ''' note_classifier 属性
        ''' </summary>
        <JsonProperty("note_classifier")>
        Public Property NoteClassifier As Object

    End Class

End Namespace
