Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Extensions

    <Extension>
    Public Function MultipleAlignment(input As IEnumerable(Of String)) As String()
        Return input.Select(Function(seq) New FastaSeq With {.SequenceData = seq}).MultipleAlignment(0)
    End Function
End Module
