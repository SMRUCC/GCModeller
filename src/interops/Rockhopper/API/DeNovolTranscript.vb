Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions

Namespace AnalysisAPI

    Public Module DeNovolTranscript

        Public Function LoadDocument(Path As String) As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
            Call Console.WriteLine("[Load] " & Path.ToFileURL)

            Dim ChunkBuffer As String() = System.IO.File.ReadAllLines(Path).Skip(1).ToArray
            Dim LQuery = (From i As Integer In ChunkBuffer.Sequence.AsParallel
                          Let s As String = ChunkBuffer(i)
                          Let Tokens As String() = Strings.Split(s, vbTab)
                          Select New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken With
                              {.SequenceData = Tokens(0),
                                .Attributes = New String() {$"hash={i}", $"Expression={Tokens(2)}"}}).ToArray
            Return CType(LQuery, LANS.SystemsBiology.SequenceModel.FASTA.FastaFile)
        End Function
    End Module
End Namespace