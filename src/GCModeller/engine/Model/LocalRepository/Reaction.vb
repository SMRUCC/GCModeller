Imports Microsoft.VisualBasic.Serialization.JSON

Namespace WebJSON

    Public Class Reaction

        Public Property guid As String
        Public Property name As String
        Public Property reaction As String
        Public Property left As Substrate()
        Public Property right As Substrate()
        Public Property law As LawData()

        Public Overrides Function ToString() As String
            Return $"{guid} - {name}"
        End Function
    End Class

    Public Class Substrate

        Public Property molecule_id As UInteger
        Public Property factor As Double
        Public Property location As UInteger

        Public Overrides Function ToString() As String
            Return $"{{{factor}}} {molecule_id}"
        End Function
    End Class

    Public Class LawData

        Public Property params As Dictionary(Of String, String)
        Public Property lambda As String
        Public Property metabolite_id As String
        Public Property ec_number As String

        Public Overrides Function ToString() As String
            Return $"{params.GetJson} -> {lambda}"
        End Function
    End Class
End Namespace