Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.NeedlemanWunsch
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' Clustering seeds using binary tree
''' </summary>
Public Module SeedCluster

    ''' <summary>
    ''' 将任意的两条序列转换为得分向量用以进行相似度的比较
    ''' </summary>
    ''' <param name="compares"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Vector(compares As (q$, s$)) As (q As Vector, s As Vector)
        ' 先进行全局比对，将qs序列都变为等长序列
        Dim query As New FastaSeq With {.SequenceData = compares.q.ToUpper, .Headers = {"query"}}
        Dim subject As New FastaSeq With {.SequenceData = compares.s.ToUpper, .Headers = {"subject"}}
        Dim globalAlign As GlobalAlign(Of Char) = RunNeedlemanWunsch.RunAlign(query, subject, 0).First
        Dim q = globalAlign.query.AsEnumerable
        Dim s = globalAlign.subject.AsEnumerable
        Dim a As New List(Of Double)
        Dim b As New List(Of Double)

        For Each nt As SeqValue(Of (q As Char, s As Char)) In (q, s).seqtuple
            With nt.value.ScoreTuple
                a.Add(.a)
                b.Add(.b)
            End With
        Next

        Return (a.AsVector, b.AsVector)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="nt">全部都是大写字母</param>
    ''' <returns></returns>
    <Extension>
    Private Function ScoreTuple(nt As (q As Char, s As Char)) As (a#, b#)
        Dim q = nt.q, s = nt.s

        If q = "-"c Then
            If s = "-"c Then
                Return (0.5, 0.5)
            Else
                Return (0.5, 0.0)
            End If
        End If

        ' q 不是任意碱基
        If s = "-"c Then
            If q = "-"c Then
                Return (0.5, 0.5)
            Else
                Return (0.0, 0.5)
            End If
        End If

        ' s 不是任意碱基
        If q = s Then
            Return (1.0, 1.0)
        Else
            Return (-1.0, -1.0)
        End If
    End Function
End Module
