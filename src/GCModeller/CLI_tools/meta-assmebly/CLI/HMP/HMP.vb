Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
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

    <ExportAPI("/hmp.manifest.files")>
    <Usage("/hmp.manifest.files /in <manifest.tsv> [/out <list.txt>]")>
    Public Function ExportFileList(args As CommandLine) As Integer

        VBDebugger.ForceSTDError = True

        Dim tsv$ = args.OpenStreamInput("/in").ReadToEnd
        Dim manifest As manifest() = tsv.ImportsData(Of manifest)(delimiter:=ASCII.TAB)
        Dim list$ = manifest _
            .Select(Function(sample) sample.HttpURL) _
            .Where(Function(url) Not url.StringEmpty) _
            .JoinBy(ASCII.LF)

        Using out = args.OpenStreamOutput("/out")
            Call out.WriteLine(list)
        End Using

        Return 0
    End Function
End Module
