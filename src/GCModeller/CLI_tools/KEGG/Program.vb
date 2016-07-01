Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports System.Text
Imports Microsoft.VisualBasic.Scripting.MetaData

Module Program

    Public Function Main() As Integer
        Return GetType(KEGG.CLI).RunCLI(App.CommandLine)
    End Function
End Module