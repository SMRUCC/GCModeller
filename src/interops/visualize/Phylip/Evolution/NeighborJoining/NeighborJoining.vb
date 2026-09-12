Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Distance
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Models
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolview

Namespace Evolution.NeighborJoining

    ''' <summary>
    ''' 邻接法（Neighbor-Joining, Saitou &amp; Nei 1987）：允许不同支系演化速率不等，
    ''' 从星状树出发逐步合并最近邻居，输出一棵**无根树**。
    ''' </summary>
    ''' <remarks>
    ''' 每一步不再选择原始距离最小的对，而是选择使 Q 值最小的对：
    '''
    ''' Q(i,j) = (n-2) * d(i,j) - Σ_k d(i,k) - Σ_k d(j,k)
    '''
    ''' 分支长度：
    '''
    ''' L(i,u) = 1/2 d(i,j) + 1/(2(n-2)) [ Σ_k d(i,k) - Σ_k d(j,k) ]
    ''' L(j,u) = d(i,j) - L(i,u)
    '''
    ''' 新节点到剩余节点的距离：
    '''
    ''' d(u,k) = ( d(i,k) + d(j,k) - d(i,j) ) / 2
    '''
    ''' 当剩余 3 个分类单元时，三条分支长度由三方距离公式收尾。
    ''' </remarks>
    Public Module NeighborJoining

        ''' <summary>
        ''' 由距离矩阵构建 NJ 无根树（以三叉根节点表示）。
        ''' </summary>
        Public Function Build(distances As DistanceMatrix) As PhyloNode
            Dim n As Integer = distances.Size

            If n = 0 Then
                Throw New ArgumentException("距离矩阵为空，无法构建 NJ 树！")
            ElseIf n = 1 Then
                Return New PhyloNode With {.ID = distances.Names(0), .IsLeaf = True, .IsRoot = True}
            End If

            ' 最多产生 2n-2 个节点（n 个叶节点 + n-2 个内部节点）
            Dim maxNodes As Integer = Math.Max(2 * n - 1, 3)
            Dim d(maxNodes - 1)() As Double

            For i As Integer = 0 To maxNodes - 1
                d(i) = New Double(maxNodes - 1) {}
            Next

            Dim nodes As New List(Of PhyloNode)(maxNodes)
            Dim active As New List(Of Integer)(n)

            For i As Integer = 0 To n - 1
                Dim leaf As New PhyloNode With {.ID = distances.Names(i), .IsLeaf = True, .IsRoot = False}
                nodes.Add(leaf)
                active.Add(i)
            Next

            For i As Integer = 0 To n - 1
                For j As Integer = 0 To n - 1
                    d(i)(j) = distances.Matrix(i)(j)
                Next
            Next

            Dim nextIndex As Integer = n

            Do While active.Count > 3
                Dim m As Integer = active.Count
                Dim r(m - 1) As Double

                For a As Integer = 0 To m - 1
                    Dim ia As Integer = active(a)
                    Dim sum As Double = 0

                    For b As Integer = 0 To m - 1
                        If a <> b Then
                            sum += d(ia)(active(b))
                        End If
                    Next

                    r(a) = sum
                Next

                ' 挑选 Q 值最小的对
                Dim bestA As Integer = 0, bestB As Integer = 1
                Dim bestQ As Double = Double.MaxValue

                For a As Integer = 0 To m - 2
                    For b As Integer = a + 1 To m - 1
                        Dim q As Double = (m - 2) * d(active(a))(active(b)) - r(a) - r(b)

                        If q < bestQ Then
                            bestQ = q
                            bestA = a
                            bestB = b
                        End If
                    Next
                Next

                Dim ia_ As Integer = active(bestA)
                Dim ib_ As Integer = active(bestB)
                Dim dij As Double = d(ia_)(ib_)
                Dim li As Double = 0.5 * dij + (r(bestA) - r(bestB)) / (2.0 * (m - 2))
                Dim lj As Double = dij - li

                ' 生成新的内部节点 u
                Dim u As Integer = nextIndex
                nextIndex += 1

                Dim parent As New PhyloNode With {.ID = "", .IsLeaf = False, .IsRoot = False}
                nodes(ia_).BranchLength = CSng(li)
                nodes(ib_).BranchLength = CSng(lj)
                Call parent.AddDescendent(nodes(ia_))
                Call parent.AddDescendent(nodes(ib_))
                nodes.Add(parent)

                ' 更新 d(u,k)
                For Each k As Integer In active
                    If k <> ia_ AndAlso k <> ib_ Then
                        Dim duk As Double = (d(ia_)(k) + d(ib_)(k) - dij) / 2.0
                        d(u)(k) = duk
                        d(k)(u) = duk
                    End If
                Next

                ' 更新活动集合
                Dim nextActive As New List(Of Integer)(m - 1)

                For Each k As Integer In active
                    If k <> ia_ AndAlso k <> ib_ Then
                        nextActive.Add(k)
                    End If
                Next

                nextActive.Add(u)
                active = nextActive
            Loop

            Select Case active.Count
                Case 3
                    ' 三方收尾：三条分支长度由三方距离公式给出
                    Dim p As Integer = active(0)
                    Dim q As Integer = active(1)
                    Dim s As Integer = active(2)
                    Dim dpq As Double = d(p)(q)
                    Dim dps As Double = d(p)(s)
                    Dim dqs As Double = d(q)(s)

                    nodes(p).BranchLength = CSng(0.5 * (dpq + dps - dqs))
                    nodes(q).BranchLength = CSng(0.5 * (dpq + dqs - dps))
                    nodes(s).BranchLength = CSng(0.5 * (dps + dqs - dpq))

                    Dim root As New PhyloNode With {.ID = "", .IsLeaf = False, .IsRoot = True}
                    Call root.AddDescendent(nodes(p))
                    Call root.AddDescendent(nodes(q))
                    Call root.AddDescendent(nodes(s))

                    Return root
                Case 2
                    ' 只有两个分类单元时的退化情形：对半分配
                    Dim p As Integer = active(0)
                    Dim q As Integer = active(1)
                    Dim half As Double = d(p)(q) / 2.0

                    nodes(p).BranchLength = CSng(half)
                    nodes(q).BranchLength = CSng(half)

                    Dim root As New PhyloNode With {.ID = "", .IsLeaf = False, .IsRoot = True}
                    Call root.AddDescendent(nodes(p))
                    Call root.AddDescendent(nodes(q))

                    Return root
                Case Else
                    ' active.Count = 1
                    Dim only As PhyloNode = nodes(active(0))
                    only.IsRoot = True
                    only.IsLeaf = True

                    Return only
            End Select
        End Function

        ''' <summary>
        ''' 由字符矩阵（比对位点矩阵）先行估计距离，再构建 NJ 树。
        ''' </summary>
        Public Function Build(matrix As CharacterMatrix,
                              Optional model As DistanceModel = DistanceModel.PoissonCorrection) As PhyloNode

            Return Build(SequenceDistance.PairwiseMatrix(matrix, model))
        End Function
    End Module

End Namespace
