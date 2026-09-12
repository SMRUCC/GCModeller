Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering.Hierarchy
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Models
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolview

Namespace Evolution.UPGMA

    ''' <summary>
    ''' UPGMA（非加权组平均法，Unweighted Pair Group Method with Arithmetic mean）：
    ''' 每轮合并距离最近的两个簇，新簇与其他簇的距离取算术平均：
    ''' <c>D_kl = (n_i * D_ki + n_j * D_kj) / (n_i + n_j)</c>。
    ''' 该方法假设严格分子钟，输出一棵**有根树**。
    ''' </summary>
    ''' <remarks>
    ''' 凝聚过程**直接复用**基础库 <c>hierarchical-clustering</c> 中的
    ''' <see cref="AverageLinkageStrategy"/>（即 UPGMA 连锁准则）、<see cref="HierarchyBuilder"/> 与
    ''' <see cref="DistanceMap"/>，本项目只负责把聚类得到的 <see cref="Cluster"/> 树
    ''' 转换为系统发育树节点 <see cref="PhyloNode"/>（分支长度 = 合并高度之差）。
    ''' </remarks>
    Public Module UpgmaTree

        ''' <summary>
        ''' 由距离矩阵构建 UPGMA 有根树。
        ''' </summary>
        ''' <param name="distances">对称距离方阵</param>
        ''' <param name="linkageStrategy">
        ''' 连锁准则，默认为 <see cref="AverageLinkageStrategy"/>（UPGMA）。
        ''' 传入 <see cref="WeightedLinkageStrategy"/> 即得到 WPGMA。
        ''' </param>
        Public Function Build(distances As DistanceMatrix,
                              Optional linkageStrategy As LinkageStrategy = Nothing) As PhyloNode

            If linkageStrategy Is Nothing Then
                linkageStrategy = New AverageLinkageStrategy
            End If

            Dim n As Integer = distances.Size

            If n = 0 Then
                Throw New ArgumentException("距离矩阵为空，无法构建 UPGMA 树！")
            ElseIf n = 1 Then
                Return New PhyloNode With {.ID = distances.Names(0), .IsLeaf = True, .IsRoot = True}
            End If

            Dim clusters As List(Of Cluster) = distances.Names _
                .Select(Function(name) New Cluster(name)) _
                .AsList
            Dim linkages As New List(Of HierarchyTreeNode)

            For i As Integer = 0 To n - 2
                For j As Integer = i + 1 To n - 1
                    linkages.Add(New HierarchyTreeNode(clusters(i), clusters(j), distances.Matrix(i)(j)))
                Next
            Next

            Dim builder As New HierarchyBuilder(clusters, New DistanceMap(linkages))

            Do While Not builder.TreeComplete
                Call builder.Agglomerate(linkageStrategy)
            Loop

            Dim root As Cluster = builder.RootCluster

            Return Convert(root, root.DistanceValue)
        End Function

        ''' <summary>
        ''' 将凝聚层次聚类树节点转换为系统发育树节点。
        ''' </summary>
        ''' <param name="cluster">当前聚类节点</param>
        ''' <param name="parentDistance">
        ''' 父节点的合并高度；子节点分支长度 = 父节点合并高度 - 当前节点合并高度。
        ''' </param>
        Private Function Convert(cluster As Cluster, parentDistance As Double) As PhyloNode
            Dim node As New PhyloNode()
            Dim branch As Double = parentDistance - cluster.DistanceValue

            If branch < 0 Then
                branch = 0
            End If

            node.BranchLength = CSng(branch)

            If cluster.isLeaf Then
                node.ID = cluster.Name
                node.IsLeaf = True
                node.IsRoot = False
            Else
                node.ID = ""
                node.IsLeaf = False
                node.IsRoot = cluster.Parent Is Nothing

                For Each child As Cluster In cluster.Children
                    Call node.AddDescendent(Convert(child, cluster.DistanceValue))
                Next
            End If

            Return node
        End Function
    End Module

End Namespace
