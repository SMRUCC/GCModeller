Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module GraphMatrix

    ReadOnly aa As String() = SequenceModel.AA _
        .Select(Function(a) a.ToString) _
        .ToArray

    ReadOnly nt As String() = SequenceModel.NT _
        .Select(Function(n) n.ToString) _
        .ToArray

    <Extension>
    Public Function CreateProteinMatrix(prot As FastaSeq) As NumericMatrix
        Dim aa = prot.SequenceData.CreateSlideWindows(2)
        Dim graph As New List(Of SparseGraph.Edge)

        For Each tuple As SlideWindow(Of Char) In aa
            Call graph.Add(New SparseGraph.Edge(tuple.First, tuple.Second))
        Next

        Return New SparseGraph(graph).CreateMatrix(keys:=GraphMatrix.aa)
    End Function

    Public Function CreateNucleotideMatrix(nucl As FastaSeq) As NumericMatrix
        Dim nt = nucl.SequenceData.CreateSlideWindows(2)
        Dim graph As New List(Of SparseGraph.Edge)

        For Each tuple As SlideWindow(Of Char) In nt
            Call graph.Add(New SparseGraph.Edge(tuple.First, tuple.Second))
        Next

        Return New SparseGraph(graph).CreateMatrix(keys:=GraphMatrix.nt)
    End Function

End Module
