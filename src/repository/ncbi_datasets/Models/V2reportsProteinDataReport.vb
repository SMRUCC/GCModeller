' ============================================================================
' V2reportsProteinDataReport.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsProteinDataReport
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsProteinDataReport

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <Field("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <Field("description")>
        Public Property Description As String

        ''' <summary>
        ''' length 属性
        ''' </summary>
        <Field("length")>
        Public Property Length As Integer?

        ''' <summary>
        ''' gene_id 属性
        ''' </summary>
        <Field("gene_id")>
        Public Property GeneId As Integer?

        ''' <summary>
        ''' identical_protein_group 属性
        ''' </summary>
        <Field("identical_protein_group")>
        Public Property IdenticalProteinGroup As Integer?

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <Field("tax_id")>
        Public Property TaxId As Integer?

        ''' <summary>
        ''' conserved_domains 属性
        ''' </summary>
        <Field("conserved_domains")>
        Public Property ConservedDomains As List(Of Object)

        ''' <summary>
        ''' functional_sites 属性
        ''' </summary>
        <Field("functional_sites")>
        Public Property FunctionalSites As List(Of Object)

        ''' <summary>
        ''' protein_families 属性
        ''' </summary>
        <Field("protein_families")>
        Public Property ProteinFamilies As List(Of Object)

    End Class

End Namespace
