Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/Fasta.Filters",
               Usage:="/Fasta.Filters /in <nt.fasta> /key <regex> [/out <out.fasta>]")>
    Public Function Filter(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim key As String = args("/key")
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & "-" & key.NormalizePathString.Replace(" ", "_") & ".fasta")
        Dim source As New StreamIterator([in])

        Call "".SaveTo(out)

        Using file As New StreamWriter(New FileStream(out, FileMode.OpenOrCreate), Encoding.ASCII)
            Dim regex As New Regex(key, RegexICSng)

            For Each fa As FastaToken In source.ReadStream
                If regex.Match(fa.Title).Success Then
                    Call file.WriteLine(fa.GenerateDocument(-1))
                End If
            Next
        End Using

        Return 0
    End Function
End Module
