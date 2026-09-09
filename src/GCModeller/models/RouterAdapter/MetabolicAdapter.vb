Imports SMRUCC.genomics.Analysis.RetroPath.Model
Imports SMRUCC.genomics.Analysis.RetroPath.Search
Imports SMRUCC.genomics.MetabolicModel

Public Class MetabolicAdapter : Implements IRouter

    Sub New(compounds As IEnumerable(Of MetabolicCompound), metabolic As IEnumerable(Of MetabolicReaction),
            Optional opts As SearchOptions = Nothing,
            Optional w As ScoreWeights = Nothing)

    End Sub

    Public Function FindPathway(targetSmiles As String) As PathReport Implements IRouter.FindPathway
        Throw New NotImplementedException()
    End Function
End Class
