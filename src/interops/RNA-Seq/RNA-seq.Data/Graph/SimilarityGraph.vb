Imports SMRUCC.genomics.SequenceModel.FQ
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

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

        Public Sub New(reads As IEnumerable(Of FastQ), Optional k As Integer = 31, Optional threshold As Double = 0.95)
            MyBase.New(reads)
            Me.k = k
            Me.threshold = threshold
        End Sub

        Protected Overrides Sub ProcessReads(reads As IEnumerable(Of FQ.FastQ))
            Dim kmer2reads As New Dictionary(Of String, HashSet(Of String))
            Dim kmers As New Dictionary(Of String, HashSet(Of String))

            ' --- 第一步：创建所有 Read 节点 ---
            For Each read As FastQ In reads
                Dim read_seq As String = NucleicAcid.Canonical(read.SequenceData)

                With g.CreateNode(read.SEQ_ID)
                    .data("reads") = read_seq
                    .data("len") = read.Length
                End With

                Dim cache As New HashSet(Of String)

                ' 2. 构建倒排索引：kmer -> {readIDs}
                ' 生成 canonical k‑mers
                For i As Integer = 0 To read_seq.Length - k
                    Dim kmer As String = read_seq.Substring(i, k)
                    Dim canon As String = NucleicAcid.Canonical(kmer)   ' 见下方函数

                    If Not kmer2reads.ContainsKey(canon) Then
                        kmer2reads(canon) = New HashSet(Of String)
                    End If

                    cache.Add(canon)
                    kmer2reads(canon).Add(read.SEQ_ID)
                Next

                kmers(read.SEQ_ID) = cache
            Next

            ' 3. 收集候选对：共享至少一个 k‑mer 的 read 对
            Dim candidates As New Dictionary(Of (String, String), Boolean)
            For Each pair In kmer2reads
                Dim ids = pair.Value
                Dim idList = ids.ToArray()
                For i As Integer = 0 To idList.Length - 1
                    For j As Integer = i + 1 To idList.Length - 1
                        Dim idA = idList(i), idB = idList(j)
                        Dim key = If(String.Compare(idA, idB) < 0, (idA, idB), (idB, idA))
                        candidates(key) = True
                    Next
                Next
            Next

            ' 4. 对候选对精确计算 Jaccard（只计算一次）
            For Each pair In candidates.Keys
                Dim idA = pair.Item1
                Dim idB = pair.Item2

                ' 动态重建 k‑mer 集合（如果内存允许，也可预存）
                Dim setA = kmers(g.GetElementByID(idA).label)
                Dim setB = kmers(g.GetElementByID(idB).label)

                Dim jaccard = ComputeJaccard(setA, setB)

                If jaccard > threshold Then
                    ' 假定 CreateEdge 是无向的
                    g.CreateEdge(g.GetElementByID(idA), g.GetElementByID(idB), jaccard)
                End If
            Next
        End Sub

        Private Shared Function ComputeJaccard(setA As HashSet(Of String), setB As HashSet(Of String)) As Double
            Dim intersect = 0
            Dim smaller = If(setA.Count < setB.Count, setA, setB)
            Dim larger = If(smaller Is setA, setB, setA)

            For Each k As String In smaller
                If larger.Contains(k) Then intersect += 1
            Next

            Dim union = setA.Count + setB.Count - intersect
            Return If(union = 0, 0.0, intersect / CDbl(union))
        End Function
    End Class
End Namespace