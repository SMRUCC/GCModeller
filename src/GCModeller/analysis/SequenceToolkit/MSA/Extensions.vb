Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Extensions

    <Extension>
    Public Function MultipleAlignment(input As IEnumerable(Of FastaSeq), matrix As Char()()) As String
        Return New CenterStar(input).Compute(matrix)
    End Function

    <Extension>
    Public Function MultipleAlignment(input As IEnumerable(Of String), matrix As Char()()) As String
        Return New CenterStar(input).Compute(matrix)
    End Function
End Module
