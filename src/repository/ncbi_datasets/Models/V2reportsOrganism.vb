' ============================================================================
' V2reportsOrganism.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsOrganism
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsOrganism

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <Field("tax_id")>
        Public Property TaxId As Integer?

        ''' <summary>
        ''' sci_name 属性
        ''' </summary>
        <Field("sci_name")>
        Public Property SciName As String

        ''' <summary>
        ''' organism_name 属性
        ''' </summary>
        <Field("organism_name")>
        Public Property OrganismName As String

        ''' <summary>
        ''' common_name 属性
        ''' </summary>
        <Field("common_name")>
        Public Property CommonName As String

        ''' <summary>
        ''' lineage 属性
        ''' </summary>
        <Field("lineage")>
        Public Property Lineage As List(Of Object)

        ''' <summary>
        ''' strain 属性
        ''' </summary>
        <Field("strain")>
        Public Property Strain As String

        ''' <summary>
        ''' pangolin_classification 属性
        ''' </summary>
        <Field("pangolin_classification")>
        Public Property PangolinClassification As String

        ''' <summary>
        ''' infraspecific_names 属性
        ''' </summary>
        <Field("infraspecific_names")>
        Public Property InfraspecificNames As Object

    End Class

End Namespace
