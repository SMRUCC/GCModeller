Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DataMining
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class CenterStar

    Dim algorithm As DynamicProgramming.CenterStar

    Sub New(seqs As IEnumerable(Of String))
        algorithm = New DynamicProgramming.CenterStar(seqs)
    End Sub

    Sub New(seqs As IEnumerable(Of FastaSeq))
        algorithm = New DynamicProgramming.CenterStar(seqs.Select(Function(f) New NamedValue(Of String)(f.Title, f.SequenceData)))
    End Sub

    Public Function Compute(matrix As ScoreMatrix) As MSAOutput
        Dim alignments As String() = Nothing
        Dim cost As Double = algorithm.Compute(matrix Or ScoreMatrix.DefaultMatrix, alignments)
        Dim output As New MSAOutput With {
            .cost = cost,
            .MSA = alignments,
            .names = algorithm.NameList
        }

        Return output
    End Function
End Class
