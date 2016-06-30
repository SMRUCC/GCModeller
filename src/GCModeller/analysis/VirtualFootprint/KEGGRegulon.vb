Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class KEGGRegulon

    Public Property Regulator As String()
    Public Property Members As String()
    Public Property Family As String()
    Public Property ModId As String
    Public Property Type As String
    Public Property [Class] As String
    Public Property Category As String
    Public Property Name As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
