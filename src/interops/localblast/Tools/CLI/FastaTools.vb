Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Parallel.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/Fasta.Filters",
               Usage:="/Fasta.Filters /in <nt.fasta> /key <regex> [/out <out.fasta> /p]")>
    <ParameterInfo("/p",
                   True,
                   AcceptTypes:={GetType(Boolean)},
                   Description:="Using the parallel edition?? If GCModeller running in a 32bit environment, do not use this option.")>
    Public Function Filter(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim key As String = args("/key")
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & "-" & key.NormalizePathString.Replace(" ", "_") & ".fasta")
        Dim source As New StreamIterator([in])
        Dim parallel As Boolean = args.GetBoolean("/p")

        Call "".SaveTo(out)

        If parallel Then
            Call "Using parallel edition!".__DEBUG_ECHO
        Else
            ' Call "Using single thread mode on the 32bit platform".__DEBUG_ECHO
        End If

        Using file As New StreamWriter(New FileStream(out, FileMode.OpenOrCreate), Encoding.ASCII)
            Dim regex As New Regex(key, RegexICSng)

            file.AutoFlush = True

            If parallel Then
                For Each block In LQuerySchedule.Where(source.ReadStream, Function(fa) regex.Match(fa.Title).Success)
                    For Each x In block
                        Call file.WriteLine(x.GenerateDocument(-1))
                    Next
                Next
            Else
                For Each fa As FastaToken In source.ReadStream
                    If regex.Match(fa.Title).Success Then
                        Call file.WriteLine(fa.GenerateDocument(-1))
                        ' Call fa.Title.__DEBUG_ECHO
                    End If
                Next
            End If
        End Using

        Return 0
    End Function
End Module
