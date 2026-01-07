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
            ' --- 第一步：统计 K-mer 计数 ---
            ' 注意：这里我们不需要先存所有的 k-mer，可以流式处理，
            ' 但为了后续建图方便，我们通常还是存 k-mer 计数。
            For Each read As FastQ In reads
                For Each kmer As String In KSeq.KmerSpans(NucleicAcid.Canonical(read.SequenceData), k)
                    If kmerCounts.ContainsKey(kmer) Then
                        kmerCounts(kmer) += 1
                    Else
                        kmerCounts(kmer) = 1
                    End If
                Next
            Next

            ' --- 第二步：构建节点((k-1)-mer) 和 边 ---
            ' 在这个算法中：节点 = (k-1)-mer，边 = k-mer
            ' 每个 k-mer 直接就定义了起点和终点节点。
            For Each kmer As String In kmerCounts.Keys
                ' 过滤低频 k-mer（去噪），比如小于 2 的可能是错误
                If kmerCounts(kmer) < 2 Then
                    Continue For
                End If

                ' 1. 获取左右 (k-1)-mer
                Dim leftNode As String = kmer.Substring(0, k - 1)
                Dim rightNode As String = kmer.Substring(1, k - 1)

                ' 2. 创建或获取节点（如果图库自动处理重复则更好，否则需要查重）
                ' 这里假设 CreateNode 是幂等的（已存在则返回已有对象）
                Dim nodeL = If(g.GetElementByID(leftNode), g.CreateNode(leftNode))
                Dim nodeR = If(g.GetElementByID(rightNode), g.CreateNode(rightNode))
                ' 3. 创建边
                ' 边本身就代表了这个 k-mer 序列
                Dim weight As Double = kmerCounts(kmer)

                ' 检查边是否已存在（因为两条相同的 k-mer 应该合并为一条边，权重累加或取最大值）
                ' 这里假设 CreateEdge 能处理多重边，或者我们先检查
                Dim edge = g.GetEdge(nodeL, nodeR)

                If edge Is Nothing Then
                    ' 创建kmer边连接
                    ' 将 k-mer 序列存入边的数据中，以便后续还原序列
                    edge = g.CreateEdge(nodeL, nodeR, weight:=weight, New EdgeData With {
                        .Properties = New Dictionary(Of String, String) From {
                            {"Sequence", kmer}
                        }
                    })
                Else
                    edge.weight += weight
                End If
            Next
        End Sub
    End Class
End Namespace