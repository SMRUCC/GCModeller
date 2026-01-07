Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Math.Statistics.Linq
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

                ' 2. 创建或获取节点
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

        Public Function AssembleContigs() As List(Of String)
            Dim contigs As New List(Of String)
            Dim visitedEdges As New HashSet(Of Object) ' 用于记录已经处理过的边，防止重复计算

            ' --- 第一阶段：从“非线性”节点出发，寻找线性路径 ---
            ' 遍历所有节点，寻找路径的起点（入度!=1 或 出度!=1）
            For Each node As Node In g.vertex
                Dim inDegree As Integer = node.degree.In
                Dim outDegree As Integer = node.degree.Out

                ' 判断是否为路径的“起始/终止/分支”点
                Dim isBranchOrTip As Boolean = (inDegree <> 1) OrElse (outDegree <> 1)

                If isBranchOrTip Then
                    ' 从该节点的每一条未访问的出边出发进行行走
                    For Each startEdge As Edge In node.directedVertex.outgoingEdges
                        If Not visitedEdges.Contains(startEdge) Then
                            Dim pathEdges As New List(Of Edge)
                            WalkGraph(startEdge, pathEdges, visitedEdges)

                            ' 将路径还原为序列
                            contigs.Add(ReconstructSequence(pathEdges))
                        End If
                    Next
                End If
            Next

            ' --- 第二阶段：处理孤立环 ---
            ' 上面的循环会漏掉那种没有起止点的“完美闭环”（每个节点 入度=1 出度=1）。
            ' 我们需要检查是否还有未被访问的边，如果有的话，它们一定属于某个孤立环。
            For Each edge As Edge In g.graphEdges  ' 假设 g.Edges 返回所有边
                If Not visitedEdges.Contains(edge) Then
                    Dim pathEdges As New List(Of Edge)
                    WalkGraph(edge, pathEdges, visitedEdges)
                    contigs.Add(ReconstructSequence(pathEdges))
                End If
            Next

            Return contigs
        End Function

        ''' <summary>
        ''' 递归或循环地沿着图走，直到遇到分支点或端点
        ''' </summary>
        Private Sub WalkGraph(currentEdge As Edge, path As List(Of Edge), visitedEdges As HashSet(Of Object))
            ' 将当前边加入路径，并标记为已访问
            path.Add(currentEdge)
            visitedEdges.Add(currentEdge)

            ' 获取当前边的目标节点
            Dim nextNode As Node = currentEdge.V  ' 假设属性是 To 或 Target

            ' 检查是否可以继续前行：
            ' 继续的条件是：目标节点必须只有 1 个入边 AND 1 个出边 (线性节点)
            ' 并且下一条边还没有被访问过
            If nextNode.degree.In = 1 AndAlso nextNode.degree.Out = 1 Then
                Dim nextEdge As Edge = nextNode.directedVertex.outgoingEdges.First() ' 取唯一的出边

                If Not visitedEdges.Contains(nextEdge) Then
                    ' 继续递归/循环
                    WalkGraph(nextEdge, path, visitedEdges)
                End If
            End If
            ' 如果不是线性节点，路径在此终止
        End Sub

        ''' <summary>
        ''' 将边列表还原为 DNA 序列
        ''' </summary>
        Private Function ReconstructSequence(path As List(Of Edge)) As String
            If path.Count = 0 Then Return ""

            ' 第一条边：取完整的 K-mer 序列
            Dim sb As New Text.StringBuilder(path(0)("Sequence"))

            ' 后续每条边：只取最后一个碱基 (因为前 k-1 个碱基已经重叠了)
            For i As Integer = 1 To path.Count - 1
                Dim kmerSeq As String = path(i)("Sequence")
                If kmerSeq.Length > 0 Then
                    sb.Append(kmerSeq(kmerSeq.Length - 1)) ' 取最后一个字符
                End If
            Next

            Return sb.ToString()
        End Function

    End Class
End Namespace