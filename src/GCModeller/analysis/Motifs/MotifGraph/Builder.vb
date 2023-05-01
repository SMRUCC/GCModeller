Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Builder

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DNAGraph(seq As FastaSeq) As SequenceGraph
        Return SequenceGraph(seq.SequenceData, SequenceModel.NT)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function RNAGraph(seq As FastaSeq) As SequenceGraph
        Return SequenceGraph(seq.SequenceData, SequenceModel.RNA)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function PolypeptideGraph(seq As FastaSeq) As SequenceGraph
        Return SequenceGraph(seq.SequenceData, SequenceModel.AA)
    End Function

    Public Function SequenceGraph(seq As String, components As IReadOnlyCollection(Of Char)) As SequenceGraph
        Dim c = ISequenceModel.GetCompositionVector(seq, components)
    End Function
End Module
