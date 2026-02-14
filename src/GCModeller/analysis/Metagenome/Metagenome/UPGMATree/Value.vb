Imports Microsoft.VisualBasic.Serialization.JSON

Namespace UPGMATree

    Public Class Value

        Public Property size%
        Public Property distance#

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace