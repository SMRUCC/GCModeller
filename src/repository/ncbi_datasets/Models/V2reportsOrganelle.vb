' ============================================================================
' V2reportsOrganelle.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsOrganelle
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsOrganelle

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <Field("description")>
        Public Property Description As Object

        ''' <summary>
        ''' genbank 属性
        ''' </summary>
        <Field("genbank")>
        Public Property Genbank As Object

        ''' <summary>
        ''' refseq 属性
        ''' </summary>
        <Field("refseq")>
        Public Property Refseq As Object

        ''' <summary>
        ''' organism 属性
        ''' </summary>
        <Field("organism")>
        Public Property Organism As Object

        ''' <summary>
        ''' bioprojects 属性
        ''' </summary>
        <Field("bioprojects")>
        Public Property Bioprojects As List(Of Object)

        ''' <summary>
        ''' biosample 属性
        ''' </summary>
        <Field("biosample")>
        Public Property Biosample As Object

        ''' <summary>
        ''' gene_counts 属性
        ''' </summary>
        <Field("gene_counts")>
        Public Property GeneCounts As Object

        ''' <summary>
        ''' length 属性
        ''' </summary>
        <Field("length")>
        Public Property Length As Integer?

        ''' <summary>
        ''' topology 属性
        ''' </summary>
        <Field("topology")>
        Public Property Topology As Object

        ''' <summary>
        ''' gene_count 属性
        ''' </summary>
        <Field("gene_count")>
        Public Property GeneCount As Integer?

    End Class

End Namespace
