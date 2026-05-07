' ============================================================================
' V2reportsTaxonomyNode.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsTaxonomyNode
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsTaxonomyNode

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <Field("tax_id")>
        Public Property TaxId As Integer?

        ''' <summary>
        ''' rank 属性
        ''' </summary>
        <Field("rank")>
        Public Property Rank As Object

        ''' <summary>
        ''' current_scientific_name 属性
        ''' </summary>
        <Field("current_scientific_name")>
        Public Property CurrentScientificName As Object

        ''' <summary>
        ''' basionym 属性
        ''' </summary>
        <Field("basionym")>
        Public Property Basionym As Object

        ''' <summary>
        ''' curator_common_name 属性
        ''' </summary>
        <Field("curator_common_name")>
        Public Property CuratorCommonName As String

        ''' <summary>
        ''' group_name 属性
        ''' </summary>
        <Field("group_name")>
        Public Property GroupName As String

        ''' <summary>
        ''' has_type_material 属性
        ''' </summary>
        <Field("has_type_material")>
        Public Property HasTypeMaterial As Boolean?

        ''' <summary>
        ''' classification 属性
        ''' </summary>
        <Field("classification")>
        Public Property Classification As Object

        ''' <summary>
        ''' parents 属性
        ''' </summary>
        <Field("parents")>
        Public Property Parents As List(Of Integer)

        ''' <summary>
        ''' children 属性
        ''' </summary>
        <Field("children")>
        Public Property Children As List(Of Integer)

        ''' <summary>
        ''' counts 属性
        ''' </summary>
        <Field("counts")>
        Public Property Counts As List(Of Object)

        ''' <summary>
        ''' genomic_moltype 属性
        ''' </summary>
        <Field("genomic_moltype")>
        Public Property GenomicMoltype As String

        ''' <summary>
        ''' current_scientific_name_is_formal 属性
        ''' </summary>
        <Field("current_scientific_name_is_formal")>
        Public Property CurrentScientificNameIsFormal As Boolean?

        ''' <summary>
        ''' secondary_tax_ids 属性
        ''' </summary>
        <Field("secondary_tax_ids")>
        Public Property SecondaryTaxIds As List(Of Integer)

        ''' <summary>
        ''' extinct 属性
        ''' </summary>
        <Field("extinct")>
        Public Property Extinct As Boolean?

    End Class

End Namespace
