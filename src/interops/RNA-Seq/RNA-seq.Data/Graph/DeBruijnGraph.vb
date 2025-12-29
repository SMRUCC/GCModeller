Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.FQ
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Graph

    ''' <summary>
    ''' De Bruijn 图
    ''' 
    ''' 节点是 K-mer（Reads 的子片段），边代表 K-mer 之间有 k−1 个碱基的重叠。
    ''' </summary>
    Public Class DeBruijnGraph : Inherits Builder

        ReadOnly k As Integer
        ' 使用 Dictionary 来存储 K-mer 到节点 ID 的映射，或者直接存储节点对象
        ' 为了覆盖度统计，这里建议使用 Dictionary(Of String, Integer) 来计数
        ReadOnly kmerCounts As New Dictionary(Of String, Integer)

        Public Sub New(reads As IEnumerable(Of FastQ), Optional k As Integer = 31)
            MyBase.New(reads)
            Me.k = k
        End Sub

        Protected Overrides Sub ProcessReads(reads As IEnumerable(Of FQ.FastQ))
            ' --- 第一步：统计 K-mer 并创建节点 ---
            ' 优化点：在遍历 Reads 时直接计数，而不是仅仅去重
            For Each read As FastQ In reads
                For Each kmer As String In KSeq.KmerSpans(NucleicAcid.Canonical(read.SequenceData), k)
                    If kmerCounts.ContainsKey(kmer) Then
                        kmerCounts(kmer) += 1
                    Else
                        kmerCounts(kmer) = 1
                        ' 创建节点（如果图库允许重复创建检查，也可以在这里创建）
                        ' 假设 g.CreateNode 内部有去重机制，或者这里仅做数据准备
                        Call g.CreateNode(kmer)
                    End If
                Next
            Next

            ' 获取所有唯一的 K-mer 序列
            Dim uniqueKmerList As List(Of String) = kmerCounts.Keys.ToList()

            ' --- 第二步：构建快速查找索引 ---
            ' 逻辑：构建一个字典，Key 是 K-1 长度的序列，Value 是以该序列为后缀的 K-mer 列表
            Dim suffixIndex As New Dictionary(Of String, List(Of String))

            For Each kmer As String In uniqueKmerList
                ' 获取后缀 (k-1 个碱基)
                Dim suffix As String = kmer.Substring(1, k - 1)

                If Not suffixIndex.ContainsKey(suffix) Then
                    suffixIndex(suffix) = New List(Of String)()
                End If
                suffixIndex(suffix).Add(kmer)
            Next

            ' --- 第三步：构建边 (复杂度降为 O(N)) ---
            ' 逻辑：对于每个 KmerA，找它的前缀。如果某 KmerB 的后缀 == A的前缀，则 A->B 有边
            For Each kmerA As String In uniqueKmerList
                ' 获取前缀
                Dim prefixA As String = kmerA.Substring(0, k - 1)

                ' 查找是否有其他 K-mer 的后缀等于这个前缀
                If suffixIndex.ContainsKey(prefixA) Then
                    Dim targets As List(Of String) = suffixIndex(prefixA)

                    For Each kmerB As String In targets
                        ' 避免自环（如果 De Bruijn 图允许自环，去掉这个判断）
                        ' 注：在 De Bruijn 图中，如果 kmerA == kmerB，意味着它由完全重复的序列组成（如 AAAA），通常也是允许的边
                        If kmerA <> kmerB Then
                            ' 获取权重：这里可以使用 kmerCounts(kmerA) 或 kmerCounts(kmerB) 
                            ' 或者是更复杂的边覆盖度计算。
                            ' 简单起见，这里使用源 K-mer 的计数作为边的权重参考
                            Dim weight As Double = kmerCounts(kmerA)

                            Dim edge = g.CreateEdge(kmerA, kmerB, weight:=weight)
                            edge.data("LinkType") = "DeBruijn"
                            edge.data("OverlapSeq") = prefixA ' 记录重叠序列
                        End If
                    Next
                End If
            Next
        End Sub
    End Class
End Namespace