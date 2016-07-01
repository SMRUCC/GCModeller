Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.AnalysisTools.SequenceTools.SequencePatterns

Module Program

    Public Function Main() As Integer
        Return GetType(Utilities).RunCLI(App.CommandLine)
    End Function
End Module