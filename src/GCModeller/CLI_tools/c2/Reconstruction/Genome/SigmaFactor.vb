Namespace Reconstruction

    Public Class SigmaFactor : Inherits Operation

        Sub New(Session As OperationSession)
            Call MyBase.New(Session)
        End Sub

        Public Overrides Function Performance() As Integer
            Dim sbjSigmaFactors = (From Protein In MyBase.Subject.GetProteins Where Protein.Types.IndexOf("Sigma-Factors") > -1 Select Protein).ToArray
            Throw New NotImplementedException
        End Function
    End Class
End Namespace