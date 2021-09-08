
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("blast+")>
Module blastPlus

    <ExportAPI("makeblastdb")>
    Public Function makeblastdb([in] As String, dbtype As String, Optional env As Environment = Nothing)

    End Function

    <ExportAPI("blastp")>
    Public Function blastp()

    End Function

    <ExportAPI("blastn")>
    Public Function blastn()

    End Function

    <ExportAPI("blastx")>
    Public Function blastx()

    End Function

End Module
