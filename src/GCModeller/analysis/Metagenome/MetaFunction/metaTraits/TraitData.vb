Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

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
End Namespace