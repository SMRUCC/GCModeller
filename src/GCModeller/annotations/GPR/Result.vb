Public Class ScoredReaction
    Public Property Id As String
    Public Property Score As Double
End Class

Public Class GeneAssociation
    Public Property GeneId As String
    Public Property Reactions As List(Of ScoredReaction)
End Class