Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.HashMaps.MinHash
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class CDHit

    ReadOnly k As Integer = 31

    Dim seqPool As FastaSeq()
    Dim minHash As SequenceItem()

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="k">
    ''' protein - k=5aa
    ''' nucleotide - k=12nt
    ''' genomics - k=31nt
    ''' </param>
    Sub New(Optional k As Integer = 12)
        Me.k = k
    End Sub

    Public Function Setup(seqs As IEnumerable(Of FastaSeq)) As CDHit
        seqPool = (From seq As FastaSeq In seqs Order By seq.Length Descending).ToArray
        minHash = seqPool.SeqIterator.ToArray _
            .AsParallel _
            .Select(Function(s)
                        ' MinHash.CreateSequenceData
                        Return KSeq _
                            .KmerSpans(s.value.SequenceData, k) _
                            .CreateSequenceData(id:=s.i)
                    End Function) _
            .ToArray

        Return Me
    End Function

    Public Iterator Function SimilarGraph() As IEnumerable(Of SimilarHit)
        Dim similars As New Dictionary(Of Integer, SimilarHit)

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
    ''' <param name="seqs"></param>
    ''' <returns></returns>
    Public Iterator Function FindSimilar(seqs As IEnumerable(Of FastaSeq)) As IEnumerable(Of SimilarHit)
        ' 提前计算所有相似对，构建图结构
        Dim adjList As New Dictionary(Of Integer, HashSet(Of Integer))()

        For Each result As SimilarityIndex In LSH.FindSimilarItems(minHash, produceUniqueHit:=False)
            ' 构建邻接表：u -> v 和 v -> u
            If Not adjList.ContainsKey(result.U) Then adjList(result.U) = New HashSet(Of Integer)()
            If Not adjList.ContainsKey(result.V) Then adjList(result.V) = New HashSet(Of Integer)()

            adjList(result.U).Add(result.V)
            adjList(result.V).Add(result.U)
        Next

        ' 2. CD-HIT 核心：贪婪聚类
        Dim isClustered(seqPool.Length - 1) As Boolean ' 标记是否已被归入某个簇

        For i As Integer = 0 To seqPool.Length - 1
            If isClustered(i) Then Continue For ' 如果已经被归簇，跳过

            ' i 作为代表序列
            Dim cluster As New SimilarHit With {.SeqID = seqPool(i).Title}

            ' 遍历所有与 i 相似的邻居
            If adjList.ContainsKey(i) Then
                For Each neighbor In adjList(i)
                    If Not isClustered(neighbor) Then
                        ' 这里可以加上阈值的二次确认，虽然 LSH 已经筛选过了
                        ' CD-HIT 逻辑：将邻居标记为已归簇
                        isClustered(neighbor) = True

                        ' 记录相似度信息 (需要从之前的计算中获取，或重新计算)
                        ' 为了简化示例，这里假设记录 ID 即可
                        cluster.Similar.Add(seqPool(neighbor).Title, 1.0) ' Similarity 需从 result 获取
                    End If
                Next
            End If

            Yield cluster
        Next
    End Function

End Class

Public Class SimilarHit

    Public Property SeqID As String
    Public Property Similar As Dictionary(Of String, Double)

    Public ReadOnly Property IsUniqued As Boolean
        Get
            Return Similar.IsNullOrEmpty
        End Get
    End Property

End Class
