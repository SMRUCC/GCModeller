Imports SMRUCC.genomics.Analysis.RetroPath.Chem
Imports SMRUCC.genomics.Analysis.RetroPath.Search

Public Class Walk

    Public ReadOnly Property opts As SearchOptions
    Public ReadOnly Property w As ScoreWeights
    Public ReadOnly Property rules As List(Of Rule)
    Public ReadOnly Property sink As List(Of Tuple(Of String, String))


    Public Function Search(targetSmiles As String)

    End Function
End Class
