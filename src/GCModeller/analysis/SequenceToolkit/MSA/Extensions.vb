Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Extensions

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function MultipleAlignment(input As IEnumerable(Of FastaSeq), matrix As Char()()) As MSAOutput
        Return New CenterStar(input).Compute(matrix)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function MultipleAlignment(input As IEnumerable(Of String), matrix As Char()()) As MSAOutput
        Return New CenterStar(input).Compute(matrix)
    End Function
End Module
