Imports HMMER3
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("hmmer")>
Module hmmer

    <ExportAPI("parse_hmmer_model")>
    Public Function parse_hmmer_model(x As String) As ProfileHMM
        Return HMMER3Parser.ParseContent(x.SolveStream)
    End Function

End Module
