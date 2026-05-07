' ============================================================================
' Ncbiprotddv2ChainFootprint.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2ChainFootprint
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2ChainFootprint

        ''' <summary>
        ''' query_from 属性
        ''' </summary>
        <JsonProperty("query_from")>
        Public Property QueryFrom As Integer?

        ''' <summary>
        ''' query_to 属性
        ''' </summary>
        <JsonProperty("query_to")>
        Public Property QueryTo As Integer?

        ''' <summary>
        ''' dependent_from 属性
        ''' </summary>
        <JsonProperty("dependent_from")>
        Public Property DependentFrom As Integer?

        ''' <summary>
        ''' dependent_to 属性
        ''' </summary>
        <JsonProperty("dependent_to")>
        Public Property DependentTo As Integer?

    End Class

End Namespace
