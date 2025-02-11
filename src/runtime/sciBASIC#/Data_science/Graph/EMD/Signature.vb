Imports Microsoft.VisualBasic.Serialization.JSON

Namespace EMD

    ''' <summary>
    ''' @author Telmo Menezes (telmo@telmomenezes.com)
    ''' 
    ''' </summary>
    Public Class Signature

        Public Overridable Property NumberOfFeatures As Integer
        Public Overridable Property Features As Feature()
        Public Overridable Property Weights As Double()

        Public Overrides Function ToString() As String
            Return $"[{NumberOfFeatures}] {Weights.GetJson}"
        End Function

    End Class
End Namespace
