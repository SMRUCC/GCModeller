' ============================================================================
' V2reportsSequenceDataReport.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsSequenceDataReport
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Newtonsoft.Json

Namespace ncbi_datasets.Models

    Public Class V2reportsSequenceDataReport

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <JsonProperty("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' organism_name 属性
        ''' </summary>
        <JsonProperty("organism_name")>
        Public Property OrganismName As String

        ''' <summary>
        ''' length 属性
        ''' </summary>
        <JsonProperty("length")>
        Public Property Length As Integer?

        ''' <summary>
        ''' units 属性
        ''' </summary>
        <JsonProperty("units")>
        Public Property Units As String

        ''' <summary>
        ''' molecule_type 属性
        ''' </summary>
        <JsonProperty("molecule_type")>
        Public Property MoleculeType As String

        ''' <summary>
        ''' source_database 属性
        ''' </summary>
        <JsonProperty("source_database")>
        Public Property SourceDatabase As String

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <JsonProperty("description")>
        Public Property Description As String

        ''' <summary>
        ''' source_mrna 属性
        ''' </summary>
        <JsonProperty("source_mrna")>
        Public Property SourceMrna As String

        ''' <summary>
        ''' encoded_proteins 属性
        ''' </summary>
        <JsonProperty("encoded_proteins")>
        Public Property EncodedProteins As List(Of Object)

        ''' <summary>
        ''' publication_date 属性
        ''' </summary>
        <JsonProperty("publication_date")>
        Public Property PublicationDate As String

        ''' <summary>
        ''' latest_update_date 属性
        ''' </summary>
        <JsonProperty("latest_update_date")>
        Public Property LatestUpdateDate As String

        ''' <summary>
        ''' gene_context 属性
        ''' </summary>
        <JsonProperty("gene_context")>
        Public Property GeneContext As Object

        ''' <summary>
        ''' features 属性
        ''' </summary>
        <JsonProperty("features")>
        Public Property Features As List(Of Object)

        ''' <summary>
        ''' external_ids 属性
        ''' </summary>
        <JsonProperty("external_ids")>
        Public Property ExternalIds As List(Of Object)

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <JsonProperty("tax_id")>
        Public Property TaxId As Integer?

        ''' <summary>
        ''' submissions 属性
        ''' </summary>
        <JsonProperty("submissions")>
        Public Property Submissions As List(Of Object)

        ''' <summary>
        ''' publications 属性
        ''' </summary>
        <JsonProperty("publications")>
        Public Property Publications As List(Of Object)

    End Class

End Namespace
