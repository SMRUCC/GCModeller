Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Program

    Public Function Main() As Integer
        Return GetType(Utilities).RunCLI(App.CommandLine)
    End Function
End Module