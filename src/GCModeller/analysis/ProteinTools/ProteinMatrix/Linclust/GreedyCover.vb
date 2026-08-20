' Linclust 阶段五:贪心集合覆盖聚类
'
' 输入:一张有向边图(成员 -> 中心)。
' 1. 读入所有中心 -> 成员有向边,并补上反向边(使关系对称)。
' 2. 把所有序列按长度降序排序。
' 3. 循环:取顶部序列 s,把所有与 s 有边相连且仍在列表中的序列一并移除,构成一个新簇,s 为簇代表。
' 4. 重复直到列表为空。

Namespace SMRUCC.genomics.Model.MotifGraph.ProteinStructure.Linclust

    Public Module GreedyCover

        ''' <summary>
        ''' 阶段五贪心集合覆盖。
        ''' </summary>
        ''' <param name="edges">有向边列表(From=成员, To=中心)</param>
        ''' <param name="seqLengths">序列 ID -> 序列长度</param>
        ''' <returns>聚类结果(每簇代表为最长成员)</returns>
        Public Function Cluster(edges As List(Of (From As Integer, [To] As Integer)), seqLengths As Dictionary(Of Integer, Integer)) As List(Of Cluster)
            ' 构建无向邻接表(补全反向边)
            Dim adj As New Dictionary(Of Integer, HashSet(Of Integer))

            Dim addEdge = Sub(a As Integer, b As Integer)
                             If Not adj.ContainsKey(a) Then
                                 adj(a) = New HashSet(Of Integer)
                             End If
                             adj(a).Add(b)
                         End Sub

            For Each e In edges
                addEdge(e.From, e.[To])
                addEdge(e.[To], e.From)
            Next

            ' 剩余待处理序列,按长度降序(长度相同按 ID 升序保证确定性)
            Dim remaining As New List(Of Integer)(seqLengths.Keys)
            remaining.Sort(Function(x, y)
                               Dim c = seqLengths(y).CompareTo(seqLengths(x))  ' 长度降序
                               If c <> 0 Then
                                   Return c
                               End If
                               Return x.CompareTo(y)
                           End Function)

            Dim inList As New HashSet(Of Integer)(remaining)
            Dim clusters As New List(Of Cluster)

            For Each s In remaining
                If Not inList.Contains(s) Then
                    Continue For
                End If

                ' s 即新簇代表;收集所有与 s 有边且仍在列表中的序列
                Dim members As New List(Of Integer) From {s}
                inList.Remove(s)

                If adj.ContainsKey(s) Then
                    For Each nb In adj(s)
                        If inList.Contains(nb) Then
                            members.Add(nb)
                            inList.Remove(nb)
                        End If
                    Next
                End If

                clusters.Add(New Cluster With {
                    .representative = s,
                    .members = members
                })
            Next

            Return clusters
        End Function
    End Module
End Namespace
