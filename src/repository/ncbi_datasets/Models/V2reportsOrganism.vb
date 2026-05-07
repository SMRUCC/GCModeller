' ============================================================================
' V2reportsOrganism.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsOrganism
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsOrganism

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <JsonProperty("tax_id")>
        Public Property TaxId As Integer?

        ''' <summary>
        ''' sci_name 属性
        ''' </summary>
        <JsonProperty("sci_name")>
        Public Property SciName As String

        ''' <summary>
        ''' organism_name 属性
        ''' </summary>
        <JsonProperty("organism_name")>
        Public Property OrganismName As String

        ''' <summary>
        ''' common_name 属性
        ''' </summary>
        <JsonProperty("common_name")>
        Public Property CommonName As String

        ''' <summary>
        ''' lineage 属性
        ''' </summary>
        <JsonProperty("lineage")>
        Public Property Lineage As List(Of Object)

        ''' <summary>
        ''' strain 属性
        ''' </summary>
        <JsonProperty("strain")>
        Public Property Strain As String

        ''' <summary>
        ''' pangolin_classification 属性
        ''' </summary>
        <JsonProperty("pangolin_classification")>
        Public Property PangolinClassification As String

        ''' <summary>
        ''' infraspecific_names 属性
        ''' </summary>
        <JsonProperty("infraspecific_names")>
        Public Property InfraspecificNames As Object

    End Class

End Namespace
