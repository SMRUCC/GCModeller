' ============================================================================
' V2reportsBiocollectionInstitution.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsBiocollectionInstitution
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsBiocollectionInstitution

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' country 属性
        ''' </summary>
        <Field("country")>
        Public Property Country As String

        ''' <summary>
        ''' address 属性
        ''' </summary>
        <Field("address")>
        Public Property Address As String

        ''' <summary>
        ''' url 属性
        ''' </summary>
        <Field("url")>
        Public Property Url As String

        ''' <summary>
        ''' comments 属性
        ''' </summary>
        <Field("comments")>
        Public Property Comments As String

        ''' <summary>
        ''' bio_collections 属性
        ''' </summary>
        <Field("bio_collections")>
        Public Property BioCollections As List(Of Object)

    End Class

End Namespace
