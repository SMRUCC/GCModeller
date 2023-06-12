Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998.CAI
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Builder

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function DNAGraph(seq As FastaSeq) As SequenceGraph
        Return SequenceGraph(seq.SequenceData, SequenceModel.NT, id:=seq.Title)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function RNAGraph(seq As FastaSeq) As SequenceGraph
        Return SequenceGraph(seq.SequenceData, SequenceModel.RNA, id:=seq.Title)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function PolypeptideGraph(seq As FastaSeq) As SequenceGraph
        Return SequenceGraph(seq.SequenceData, SequenceModel.AA, id:=seq.Title)
    End Function

    Public Function SequenceGraph(seq As String, components As IReadOnlyCollection(Of Char), Optional id As String = "unknown") As SequenceGraph
        Dim c As New Vector(integers:=ISequenceModel.GetCompositionVector(seq, components))
        Dim cv As New Dictionary(Of Char, Double)
        Dim i As Integer = Scan0
        Dim g As New Dictionary(Of Char, Dictionary(Of Char, Double))
        Dim nsize As Integer = seq.Length
        Dim triples As New Dictionary(Of String, Double)
        Dim distance As Dictionary(Of String, Double) = DistanceGraph.TupleDistanceGraph(seq, components)

        c = c / nsize

        For Each ci As Char In components.OrderBy(Function(a) a)
            cv.Add(ci, c(i))
            i += 1
        Next

        For Each ci As Char In components
            Dim gi As New Dictionary(Of Char, Double)

            For Each cj As Char In components
                Call gi.Add(cj, seq.Count(New String({ci, cj})) / (nsize / 2))
            Next

            Call g.Add(ci, gi)
        Next

        If c.Dim <= 5 Then
            ' skip triple pattern for protein sequence
            ' or the result matrix will be ultra large
            ' comsume too much memory
            For Each t As String In CodonBiasVector.PopulateTriples(components)
                Call triples.Add(t, seq.Count(t) / (nsize / 3))
            Next
        End If

        Return New SequenceGraph With {
            .composition = cv,
            .graph = g,
            .triple = triples,
            .tuple_distance = distance,
            .id = id
        }
    End Function
End Module
