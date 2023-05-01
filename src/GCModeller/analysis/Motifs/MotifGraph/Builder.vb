Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.LinearAlgebra
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
        Dim c As New Vector(integers:=ISequenceModel.GetCompositionVector(seq, components))
        Dim cv As New Dictionary(Of Char, Double)
        Dim i As Integer = Scan0

        c = c / c.Max

        For Each ci As Char In components
            cv.Add(ci, c(i))
            i += 1
        Next

        Return New SequenceGraph With {.Compositions = cv}
    End Function
End Module
