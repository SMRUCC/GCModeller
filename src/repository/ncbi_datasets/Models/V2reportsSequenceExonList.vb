' ============================================================================
' V2reportsSequenceExonList.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsSequenceExonList
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsSequenceExonList

        ''' <summary>
        ''' count 属性
        ''' </summary>
        <Field("count")>
        Public Property Count As Integer?

        ''' <summary>
        ''' inference_method 属性
        ''' </summary>
        <Field("inference_method")>
        Public Property InferenceMethod As String

        ''' <summary>
        ''' items 属性
        ''' </summary>
        <Field("items")>
        Public Property Items As List(Of Object)

    End Class

End Namespace
