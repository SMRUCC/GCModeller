' ============================================================================
' V2reportsProteinFamily.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsProteinFamily
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsProteinFamily

        ''' <summary>
        ''' method 属性
        ''' </summary>
        <Field("method")>
        Public Property Method As String

        ''' <summary>
        ''' identifier 属性
        ''' </summary>
        <Field("identifier")>
        Public Property Identifier As Integer?

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <Field("description")>
        Public Property Description As String

        ''' <summary>
        ''' type 属性
        ''' </summary>
        <Field("type")>
        Public Property Type As String

    End Class

End Namespace
