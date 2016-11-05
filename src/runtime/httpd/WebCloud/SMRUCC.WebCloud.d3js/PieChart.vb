Imports Microsoft.VisualBasic.Serialization.JSON

Namespace PieChart

    Public Class Slice

        Public Property name As String
        Public Property color As String
        Public Property value As Double
        Public Property Data As Dictionary(Of String, String)

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
