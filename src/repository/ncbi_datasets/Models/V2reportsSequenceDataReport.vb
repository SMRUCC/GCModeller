' ============================================================================
' V2reportsSequenceDataReport.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsSequenceDataReport
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsSequenceDataReport

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <Field("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' organism_name 属性
        ''' </summary>
        <Field("organism_name")>
        Public Property OrganismName As String

        ''' <summary>
        ''' length 属性
        ''' </summary>
        <Field("length")>
        Public Property Length As Integer?

        ''' <summary>
        ''' units 属性
        ''' </summary>
        <Field("units")>
        Public Property Units As String

        ''' <summary>
        ''' molecule_type 属性
        ''' </summary>
        <Field("molecule_type")>
        Public Property MoleculeType As String

        ''' <summary>
        ''' source_database 属性
        ''' </summary>
        <Field("source_database")>
        Public Property SourceDatabase As String

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <Field("description")>
        Public Property Description As String

        ''' <summary>
        ''' source_mrna 属性
        ''' </summary>
        <Field("source_mrna")>
        Public Property SourceMrna As String

        ''' <summary>
        ''' encoded_proteins 属性
        ''' </summary>
        <Field("encoded_proteins")>
        Public Property EncodedProteins As List(Of Object)

        ''' <summary>
        ''' publication_date 属性
        ''' </summary>
        <Field("publication_date")>
        Public Property PublicationDate As String

        ''' <summary>
        ''' latest_update_date 属性
        ''' </summary>
        <Field("latest_update_date")>
        Public Property LatestUpdateDate As String

        ''' <summary>
        ''' gene_context 属性
        ''' </summary>
        <Field("gene_context")>
        Public Property GeneContext As Object

        ''' <summary>
        ''' features 属性
        ''' </summary>
        <Field("features")>
        Public Property Features As List(Of Object)

        ''' <summary>
        ''' external_ids 属性
        ''' </summary>
        <Field("external_ids")>
        Public Property ExternalIds As List(Of Object)

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <Field("tax_id")>
        Public Property TaxId As Integer?

        ''' <summary>
        ''' submissions 属性
        ''' </summary>
        <Field("submissions")>
        Public Property Submissions As List(Of Object)

        ''' <summary>
        ''' publications 属性
        ''' </summary>
        <Field("publications")>
        Public Property Publications As List(Of Object)

    End Class

End Namespace
