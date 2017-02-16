Imports Microsoft.VisualBasic.Serialization.JSON

Public Class GoBriefTable

    Public Property goID As String
    Public Property [namespace] As String
    Public Property name As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
