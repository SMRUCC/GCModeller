#Region "Metabopolis: graph skeleton"

' ============================================================================
' 图骨架构建（论文第二节）
' ----------------------------------------------------------------------------
' 类别连接图通常非常稠密，不可能所有边都用「街区相邻」来表达，因此论文要求
' 骨架满足三个结构性约束：
'     (1) 平面图      —— 骨架可以无交叉地画在平面上；
'     (2) 弦无图      —— 只包含边不相交的环，且环上没有弦；
'     (3) 最大度数<=4 —— 一个矩形最多只能与四个方向的街区相邻。
'
' 构造方式：
'     * 以「两类别共享代谢物频次」为边权，先用 Kruskal 取最大权生成树；
'     * 再按边权降序贪心加入剩余的边，每条候选边都要通过三重校验；
'     * 最后按到「测地中心」的拓扑距离把类别放在同心圆环上，
'       得到一份无交叉的初始布局，作为后续 floor-planning 的参照位置。
'
' 三重校验的实现依据：
'     * 平面性 —— 以「初始布局 + 只接受不与已有骨架边相交的候选边」作为
'                 平面嵌入的构造性证据（witness），比通用平面性判定更稳且更快；
'     * 弦无 + 边不相交环 —— 只有当两个端点属于不同的「2-边连通分量」时才接受。
'                 此时两点之间的唯一路径全部由桥组成，新边与桥路径构成一个
'                 诱导环（环上不可能存在弦，因为桥不可能落在任何环上），
'                 且该环与已有环不共享任何边；
'     * 度数 <= 4 —— 直接检查两个端点的当前度数。
' ============================================================================

Imports System.Drawing
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports Metabopolis.Model

