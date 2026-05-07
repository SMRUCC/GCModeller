' ============================================================================
' V2reportsSequenceInformation.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsSequenceInformation
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsSequenceInformation

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <JsonProperty("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' submission_date 属性
        ''' </summary>
        <JsonProperty("submission_date")>
        Public Property SubmissionDate As String

        ''' <summary>
        ''' submitter 属性
        ''' </summary>
        <JsonProperty("submitter")>
        Public Property Submitter As String

    End Class

End Namespace
