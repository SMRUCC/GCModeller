Namespace Reconstruction.ObjectEquals

    Friend Class Session

        Public ProteinEquals As c2.Reconstruction.ObjectEquals.Proteins
        Public PromoterEquals As c2.Reconstruction.ObjectEquals.Promoters
        Public ReactionEquals As c2.Reconstruction.ObjectEquals.Reactions

        Sub New(Session As c2.Reconstruction.Operation.OperationSession)
            ProteinEquals = New Proteins(Session)
            ReactionEquals = New Reactions(Session)
        End Sub

        Public Sub Initialize()
            Call ProteinEquals.Initialize()
            Call ReactionEquals.Initialize()
        End Sub
    End Class
End Namespace