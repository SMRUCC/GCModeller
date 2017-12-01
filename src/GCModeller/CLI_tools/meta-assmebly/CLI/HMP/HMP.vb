Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Data.Repository.NIH.HMP

Partial Module CLI

    <ExportAPI("/handle.hmp.manifest")>
    <Usage("/handle.hmp.manifest /in <manifest.tsv> [/out <save.directory>]")>
    Public Function Download16sSeq(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = (args <= "/out") Or $"{[in].TrimSuffix}/".AsDefault

        Return manifest _
            .LoadTable([in]) _
            .HandleFileDownloads(save:=out) _
            .ToArray _
            .SaveTo(out & "/HMP_client.log", Encoding.UTF8) _
            .CLICode
    End Function
End Module
