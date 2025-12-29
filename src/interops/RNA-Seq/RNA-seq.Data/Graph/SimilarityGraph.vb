Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.FQ

Namespace Graph

    ''' <summary>
    ''' Reads 相似性网络
    ''' 
    ''' 节点是 Read，边代表 Reads 之间的整体相似度高（例如 Jaccard 指数或比对得分）。
    ''' </summary>
    Public Class SimilarityGraph : Inherits Builder

        ReadOnly k As Integer
        ''' <summary>
        ''' 相似度阈值 (例如 0.95)
        ''' </summary>
        ReadOnly threshold As Double

        Public Sub New(reads As IEnumerable(Of FastQ))
            MyBase.New(reads)
        End Sub

        Protected Overrides Sub ProcessReads(reads As IEnumerable(Of FQ.FastQ))
            Dim kmers As New Dictionary(Of String, String())

            ' --- 第一步：创建所有 Read 节点 ---
            For Each read As FastQ In reads
                With g.CreateNode(read.SEQ_ID)
                    .data("reads") = read.SequenceData
                End With

                kmers(read.SEQ_ID) = KSeq.KmerSpans(read.SequenceData, k).Distinct.ToArray
            Next

            ' --- 第二步：基于相似度矩阵连边 ---
            For Each u As Node In TqdmWrapper.Wrap(g.vertex.ToArray)
                Dim reads_u As String = u!reads

                For Each v As Node In g.vertex
                    If u Is v Then
                        Continue For
                    End If

                    Dim k1 = kmers(u.label)
                    Dim k2 = kmers(v.label)
                    Dim kjac As Double = k1.jaccard_coeff(k2)

                    If kjac > threshold Then
                        g.CreateEdge(u, v, kjac)
                    End If
                Next
            Next
        End Sub
    End Class
End Namespace