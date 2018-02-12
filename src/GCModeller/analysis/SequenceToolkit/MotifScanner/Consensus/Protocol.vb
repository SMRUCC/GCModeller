Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Abstract
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports Microsoft.VisualBasic.Language

Public Module Protocol

    <Extension>
    Public Iterator Function PopulateMotifs(inputs As IEnumerable(Of FastaSeq)) As IEnumerable(Of Probability)
        Dim regions As FastaSeq() = inputs.ToArray
        Dim seeds As New List(Of HSP)

        ' 先进行两两局部最优比对，得到最基本的种子
        For Each q As FastaSeq In regions
            For Each s As FastaSeq In regions.Where(Function(seq) Not seq Is q)
                seeds += pairwiseSeeding(q, s)
            Next
        Next


    End Function

    Public Function pairwiseSeeding(q As FastaSeq, s As FastaSeq) As IEnumerable(Of HSP)
        Dim smithWaterman As New SmithWaterman(q.SequenceData, s.SequenceData)
        Dim result = smithWaterman.GetOutput(0.3, 6)
        Return result.HSP
    End Function
End Module
