Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolview

Namespace Evolution.TreeSearch

    ''' <summary>
    ''' NNI（最近邻交换，Nearest Neighbor Interchange）拓扑移动：
    ''' 针对内部边 (U -> V)，交换 V 的一个子节点 A 与 U 的另一个子节点 W。
    ''' </summary>
    ''' <remarks>
    ''' 由于移动以节点索引描述，而 <see cref="PhyloNode.Clone"/> 会保持子节点的原始顺序，
    ''' 因此同一次枚举得到的索引可以安全地作用在同一结构（或其克隆体）之上。
    ''' </remarks>
    Public Structure NniMove
        Public U As Integer
        Public V As Integer
        Public A As Integer
        Public W As Integer
    End Structure

    ''' <summary>
    ''' SPR（子树修剪重接，Subtree Pruning and Regrafting）拓扑移动：
    ''' 剪除 U 的子节点 V（连同其子树），再把它重新接到边 (P -> C) 上。
    ''' </summary>
    Public Structure SprMove
        Public U As Integer
        Public V As Integer
        Public P As Integer
        Public C As Integer
    End Structure

    ''' <summary>
    ''' 系统发育树拓扑重排算子（NNI / SPR），用于最大简约法与最大似然法的启发式树搜索。
    ''' </summary>
    Public Module TreeRearrangement

        ''' <summary>
        ''' 以确定性的先序遍历返回树中的所有节点，索引即移动算子所使用的坐标。
        ''' </summary>
        Public Function EnumerateNodes(root As PhyloNode) As List(Of PhyloNode)
            Dim list As New List(Of PhyloNode)
            Call __travel(root, list)

            Return list
        End Function

        Private Sub __travel(node As PhyloNode, list As List(Of PhyloNode))
            If node Is Nothing Then
                Return
            End If

            list.Add(node)

            For Each child As PhyloNode In node.Descendents
                Call __travel(child, list)
            Next
        End Sub

        ''' <summary>
        ''' 深拷贝树拓扑（用于移动试算与状态回滚）。
        ''' </summary>
        Public Function Clone(root As PhyloNode) As PhyloNode
            Return root.Clone()
        End Function

        ''' <summary>
        ''' 判断 <paramref name="node"/> 是否位于 <paramref name="possibleAncestor"/> 的子树内部（含自身）。
        ''' </summary>
        Public Function InSubtree(node As PhyloNode, possibleAncestor As PhyloNode) As Boolean
            Dim current As PhyloNode = node

            While current IsNot Nothing
                If current Is possibleAncestor Then
                    Return True
                End If

                current = current.Parent
            End While

            Return False
        End Function

        ''' <summary>
        ''' 枚举树中所有合法的 NNI 移动。
        ''' </summary>
        Public Function NniMoves(root As PhyloNode) As List(Of NniMove)
            Dim nodes As List(Of PhyloNode) = EnumerateNodes(root)
            Dim index As New Dictionary(Of PhyloNode, Integer)

            For i As Integer = 0 To nodes.Count - 1
                index(nodes(i)) = i
            Next

            Dim moves As New List(Of NniMove)

            For v As Integer = 0 To nodes.Count - 1
                Dim nodeV As PhyloNode = nodes(v)

                ' 只对内部节点（至少两个子节点）进行交换
                If nodeV.Descendents.Count < 2 Then
                    Continue For
                End If

                Dim nodeU As PhyloNode = nodeV.Parent

                ' 根节点没有父节点，但三叉根节点的子内部边依然是合法的内部边；
                ' 这里通过“父节点至少还有一个别的子节点”来判定。
                If nodeU Is Nothing OrElse nodeU.Descendents.Count < 2 Then
                    Continue For
                End If

                Dim u As Integer = index(nodeU)

                For Each a As PhyloNode In nodeV.Descendents
                    For Each w As PhyloNode In nodeU.Descendents
                        If w Is nodeV Then
                            Continue For
                        End If

                        moves.Add(New NniMove With {
                            .U = u,
                            .V = v,
                            .A = index(a),
                            .W = index(w)
                        })
                    Next
                Next
            Next

            Return moves
        End Function

        ''' <summary>
        ''' 应用一个 NNI 移动。
        ''' </summary>
        Public Sub ApplyNni(root As PhyloNode, move As NniMove)
            Dim nodes As List(Of PhyloNode) = EnumerateNodes(root)
            Dim u As PhyloNode = nodes(move.U)
            Dim v As PhyloNode = nodes(move.V)
            Dim a As PhyloNode = nodes(move.A)
            Dim w As PhyloNode = nodes(move.W)

            If a Is w Then
                Return
            End If

            Call v.RemoveDescendent(a)
            Call u.RemoveDescendent(w)
            Call u.AddDescendent(a)
            Call v.AddDescendent(w)
        End Sub

        ''' <summary>
        ''' 枚举（或随机采样）合法的 SPR 移动。
        ''' </summary>
        ''' <param name="root">树的根节点</param>
        ''' <param name="maxCount">最多返回的移动数目</param>
        ''' <param name="rand">可选的随机数发生器，用于采样；为空时按确定性顺序截取</param>
        Public Function SprMoves(root As PhyloNode,
                                 Optional maxCount As Integer = 100,
                                 Optional rand As Random = Nothing) As List(Of SprMove)

            Dim nodes As List(Of PhyloNode) = EnumerateNodes(root)
            Dim index As New Dictionary(Of PhyloNode, Integer)

            For i As Integer = 0 To nodes.Count - 1
                index(nodes(i)) = i
            Next

            Dim candidates As New List(Of SprMove)

            For v As Integer = 0 To nodes.Count - 1
                Dim nodeV As PhyloNode = nodes(v)
                Dim nodeU As PhyloNode = nodeV.Parent

                If nodeU Is Nothing Then
                    Continue For
                End If

                ' 剪除之后需保证 U 仍然是一个合法的内部节点（至少两个子节点），
                ' 否则需要做节点压制，这里为简化起见只考虑 U 的子节点数 >= 3 的情形。
                If nodeU.Descendents.Count < 3 Then
                    Continue For
                End If

                For p As Integer = 0 To nodes.Count - 1
                    Dim nodeP As PhyloNode = nodes(p)

                    ' 目标边不能位于被剪除的子树内部，也不能就是被剪除的那条边
                    If InSubtree(nodeP, nodeV) Then
                        Continue For
                    End If

                    For Each nodeC As PhyloNode In nodeP.Descendents
                        If nodeC Is nodeV Then
                            Continue For
                        End If
                        If InSubtree(nodeC, nodeV) Then
                            Continue For
                        End If

                        candidates.Add(New SprMove With {
                            .U = index(nodeU),
                            .V = v,
                            .P = p,
                            .C = index(nodeC)
                        })
                    Next
                Next
            Next

            If candidates.Count <= maxCount Then
                Return candidates
            End If

            If rand Is Nothing Then
                Return candidates.Take(maxCount).ToList
            End If

            ' Fisher-Yates 洗牌后截取
            Dim array As SprMove() = candidates.ToArray
            Dim n As Integer = array.Length

            For i As Integer = 0 To maxCount - 1
                Dim j As Integer = rand.Next(i, n)
                Dim tmp As SprMove = array(i)
                array(i) = array(j)
                array(j) = tmp
            Next

            Return array.Take(maxCount).ToList
        End Function

        ''' <summary>
        ''' 应用一个 SPR 移动：剪除子树 V 并重接到边 (P -> C) 上。
        ''' </summary>
        Public Sub ApplySpr(root As PhyloNode, move As SprMove)
            Dim nodes As List(Of PhyloNode) = EnumerateNodes(root)
            Dim u As PhyloNode = nodes(move.U)
            Dim v As PhyloNode = nodes(move.V)
            Dim p As PhyloNode = nodes(move.P)
            Dim c As PhyloNode = nodes(move.C)

            If v Is p OrElse v Is c OrElse InSubtree(p, v) OrElse InSubtree(c, v) Then
                Return
            End If

            ' 1. 剪除 V
            Call u.RemoveDescendent(v)

            ' 2. 在边 (P -> C) 上插入新的内部节点 N，N 的子节点为 C 与 V
            Dim n As New PhyloNode With {
                .ID = "",
                .IsLeaf = False,
                .IsRoot = False,
                .BranchLength = v.BranchLength
            }

            Call p.RemoveDescendent(c)
            Call p.AddDescendent(n)
            Call n.AddDescendent(c)
            Call n.AddDescendent(v)
        End Sub
    End Module

End Namespace
