Namespace Clustal

    ''' <summary>
    ''' UPGMA(非加权组平均法)聚类：每次合并平均距离最小的一对簇，簇间距离按簇大小加权平均。
    ''' </summary>
    ''' <remarks>
    ''' 为了保证同一份输入多次运行的结果完全一致，最小距离的查找使用"严格小于"的比较，
    ''' 因此在距离相同的情况下总是优先合并索引较小的一对簇，不存在任何随机性。
    ''' </remarks>
    Public Module UPGMA

        ''' <summary>
        ''' 由距离矩阵构建 UPGMA 树
        ''' </summary>
        ''' <param name="dist">对称的距离矩阵</param>
        ''' <param name="labels">可选的叶子名称</param>
        Public Shared Function Build(dist As Double()(), Optional labels As String() = Nothing) As GuideTree
            Dim n As Integer = If(dist Is Nothing, 0, dist.Length)

            If n = 0 Then
                Return New GuideTree(Nothing, 0)
            End If

            If n = 1 Then
                Dim single As New GuideTreeNode With {
                    .Index = 0,
                    .Label = If(labels IsNot Nothing AndAlso labels.Length > 0, labels(0), "seq_1"),
                    .Height = 0,
                    .LeafCount = 1
                }

                Return New GuideTree(single, 1)
            End If

            Dim total As Integer = 2 * n - 1
            Dim nodes As GuideTreeNode() = New GuideTreeNode(total - 1) {}
            Dim sizes As Integer() = New Integer(total - 1) {}
            Dim d As Double(,) = New Double(total - 1, total - 1) {}

            For i As Integer = 0 To n - 1
                nodes(i) = New GuideTreeNode With {
                    .Index = i,
                    .Label = If(labels IsNot Nothing AndAlso i < labels.Length, labels(i), $"seq_{i + 1}"),
                    .Height = 0,
                    .LeafCount = 1
                }
                sizes(i) = 1
            Next

            For i As Integer = 0 To n - 1
                For j As Integer = 0 To n - 1
                    d(i, j) = If(i = j, 0.0R, dist(i)(j))
                Next
            Next

            For step As Integer = 0 To n - 2
                Dim minI As Integer = -1
                Dim minJ As Integer = -1
                Dim minValue As Double = Double.MaxValue

                For i As Integer = 0 To n + step - 1
                    If sizes(i) <= 0 Then
                        Continue For
                    End If

                    For j As Integer = i + 1 To n + step - 1
                        If sizes(j) <= 0 Then
                            Continue For
                        End If

                        Dim value As Double = d(i, j)

                        If Double.IsNaN(value) Then
                            Continue For
                        End If
                        If value < minValue Then
                            minValue = value
                            minI = i
                            minJ = j
                        End If
                    Next
                Next

                If minI < 0 Then
                    ' 理论上不会发生，防御性处理：把剩余的簇顺序串成一棵退化树
                    minI = -1

                    For i As Integer = 0 To n + step - 1
                        If sizes(i) > 0 Then
                            If minI < 0 Then
                                minI = i
                            Else
                                minJ = i
                                Exit For
                            End If
                        End If
                    Next

                    minValue = 0
                End If

                If minI < 0 OrElse minJ < 0 Then
                    ' 只剩一个簇时没有可以合并的对象，直接结束
                    Exit For
                End If

                Dim node As Integer = n + step
                Dim si As Integer = sizes(minI)
                Dim sj As Integer = sizes(minJ)

                nodes(node) = New GuideTreeNode With {
                    .Index = -1,
                    .Left = nodes(minI),
                    .Right = nodes(minJ),
                    .Height = minValue / 2.0R,
                    .LeafCount = si + sj
                }
                sizes(node) = si + sj
                sizes(minI) = 0
                sizes(minJ) = 0

                For k As Integer = 0 To n + step - 1
                    If sizes(k) <= 0 Then
                        d(node, k) = Double.MaxValue
                        d(k, node) = Double.MaxValue
                        Continue For
                    End If

                    Dim weighted As Double = (si * d(minI, k) + sj * d(minJ, k)) / (si + sj)

                    d(node, k) = weighted
                    d(k, node) = weighted
                Next

                d(node, node) = 0
            Next

            Return New GuideTree(nodes(total - 1), n)
        End Function
    End Class
End Namespace
