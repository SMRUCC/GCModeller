Namespace Clustal

    ''' <summary>
    ''' 分簇 + 建树的中间结果
    ''' </summary>
    Public Class ClusterTree

        ''' <summary>
        ''' 全局引导树
        ''' </summary>
        Public Property Tree As GuideTree
        ''' <summary>
        ''' 每条序列所属的簇编号
        ''' </summary>
        Public Property Assignments As Integer()
        ''' <summary>
        ''' 每个簇包含的序列条数
        ''' </summary>
        Public Property ClusterSizes As Integer()
        ''' <summary>
        ''' 簇数量
        ''' </summary>
        Public Property ClusterCount As Integer

        Public Overrides Function ToString() As String
            Return $"ClusterTree(clusters={ClusterCount}, leaves={If(Tree Is Nothing, 0, Tree.LeafCount)})"
        End Function
    End Class

    ''' <summary>
    ''' 按 clustalo 的层次化结构构造引导树：
    ''' mBed 低维坐标 -> k-means 分簇 -> 簇内全距离 UPGMA 子树 -> 簇间代表序列 UPGMA 上层树 -> 拼接。
    ''' </summary>
    Public Module ClusterTreeBuilder

        ''' <summary>
        ''' 构造全局引导树
        ''' </summary>
        ''' <param name="seqs">归一化之后的序列</param>
        ''' <param name="mbed">mBed 投影结果(或者完整的距离矩阵)</param>
        ''' <param name="alphabet">字母表</param>
        ''' <param name="kmer">k-mer 长度</param>
        ''' <param name="options">参数</param>
        Public Function Build(seqs As String(), mbed As MBedDistance, alphabet As SequenceAlphabet,
                                     kmer As Integer, options As ClustalOmegaOptions) As ClusterTree

            Dim n As Integer = seqs.Length

            If n = 0 Then
                Return New ClusterTree With {
                    .Tree = New GuideTree(Nothing, 0),
                    .Assignments = New Integer() {},
                    .ClusterSizes = New Integer() {},
                    .ClusterCount = 0
                }
            End If

            If n = 1 Then
                Dim leaf As New GuideTreeNode With {.Index = 0, .Label = "seq_1", .Height = 0, .LeafCount = 1}

                Return New ClusterTree With {
                    .Tree = New GuideTree(leaf, 1),
                    .Assignments = New Integer() {0},
                    .ClusterSizes = New Integer() {1},
                    .ClusterCount = 1
                }
            End If

            ' 完整距离矩阵分支：直接对全部序列做一次 UPGMA
            If mbed.IsFull Then
                Return New ClusterTree With {
                    .Tree = UPGMA.Build(mbed.Matrix, Nothing),
                    .Assignments = New Integer(n - 1) {},
                    .ClusterSizes = New Integer() {n},
                    .ClusterCount = 1
                }
            End If

            Dim clusterCount As Integer = Math.Max(1, CInt(Math.Ceiling(CDbl(n) / Math.Max(2, options.ClusterSize))))
            Dim assignments As Integer() = KMeansCluster.Cluster(mbed.Feature, clusterCount, options.Seed)
            Dim members As List(Of Integer)() = GroupMembers(assignments, clusterCount)
            Dim subtrees As GuideTreeNode() = New GuideTreeNode(clusterCount - 1) {}
            Dim medoids As Integer() = New Integer(clusterCount - 1) {}
            Dim sizes As Integer() = New Integer(clusterCount - 1) {}

            For c As Integer = 0 To clusterCount - 1
                Dim list As List(Of Integer) = members(c)

                If list Is Nothing OrElse list.Count = 0 Then
                    subtrees(c) = New GuideTreeNode With {.Index = -1, .Label = $"empty_{c + 1}", .LeafCount = 0}
                    medoids(c) = -1
                    sizes(c) = 0
                    Continue For
                End If

                Dim index As Integer() = list.ToArray()
                Dim dist As Double()() = IntraClusterDistance(seqs, index, alphabet, kmer)

                subtrees(c) = UPGMA.Build(dist, Nothing).Root

                ' 子树内部的叶子索引需要重映射回全局索引
                Call RemapLeaves(subtrees(c), index)

                medoids(c) = SelectMedoid(mbed.Feature, index)
                sizes(c) = index.Length
            Next

            ' 簇间距离：以每个簇的 medoid 作为代表
            Dim clusterDist As Double()() = New Double(clusterCount - 1)() {}

            For a As Integer = 0 To clusterCount - 1
                clusterDist(a) = New Double(clusterCount - 1) {}
            Next

            For a As Integer = 0 To clusterCount - 1
                For b As Integer = a + 1 To clusterCount - 1
                    Dim d As Double

                    If medoids(a) < 0 OrElse medoids(b) < 0 Then
                        d = 1.0R
                    Else
                        d = mbed.PairDistance(medoids(a), medoids(b))
                    End If

                    clusterDist(a)(b) = d
                    clusterDist(b)(a) = d
                Next
            Next

            Dim upper As GuideTree = UPGMA.Build(clusterDist, Nothing)
            Dim root As GuideTreeNode = Splice(upper.Root, subtrees)

            Return New ClusterTree With {
                .Tree = New GuideTree(root, n),
                .Assignments = assignments,
                .ClusterSizes = sizes,
                .ClusterCount = clusterCount
            }
        End Function

        Private Function GroupMembers(assignments As Integer(), clusterCount As Integer) As List(Of Integer)()
            Dim members As List(Of Integer)() = New List(Of Integer)(clusterCount - 1) {}

            For c As Integer = 0 To clusterCount - 1
                members(c) = New List(Of Integer)()
            Next

            For i As Integer = 0 To assignments.Length - 1
                Dim c As Integer = Math.Max(0, Math.Min(clusterCount - 1, assignments(i)))
                Call members(c).Add(i)
            Next

            Return members
        End Function

        ''' <summary>
        ''' 簇内精确的 k-mer 距离矩阵
        ''' </summary>
        Private Function IntraClusterDistance(seqs As String(), index As Integer(),
                                                     alphabet As SequenceAlphabet, kmer As Integer) As Double()()

            Dim c As Integer = index.Length
            Dim counters As KmerCounter() = New KmerCounter(c - 1) {}
            Dim dist As Double()() = New Double(c - 1)() {}

            For i As Integer = 0 To c - 1
                counters(i) = KmerCounter.Build(seqs(index(i)), alphabet, kmer)
                dist(i) = New Double(c - 1) {}
            Next

            For i As Integer = 0 To c - 1
                For j As Integer = i + 1 To c - 1
                    Dim value As Double = counters(i).Distance(counters(j))
                    dist(i)(j) = value
                    dist(j)(i) = value
                Next
            Next

            Return dist
        End Function

        ''' <summary>
        ''' 把子树里的局部叶子索引(0..c-1)重映射为全局序列索引
        ''' </summary>
        Private Sub RemapLeaves(node As GuideTreeNode, index As Integer())
            If node Is Nothing Then
                Return
            End If

            If node.IsLeaf Then
                If node.Index >= 0 AndAlso node.Index < index.Length Then
                    node.Index = index(node.Index)
                End If

                Return
            End If

            Call RemapLeaves(node.Left, index)
            Call RemapLeaves(node.Right, index)
        End Sub

        ''' <summary>
        ''' 选取离簇质心最近的成员作为代表序列
        ''' </summary>
        Private Function SelectMedoid(feature As Double()(), index As Integer()) As Integer
            If index.Length = 0 Then
                Return -1
            End If
            If feature Is Nothing OrElse feature(index(0)) Is Nothing OrElse feature(index(0)).Length = 0 Then
                Return index(0)
            End If

            Dim d As Integer = feature(index(0)).Length
            Dim centroid As Double() = New Double(d - 1) {}

            For Each member As Integer In index
                Dim row As Double() = feature(member)

                For t As Integer = 0 To d - 1
                    centroid(t) += row(t)
                Next
            Next

            For t As Integer = 0 To d - 1
                centroid(t) /= index.Length
            Next

            Dim best As Integer = index(0)
            Dim bestDist As Double = Double.MaxValue

            For Each member As Integer In index
                Dim row As Double() = feature(member)
                Dim acc As Double = 0

                For t As Integer = 0 To d - 1
                    Dim diff As Double = row(t) - centroid(t)
                    acc += diff * diff
                Next

                If acc < bestDist Then
                    bestDist = acc
                    best = member
                End If
            Next

            Return best
        End Function

        ''' <summary>
        ''' 把上层树的每一个叶子替换为对应的簇内子树
        ''' </summary>
        Private Function Splice(node As GuideTreeNode, subtrees As GuideTreeNode()) As GuideTreeNode
            If node Is Nothing Then
                Return Nothing
            End If

            If node.IsLeaf Then
                If node.Index >= 0 AndAlso node.Index < subtrees.Length Then
                    Return subtrees(node.Index)
                Else
                    Return New GuideTreeNode With {.Index = -1, .LeafCount = 0, .Label = "empty"}
                End If
            End If

            Dim left As GuideTreeNode = Splice(node.Left, subtrees)
            Dim right As GuideTreeNode = Splice(node.Right, subtrees)

            Return New GuideTreeNode With {
                .Index = -1,
                .Left = left,
                .Right = right,
                .Height = node.Height,
                .LeafCount = If(left Is Nothing, 0, left.LeafCount) + If(right Is Nothing, 0, right.LeafCount)
            }
        End Function
    End Module
End Namespace
