Imports c2.Reconstruction.Operation

Namespace Reconstruction.ObjectEquals

    Friend Class Promoters : Inherits c2.Reconstruction.ObjectEquals.EqualsOperation

        Sub New(Session As OperationSession, Promoters As c2.Reconstruction.Promoters)
            Call MyBase.New(Session)
            Dim LQuery = From Item In Promoters.ReconstructList Select New KeyValuePair(Of String, String())(Item.Key, New String() {Item.Value.Identifier}) '
            Me.EqualsList = LQuery.ToArray
        End Sub

        Public Overrides Function Initialize() As Integer
            Return 0
        End Function

        Public Shared Function Create(Promoters As c2.Reconstruction.Promoters) As Promoters
            Return New Promoters(Nothing, Promoters)
        End Function
    End Class
End Namespace