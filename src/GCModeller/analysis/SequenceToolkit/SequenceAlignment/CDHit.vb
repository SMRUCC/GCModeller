#Region "Microsoft.VisualBasic::d88ed9054aae6f0f149285ca6017d40e, analysis\SequenceToolkit\SequenceAlignment\CDHit.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 146
    '    Code Lines: 96 (65.75%)
    ' Comment Lines: 26 (17.81%)
    '    - Xml Docs: 53.85%
    ' 
    '   Blank Lines: 24 (16.44%)
    '     File Size: 5.66 KB


    ' Class CDHit
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: FindSimilar, GetSequencePool, NrSeqs, Setup, SimilarGraph
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Diagnostics
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.HashMaps.MinHash
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class CDHit

    ReadOnly k As Integer = 31
    ReadOnly threads As Integer?

    ''' <summary>
    ''' sort by sequence length in desc order
    ''' </summary>
    Dim hash As CDHashTask

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="k">
    ''' protein - k=5aa
    ''' nucleotide - k=12nt
    ''' genomics - k=31nt
    ''' </param>
    ''' <param name="n_threads">
    ''' 用于并行计算的工作线程数量；在没有指定的时候会按照当前机器的CPU核数来自动选取
    ''' </param>
    Sub New(Optional k As Integer = 12, Optional n_threads As Integer? = Nothing)
        Me.k = k
        ' 修复：原来的代码是 Me.threads = threads（字段自赋值），导致 n_threads 参数被完全丢弃，
        ' CDHashTask 只会回退到 VectorTask.n_threads（默认值4）个工作线程。
        Me.threads = If(n_threads.HasValue, n_threads.Value, Environment.ProcessorCount)
    End Sub

    ''' <summary>
    ''' 实际用于并行计算的工作线程数量
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property workerThreads As Integer
        Get
            Return If(threads.HasValue, threads.Value, 1)
        End Get
    End Property

    Public Function GetSequencePool() As FastaSeq()
        Return hash.seqPool
    End Function

    Public Function Setup(seqs As IEnumerable(Of FastaSeq)) As CDHit
        Dim sw As Stopwatch = Stopwatch.StartNew
        Dim pool As FastaSeq() = seqs.SafeQuery.ToArray
        Dim n As Integer = pool.Length
        Dim lengths As Integer() = New Integer(n - 1) {}
        Dim order As Integer() = New Integer(n - 1) {}

        For i As Integer = 0 To n - 1
            lengths(i) = pool(i).Length
            order(i) = i
        Next

        ' 按照序列长度降序排序，并且使用原始下标作为tie-breaker：
        ' 这样子在不使用LINQ委派的同时仍然保持了与 OrderBy 一致的稳定性
        Call Array.Sort(order, New Comparison(Of Integer)(
            Function(a As Integer, b As Integer) As Integer
                Dim c As Integer = lengths(b).CompareTo(lengths(a))

                If c = 0 Then
                    Return a.CompareTo(b)
                Else
                    Return c
                End If
            End Function))

        Dim sorted As FastaSeq() = New FastaSeq(n - 1) {}

        For i As Integer = 0 To n - 1
            sorted(i) = pool(order(i))
        Next

        Call $"[cdhit] setup: {n} sequences sorted by length, elapsed {sw.ElapsedMilliseconds} ms".debug

        sw.Restart()

        Dim unique As IEnumerable(Of FastaSeq) = sorted.UniqueTitle

        Call "run data setup...".info
        hash = New CDHashTask(unique.ToArray, workers:=threads) With {.k = k}

        Call $"[cdhit] make unique sequence pool, elapsed {sw.ElapsedMilliseconds} ms; run min-hash in parallel with {workerThreads} threads...".debug
        Call "create min hash sequence data in parallel".info
        Call hash.RunDynamic()
        Call "make hash job done!".info

        Return Me
    End Function

    Public Iterator Function SimilarGraph() As IEnumerable(Of SimilarHit)
        Dim similars As New Dictionary(Of Integer, SimilarHit)
        Dim minHash = hash.minHash
        Dim seqPool = hash.seqPool

        For Each result As SimilarityIndex In LSH.FindSimilarItems(minHash, produceUniqueHit:=True)
            If result.IsUniqueHit Then
                Yield New SimilarHit With {.SeqID = seqPool(result.U).Title}
            Else
                If Not similars.ContainsKey(result.U) Then
                    Call similars.Add(result.U, New SimilarHit With {.SeqID = seqPool(result.U).Title})
                End If

                Call similars(result.U).Similar.Add(seqPool(result.V).Title, result.Similarity)
            End If
        Next

        For Each similar As SimilarHit In similars.Values
            Yield similar
        Next
    End Function

    ''' <summary>
    ''' implements of the CD-hit liked sequence similarity clustering
    ''' </summary>
    ''' <returns></returns>
    Public Iterator Function FindSimilar(Optional threshold As Double = 0.8) As IEnumerable(Of SimilarHit)
        Dim sw As Stopwatch = Stopwatch.StartNew
        Dim jaccardTh As Double = LSHParameterEstimator.GetThresholdFromIdentity(threshold, k)
        Dim minHash = hash.minHash
        Dim seqPool = hash.seqPool
        Dim n As Integer = seqPool.Length
        ' 只需要建立LSH分桶索引，不需要提前物化数十亿条的相似序列对
        Dim buckets As CDHitLSHBuckets = CDHitLSH.BuildBuckets(minHash, workerThreads)

        Call $"[cdhit] LSH buckets ready, elapsed {sw.ElapsedMilliseconds} ms".debug

        sw.Restart()

        ' 2. CD-HIT 核心：贪婪聚类（必须串行）
        ' 标记是否已被归入某个簇
        Dim isClustered(n - 1) As Boolean
        ' 候选序列去重：同一条序列可能在当前代表序列的多个波段桶里面重复出现
        Dim visited(n - 1) As Integer
        Dim visitMark As Integer = 0
        Dim clusters As Integer = 0
        Dim relations As Long = 0

        ' 注意：代表序列不需要标记 isClustered(i)，因为循环只会按照下标递增的方向前进
        For i As Integer = 0 To n - 1
            If isClustered(i) Then
                ' 如果已经被归簇，跳过
                Continue For
            End If

            ' i 作为代表序列
            Dim cluster As New SimilarHit With {
                .SeqID = seqPool(i).Title,
                .Similar = New Dictionary(Of String, Double)()
            }

            visitMark += 1

            For band As Integer = 0 To buckets.NumBands - 1
                For Each candidate As Integer In buckets.BucketMembers(i, band)
                    If isClustered(candidate) OrElse visited(candidate) = visitMark Then
                        Continue For
                    End If

                    visited(candidate) = visitMark

                    Dim similarity As Double = CDHitLSH.SignatureSimilarity(minHash(i).Signature, minHash(candidate).Signature)

                    If similarity >= jaccardTh Then
                        ' CD-HIT 逻辑：将邻居标记为已归簇
                        isClustered(candidate) = True
                        cluster.Similar.Add(seqPool(candidate).Title, similarity)
                        relations += 1
                    End If
                Next
            Next

            clusters += 1
            Yield cluster
        Next

        Call $"[cdhit] greedy clustering done: {clusters} clusters, {relations} similar relations, elapsed {sw.ElapsedMilliseconds} ms".debug
    End Function

    Public Iterator Function NrSeqs(Optional threshold As Double = 0.8) As IEnumerable(Of FastaSeq)
        Dim seqIndex As Dictionary(Of String, FastaSeq) = hash.seqPool.ToDictionary(Function(s) s.Title)

        For Each cluster As SimilarHit In FindSimilar(threshold)
            Dim nr_rep = seqIndex(cluster.SeqID)

            If cluster.IsUniqued Then
                Yield nr_rep
            Else
                Yield New FastaSeq(nr_rep.SequenceData) With {
                    .Headers = {
                        cluster.SeqID,
                        $"{cluster.Size} cluster members",
                        cluster.Similar.Keys.GetJson
                    }
                }
            End If
        Next
    End Function

End Class
