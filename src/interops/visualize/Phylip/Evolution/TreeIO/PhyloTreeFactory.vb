Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolview

Namespace Evolution.TreeIO

    ''' <summary>
    ''' 建树算法结果（<see cref="PhyloNode"/> 拓扑）与项目内现有树模型 <see cref="PhyloTree"/> 之间的适配层，
    ''' 同时提供遍历、叶集合提取、split（分支 bipartition）编码等树操作工具。
    ''' </summary>
    Public Module PhyloTreeFactory

        ''' <summary>
        ''' 将算法计算得到的树拓扑转换为 <see cref="PhyloTree"/> 对象。
        ''' </summary>
        ''' <remarks>
        ''' 当树中存在 bootstrap 支持度时，会自动标记该树含有支持度数据，
        ''' 以便后续 <see cref="PhyloTree.toTreeString"/> 可以输出支持度标注。
        ''' </remarks>
        Public Function ToTree(root As PhyloNode, Optional name As String = "Evolution Tree") As PhyloTree
            Dim tree As PhyloTree = PhyloTree.FromNodes(name, root)

            If EnumerateNodes(root).Any(Function(n) n.BootStrap > 0) Then
                Call tree.MarkBootstrapScores()
            End If

            Return tree
        End Function

        ''' <summary>
        ''' 输出 Newick 文本（可选是否输出分支长度与 bootstrap 支持度）。
        ''' </summary>
        Public Function CreateNewick(root As PhyloNode,
                                     Optional name As String = "Evolution Tree",
                                     Optional showBootstrap As Boolean = True,
                                     Optional showBranchLength As Boolean = True) As String

            Dim tree As PhyloTree = ToTree(root, name)

            Return tree.toTreeString("newick", showBootstrap, False, False)
        End Function

        ''' <summary>
        ''' 以先序遍历的方式枚举根节点下的所有节点（包含根节点自身）。
        ''' </summary>
        Public Iterator Function EnumerateNodes(root As PhyloNode) As IEnumerable(Of PhyloNode)
            If root Is Nothing Then
                Return
            End If

            Yield root

            For Each child As PhyloNode In root.Descendents
                For Each node As PhyloNode In EnumerateNodes(child)
                    Yield node
                Next
            Next
        End Function

        ''' <summary>
        ''' 枚举树中的所有叶节点。
        ''' </summary>
        Public Function EnumerateLeaves(root As PhyloNode) As IEnumerable(Of PhyloNode)
            Return EnumerateNodes(root).Where(Function(n) n.Descendents.Count = 0)
        End Function

        ''' <summary>
        ''' 返回树中所有叶节点的名称。
        ''' </summary>
        Public Function LeafLabels(root As PhyloNode) As String()
            Return EnumerateLeaves(root).Select(Function(n) n.ID).ToArray
        End Function

        ''' <summary>
        ''' 递归收集指定节点子树下的所有叶节点名称。
        ''' </summary>
        Public Function GetLeafNames(node As PhyloNode) As List(Of String)
            Dim names As New List(Of String)

            If node.Descendents.Count = 0 Then
                names.Add(node.ID)
            Else
                For Each child As PhyloNode In node.Descendents
                    names.AddRange(GetLeafNames(child))
                Next
            End If

            Return names
        End Function

        ''' <summary>
        ''' 为一组叶节点成员计算规范化的 split 键：取 bipartition 中较小的一侧
        ''' （两侧大小相同时取字典序较小者），从而使得同一分支在参考树与重采样树中具有相同的键。
        ''' </summary>
        Public Function SplitKey(members As IEnumerable(Of String), allLeaves As IEnumerable(Of String)) As String
            Dim set_ As New HashSet(Of String)(members)
            Dim all As String() = allLeaves.Distinct.OrderBy(Function(s) s, StringComparer.Ordinal).ToArray
            Dim other As String() = all.Where(Function(s) Not set_.Contains(s)).ToArray
            Dim side As String() = set_.OrderBy(Function(s) s, StringComparer.Ordinal).ToArray

            Dim chosen As String()

            If side.Length < other.Length Then
                chosen = side
            ElseIf other.Length < side.Length Then
                chosen = other
            Else
                chosen = If(String.CompareOrdinal(String.Join("|", side), String.Join("|", other)) <= 0, side, other)
            End If

            Return String.Join("|", chosen)
        End Function

        ''' <summary>
        ''' 计算树中每一个内部分支（非根、非叶节点）所对应的 split 键，并映射到对应的节点。
        ''' 返回的键为 <see cref="SplitKey"/> 生成的规范化字符串。
        ''' </summary>
        Public Function AllSplits(root As PhyloNode) As Dictionary(Of String, PhyloNode)
            Dim leaves As String() = LeafLabels(root)
            Dim splits As New Dictionary(Of String, PhyloNode)

            For Each node As PhyloNode In EnumerateNodes(root)
                If node Is root OrElse node.Descendents.Count = 0 Then
                    Continue For
                End If

                Dim key As String = SplitKey(GetLeafNames(node), leaves)

                If Not splits.ContainsKey(key) Then
                    splits(key) = node
                End If
            Next

            Return splits
        End Function

        ''' <summary>
        ''' 将一棵树中所有内部节点的支持度数据转移到目标树（依据 split 键进行匹配）。
        ''' </summary>
        Public Sub TransferSupport(reference As PhyloNode, bootstrapCounts As Dictionary(Of String, Integer), replicates As Integer)
            Dim leaves As String() = LeafLabels(reference)

            For Each node As PhyloNode In EnumerateNodes(reference)
                If node Is reference OrElse node.Descendents.Count = 0 Then
                    Continue For
                End If

                Dim key As String = SplitKey(GetLeafNames(node), leaves)
                Dim count As Integer = 0

                If bootstrapCounts.TryGetValue(key, count) AndAlso replicates > 0 Then
                    node.BootStrap = CSng(count * 100.0 / replicates)
                End If
            Next
        End Sub
    End Module

End Namespace
