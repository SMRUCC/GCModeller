' ============================================================
' BayesianNetwork.vb - 贝叶斯网络核心数据结构
' ============================================================
' 实现 DAG（有向无环图）结构，支持：
'   - 节点（基因）管理
'   - 有向边（调控关系）管理
'   - 拓扑排序
'   - DAG 验证
'   - 白名单/黑名单约束
' ============================================================

Imports System.Text

Namespace Core

    ''' <summary>
    ''' 贝叶斯网络节点 —— 对应一个基因
    ''' </summary>
    Public Class BnNode

        ''' <summary>节点名称（基因名）</summary>
        Public Property Name As String = ""

        ''' <summary>节点索引</summary>
        Public Property Index As Integer = -1

        ''' <summary>父节点索引列表（上游调控基因）</summary>
        Public Property Parents As New List(Of Integer)()

        ''' <summary>子节点索引列表（下游靶基因）</summary>
        Public Property Children As New List(Of Integer)()

        ''' <summary>该节点的条件概率分布参数</summary>
        Public Property CPD As BnCPD = Nothing

        ''' <summary>节点层级（拓扑排序后）</summary>
        Public Property Level As Integer = 0

        Public Overrides Function ToString() As String
            Return String.Format("{0}(parents=[{1}])", Name, String.Join(",", Parents))
        End Function

    End Class

    ''' <summary>
    ''' 贝叶斯网络 —— DAG 结构
    ''' </summary>
    Public Class BayesianNetwork

        ' ==================== 核心数据 ====================

        ''' <summary>所有节点</summary>
        Public Property Nodes As New List(Of BnNode)()

        ''' <summary>节点名 → 索引 的映射</summary>
        Public Property NameToIndex As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        ''' <summary>邻接矩阵：adj(i,j)=True 表示 i→j 存在有向边</summary>
        Public Property Adjacency As Boolean(,)

        ''' <summary>白名单边（必须存在的调控关系，来自TF注释）</summary>
        Public Property Whitelist As New List(Of (FromIdx As Integer, ToIdx As Integer))()

        ''' <summary>黑名单边（禁止存在的边）</summary>
        Public Property Blacklist As New List(Of (FromIdx As Integer, ToIdx As Integer))()

        ''' <summary>拓扑排序结果（缓存）</summary>
        Private _topoOrder As Integer() = Nothing

        ''' <summary>拓扑排序是否需要重新计算</summary>
        Private _topoDirty As Boolean = True

        ' ==================== 构造 ====================

        Public Sub New()
        End Sub

        ''' <summary>根据基因名列表创建空网络</summary>
        Public Sub New(geneNames As String())
            For i = 0 To geneNames.Length - 1
                AddNode(geneNames(i))
            Next
        End Sub

        ' ==================== 节点操作 ====================

        ''' <summary>添加节点</summary>
        Public Function AddNode(name As String) As Integer
            If NameToIndex.ContainsKey(name) Then Return NameToIndex(name)
            Dim idx As Integer = Nodes.Count
            Dim node As New BnNode() With {.Name = name, .Index = idx}
            Nodes.Add(node)
            NameToIndex(name) = idx
            ResizeAdjacency()
            _topoDirty = True
            Return idx
        End Function

        ''' <summary>获取节点索引，不存在返回 -1</summary>
        Public Function GetNodeIndex(name As String) As Integer
            Dim idx As Integer
            If NameToIndex.TryGetValue(name, idx) Then Return idx
            Return -1
        End Function

        Friend blackEdges As New HashSet(Of (Integer, Integer))

        Public Function MakeBlackIndex() As BayesianNetwork
            blackEdges = New HashSet(Of (Integer, Integer))

            For Each bl In Blacklist
                Call blackEdges.Add(bl)
            Next

            Return Me
        End Function

        ' ==================== 边操作 ====================

        ''' <summary>添加有向边 fromIdx → toIdx</summary>
        Public Function AddEdge(fromIdx As Integer, toIdx As Integer) As Boolean
            If fromIdx = toIdx Then Return False
            If fromIdx < 0 OrElse fromIdx >= Nodes.Count Then Return False
            If toIdx < 0 OrElse toIdx >= Nodes.Count Then Return False
            If Adjacency(fromIdx, toIdx) Then Return False

            ' 检查黑名单
            If blackEdges.Contains((fromIdx, toIdx)) Then
                Return False
            End If

            ' 检查是否会形成环
            If WouldCreateCycle(fromIdx, toIdx) Then Return False

            Adjacency(fromIdx, toIdx) = True
            If Not Nodes(toIdx).Parents.Contains(fromIdx) Then
                Nodes(toIdx).Parents.Add(fromIdx)
            End If
            If Not Nodes(fromIdx).Children.Contains(toIdx) Then
                Nodes(fromIdx).Children.Add(toIdx)
            End If

            _topoDirty = True
            Return True
        End Function

        ''' <summary>删除有向边</summary>
        Public Sub RemoveEdge(fromIdx As Integer, toIdx As Integer)
            If fromIdx < 0 OrElse fromIdx >= Nodes.Count Then Return
            If toIdx < 0 OrElse toIdx >= Nodes.Count Then Return
            Adjacency(fromIdx, toIdx) = False
            Nodes(toIdx).Parents.Remove(fromIdx)
            Nodes(fromIdx).Children.Remove(toIdx)
            _topoDirty = True
        End Sub

        ''' <summary>检查边是否存在</summary>
        Public Function HasEdge(fromIdx As Integer, toIdx As Integer) As Boolean
            If fromIdx < 0 OrElse fromIdx >= Nodes.Count Then Return False
            If toIdx < 0 OrElse toIdx >= Nodes.Count Then Return False
            Return Adjacency(fromIdx, toIdx)
        End Function

        ''' <summary>添加白名单边</summary>
        Public Sub AddWhitelistEdge(fromIdx As Integer, toIdx As Integer)
            Whitelist.Add((fromIdx, toIdx))
            ' 白名单边也隐含禁止反向边
            Blacklist.Add((toIdx, fromIdx))
        End Sub

        ''' <summary>添加黑名单边</summary>
        Public Sub AddBlacklistEdge(fromIdx As Integer, toIdx As Integer)
            Blacklist.Add((fromIdx, toIdx))
        End Sub

        ' ==================== DAG 验证 ====================

        ''' <summary>检查添加边是否会形成环（DFS检测）</summary>
        Public Function WouldCreateCycle(fromIdx As Integer, toIdx As Integer) As Boolean
            ' 如果从 toIdx 出发能到达 fromIdx，则添加 fromIdx→toIdx 会形成环
            ' 使用 BFS 从 toIdx 搜索 fromIdx
            Dim visited As New HashSet(Of Integer)()
            Dim queue As New Queue(Of Integer)()
            queue.Enqueue(toIdx)
            visited.Add(toIdx)

            While queue.Count > 0
                Dim current As Integer = queue.Dequeue()
                If current = fromIdx Then Return True

                For j = 0 To Nodes.Count - 1
                    If Adjacency(current, j) AndAlso Not visited.Contains(j) Then
                        visited.Add(j)
                        queue.Enqueue(j)
                    End If
                Next
            End While

            Return False
        End Function

        ''' <summary>验证当前网络是否为 DAG</summary>
        Public Function IsDAG() As Boolean
            ' 使用 Kahn 算法检测环
            Dim inDegree As Integer() = New Integer(Nodes.Count - 1) {}
            For i = 0 To Nodes.Count - 1
                For j = 0 To Nodes.Count - 1
                    If Adjacency(i, j) Then inDegree(j) += 1
                Next
            Next

            Dim queue As New Queue(Of Integer)()
            For i = 0 To Nodes.Count - 1
                If inDegree(i) = 0 Then queue.Enqueue(i)
            Next

            Dim count As Integer = 0
            While queue.Count > 0
                Dim u As Integer = queue.Dequeue()
                count += 1
                For v = 0 To Nodes.Count - 1
                    If Adjacency(u, v) Then
                        inDegree(v) -= 1
                        If inDegree(v) = 0 Then queue.Enqueue(v)
                    End If
                Next
            End While

            Return count = Nodes.Count
        End Function

        ' ==================== 拓扑排序 ====================

        ''' <summary>获取拓扑排序（Kahn算法）</summary>
        Public Function TopologicalSort() As Integer()
            If Not _topoDirty AndAlso _topoOrder IsNot Nothing Then Return _topoOrder

            Dim n As Integer = Nodes.Count
            Dim inDegree As Integer() = New Integer(n - 1) {}
            For i = 0 To n - 1
                For j = 0 To n - 1
                    If Adjacency(i, j) Then inDegree(j) += 1
                Next
            Next

            Dim queue As New Queue(Of Integer)()
            For i = 0 To n - 1
                If inDegree(i) = 0 Then queue.Enqueue(i)
            Next

            Dim result As New List(Of Integer)()
            While queue.Count > 0
                Dim u As Integer = queue.Dequeue()
                result.Add(u)
                For v = 0 To n - 1
                    If Adjacency(u, v) Then
                        inDegree(v) -= 1
                        If inDegree(v) = 0 Then queue.Enqueue(v)
                    End If
                Next
            End While

            _topoOrder = result.ToArray()
            _topoDirty = False

            ' 设置层级
            For i = 0 To _topoOrder.Length - 1
                Nodes(_topoOrder(i)).Level = i
            Next

            Return _topoOrder
        End Function

        ' ==================== 辅助 ====================

        Private Sub ResizeAdjacency()
            Dim n As Integer = Nodes.Count
            Dim newAdj As Boolean(,) = New Boolean(n - 1, n - 1) {}
            If Adjacency IsNot Nothing Then
                Dim oldN As Integer = Math.Min(n, Adjacency.GetLength(0))
                For i = 0 To oldN - 1
                    For j = 0 To oldN - 1
                        newAdj(i, j) = Adjacency(i, j)
                    Next
                Next
            End If
            Adjacency = newAdj
        End Sub

        ''' <summary>获取网络的边列表</summary>
        Public Function GetEdges() As List(Of (FromIdx As Integer, ToIdx As Integer))
            Dim edges As New List(Of (Integer, Integer))()
            Dim n As Integer = Nodes.Count
            For i = 0 To n - 1
                For j = 0 To n - 1
                    If Adjacency(i, j) Then edges.Add((i, j))
                Next
            Next
            Return edges
        End Function

        ''' <summary>获取边数</summary>
        Public ReadOnly Property EdgeCount As Integer
            Get
                Dim count As Integer = 0
                Dim n As Integer = Nodes.Count
                For i = 0 To n - 1
                    For j = 0 To n - 1
                        If Adjacency(i, j) Then count += 1
                    Next
                Next
                Return count
            End Get
        End Property

        ''' <summary>深拷贝网络结构（不拷贝CPD）</summary>
        Public Function CloneStructure() As BayesianNetwork
            Dim net As New BayesianNetwork()
            For Each node In Nodes
                net.AddNode(node.Name)
            Next
            For Each edge In GetEdges()
                net.AddEdge(edge.FromIdx, edge.ToIdx)
            Next
            For Each wl In Whitelist
                net.Whitelist.Add(wl)
            Next
            For Each bl In Blacklist
                net.Blacklist.Add(bl)
            Next
            Return net
        End Function

        ''' <summary>输出网络摘要</summary>
        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.AppendLine(String.Format("BayesianNetwork: {0} nodes, {1} edges", Nodes.Count, EdgeCount))
            For Each node In Nodes
                If node.Parents.Count > 0 Then
                    Dim parentNames = node.Parents.Select(Function(p) Nodes(p).Name).ToArray()
                    sb.AppendLine(String.Format("  {0} ← [{1}]", node.Name, String.Join(", ", parentNames)))
                Else
                    sb.AppendLine(String.Format("  {0} (root)", node.Name))
                End If
            Next
            Return sb.ToString()
        End Function

    End Class

End Namespace
