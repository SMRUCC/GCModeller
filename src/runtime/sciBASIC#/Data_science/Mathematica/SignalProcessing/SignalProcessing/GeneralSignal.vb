Imports System.Text

Public Class GeneralSignal

    Public Property Measures As Double()
    Public Property Strength As Double()

    Public Property MeasureUnit As String
    Public Property Description As String

    Public Overrides Function ToString() As String
        Return Description
    End Function

    Public Function GetText() As String
        Dim sb As New StringBuilder

        Return sb.ToString
    End Function

End Class
