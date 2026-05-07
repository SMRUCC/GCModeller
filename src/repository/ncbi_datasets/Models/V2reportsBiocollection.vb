' ============================================================================
' V2reportsBiocollection.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsBiocollection
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsBiocollection

        ''' <summary>
        ''' bio_collection_id 属性
        ''' </summary>
        <Field("bio_collection_id")>
        Public Property BioCollectionId As String

        ''' <summary>
        ''' code 属性
        ''' </summary>
        <Field("code")>
        Public Property Code As String

        ''' <summary>
        ''' ncbi_unique_code 属性
        ''' </summary>
        <Field("ncbi_unique_code")>
        Public Property NcbiUniqueCode As String

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' type 属性
        ''' </summary>
        <Field("type")>
        Public Property Type As String

        ''' <summary>
        ''' comments 属性
        ''' </summary>
        <Field("comments")>
        Public Property Comments As String

    End Class

End Namespace
