Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.KEGG.Metabolism

Public Class RepositoryArguments

    Public KEGGCompounds As String
    Public KEGGReactions As String
    Public KEGGPathway As String

    Dim compoundsRepository As CompoundRepository
    Dim reactionsRepository As ReactionRepository
    Dim pathwayRepository As PathwayRepository

    Public Function GetCompounds() As CompoundRepository
        If compoundsRepository Is Nothing Then
            compoundsRepository = KEGGCompounds.FetchCompoundRepository
        End If

        Return compoundsRepository
    End Function

    Public Function GetReactions() As ReactionRepository
        If reactionsRepository Is Nothing Then
            reactionsRepository = KEGGReactions.FetchReactionRepository
        End If

        Return reactionsRepository
    End Function

    Public Function GetPathways() As PathwayRepository
        If pathwayRepository Is Nothing Then
            pathwayRepository = KEGGPathway.FetchPathwayRepository
        End If

        Return pathwayRepository
    End Function

End Class
