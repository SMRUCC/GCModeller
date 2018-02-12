Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Abstract
Imports SMRUCC.genomics.SequenceModel.FASTA

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

        ' 之后对得到的种子序列进行两两全局比对，得到距离矩阵
        Dim matrix As New List(Of DataSet)

        For Each q As HSP In seeds
            Dim row As New DataSet With {
                .ID = q.Query,
                .Properties = New Dictionary(Of String, Double)
            }

            For Each s As HSP In seeds
                ' 因为在这里需要构建一个矩阵，所以自己比对自己这个情况也需要放进去了
                Dim score = RunNeedlemanWunsch.RunAlign(
                    New FastaSeq With {.SequenceData = q.Query},
                    New FastaSeq With {.SequenceData = s.Query},
                    [single]:=True,
                    echo:=False
                )


            Next
        Next
    End Function

    Public Function pairwiseSeeding(q As FastaSeq, s As FastaSeq) As IEnumerable(Of HSP)
        Dim smithWaterman As New SmithWaterman(q.SequenceData, s.SequenceData)
        Dim result = smithWaterman.GetOutput(0.3, 6)
        Return result.HSP
    End Function
End Module
