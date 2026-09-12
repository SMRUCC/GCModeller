#Region "Metabopolis: minimum cost maximum flow"

' ============================================================================
' 最小费用最大流（successive shortest path）
' ----------------------------------------------------------------------------
' 论文的局部流网络 G_M 与全局流网络 G_N 都要求解「最小费用最大流」：
'   * G_M 用来为「街区边界上的枢纽代谢物 → 街区内部反应」生成车道，
'     边权惩罚长度与和已有正交布局的交叉；
'   * G_N 用来为「跨街区的同一代谢物副本」做块间边路由，并通过容量分配
'     控制单条道路上的边密度。
'
' 基础库中的 GraphTheory.EMD.MinCostFlow 是 Friend（程序集内部）且语义面向 EMD，
' 无法跨程序集复用，因此这里按论文引用的 successive shortest path 算法自行实现：
' 每次用 SPFA 求一条费用最小的增广路，沿瓶颈容量增广，直到无增广路为止。
' ============================================================================

Imports System.Text
Imports Microsoft.VisualBasic.Linq

Namespace Routing

    ''' <summary>
    ''' 流网络中的一条有向弧。
    ''' </summary>
    Public Class FlowArc

        Public Property [To] As Integer
        Public Property Capacity As Integer
        Public Property Flow As Integer
        Public Property Cost As Double

        ''' <summary>反向弧在弧表中的下标。</summary>
        Public Property Reverse As Integer

        ''' <summary>残余容量。</summary>
        Public ReadOnly Property Residual As Integer
            Get
                Return Capacity - Flow
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"-> {[To]} cap={Capacity} flow={Flow} cost={Cost:0.###}"
        End Function

    End Class

    ''' <summary>
    ''' 最小费用流的求解结果。
    ''' </summary>
    Public Class FlowSolution

        Friend Property Arcs As List(Of FlowArc)
        Friend Property Heads As List(Of List(Of Integer))

        ''' <summary>源点。</summary>
        Public Property Source As Integer

        ''' <summary>汇点。</summary>
        Public Property Sink As Integer

        ''' <summary>最大流。</summary>
        Public Property Flow As Integer

        ''' <summary>最小费用。</summary>
        Public Property Cost As Double

        ''' <summary>查询某条弧上的流量。</summary>
        Public Function FlowOn(arcIndex As Integer) As Integer
            Return Arcs(arcIndex).Flow
        End Function

        ''' <summary>
        ''' 提取全部增广路径（每条路径 = 节点序列）。流会被内部复制，不影响原结果。
        ''' </summary>
        Public Function ExtractPaths() As List(Of List(Of Integer))
            Dim remaining As New Dictionary(Of Integer, Integer)()
            Dim result As New List(Of List(Of Integer))()

            For i As Integer = 0 To Arcs.Count - 1
                If Arcs(i).Flow > 0 Then
                    remaining(i) = Arcs(i).Flow
                End If
            Next

            ' 单条弧上的流量可能大于 1，需要拆成多条单位路径
            Do
                Dim path As List(Of Integer) = FindResidualPath(remaining)

                If path Is Nothing Then
                    Exit Do
                End If

                Dim bottleneck As Integer = Integer.MaxValue

                For i As Integer = 0 To path.Count - 1
                    bottleneck = Math.Min(bottleneck, remaining(path(i)))
                Next

                For Each arcIndex As Integer In path
                    remaining(arcIndex) -= bottleneck

                    If remaining(arcIndex) <= 0 Then
                        remaining.Remove(arcIndex)
                    End If
                Next

                ' 记录路径（含中间节点）；一条路径上可能承载多条单位流，
                ' 这里按单位流展开，方便调用方「一条流 = 一条车道」地绘制
                Dim nodes As New List(Of Integer)()
                nodes.Add(Source)

                For Each arcIndex As Integer In path
                    nodes.Add(Arcs(arcIndex).[To])
                Next

                For unit As Integer = 1 To bottleneck
                    result.Add(New List(Of Integer)(nodes))
                Next
            Loop

            Return result
        End Function

        Private Function FindResidualPath(remaining As Dictionary(Of Integer, Integer)) As List(Of Integer)
            ' 在「仍有剩余流量」的弧上做 BFS，找一条 source -> sink 的路径
            Dim predecessorArc As New Dictionary(Of Integer, Integer)()
            Dim visited As New HashSet(Of Integer)()
            Dim queue As New Queue(Of Integer)()

            queue.Enqueue(Source)
            visited.Add(Source)

            While queue.Count > 0
                Dim u As Integer = queue.Dequeue()

                If u = Sink Then
                    Exit While
                End If

                For Each arcIndex As Integer In Heads(u)
                    If Not remaining.ContainsKey(arcIndex) Then
                        Continue For
                    End If

                    Dim v As Integer = Arcs(arcIndex).[To]

                    If visited.Contains(v) Then
                        Continue For
                    End If

                    visited.Add(v)
                    predecessorArc(v) = arcIndex
                    queue.Enqueue(v)
                Next
            End While

            If Not predecessorArc.ContainsKey(Sink) Then
                Return Nothing
            End If

            Dim reversed As New List(Of Integer)()
            Dim cursor As Integer = Sink

            While cursor <> Source
                Dim arcIndex As Integer = predecessorArc(cursor)
                reversed.Add(arcIndex)
                cursor = Arcs(arcIndex ^ 1).[To]
            End While

            reversed.Reverse()
            Return reversed
        End Function

        Public Overrides Function ToString() As String
            Return $"min-cost flow: {Flow} units, cost={Cost:0.###}"
        End Function

    End Class

    ''' <summary>
    ''' 最小费用最大流求解器（successive shortest path + SPFA）。
    ''' </summary>
    Public Class MinCostFlow

        Private ReadOnly arcs As New List(Of FlowArc)()
        Private ReadOnly heads As New List(Of List(Of Integer))()

        ''' <summary>节点数量。</summary>
        Public ReadOnly Property NodeCount As Integer
            Get
                Return heads.Count
            End Get
        End Property

        ''' <summary>弧数量。</summary>
        Public ReadOnly Property ArcCount As Integer
            Get
                Return arcs.Count
            End Get
        End Property

        ''' <summary>新增一个节点，返回其编号。</summary>
        Public Function AddNode() As Integer
            heads.Add(New List(Of Integer)())
            Return heads.Count - 1
        End Function

        ''' <summary>批量新增节点。</summary>
        Public Sub AddNodes(count As Integer)
            For i As Integer = 1 To count
                AddNode()
            Next
        End Sub

        ''' <summary>
        ''' 新增一条有向边（同时建立反向弧），返回正向弧的下标。
        ''' </summary>
        Public Function AddEdge(u As Integer, v As Integer, capacity As Integer, cost As Double) As Integer
            Dim forward As New FlowArc With {
                .[To] = v,
                .Capacity = Math.Max(0, capacity),
                .Flow = 0,
                .Cost = cost
            }
            Dim backward As New FlowArc With {
                .[To] = u,
                .Capacity = 0,
                .Flow = 0,
                .Cost = -cost
            }

            Dim index As Integer = arcs.Count
            forward.Reverse = index + 1
            backward.Reverse = index

            arcs.Add(forward)
            arcs.Add(backward)
            heads(u).Add(index)
            heads(v).Add(index + 1)

            Return index
        End Function

        ''' <summary>
        ''' 求解从 source 到 sink 的最小费用最大流。
        ''' </summary>
        Public Function Solve(source As Integer, sink As Integer, Optional maxFlow As Integer = Integer.MaxValue) As FlowSolution
            Dim totalFlow As Integer = 0
            Dim totalCost As Double = 0

            Do
                Dim distance As New Dictionary(Of Integer, Double)()
                Dim inQueue As New HashSet(Of Integer)()
                Dim predecessor As New Dictionary(Of Integer, Integer)()
                Dim queue As New Queue(Of Integer)()

                For i As Integer = 0 To heads.Count - 1
                    distance(i) = Double.PositiveInfinity
                Next

                distance(source) = 0
                queue.Enqueue(source)
                inQueue.Add(source)

                ' SPFA：允许负费用反向弧参与最短路
                While queue.Count > 0
                    Dim u As Integer = queue.Dequeue()
                    inQueue.Remove(u)

                    For Each arcIndex As Integer In heads(u)
                        Dim arc As FlowArc = arcs(arcIndex)

                        If arc.Residual <= 0 Then
                            Continue For
                        End If

                        Dim nd As Double = distance(u) + arc.Cost

                        If nd < distance(arc.[To]) - 1E-09 Then
                            distance(arc.[To]) = nd
                            predecessor(arc.[To]) = arcIndex

                            If inQueue.Add(arc.[To]) Then
                                queue.Enqueue(arc.[To])
                            End If
                        End If
                    Next
                End While

                If Not predecessor.ContainsKey(sink) Then
                    Exit Do
                End If

                ' 找瓶颈容量
                Dim bottleneck As Integer = maxFlow - totalFlow
                Dim cursor As Integer = sink

                While cursor <> source
                    Dim arcIndex As Integer = predecessor(cursor)
                    bottleneck = Math.Min(bottleneck, arcs(arcIndex).Residual)
                    cursor = arcs(arcIndex ^ 1).[To]
                End While

                If bottleneck <= 0 Then
                    Exit Do
                End If

                ' 增广
                cursor = sink

                While cursor <> source
                    Dim arcIndex As Integer = predecessor(cursor)
                    arcs(arcIndex).Flow += bottleneck
                    arcs(arcIndex ^ 1).Flow -= bottleneck
                    totalCost += bottleneck * arcs(arcIndex).Cost
                    cursor = arcs(arcIndex ^ 1).[To]
                End While

                totalFlow += bottleneck
            Loop While totalFlow < maxFlow

            Return New FlowSolution With {
                .Arcs = arcs,
                .Heads = heads,
                .Source = source,
                .Sink = sink,
                .Flow = totalFlow,
                .Cost = totalCost
            }
        End Function

        Public Overrides Function ToString() As String
            Return $"network: {heads.Count} nodes / {arcs.Count \ 2} edges"
        End Function

    End Class

    ''' <summary>
    ''' 把「供应点 → 需求点」的流量需求打包成一次最小费用最大流求解。
    ''' </summary>
    ''' <remarks>
    ''' 两个路由阶段都遵循同一模式：若干供应点各有 supply 单位流量，
    ''' 需求点各自需要 demand 单位；用超级源/超级汇把多源多汇转成单源单汇。
    ''' </remarks>
    Public Class FlowDemand

        ''' <summary>供应点编号。</summary>
        Public Property Node As Integer

        ''' <summary>供应量。</summary>
        Public Property Amount As Integer = 1

        Public Overrides Function ToString() As String
            Return $"node {Node} supply {Amount}"
        End Function

    End Class

End Namespace

#End Region
