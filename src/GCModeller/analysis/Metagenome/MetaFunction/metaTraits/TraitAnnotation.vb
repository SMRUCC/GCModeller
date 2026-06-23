Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection

Namespace metaTraits

    Public Class TraitData

        Public Property trait_name As String
        Public Property unit As String
        Public Property database_count As String
        Public Property total_observations As String
        Public Property consensus_value As String
        Public Property consensus_count As String
        Public Property consensus_percentage As String
        Public Property minimum As String
        Public Property median As String
        Public Property mean As String
        Public Property maximum As String
        <Collection("discrete_values", ";")> Public Property discrete_values As String()
        <Collection("databases", ";")> Public Property databases As String()
        Public Property group_1 As String
        Public Property group_2 As String
        <Collection("ontology_ids", ";")> Public Property ontology_ids As String()

    End Class

    ''' <summary>
    ''' table parser model for ncbi_species_summary_no_predictions.tsv
    ''' </summary>
    Public Class TraitAnnotation : Inherits TraitData

        Public Property taxon_id As UInteger
        Public Property taxon_name As String

        <Collection("taxon_lineage", "|")> Public Property taxon_lineage As String()

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="file">the file path to the tsv table file</param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseTable(file As String) As IEnumerable(Of TraitAnnotation)
            Return file.LoadCsv(Of TraitAnnotation)(mute:=True, tsv:=True)
        End Function

    End Class
End Namespace