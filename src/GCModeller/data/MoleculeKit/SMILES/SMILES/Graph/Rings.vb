Namespace Graph

    ''' <summary>
    ''' 分子图中的一个环：按环上顺序排列的原子下标序列。
    ''' </summary>
    Public Class Ring

        ''' <summary>
        ''' 环上的原子下标，按在环上相邻的顺序排列；首尾原子互相成键。
        ''' </summary>
        Public Property Atoms As Integer()

        ''' <summary>
        ''' 环上原子的个数（即环的大小）。
        ''' </summary>
        Public ReadOnly Property Size As Integer
            Get
                Return If(Atoms Is Nothing, 0, Atoms.Length)
            End Get
        End Property

        ''' <summary>
        ''' 该环包含的全部键（用于 GF(2) 独立性判定）。
        ''' </summary>
        Public Property BondIndices As Integer()

        ''' <summary>
        ''' 索引化的原子集合，用于快速求交与去重。
        ''' </summary>
        Public ReadOnly Property AtomSet As HashSet(Of Integer)
            Get
                Return New HashSet(Of Integer)(Atoms)
            End Get
        End Property

        ''' <summary>
        ''' 该环是否与另一个环共享至少一条键（稠合或螺接）。
        ''' </summary>
        ''' <param name="other">另一个环。</param>
        Public Function SharesAtomsWith(other As Ring) As Integer
            Dim sharedCount As Integer = 0
            Dim otherSet As HashSet(Of Integer) = other.AtomSet

            For Each a As Integer In Atoms
                If otherSet.Contains(a) Then sharedCount += 1
            Next

            Return sharedCount
        End Function

        ''' <summary>
        ''' 调试用的紧凑描述，例如 <c>[1, 2, 3, 4, 5, 6]</c>。
        ''' </summary>
        Public Overrides Function ToString() As String
            Return "[" & String.Join(", ", Atoms) & "]"
        End Function

    End Class

    ''' <summary>
    ''' 最小环基（SSSR）求解器。
    ''' </summary>
    ''' <remarks>
    ''' 做法：对每条键临时删除后求两端点之间的最短路，得到一个候选环；
    ''' 按环大小升序贪心选取，用边的 GF(2) 行阶梯基剔除线性相关的候选，
    ''' 最终得到的环个数恰好等于 <c>E − V + C</c>（回路空间维数）。
    ''' 时间复杂度 <c>O(E·(V+E))</c>，在代谢物尺度（≤200 原子）下可忽略。
    ''' </remarks>
    Public Module Rings

        ''' <summary>
        ''' 计算分子图的最小环基。
        ''' </summary>
        ''' <param name="graph">目标分子图。</param>
        ''' <returns>最小环基列表（按环大小升序）。</returns>
        Public Function FindRings(graph As MoleculeGraph) As List(Of Ring)
            Dim adjacency As List(Of List(Of Integer)) = graph.Adjacency()
            Dim candidates As New List(Of Ring)
            Dim seen As New HashSet(Of String)

            For e As Integer = 0 To graph.Bonds.Count - 1
                Dim bond As Bond = graph.Bonds(e)
                Dim path As List(Of Integer) = shortestPath(adjacency, graph.Bonds, bond.a, bond.b, e)

                If path Is Nothing OrElse path.Count < 3 Then Continue For

                Dim key As String = String.Join(",", path.OrderBy(Function(i) i))

                If Not seen.Add(key) Then Continue For

                Dim bondIds As New List(Of Integer)

                For i = 0 To path.Count - 1
                    Dim x As Integer = path(i)
                    Dim y As Integer = path((i + 1) Mod path.Count)
                    Dim id As Integer = findBond(adjacency, graph.Bonds, x, y)

                    If id >= 0 Then bondIds.Add(id)
                Next

                If bondIds.Count <> path.Count Then Continue For

                candidates.Add(New Ring With {
                    .Atoms = path.ToArray,
                    .BondIndices = bondIds.ToArray
                })
            Next

            candidates.Sort(Function(x, y) x.Size.CompareTo(y.Size))

            Dim basis As New Dictionary(Of Integer, HashSet(Of Integer))
            Dim result As New List(Of Ring)

            For Each ring As Ring In candidates
                If addToBasis(basis, ring.BondIndices) Then result.Add(ring)
            Next

            For Each bond As Bond In graph.Bonds
                bond.IsRing = False
            Next

            For Each ring As Ring In result
                For Each id As Integer In ring.BondIndices
                    graph.Bonds(id).IsRing = True
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 把环划分成环系：共享至少一条键的环归为同一个环系。
        ''' </summary>
        ''' <param name="rings">最小环基。</param>
        ''' <returns>每个环系一个环列表。</returns>
        Public Function RingSystems(rings As List(Of Ring)) As List(Of List(Of Ring))
            Dim systems As New List(Of List(Of Ring))

            For Each ring As Ring In rings
                Dim hit As List(Of Ring) = Nothing

                For Each system As List(Of Ring) In systems
                    If system.Any(Function(r) sharesBond(r, ring)) Then
                        hit = system
                        Exit For
                    End If
                Next

                If hit Is Nothing Then
                    systems.Add(New List(Of Ring) From {ring})
                Else
                    hit.Add(ring)
                End If
            Next

            ' 环系可能因为桥接环合并而不完整，做一次并查式合并
            Dim merged As Boolean = True

            While merged
                merged = False

                For i = 0 To systems.Count - 1
                    For j = i + 1 To systems.Count - 1
                        If systems(i).Any(Function(a) systems(j).Any(Function(b) sharesBond(a, b))) Then
                            systems(i).AddRange(systems(j))
                            systems.RemoveAt(j)
                            merged = True
                            Exit For
                        End If
                    Next

                    If merged Then Exit For
                Next
            End While

            Return systems
        End Function

        Private Function sharesBond(x As Ring, y As Ring) As Boolean
            If x.BondIndices Is Nothing OrElse y.BondIndices Is Nothing Then
                Return x.SharesAtomsWith(y) >= 2
            End If

            Dim otherSet As New HashSet(Of Integer)(y.BondIndices)

            Return x.BondIndices.Any(Function(id) otherSet.Contains(id))
        End Function

        ''' <summary>
        ''' 把候选环加入 GF(2) 基；若线性相关（不扩展回路空间）则返回 <c>False</c>。
        ''' </summary>
        Private Function addToBasis(basis As Dictionary(Of Integer, HashSet(Of Integer)), edges As Integer()) As Boolean
            Dim vec As New HashSet(Of Integer)(edges)

            While vec.Count > 0
                Dim pivot As Integer = vec.Min()
                Dim row As HashSet(Of Integer) = Nothing

                If basis.TryGetValue(pivot, row) Then
                    vec.SymmetricExceptWith(row)
                Else
                    basis(pivot) = vec
                    Return True
                End If
            End While

            Return False
        End Function

        Private Function findBond(adjacency As List(Of List(Of Integer)), bonds As List(Of Bond), x As Integer, y As Integer) As Integer
            For Each id As Integer In adjacency(x)
                Dim bond As Bond = bonds(id)

                If (bond.a = x AndAlso bond.b = y) OrElse (bond.a = y AndAlso bond.b = x) Then Return id
            Next

            Return -1
        End Function

        ''' <summary>
        ''' 在排除指定键的前提下求两点之间的最短路（BFS，无权重）。
        ''' </summary>
        Private Function shortestPath(adjacency As List(Of List(Of Integer)),
                                      bonds As List(Of Bond),
                                      start As Integer,
                                      goal As Integer,
                                      excludeBond As Integer) As List(Of Integer)

            Dim parent As New Dictionary(Of Integer, Integer)
            Dim queue As New Queue(Of Integer)
            Dim visited As New HashSet(Of Integer)

            visited.Add(start)
            queue.Enqueue(start)

            While queue.Count > 0
                Dim current As Integer = queue.Dequeue()

                If current = goal Then Exit While

                For Each id As Integer In adjacency(current)
                    If id = excludeBond Then Continue For

                    Dim bond As Bond = bonds(id)
                    Dim nextNode As Integer = If(bond.a = current, bond.b, bond.a)

                    If visited.Contains(nextNode) Then Continue For

                    visited.Add(nextNode)
                    parent(nextNode) = current
                    queue.Enqueue(nextNode)
                Next
            End While

            If Not visited.Contains(goal) Then Return Nothing

            Dim path As New List(Of Integer)
            Dim node As Integer = goal

            While node <> start
                path.Add(node)
                node = parent(node)
            End While

            path.Add(start)
            path.Reverse()

            Return path
        End Function

    End Module

End Namespace
