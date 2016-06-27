Imports System.Text.RegularExpressions
Imports LANS.SystemsBiology.SequenceModel.FASTA
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic
Imports LANS.SystemsBiology.AnalysisTools.SequenceTools.SequencePatterns

Module Program

    Public Function Main() As Integer
        Return GetType(Utilities).RunCLI(App.CommandLine)
    End Function
End Module