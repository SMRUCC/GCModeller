Imports System.IO
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Partial Module CLI

    <ExportAPI("/create.debugger.view")>
    <Usage("/create.debugger.view /in <seq.fasta> [/width <default=200> /out <view.txt>]")>
    <Group(CLIGrouping.DebuggerCLI)>
    Public Function SeqDebugger(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.debug.txt"
        Dim seq As FastaSeq = FastaSeq.Load([in])
        Dim width% = args("/width") Or 200
        Dim segments = seq.SequenceData _
            .Split(partitionSize:=width) _
            .Select(AddressOf StringHelpers.CharString) _
            .ToArray
        Dim createLines = Iterator Function() As IEnumerable(Of (seq$, left%))
                              Dim i As i32 = 1

                              For Each part As String In segments
                                  Yield (part, i = i + part.Length)
                              Next
                          End Function().ToArray

        Using output As StreamWriter = out.OpenWriter
            Dim padding = createLines _
                .Select(Function(f) f.left.ToString) _
                .MaxLengthString _
                .Length _
                .DoCall(Function(l) New String(" "c, l))
            Dim left$, right

            Call output.WriteLine(seq.Title)
            Call output.WriteLine($"  length={seq.Length}")
            Call output.WriteLine($"  gc%={seq.GCContent}")

            For Each fragment In createLines
                left = (fragment.left - width).FormatZero(padding)
                right = fragment.left - 1

                If fragment.seq.Length < width Then
                    Call output.WriteLine($"{left} {fragment.seq}{New String(" "c, width - fragment.seq.Length)} {right}")
                Else
                    Call output.WriteLine($"{left} {fragment.seq} {right}")
                End If
            Next
        End Using

        Return 0
    End Function
End Module