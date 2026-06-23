Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Metagenomics

Namespace metaTraits

    Public Class TraitData

        Public Property trait_name As String
        Public Property unit As String
        Public Property database_count As Integer
        Public Property total_observations As Integer
        Public Property consensus_value As String
        Public Property consensus_count As String
        Public Property consensus_percentage As String
        Public Property minimum As String
        Public Property median As String
        Public Property mean As String
        Public Property maximum As String
        Public Property discrete_values As Dictionary(Of String, String)
        Public Property databases As Dictionary(Of String, String)

        ''' <summary>
        ''' main class of the <see cref="trait_name"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property group_1 As String
        ''' <summary>
        ''' sub class of the <see cref="trait_name"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property group_2 As String
        Public Property ontology_ids As String()

        Sub New()
        End Sub

        Sub New(trait As TraitAnnotation)
            trait_name = trait.trait_name
            unit = trait.unit
            database_count = trait.database_count
            total_observations = trait.total_observations
            consensus_count = trait.consensus_count
            consensus_percentage = trait.consensus_percentage
            consensus_value = trait.consensus_value
            minimum = trait.minimum
            median = trait.median
            mean = trait.mean
            maximum = trait.maximum
            discrete_values = ParseInnerTable(trait.discrete_values)
            databases = ParseInnerTable(trait.databases)
            group_1 = trait.group_1
            group_2 = trait.group_2
            ontology_ids = trait.ontology_ids
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function ParseInnerTable(list As String()) As Dictionary(Of String, String)
            If list.TryCount = 1 AndAlso list(0).StringEmpty(, True) Then
                Return New Dictionary(Of String, String)
            End If

            Return list _
                .SafeQuery _
                .Select(Function(s) s.GetTagValue("=")) _
                .ToDictionary(Function(a) a.Name,
                              Function(a)
                                  Return a.Value
                              End Function)
        End Function

        Public Overrides Function ToString() As String
            Return $"[{group_1}/{group_2}] {trait_name}({unit}) ~ {ontology_ids.GetJson}"
        End Function

    End Class

    ''' <summary>
    ''' table parser model for ncbi_species_summary_no_predictions.tsv
    ''' </summary>
    Public Class TraitAnnotation

        Public Property taxon_id As UInteger
        Public Property taxon_name As String
        Public Property trait_name As String
        Public Property unit As String
        Public Property database_count As Integer
        Public Property total_observations As Integer
        Public Property consensus_value As String
        Public Property consensus_count As String
        Public Property consensus_percentage As String
        Public Property minimum As String
        Public Property median As String
        Public Property mean As String
        Public Property maximum As String
        <Collection("discrete_values", ";")> Public Property discrete_values As String()
        <Collection("databases", ";")> Public Property databases As String()

        ''' <summary>
        ''' main class of the <see cref="trait_name"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property group_1 As String
        ''' <summary>
        ''' sub class of the <see cref="trait_name"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property group_2 As String
        <Collection("ontology_ids", ";")> Public Property ontology_ids As String()
        <Collection("taxon_lineage", "|")> Public Property taxon_lineage As String()

        Public Overrides Function ToString() As String
            Return $"[{group_1}/{group_2}] {trait_name}({unit}) ~ {ontology_ids.GetJson}"
        End Function

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

        Public Shared Iterator Function CreateProfiles(annos As IEnumerable(Of TraitAnnotation)) As IEnumerable(Of metaTraitData)
            For Each tax In annos.GroupBy(Function(a) a.taxon_id)
                Dim meta As TraitAnnotation = tax.First
                Dim data As TraitData() = tax _
                    .Select(Function(t)
                                Return New TraitData(t)
                            End Function) _
                    .ToArray

                Yield New metaTraitData With {
                    .taxon_id = tax.Key,
                    .taxon_name = meta.taxon_name,
                    .taxon_lineage = New Taxonomy(meta.taxon_lineage),
                    .traits = data
                }
            Next
        End Function

    End Class
End Namespace