Namespace Skeleton

    ''' <summary>
    ''' 骨架中的一条连接。
    ''' </summary>
    Public Class SkeletonEdge

        ''' <summary>端点类别编号（无向）。</summary>
        Public Property Source As String

        ''' <summary>端点类别编号（无向）。</summary>
        Public Property Target As String

        ''' <summary>边权：两个类别共享的代谢物数量。</summary>
        Public Property Weight As Integer

        ''' <summary>是否为最大权生成树的树边。</summary>
        Public Property IsTreeEdge As Boolean

        ''' <summary>两个类别共享的代谢物编号。</summary>
        Public Property SharedMetabolites As String()

        ''' <summary>无向边编号，保证 (a,b) 与 (b,a) 一致。</summary>
        Public ReadOnly Property Id As String
            Get
                If String.CompareOrdinal(Source, Target) <= 0 Then
                    Return $"{Source}|{Target}"
                Else
                    Return $"{Target}|{Source}"
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Id} w={Weight}{If(IsTreeEdge, " (tree)", "")}"
        End Function

    End Class

    ''' <summary>
    ''' 骨架构建结果：满足平面/弦无/度&lt;=4 的连接图 + 无交叉初始布局。
    ''' </summary>
    Public Class SkeletonResult

        ''' <summary>全部类别编号。</summary>
        Public Property Nodes As String()

        ''' <summary>全部骨架边。</summary>
        Public Property Edges As SkeletonEdge()

        ''' <summary>
        ''' 初始布局坐标（相对坐标，单位无实际意义）。
        ''' 后续 floor-planning 用相邻街区连线的法向方向来维持相对位置（CH3）。
        ''' </summary>
        Public Property Positions As Dictionary(Of String, PointF)

        ''' <summary>每个连通分量的类别编号（用于分区放置与调试）。</summary>
        Public Property Components As String()()

        Private _adjacency As Dictionary(Of String, List(Of String))
        Private _degree As Dictionary(Of String, Integer)

        ''' <summary>查询一个类别的全部邻居。</summary>
        Public Function Neighbors(categoryId As String) As String()
            If _adjacency Is Nothing Then
                _adjacency = New Dictionary(Of String, List(Of String))(StringComparer.Ordinal)

                For Each edge As SkeletonEdge In Edges.SafeQuery
                    Append(_adjacency, edge.Source, edge.Target)
                    Append(_adjacency, edge.Target, edge.Source)
                Next
            End If

            Dim bucket As List(Of String) = Nothing

            If Not String.IsNullOrEmpty(categoryId) AndAlso _adjacency.TryGetValue(categoryId, bucket) Then
                Return bucket.ToArray
            Else
                Return New String() {}
            End If
        End Function

        ''' <summary>查询一个类别的骨架度数。</summary>
        Public Function Degree(categoryId As String) As Integer
            If _degree Is Nothing Then
                _degree = New Dictionary(Of String, Integer)(StringComparer.Ordinal)

                For Each edge As SkeletonEdge In Edges.SafeQuery
                    Bump(_degree, edge.Source)
                    Bump(_degree, edge.Target)
                Next
            End If

            Dim value As Integer = 0
            _degree.TryGetValue(If(categoryId, ""), value)

            Return value
        End Function

        ''' <summary>两个类别之间是否有骨架边。</summary>
        Public Function HasEdge(a As String, b As String) As Boolean
            For Each edge As SkeletonEdge In Edges.SafeQuery
                If (String.Equals(edge.Source, a, StringComparison.Ordinal) AndAlso String.Equals(edge.Target, b, StringComparison.Ordinal)) OrElse
                   (String.Equals(edge.Source, b, StringComparison.Ordinal) AndAlso String.Equals(edge.Target, a, StringComparison.Ordinal)) Then
                    Return True
                End If
            Next

            Return False
        End Function

        ''' <summary>查询初始布局坐标。</summary>
        Public Function PositionOf(categoryId As String) As PointF
            Dim hit As PointF

            If Not String.IsNullOrEmpty(categoryId) AndAlso Positions IsNot Nothing AndAlso Positions.TryGetValue(categoryId, hit) Then
                Return hit
            Else
                Return New PointF(0, 0)
            End If
        End Function

        Private Shared Sub Append(table As Dictionary(Of String, List(Of String)), key As String, value As String)
            Dim bucket As List(Of String) = Nothing

            If Not table.TryGetValue(key, bucket) Then
                bucket = New List(Of String)()
                table.Add(key, bucket)
            End If

            bucket.Add(value)
        End Sub

        Private Shared Sub Bump(table As Dictionary(Of String, Integer), key As String)
            Dim value As Integer = 0
            table.TryGetValue(key, value)
            table(key) = value + 1
        End Sub

        ''' <summary>生成统计摘要。</summary>
        Public Function Statistics() As String
            Dim sb As New StringBuilder()
            Dim treeEdges As Integer = Edges.SafeQuery.Count(Function(e) e.IsTreeEdge)
            Dim maxDegree As Integer = 0

            For Each node As String In Nodes.SafeQuery
                maxDegree = Math.Max(maxDegree, Degree(node))
            Next

            sb.AppendLine($"categories   : {If(Nodes Is Nothing, 0, Nodes.Length)}")
            sb.AppendLine($"components   : {If(Components Is Nothing, 0, Components.Length)}")
            sb.AppendLine($"skeleton edg : {If(Edges Is Nothing, 0, Edges.Length)} (tree {treeEdges})")
            sb.AppendLine($"max degree   : {maxDegree}")

            Return sb.ToString()
        End Function

        Public Overrides Function ToString() As String
            Return $"skeleton: {If(Nodes Is Nothing, 0, Nodes.Length)} nodes / {If(Edges Is Nothing, 0, Edges.Length)} edges"
        End Function

    End Class

    ''' <summary>
    ''' 骨架构建器。
    ''' </summary>
    Public Class GraphSkeletonBuilder

        ''' <summary>度数上限（矩形只有四条边可以贴合相邻街区）。</summary>
        Public Property MaxDegree As Integer = 4

        ''' <summary>同心圆环之间的半径步长。</summary>
        public property RingStep As Double = 120

        ''' <summary>
        ''' 构建骨架。
        ''' </summary>
        Public Function Build(network As MetabolicNetwork) As SkeletonResult
            If network Is Nothing Then
                Throw New ArgumentNullException(NameOf(network))
            End If

            Dim nodes As String() = network.Categories.SafeQuery.Select(Function(c) c.Id).ToArray()
            Dim candidates As CategoryLink() = network.CategoryLinks()

            If nodes.Length = 0 Then
                Return New SkeletonResult With {
                    .Nodes = New String() {},
                    .Edges = New SkeletonEdge() {},
                    .Positions = New Dictionary(Of String, PointF)(StringComparer.Ordinal),
                    .Components = New String()() {}
                }
            End If

            ' 1. 连通分量划分（基于候选边）
            Dim groups As List(Of List(Of String)) = FindComponents(nodes, candidates)

            Dim accepted As New List(Of SkeletonEdge)()
            Dim treeEdgeIds As New HashSet(Of String)(StringComparer.Ordinal)
            Dim positions As New Dictionary(Of String, PointF)(StringComparer.Ordinal)

            Dim offsetX As Double = 0

            For Each group As List(Of String) In groups
                Dim subset As CategoryLink() = candidates _
                    .Where(Function(l) group.Contains(l.Source) AndAlso group.Contains(l.Target)) _
                    .ToArray

                ' 2. 生成树（带度数上限的 Kruskal）
                Dim tree As List(Of SkeletonEdge) = BuildSpanningTree(group, subset, treeEdgeIds)

                ' 3. 该分量的无交叉初始布局（径向树布局，树边必然可无交叉绘制）
                Dim localPositions As Dictionary(Of String, PointF) = RadialTreeLayout(group, tree)

                ' 4. 以真实坐标为参照执行贪心扩展（平面 / 弦无 / 度<=4）
                Dim extraEdges As List(Of SkeletonEdge) = GreedyExtend(group, subset, tree, treeEdgeIds, localPositions)

                For Each edge As SkeletonEdge In extraEdges
                    accepted.Add(edge)
                Next

                ' 6. 把该分量的坐标平移到互不重叠的区域
                Dim minX As Double = Double.MaxValue
                Dim maxX As Double = Double.MinValue
                Dim minY As Double = Double.MaxValue

                For Each node As String In group
                    Dim pt As PointF = localPositions(node)
                    minX = Math.Min(minX, pt.X)
                    maxX = Math.Max(maxX, pt.X)
                    minY = Math.Min(minY, pt.Y)
                Next

                For Each node As String In group
                    Dim pt As PointF = localPositions(node)
                    positions(node) = New PointF(CSng(pt.X - minX + offsetX), CSng(pt.Y - minY))
                Next

                offsetX += (maxX - minX) + RingStep * 2
            Next

            Return New SkeletonResult With {
                .Nodes = nodes,
                .Edges = accepted.ToArray,
                .Positions = positions,
                .Components = groups.Select(Function(g) g.ToArray).ToArray
            }
        End Function

        ''' <summary>
        ''' 在已有坐标的前提下执行贪心扩展：只有当候选边不与任何已接受的边相交时接受。
        ''' </summary>
        Private Function GreedyExtend(group As List(Of String),
                                      subset As CategoryLink(),
                                      tree As List(Of SkeletonEdge),
                                      treeEdgeIds As HashSet(Of String),
                                      positions As Dictionary(Of String, PointF)) As List(Of SkeletonEdge)

            Dim adjacency As New Dictionary(Of String, List(Of String))(StringComparer.Ordinal)
            Dim degree As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For Each node As String In group
                adjacency(node) = New List(Of String)()
                degree(node) = 0
            Next

            For Each edge As SkeletonEdge In tree
                Link(adjacency, degree, edge)
            Next

            Dim acceptedTree As New List(Of SkeletonEdge)(tree)
            Dim acceptedExtra As New List(Of SkeletonEdge)()
            Dim occupied As New HashSet(Of String)(treeEdgeIds, StringComparer.Ordinal)

            For Each link_ As CategoryLink In subset
                Dim edge As New SkeletonEdge With {
                    .Source = link_.Source,
                    .Target = link_.Target,
                    .Weight = link_.Weight,
                    .SharedMetabolites = link_.SharedMetabolites,
                    .IsTreeEdge = False
                }

                If occupied.Contains(edge.Id) Then
                    Continue For
                End If

                If degree(edge.Source) >= MaxDegree OrElse degree(edge.Target) >= MaxDegree Then
                    Continue For
                End If

                If Not IsDifferentTwoEdgeComponent(group, adjacency, edge.Source, edge.Target) Then
                    Continue For
                End If

                If CrossesAny(acceptedTree, acceptedExtra, positions, edge) Then
                    Continue For
                End If

                Link(adjacency, degree, edge)
                acceptedExtra.Add(edge)
                occupied.Add(edge.Id)
            Next

            Return acceptedExtra
        End Function

        Private Shared Function CrossesAny(tree As List(Of SkeletonEdge),
                                           extra As List(Of SkeletonEdge),
                                           positions As Dictionary(Of String, PointF),
                                           candidate As SkeletonEdge) As Boolean

            Dim a1 As PointF = positions(candidate.Source)
            Dim a2 As PointF = positions(candidate.Target)

            For Each edge As SkeletonEdge In tree.Concat(extra)
                If String.Equals(edge.Source, candidate.Source, StringComparison.Ordinal) OrElse
                   String.Equals(edge.Source, candidate.Target, StringComparison.Ordinal) OrElse
                   String.Equals(edge.Target, candidate.Source, StringComparison.Ordinal) OrElse
                   String.Equals(edge.Target, candidate.Target, StringComparison.Ordinal) Then
                    Continue For
                End If

                Dim b1 As PointF = positions(edge.Source)
                Dim b2 As PointF = positions(edge.Target)

                If ProperlyCross(a1, a2, b1, b2) Then
                    Return True
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' 带度数上限的最大权生成树（Kruskal）。
        ''' </summary>
        Private Function BuildSpanningTree(group As List(Of String),
                                           subset As CategoryLink(),
                                           treeEdgeIds As HashSet(Of String)) As List(Of SkeletonEdge)

            Dim set_ As New DisjointSet(Of String)(StringComparer.Ordinal)
            Dim degree As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For Each node As String In group
                set_.Add(node)
                degree(node) = 0
            Next

            Dim result As New List(Of SkeletonEdge)()

            For Each link_ As CategoryLink In subset.OrderByDescending(Function(l) l.Weight)
                If degree(link_.Source) >= MaxDegree OrElse degree(link_.Target) >= MaxDegree Then
                    Continue For
                End If

                If set_.Union(link_.Source, link_.Target) Then
                    Dim edge As New SkeletonEdge With {
                        .Source = link_.Source,
                        .Target = link_.Target,
                        .Weight = link_.Weight,
                        .SharedMetabolites = link_.SharedMetabolites,
                        .IsTreeEdge = True
                    }

                    result.Add(edge)
                    treeEdgeIds.Add(edge.Id)
                    degree(link_.Source) += 1
                    degree(link_.Target) += 1
                End If
            Next

            ' 度数上限可能导致生成森林：用剩余边按权重补齐连通性（此时允许临时超过度数上限）
            Dim remaining As CategoryLink() = subset _
                .Where(Function(l) Not treeEdgeIds.Contains(New SkeletonEdge With {.Source = l.Source, .Target = l.Target}.Id)) _
                .OrderByDescending(Function(l) l.Weight) _
                .ToArray

            For Each link_ As CategoryLink In remaining
                If set_.Union(link_.Source, link_.Target) Then
                    Dim edge As New SkeletonEdge With {
                        .Source = link_.Source,
                        .Target = link_.Target,
                        .Weight = link_.Weight,
                        .SharedMetabolites = link_.SharedMetabolites,
                        .IsTreeEdge = True
                    }

                    result.Add(edge)
                    treeEdgeIds.Add(edge.Id)
                    degree(link_.Source) += 1
                    degree(link_.Target) += 1
                End If
            Next

            Return result
        End Function

        ''' <summary>
        ''' 径向树布局：每个子树占据互不重叠的角扇区，因此树边必然无交叉。
        ''' </summary>
        Private Function RadialTreeLayout(group As List(Of String),
                                          tree As List(Of SkeletonEdge)) As Dictionary(Of String, PointF)

            Dim positions As New Dictionary(Of String, PointF)(StringComparer.Ordinal)

            If group.Count = 0 Then
                Return positions
            End If

            If group.Count = 1 Then
                positions(group(0)) = New PointF(0, 0)
                Return positions
            End If

            Dim adjacency As New Dictionary(Of String, List(Of String))(StringComparer.Ordinal)

            For Each node As String In group
                adjacency(node) = New List(Of String)()
            Next

            For Each edge As SkeletonEdge In tree
                adjacency(edge.Source).Add(edge.Target)
                adjacency(edge.Target).Add(edge.Source)
            Next

            ' 树可能仍不连通（理论上不会），按分量分别布局
            Dim visited As New HashSet(Of String)(StringComparer.Ordinal)
            Dim offsetX As Double = 0

            For Each seed As String In group
                If visited.Contains(seed) Then
                    Continue For
                End If

                Dim members As New List(Of String)()
                Dim stack As New Stack(Of String)()
                stack.Push(seed)
                visited.Add(seed)

                While stack.Count > 0
                    Dim u As String = stack.Pop()
                    members.Add(u)

                    For Each v As String In adjacency(u)
                        If visited.Add(v) Then
                            stack.Push(v)
                        End If
                    Next
                End While

                Dim layout As Dictionary(Of String, PointF) = RadialComponentLayout(seed, members, adjacency)
                Dim minX As Double = Double.MaxValue

                For Each node As String In members
                    minX = Math.Min(minX, layout(node).X)
                Next

                For Each node As String In members
                    Dim pt As PointF = layout(node)
                    positions(node) = New PointF(CSng(pt.X - minX + offsetX), pt.Y)
                Next

                Dim maxX As Double = Double.MinValue

                For Each node As String In members
                    maxX = Math.Max(maxX, layout(node).X - minX)
                Next

                offsetX += maxX + RingStep * 2
            Next

            Return positions
        End Function

        Private Function RadialComponentLayout(root As String,
                                               members As List(Of String),
                                               adjacency As Dictionary(Of String, List(Of String))) As Dictionary(Of String, PointF)

            Dim positions As New Dictionary(Of String, PointF)(StringComparer.Ordinal)
            Dim parent As New Dictionary(Of String, String)(StringComparer.Ordinal)
            Dim order As New List(Of String)()
            Dim queue As New Queue(Of String)()

            queue.Enqueue(root)
            parent(root) = Nothing
            order.Add(root)

            While queue.Count > 0
                Dim u As String = queue.Dequeue()

                For Each v As String In adjacency(u)
                    If String.Equals(v, parent(u), StringComparison.Ordinal) Then
                        Continue For
                    End If

                    If parent.ContainsKey(v) Then
                        Continue For
                    End If

                    parent(v) = u
                    order.Add(v)
                    queue.Enqueue(v)
                Next
            End While

            ' 以「子树叶子数」作为角扇区权重，保证子树之间不互相侵入
            Dim weight As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For i As Integer = order.Count - 1 To 0 Step -1
                Dim u As String = order(i)
                Dim children As Integer = 0
                Dim sum As Integer = 0

                For Each v As String In adjacency(u)
                    If String.Equals(parent(u), v, StringComparison.Ordinal) OrElse Not parent.ContainsKey(v) Then
                        Continue For
                    End If

                    If String.Equals(parent(v), u, StringComparison.Ordinal) Then
                        children += 1
                        sum += weight(v)
                    End If
                Next

                If children = 0 Then
                    weight(u) = 1
                Else
                    weight(u) = Math.Max(1, sum)
                End If
            Next

            ' 深度决定半径
            Dim depth As New Dictionary(Of String, Integer)(StringComparer.Ordinal)
            depth(root) = 0

            For i As Integer = 1 To order.Count - 1
                Dim u As String = order(i)
                depth(u) = depth(parent(u)) + 1
            Next

            positions(root) = New PointF(0, 0)

            Dim angleStart As New Dictionary(Of String, Double)(StringComparer.Ordinal)
            Dim angleEnd As New Dictionary(Of String, Double)(StringComparer.Ordinal)
            angleStart(root) = 0.0
            angleEnd(root) = Math.PI * 2

            For i As Integer = 0 To order.Count - 1
                Dim u As String = order(i)
                Dim span As Double = angleEnd(u) - angleStart(u)
                Dim childList As New List(Of String)()

                For Each v As String In adjacency(u)
                    If String.Equals(parent(v), u, StringComparison.Ordinal) Then
                        childList.Add(v)
                    End If
                Next

                If childList.Count = 0 Then
                    Continue For
                End If

                Dim total As Double = 0

                For Each v As String In childList
                    total += weight(v)
                Next

                Dim cursor As Double = angleStart(u)

                For Each v As String In childList
                    Dim part As Double = span * weight(v) / total
                    angleStart(v) = cursor
                    angleEnd(v) = cursor + part
                    cursor += part

                    Dim mid As Double = (angleStart(v) + angleEnd(v)) / 2.0
                    Dim radius As Double = depth(v) * RingStep
                    positions(v) = New PointF(CSng(Math.Cos(mid) * radius), CSng(Math.Sin(mid) * radius))
                Next
            Next

            Return positions
        End Function

        ''' <summary>按候选边划分连通分量。</summary>
        Private Function FindComponents(nodes As String(), candidates As CategoryLink()) As List(Of List(Of String))
            Dim set_ As New DisjointSet(Of String)(StringComparer.Ordinal)

            For Each node As String In nodes
                set_.Add(node)
            Next

            For Each link_ As CategoryLink In candidates
                set_.Union(link_.Source, link_.Target)
            Next

            Dim table As New Dictionary(Of String, List(Of String))(StringComparer.Ordinal)

            For Each node As String In nodes
                Dim key As String = set_.Find(node)
                Dim bucket As List(Of String) = Nothing

                If Not table.TryGetValue(key, bucket) Then
                    bucket = New List(Of String)()
                    table.Add(key, bucket)
                End If

                bucket.Add(node)
            Next

            Return table.Values _
                .OrderByDescending(Function(g) g.Count) _
                .Select(Function(g) g.OrderBy(Function(s) s, StringComparer.Ordinal).ToList) _
                .ToList
        End Function

        ''' <summary>
        ''' 判断两个端点是否属于不同的 2-边连通分量。
        ''' </summary>
        ''' <remarks>
        ''' 若二者同属一个 2-边连通分量，则它们之间存在两条边不相交的路径，
        ''' 新增边会与已有环共享边，破坏「边不相交环」与「弦无」性质。
        ''' </remarks>
        Private Shared Function IsDifferentTwoEdgeComponent(group As List(Of String),
                                                            adjacency As Dictionary(Of String, List(Of String)),
                                                            u As String,
                                                            v As String) As Boolean

            Dim componentOf As Dictionary(Of String, Integer) = TwoEdgeComponents(group, adjacency)
            Return componentOf(u) <> componentOf(v)
        End Function

        ''' <summary>
        ''' 计算 2-边连通分量：先找桥（Tarjan low-link），再在去掉桥的子图上做连通分量。
        ''' </summary>
        Private Shared Function TwoEdgeComponents(group As List(Of String),
                                                  adjacency As Dictionary(Of String, List(Of String))) As Dictionary(Of String, Integer)

            Dim disc As New Dictionary(Of String, Integer)(StringComparer.Ordinal)
            Dim low As New Dictionary(Of String, Integer)(StringComparer.Ordinal)
            Dim bridges As New HashSet(Of String)(StringComparer.Ordinal)
            Dim timer As Integer = 0

            For Each node As String In group
                disc(node) = -1
            Next

            For Each node As String In group
                If disc(node) < 0 Then
                    DfsBridge(node, Nothing, adjacency, disc, low, bridges, timer)
                End If
            Next

            ' 去掉桥之后做连通分量
            Dim componentOf As New Dictionary(Of String, Integer)(StringComparer.Ordinal)
            Dim current As Integer = 0

            For Each node As String In group
                If componentOf.ContainsKey(node) Then
                    Continue For
                End If

                Dim stack As New Stack(Of String)()
                stack.Push(node)
                componentOf(node) = current

                While stack.Count > 0
                    Dim u As String = stack.Pop()

                    For Each v As String In adjacency(u)
                        If componentOf.ContainsKey(v) Then
                            Continue For
                        End If

                        If bridges.Contains(BridgeId(u, v)) Then
                            Continue For
                        End If

                        componentOf(v) = current
                        stack.Push(v)
                    End While
                End While

                current += 1
            Next

            Return componentOf
        End Function

        Private Shared Sub DfsBridge(u As String,
                                     parent As String,
                                     adjacency As Dictionary(Of String, List(Of String)),
                                     disc As Dictionary(Of String, Integer),
                                     low As Dictionary(Of String, Integer),
                                     bridges As HashSet(Of String),
                                     ByRef timer As Integer)

            disc(u) = timer
            low(u) = timer
            timer += 1

            For Each v As String In adjacency(u)
                If String.Equals(v, parent, StringComparison.Ordinal) Then
                    Continue For
                End If

                If disc(v) < 0 Then
                    DfsBridge(v, u, adjacency, disc, low, bridges, timer)
                    low(u) = Math.Min(low(u), low(v))

                    If low(v) > disc(u) Then
                        bridges.Add(BridgeId(u, v))
                    End If
                Else
                    low(u) = Math.Min(low(u), disc(v))
                End If
            Next
        End Sub

        Private Shared Function BridgeId(u As String, v As String) As String
            If String.CompareOrdinal(u, v) <= 0 Then
                Return $"{u}|{v}"
            Else
                Return $"{v}|{u}"
            End If
        End Function

        Private Shared Sub Link(adjacency As Dictionary(Of String, List(Of String)),
                                degree As Dictionary(Of String, Integer),
                                edge As SkeletonEdge)

            adjacency(edge.Source).Add(edge.Target)
            adjacency(edge.Target).Add(edge.Source)
            degree(edge.Source) += 1
            degree(edge.Target) += 1
        End Sub

        ''' <summary>两条线段是否真正相交（共享端点或共线不视为相交）。</summary>
        Public Shared Function ProperlyCross(a1 As PointF, a2 As PointF, b1 As PointF, b2 As PointF) As Boolean
            Dim d1 As Double = Cross(b1, b2, a1)
            Dim d2 As Double = Cross(b1, b2, a2)
            Dim d3 As Double = Cross(a1, a2, b1)
            Dim d4 As Double = Cross(a1, a2, b2)

            Return ((d1 > 0 AndAlso d2 < 0) OrElse (d1 < 0 AndAlso d2 > 0)) AndAlso
                   ((d3 > 0 AndAlso d4 < 0) OrElse (d3 < 0 AndAlso d4 > 0))
        End Function

        Private Shared Function Cross(o As PointF, a As PointF, b As PointF) As Double
            Return (CDbl(a.X) - o.X) * (CDbl(b.Y) - o.Y) - (CDbl(a.Y) - o.Y) * (CDbl(b.X) - o.X)
        End Function

    End Class

    ''' <summary>
    ''' 并查集。
    ''' </summary>
    Friend Class DisjointSet(Of T)

        Private ReadOnly parent As Dictionary(Of T, T)
        Private ReadOnly rank As Dictionary(Of T, Integer)
        Private ReadOnly comparer As IEqualityComparer(Of T)

        Public Sub New(comparer As IEqualityComparer(Of T))
            Me.comparer = comparer
            Me.parent = New Dictionary(Of T, T)(comparer)
            Me.rank = New Dictionary(Of T, Integer)(comparer)
        End Sub

        Public Sub Add(item As T)
            If Not parent.ContainsKey(item) Then
                parent(item) = item
                rank(item) = 0
            End If
        End Sub

        Public Function Find(item As T) As T
            Add(item)

            Dim root As T = item

            While Not comparer.Equals(parent(root), root)
                root = parent(root)
            End While

            ' 路径压缩
            Dim cursor As T = item

            While Not comparer.Equals(parent(cursor), root)
                Dim next_ As T = parent(cursor)
                parent(cursor) = root
                cursor = next_
            End While

            Return root
        End Function

        Public Function Union(a As T, b As T) As Boolean
            Dim ra As T = Find(a)
            Dim rb As T = Find(b)

            If comparer.Equals(ra, rb) Then
                Return False
            End If

            If rank(ra) < rank(rb) Then
                parent(ra) = rb
            ElseIf rank(ra) > rank(rb) Then
                parent(rb) = ra
            Else
                parent(rb) = ra
                rank(ra) += 1
            End If

            Return True
        End Function

    End Class

End Namespace

#End Region
