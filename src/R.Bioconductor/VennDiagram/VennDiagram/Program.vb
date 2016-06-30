Imports Microsoft.VisualBasic.DocumentFormat.Csv

Module Program

    Sub New()
        Dim template As String = App.HOME & "/Templates/venn.csv"
        If Not template.FileExists Then
            Dim example As New DocumentStream.File

            example += {"objA", "objB", "objC", "objD", "objE"}
            example += {"1", "1", "1", "1", "1"}
            example += {"1", "", "", "", "1"}
            example += {"", "", "1", "", "1"}
            example += {"", "1", "", "", "1"}
            example += {"1", "", "", "1", ""}

            Call example.Save(template, Encodings.ASCII)
        End If
    End Sub

    Public Function Main() As Integer
        Return GetType(CLI).RunCLI(App.CommandLine, AddressOf CLI.DrawFile)
    End Function
End Module