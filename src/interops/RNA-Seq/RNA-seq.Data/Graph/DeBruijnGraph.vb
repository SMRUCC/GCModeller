Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.FQ

Namespace Graph

    ''' <summary>
    ''' De Bruijn 图
    ''' 
    ''' 节点是 K-mer（Reads 的子片段），边代表 K-mer 之间有 k−1 个碱基的重叠。
    ''' </summary>
    Public Class DeBruijnGraph : Inherits Builder

        ''' <summary>
        ''' k-mer 长度 (例如 21)
        ''' </summary>
        ReadOnly k As Integer
        ReadOnly uniqueKmers As New HashSet(Of String)

        Public Sub New(reads As IEnumerable(Of FastQ))
            MyBase.New(reads)
        End Sub

        Protected Overrides Sub ProcessReads(reads As IEnumerable(Of FQ.FastQ))
            For Each read As FastQ In reads
                For Each kmer As String In KSeq.KmerSpans(read.SequenceData, k)
                    If Not uniqueKmers.Contains(kmer) Then
                        Call uniqueKmers.Add(kmer)
                    End If
                Next
            Next

            Dim kmerIds As String() = uniqueKmers.ToArray()

            ' --- 第一步：创建所有 K-mer 节点 ---
            For Each kmerSeq As String In kmerIds
                ' 这里直接用 K-mer 的序列作为 ID
                Call g.CreateNode(kmerSeq)
            Next

            ' --- 第二步：构建 De Bruijn 连边 ---
            ' 逻辑：如果 KmerA 的后 k-1 个字符 == KmerB 的前 k-1 个字符，则连接
            For Each i As Integer In TqdmWrapper.Range(0, kmerIds.Length)
                For j As Integer = 0 To kmerIds.Length - 1
                    If i <> j Then
                        Dim kmerA As String = kmerIds(i)
                        Dim kmerB As String = kmerIds(j)

                        ' 获取后缀和前缀 (索引从 1 开始，所以是 2 到 k)
                        Dim suffixA As String = kmerA.Substring(1, k - 1)
                        Dim prefixB As String = kmerB.Substring(0, k - 1)

                        ' 核心 De Bruijn 连接条件
                        If String.Equals(suffixA, prefixB, StringComparison.OrdinalIgnoreCase) Then
                            ' 权重可以是 1，或者是该边的覆盖度
                            Dim edge = g.CreateEdge(kmerA, kmerB, weight:=1.0)

                            ' 添加边的属性数据
                            edge.data("LinkType") = "DeBruijn"
                            Edge.data("OverlapSeq") = suffixA ' 记录重叠的那一段序列
                        End If
                    End If
                Next
            Next
        End Sub
    End Class
End Namespace