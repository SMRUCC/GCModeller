Imports Microsoft.VisualBasic.DocumentFormat.Csv

Module Program

    Public Function Main() As Integer
        Return GetType(CLI).RunCLI(App.CommandLine)
    End Function
End Module


Public Class genome
    Public Property gi As String
    Public Property Sum As Double
    Public Property w As Double
    Public Property hit_name As String
    Public Property name As String

End Class