#Region "Metabopolis: intra-block layout and lane generation"

' ============================================================================
' 块内布局与车道生成（论文第四节）
' ----------------------------------------------------------------------------
' 两步：
'   1. 层级正交布局 —— 用 TreeMap 把街区划分成若干「建筑块」，
'      在每个建筑块内对通路子网络调用 HOLA 做紧凑正交布局；
'      跨类别的重要代谢物（枢纽）被放到街区边界上作为对外接口。
'   2. 局部流网络 G_M —— 把「街区边界上的枢纽 → 街区内部反应」建模为
'      最小费用最大流（successive shortest path）：
'         * 源点   —— 超级源，向每个边界枢纽供水；
'         * 供给点 —— 边界上的枢纽代谢物；
'         * 需求点 —— 街区内部需要用到该枢纽的反应；
'         * 道路点 —— 街区内部按固定步长采样的网格。
'      边权按论文公式 w(e) = p|v_a − v_b| + q·Σ_h( |e·l_h| / (‖e‖‖l_h‖) + 1 )
'      惩罚长度与和已有正交布局的交叉（越接近 90° 惩罚越小）；
'      每条网格边容量有限，避免多条车道挤在同一条道路上。
' ============================================================================

Imports System.Drawing
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.Hola
Imports Microsoft.VisualBasic.Linq
Imports Metabopolis.Model
Imports Metabopolis.Preprocess
Imports SMRUCC.genomics.MetabolicModel

Namespace Routing

    ''' <summary>
    ''' 路由参数。
    ''' </summary>
    Public Class RoutingOptions

        ''' <summary>块内道路网格的采样步长（像素）。</summary>
        Public Property GridStep As Double = 26

        ''' <summary>每条网格道路允许通过的车道数上限（论文中的容量限制）。</summary>
        Public Property LaneCapacity As Integer = 2

        ''' <summary>单个建筑块的网格节点上限（超出则自动放大步长）。</summary>
        Public Property MaxGridNodes As Integer = 6000

        ''' <summary>车道生成的配对数量上限（性能保护）。</summary>
        Public Property MaxLanePairs As Integer = 4000

        ''' <summary>边权函数中的长度权重 p。</summary>
        Public Property LengthWeight As Double = 1.0

        ''' <summary>边权函数中的交叉惩罚权重 q。</summary>
        Public Property CrossingWeight As Double = 0.6

        ''' <summary>交叉惩罚时每个网格边参考的布局边数量上限（性能保护）。</summary>
        Public Property MaxCrossingSamples As Integer = 32

        ''' <summary>街区边界到内部建筑区域的收缩量（留给边界枢纽）。</summary>
        Public Property BoundaryInset As Double = 22

        ''' <summary>建筑块之间的道路宽度。</summary>
        Public Property BuildingGap As Double = 10

        ''' <summary>每个街区内最多放置的边界枢纽数量。</summary>
        Public Property MaxJunctionsPerBlock As Integer = 48

        Private _hola As HolaOptions

        ''' <summary>HOLA 正交布局参数。</summary>
        Public Property Hola As HolaOptions
            Get
                If _hola Is Nothing Then
                    _hola = New HolaOptions With {
                        .nodeGap = 12.0,
                        .desiredEdgeLength = 34.0,
                        .maxIterations = 160,
                        .routeGridSize = 8.0,
                        .nodeRadiusPadding = 4.0
                    }
                End If

                Return _hola
            End Get
            Set(value As HolaOptions)
                _hola = value
            End Set
        End Property

    End Class

    ''' <summary>
    ''' 单个街区的路由结果。
    ''' </summary>
    Public Class BlockRoutingResult

        ''' <summary>街区（类别）编号。</summary>
        Public Property CategoryId As String

        ''' <summary>街区内的建筑块。</summary>
        Public Property Buildings As BuildingBlock()

        ''' <summary>街区边界上的枢纽代谢物。</summary>
        Public Property Junctions As Junction()

        ''' <summary>块内车道。</summary>
        Public Property Routes As RoutePolyline()

        Public Overrides Function ToString() As String
            Return $"{CategoryId}: {If(Buildings Is Nothing, 0, Buildings.Length)} buildings / {If(Routes Is Nothing, 0, Routes.Length)} lanes"
        End Function

    End Class

    ''' <summary>
    ''' 块内布局与局部流网络路由。
    ''' </summary>
    Public Class IntraBlockRouter

        ''' <summary>车道配对（反应使用到的某个枢纽）。</summary>
        Private Class LanePair

            Public Property Reaction As String
            Public Property Hub As String

            Public Overrides Function ToString() As String
                Return $"{Hub} -> {Reaction}"
            End Function

        End Class

        ''' <summary>一段布局边（用于交叉惩罚）。</summary>
        Private Class LayoutEdge

            Public Property A As PointF
            Public Property B As PointF

        End Class

        Public Property Options As RoutingOptions

        Private ReadOnly preprocessed As PreprocessedNetwork
        Private ReadOnly blocks As Dictionary(Of String, Rect)
        Private ReadOnly network As MetabolicNetwork

        Public Sub New(preprocessed As PreprocessedNetwork,
                       blocks As Dictionary(Of String, Rect),
                       Optional options As RoutingOptions = Nothing)

            Me.preprocessed = preprocessed
            Me.blocks = blocks
            Me.network = preprocessed.Network
            Me.Options = If(options, New RoutingOptions())
        End Sub

        ''' <summary>对一个街区执行块内布局与车道生成。</summary>
        Public Function Route(categoryId As String) As BlockRoutingResult
            Dim block As Rect = blocks(categoryId)
            Dim graph As NetworkGraph = preprocessed.SubGraph(categoryId)
            Dim buildings As New List(Of BuildingBlock)()
            Dim junctions As New List(Of Junction)()

            If graph.vertex Is Nothing OrElse Not graph.vertex.Any Then
                Return New BlockRoutingResult With {
                    .CategoryId = categoryId,
                    .Buildings = New BuildingBlock() {},
                    .Junctions = New Junction() {},
                    .Routes = New RoutePolyline() {}
                }
            End If

            ' 1. 块内正交布局
            Dim positions As Dictionary(Of String, PointF) = LayoutBuildings(categoryId, block, graph, buildings)

            ' 2. 边界枢纽
            Dim junctionNodes As Dictionary(Of String, Junction) = PlaceJunctions(categoryId, block, junctions)

            ' 3. 局部流网络车道生成
            Dim routes As RoutePolyline() = GenerateLanes(categoryId, block, graph, positions, junctionNodes)

            Return New BlockRoutingResult With {
                .CategoryId = categoryId,
                .Buildings = buildings.ToArray,
                .Junctions = junctions.ToArray,
                .Routes = routes
            }
        End Function

        ''' <summary>
        ''' 用 TreeMap 划分建筑块，并在每个建筑块内对子网络做 HOLA 正交布局。
        ''' </summary>
        Private Function LayoutBuildings(categoryId As String,
                                         block As Rect,
                                         graph As NetworkGraph,
                                         buildings As List(Of BuildingBlock)) As Dictionary(Of String, PointF)

            Dim positions As New Dictionary(Of String, PointF)(StringComparer.Ordinal)
            Dim interior As Rect = block.Inflate(-Options.BoundaryInset)

            If interior.Width <= 8 OrElse interior.Height <= 8 Then
                interior = Rect.FromSize(block.X, block.Y, Math.Max(8, block.Width), Math.Max(8, block.Height))
            End If

            ' 子图的连通分量 —— 每个分量成为一个建筑块
            Dim components As List(Of List(Of String)) = ConnectedComponents(graph)

            Dim items As New List(Of TreeMapItem)()

            For i As Integer = 0 To components.Count - 1
                items.Add(New TreeMapItem With {
                    .Id = i.ToString(),
                    .Weight = components(i).Count
                })
            Next

            Dim cells As TreeMapCell() = TreeMapPartition.Shrink(TreeMapPartition.Partition(interior, items), Options.BuildingGap)

            For i As Integer = 0 To components.Count - 1
                Dim cell As TreeMapCell = If(i < cells.Length, cells(i), Nothing)
                Dim area As Rect = If(cell Is Nothing, interior, cell.Rect)

                If area.Width <= 4 OrElse area.Height <= 4 Then
                    area = Rect.FromSize(area.X, area.Y, Math.Max(4, area.Width), Math.Max(4, area.Height))
                End If

                ' 对分量内部的节点做正交布局
                Dim componentGraph As NetworkGraph = SubGraphOf(graph, components(i))
                Dim local As Dictionary(Of String, PointF) = OrthogonalLayout(componentGraph)

                ' 归一化到建筑块范围内
                NormalizeInto(local, area, positions)

                ' 记录建筑块信息
                Dim reactionIds As New List(Of String)()
                Dim metaboliteIds As New List(Of String)()

                For Each label As String In components(i)
                    Dim node As Node = graph.GetElementByID(label)

                    If node Is Nothing OrElse node.data Is Nothing Then
                        Continue For
                    End If

                    If String.Equals(node.Metadata("kind"), "reaction", StringComparison.Ordinal) Then
                        reactionIds.Add(node.data.origID)
                    Else
                        metaboliteIds.Add(node.data.origID)
                    End If
                Next

                buildings.Add(New BuildingBlock With {
                    .CategoryId = categoryId,
                    .X = area.X,
                    .Y = area.Y,
                    .Width = area.Width,
                    .Height = area.Height,
                    .ReactionIds = reactionIds.ToArray,
                    .MetaboliteIds = metaboliteIds.ToArray
                })
            Next

            Return positions
        End Function

        Private Shared Function SubGraphOf(graph As NetworkGraph, labels As List(Of String)) As NetworkGraph
            Dim extracted As New NetworkGraph()
            Dim inside As New HashSet(Of String)(labels, StringComparer.Ordinal)

            For Each label As String In labels
                Dim node As Node = graph.GetElementByID(label)

                If node IsNot Nothing Then
                    extracted.AddNode(node)
                End If
            Next

            For Each edge As Edge In graph.graphEdges.SafeQuery
                If inside.Contains(edge.U.label) AndAlso inside.Contains(edge.V.label) AndAlso Not extracted.ExistEdge(edge.U.label, edge.V.label) Then
                    extracted.CreateEdge(edge.U, edge.V)
                End If
            Next

            Return extracted
        End Function

        ''' <summary>
        ''' 调用 HOLA 做紧凑正交布局，返回节点标签到局部坐标的映射。
        ''' </summary>
        Private Function OrthogonalLayout(componentGraph As NetworkGraph) As Dictionary(Of String, PointF)
            Dim result As New Dictionary(Of String, PointF)(StringComparer.Ordinal)

            If componentGraph.vertex IsNot Nothing AndAlso componentGraph.vertex.Any Then
                If componentGraph.graphEdges IsNot Nothing AndAlso componentGraph.graphEdges.Any Then
                    Try
                        Call HOLA.DoLayout(componentGraph, Options.Hola)
                    Catch ex As Exception
                        ' HOLA 对退化输入（例如单点分量）可能不适用，退化为环形散布
                        FallbackCircular(componentGraph)
                    End Try
                Else
                    FallbackCircular(componentGraph)
                End If
            End If

            For Each node As Node In componentGraph.vertex.SafeQuery
                Dim position As AbstractVector = Nothing

                If node.data IsNot Nothing Then
                    position = node.data.initialPostion
                End If

                If position IsNot Nothing Then
                    result(node.label) = New PointF(CSng(position.x), CSng(position.y))
                Else
                    result(node.label) = New PointF(0, 0)
                End If
            Next

            Return result
        End Function

        Private Shared Sub FallbackCircular(componentGraph As NetworkGraph)
            Dim nodes As Node() = componentGraph.vertex.SafeQuery.ToArray()
            Dim radius As Double = Math.Max(10.0, nodes.Length * 6.0)

            For i As Integer = 0 To nodes.Length - 1
                Dim angle As Double = Math.PI * 2 * i / Math.Max(1, nodes.Length)

                If nodes(i).data IsNot Nothing Then
                    nodes(i).data.initialPostion = New FDGVector2(Math.Cos(angle) * radius, Math.Sin(angle) * radius)
                End If
            Next
        End Sub

        ''' <summary>把局部坐标等比缩放并平移到目标矩形内。</summary>
        Private Shared Sub NormalizeInto(local As Dictionary(Of String, PointF),
                                         area As Rect,
                                         output As Dictionary(Of String, PointF))

            If local.Count = 0 Then
                Return
            End If

            Dim minX As Double = Double.MaxValue
            Dim minY As Double = Double.MaxValue
            Dim maxX As Double = Double.MinValue
            Dim maxY As Double = Double.MinValue

            For Each pt As PointF In local.Values
                minX = Math.Min(minX, pt.X)
                minY = Math.Min(minY, pt.Y)
                maxX = Math.Max(maxX, pt.X)
                maxY = Math.Max(maxY, pt.Y)
            Next

            Const padding As Double = 4
            Dim innerW As Double = Math.Max(1, area.Width - 2 * padding)
            Dim innerH As Double = Math.Max(1, area.Height - 2 * padding)
            Dim spanX As Double = Math.Max(1E-06, maxX - minX)
            Dim spanY As Double = Math.Max(1E-06, maxY - minY)
            Dim scale As Double = Math.Min(innerW / spanX, innerH / spanY)

            For Each item As KeyValuePair(Of String, PointF) In local
                Dim x As Double = area.X + padding + (item.Value.X - minX) * scale
                Dim y As Double = area.Y + padding + (item.Value.Y - minY) * scale
                output(item.Key) = New PointF(CSng(x), CSng(y))
            Next
        End Sub

        ''' <summary>
        ''' 把跨类别枢纽代谢物放到街区边界上（论文中的 conjunction）。
        ''' </summary>
        ''' <remarks>
        ''' 枢纽被安排在「朝向它所连接的其他街区」的那条边界上：
        ''' 取它参与的其他街区中心相对本街区中心的平均方向，选择最接近的边界，
        ''' 再沿该边界均匀分布。
        ''' </remarks>
        Private Function PlaceJunctions(categoryId As String,
                                        block As Rect,
                                        junctions As List(Of Junction)) As Dictionary(Of String, Junction)

            Dim result As New Dictionary(Of String, Junction)(StringComparer.Ordinal)
            Dim hubCopies As MetaboliteCopy() = preprocessed.CopiesIn(categoryId) _
                .Where(Function(c) c.IsHub) _
                .ToArray

            If hubCopies.Length = 0 Then
                Return result
            End If

            ' 按枢纽度（参与的类别数）优先，控制边界上的枢纽数量
            Dim selected As MetaboliteCopy() = hubCopies _
                .GroupBy(Function(c) c.MetaboliteId) _
                .Select(Function(g) g.First()) _
                .OrderByDescending(Function(c) preprocessed.CopiesOf(c.MetaboliteId).Length) _
                .ThenBy(Function(c) c.MetaboliteId, StringComparer.Ordinal) _
                .Take(Options.MaxJunctionsPerBlock) _
                .ToArray

            Dim bySide As New Dictionary(Of Integer, List(Of MetaboliteCopy))()

            For Each copy As MetaboliteCopy In selected
                Dim side As Integer = PreferredSide(categoryId, block, copy.MetaboliteId)
                Dim bucket As List(Of MetaboliteCopy) = Nothing

                If Not bySide.TryGetValue(side, bucket) Then
                    bucket = New List(Of MetaboliteCopy)()
                    bySide.Add(side, bucket)
                End If

                bucket.Add(copy)
            Next

            Const pad As Double = 10

            For Each item As KeyValuePair(Of Integer, List(Of MetaboliteCopy)) In bySide
                Dim side As Integer = item.Key
                Dim list As List(Of MetaboliteCopy) = item.Value

                For i As Integer = 0 To list.Count - 1
                    Dim t As Double = (i + 1.0) / (list.Count + 1.0)
                    Dim x As Double
                    Dim y As Double

                    Select Case side
                        Case 0
                            x = block.X
                            y = block.Y + pad + t * Math.Max(1.0, block.Height - 2 * pad)
                        Case 2
                            x = block.P
                            y = block.Y + pad + t * Math.Max(1.0, block.Height - 2 * pad)
                        Case 1
                            x = block.X + pad + t * Math.Max(1.0, block.Width - 2 * pad)
                            y = block.Y
                        Case Else
                            x = block.X + pad + t * Math.Max(1.0, block.Width - 2 * pad)
                            y = block.Q
                    End Select

                    Dim cpd As MetabolicCompound = network.GetCompound(list(i).MetaboliteId)

                    Dim junction As New Junction With {
                        .Id = list(i).NodeId,
                        .Label = If(cpd Is Nothing, list(i).MetaboliteId, If(cpd.name, list(i).MetaboliteId)),
                        .CategoryId = categoryId,
                        .X = x,
                        .Y = y,
                        .Degree = preprocessed.CopiesOf(list(i).MetaboliteId).Length
                    }

                    junctions.Add(junction)

                    If Not result.ContainsKey(list(i).MetaboliteId) Then
                        result.Add(list(i).MetaboliteId, junction)
                    End If
                Next
            Next

            Return result
        End Function

        ''' <summary>选择朝向其它相关街区的那条边界。</summary>
        Private Function PreferredSide(categoryId As String, block As Rect, metaboliteId As String) As Integer
            Dim center As PointF = block.Center
            Dim dx As Double = 0
            Dim dy As Double = 0
            Dim count As Integer = 0

            For Each copy As MetaboliteCopy In preprocessed.CopiesOf(metaboliteId)
                If String.Equals(copy.CategoryId, categoryId, StringComparison.Ordinal) Then
                    Continue For
                End If

                Dim other As Rect = Nothing

                If Not blocks.TryGetValue(copy.CategoryId, other) Then
                    Continue For
                End If

                Dim otherCenter As PointF = other.Center
                dx += otherCenter.X - center.X
                dy += otherCenter.Y - center.Y
                count += 1
            Next

            If count = 0 Then
                Return 2
            End If

            dx /= count
            dy /= count

            If Math.Abs(dx) >= Math.Abs(dy) Then
                Return If(dx >= 0, 2, 0)
            Else
                Return If(dy >= 0, 3, 1)
            End If
        End Function

        ''' <summary>
        ''' 构造局部流网络并求解车道。
        ''' </summary>
        Private Function GenerateLanes(categoryId As String,
                                       block As Rect,
                                       graph As NetworkGraph,
                                       positions As Dictionary(Of String, PointF),
                                       junctionNodes As Dictionary(Of String, Junction)) As RoutePolyline()

            If junctionNodes.Count = 0 Then
                Return New RoutePolyline() {}
            End If

            ' 1. 收集「 реакции ↔ 枢纽」配对
            Dim pairs As New List(Of LanePair)()
            Dim demandByReaction As New Dictionary(Of String, Integer)(StringComparer.Ordinal)
            Dim supplyByHub As New Dictionary(Of String, Integer)(StringComparer.Ordinal)
            Dim reachedLimit As Boolean = False

            For Each rxnNode As ReactionNode In preprocessed.ReactionNodesIn(categoryId)
                If reachedLimit Then
                    Exit For
                End If

                Dim rxn As MetabolicReaction = network.GetReaction(rxnNode.ReactionId)

                If rxn Is Nothing Then
                    Continue For
                End If

                For Each cpdId As String In rxn.SpeciesIds()
                    If Not junctionNodes.ContainsKey(cpdId) Then
                        Continue For
                    End If

                    If pairs.Count >= Options.MaxLanePairs Then
                        reachedLimit = True
                        Exit For
                    End If

                    pairs.Add(New LanePair With {.Reaction = rxnNode.NodeId, .Hub = cpdId})
                    Bump(demandByReaction, rxnNode.NodeId)
                    Bump(supplyByHub, cpdId)
                Next
            Next

            If pairs.Count = 0 Then
                Return New RoutePolyline() {}
            End If

            ' 2. 道路网格
            Dim interior As Rect = block.Inflate(-Options.BoundaryInset)

            If interior.Width <= 8 OrElse interior.Height <= 8 Then
                Return New RoutePolyline() {}
            End If

            Dim step_ As Double = Options.GridStep

            While ((interior.Width \ step_) + 1) * ((interior.Height \ step_) + 1) > Options.MaxGridNodes
                step_ *= 1.5
            End While

            Dim cols As Integer = Math.Max(1, CInt(interior.Width \ step_))
            Dim rows As Integer = Math.Max(1, CInt(interior.Height \ step_))
            Dim cellW As Double = interior.Width / cols
            Dim cellH As Double = interior.Height / rows

            ' 3. 流网络
            Dim flow As New MinCostFlow()
            Dim source As Integer = flow.AddNode()
            Dim sink As Integer = flow.AddNode()

            Dim junctionId As New Dictionary(Of String, Integer)(StringComparer.Ordinal)
            Dim junctionPoint As New Dictionary(Of Integer, PointF)()

            For Each item As KeyValuePair(Of String, Junction) In junctionNodes
                Dim node As Integer = flow.AddNode()
                junctionId(item.Key) = node
                junctionPoint(node) = item.Value.Point
            Next

            Dim reactionId As New Dictionary(Of String, Integer)(StringComparer.Ordinal)
            Dim reactionPoint As New Dictionary(Of Integer, PointF)()

            For Each rxnNode As ReactionNode In preprocessed.ReactionNodesIn(categoryId)
                Dim position As PointF = Nothing

                If Not positions.TryGetValue(rxnNode.NodeId, position) Then
                    Continue For
                End If

                Dim node As Integer = flow.AddNode()
                reactionId(rxnNode.NodeId) = node
                reactionPoint(node) = position
            Next

            If reactionId.Count = 0 Then
                Return New RoutePolyline() {}
            End If

            Dim reactionLabelByNode As Dictionary(Of Integer, String) = Invert(reactionId)
            Dim junctionLabelByNode As Dictionary(Of Integer, String) = Invert(junctionId)

            Dim gridNode(rows, cols) As Integer
            Dim gridPoint(rows, cols) As PointF
            Dim gridLookup As New Dictionary(Of Integer, PointF)()

            For j As Integer = 0 To rows
                For i As Integer = 0 To cols
                    gridNode(j, i) = flow.AddNode()
                    gridPoint(j, i) = New PointF(CSng(interior.X + i * cellW), CSng(interior.Y + j * cellH))
                    gridLookup(gridNode(j, i)) = gridPoint(j, i)
                Next
            Next

            ' 4. 已有正交布局边的空间索引（用于交叉惩罚）
            Dim layoutBuckets As Dictionary(Of Long, List(Of LayoutEdge)) = BuildLayoutBuckets(graph, positions, step_ * 2)

            ' 5. 超级源 -> 枢纽
            For Each item As KeyValuePair(Of String, Integer) In junctionId
                Dim supply As Integer = 0
                supplyByHub.TryGetValue(item.Key, supply)

                If supply <= 0 Then
                    Continue For
                End If

                flow.AddEdge(source, item.Value, supply, 0)

                Dim point As PointF = junctionPoint(item.Value)

                For Each corner As ValueTuple(Of Integer, Double) In NearestCorners(point, interior, gridNode, gridPoint, rows, cols, cellW, cellH)
                    flow.AddEdge(item.Value, corner.Item1, supply, corner.Item2 * Options.LengthWeight)
                Next
            Next

            ' 6. 道路网格双向边
            For j As Integer = 0 To rows
                For i As Integer = 0 To cols
                    If i < cols Then
                        Dim a As PointF = gridPoint(j, i)
                        Dim b As PointF = gridPoint(j, i + 1)
                        Dim cost As Double = Options.LengthWeight * cellW + Options.CrossingWeight * CrossingPenalty(a, b, layoutBuckets, step_ * 2)
                        flow.AddEdge(gridNode(j, i), gridNode(j, i + 1), Options.LaneCapacity, cost)
                        flow.AddEdge(gridNode(j, i + 1), gridNode(j, i), Options.LaneCapacity, cost)
                    End If

                    If j < rows Then
                        Dim a As PointF = gridPoint(j, i)
                        Dim b As PointF = gridPoint(j + 1, i)
                        Dim cost As Double = Options.LengthWeight * cellH + Options.CrossingWeight * CrossingPenalty(a, b, layoutBuckets, step_ * 2)
                        flow.AddEdge(gridNode(j, i), gridNode(j + 1, i), Options.LaneCapacity, cost)
                        flow.AddEdge(gridNode(j + 1, i), gridNode(j, i), Options.LaneCapacity, cost)
                    End If
                Next
            Next

            ' 7. 反应 -> 超级汇
            For Each item As KeyValuePair(Of Integer, PointF) In reactionPoint
                Dim label As String = Nothing

                If Not reactionLabelByNode.TryGetValue(item.Key, label) Then
                    Continue For
                End If

                Dim demand As Integer = 0
                demandByReaction.TryGetValue(label, demand)

                If demand <= 0 Then
                    Continue For
                End If

                For Each corner As ValueTuple(Of Integer, Double) In NearestCorners(item.Value, interior, gridNode, gridPoint, rows, cols, cellW, cellH)
                    flow.AddEdge(corner.Item1, item.Key, demand, corner.Item2 * Options.LengthWeight)
                Next

                flow.AddEdge(item.Key, sink, demand, 0)
            Next

            ' 8. 求解并抽取车道
            Dim solution As FlowSolution = flow.Solve(source, sink)
            Dim routes As New List(Of RoutePolyline)()
            Dim counter As Integer = 0

            For Each path As List(Of Integer) In solution.ExtractPaths()
                If path.Count < 3 Then
                    Continue For
                End If

                Dim startNode As Integer = path(1)
                Dim endNode As Integer = path(path.Count - 2)

                Dim startPoint As PointF = Nothing
                Dim endPoint As PointF = Nothing

                If Not junctionPoint.TryGetValue(startNode, startPoint) Then
                    Continue For
                End If

                If Not reactionPoint.TryGetValue(endNode, endPoint) Then
                    Continue For
                End If

                Dim hubId As String = Nothing

                If Not junctionLabelByNode.TryGetValue(startNode, hubId) Then
                    Continue For
                End If

                Dim reactionNodeId As String = Nothing

                If Not reactionLabelByNode.TryGetValue(endNode, reactionNodeId) Then
                    Continue For
                End If

                Dim polyline As New List(Of PointF)()
                polyline.Add(startPoint)

                For k As Integer = 2 To path.Count - 3
                    Dim point As PointF = Nothing

                    If gridLookup.TryGetValue(path(k), point) Then
                        polyline.Add(point)
                    End If
                Next

                polyline.Add(endPoint)

                Dim rxn As MetabolicReaction = network.GetReaction(reactionNodeId.Substring(2))
                Dim junction As Junction = junctionNodes(hubId)
                counter += 1

                routes.Add(RoutePolyline.Create(
                    id:=$"L:{categoryId}:{counter}",
                    sourceId:=hubId,
                    targetId:=reactionNodeId,
                    metaboliteId:=hubId,
                    role:=EdgeRole.ReactantToReactant,
                    directed:=Not (rxn IsNot Nothing AndAlso rxn.is_reversible),
                    points:=polyline,
                    label:=junction.Label))
            Next

            Return routes.ToArray
        End Function

        Private Shared Function Invert(table As Dictionary(Of String, Integer)) As Dictionary(Of Integer, String)
            Dim result As New Dictionary(Of Integer, String)()

            For Each item As KeyValuePair(Of String, Integer) In table
                result(item.Value) = item.Key
            Next

            Return result
        End Function

        Private Shared Sub Bump(table As Dictionary(Of String, Integer), key As String)
            Dim value As Integer = 0
            table.TryGetValue(key, value)
            table(key) = value + 1
        End Sub

        ''' <summary>
        ''' 取一个点附近网格单元的角点（含到该点的距离）。
        ''' </summary>
        Private Shared Function NearestCorners(point As PointF,
                                               interior As Rect,
                                               gridNode As Integer(,),
                                               gridPoint As PointF(,),
                                               rows As Integer,
                                               cols As Integer,
                                               cellW As Double,
                                               cellH As Double) As List(Of ValueTuple(Of Integer, Double))

            Dim result As New List(Of ValueTuple(Of Integer, Double))()
            Dim ci As Integer = Math.Max(0, Math.Min(cols, CInt(Math.Round((point.X - interior.X) / Math.Max(1E-06, cellW)))))
            Dim cj As Integer = Math.Max(0, Math.Min(rows, CInt(Math.Round((point.Y - interior.Y) / Math.Max(1E-06, cellH)))))

            For j As Integer = Math.Max(0, cj - 1) To Math.Min(rows, cj)
                For i As Integer = Math.Max(0, ci - 1) To Math.Min(cols, ci)
                    Dim gp As PointF = gridPoint(j, i)
                    Dim dx As Double = point.X - gp.X
                    Dim dy As Double = point.Y - gp.Y
                    result.Add((gridNode(j, i), Math.Sqrt(dx * dx + dy * dy)))
                Next
            Next

            Return result _
                .OrderBy(Function(t) t.Item2) _
                .Take(4) _
                .ToList
        End Function

        ''' <summary>把布局边按粗粒度空间哈希分桶。</summary>
        Private Shared Function BuildLayoutBuckets(graph As NetworkGraph,
                                                   positions As Dictionary(Of String, PointF),
                                                   cell As Double) As Dictionary(Of Long, List(Of LayoutEdge))

            Dim buckets As New Dictionary(Of Long, List(Of LayoutEdge))()
            Dim size As Double = Math.Max(1.0, cell)

            For Each edge As Edge In graph.graphEdges.SafeQuery
                Dim pa As PointF = Nothing
                Dim pb As PointF = Nothing

                If Not positions.TryGetValue(edge.U.label, pa) OrElse Not positions.TryGetValue(edge.V.label, pb) Then
                    Continue For
                End If

                Dim key As Long = BucketKey((pa.X + pb.X) / 2.0, (pa.Y + pb.Y) / 2.0, size)
                Dim bucket As List(Of LayoutEdge) = Nothing

                If Not buckets.TryGetValue(key, bucket) Then
                    bucket = New List(Of LayoutEdge)()
                    buckets.Add(key, bucket)
                End If

                bucket.Add(New LayoutEdge With {.A = pa, .B = pb})
            Next

            Return buckets
        End Function

        Private Shared Function BucketKey(x As Double, y As Double, size As Double) As Long
            Dim ix As Long = CLng(Math.Floor(x / size))
            Dim iy As Long = CLng(Math.Floor(y / size))
            Return (ix << 32) Xor (iy And &HFFFFFFFFL)
        End Function

        ''' <summary>
        ''' 论文的交叉惩罚项：Σ_h( |e·l_h| / (‖e‖‖l_h‖) + 1 )。
        ''' </summary>
        Private Function CrossingPenalty(a As PointF,
                                         b As PointF,
                                         layoutBuckets As Dictionary(Of Long, List(Of LayoutEdge)),
                                         size As Double) As Double

            If layoutBuckets.Count = 0 Then
                Return 0
            End If

            Dim ex As Double = b.X - a.X
            Dim ey As Double = b.Y - a.Y
            Dim elen As Double = Math.Sqrt(ex * ex + ey * ey)

            If elen <= 1E-09 Then
                Return 0
            End If

            Dim mx As Double = (a.X + b.X) / 2.0
            Dim my As Double = (a.Y + b.Y) / 2.0
            Dim cx As Long = CLng(Math.Floor(mx / size))
            Dim cy As Long = CLng(Math.Floor(my / size))

            Dim sum As Double = 0
            Dim samples As Integer = 0

            For dj As Long = -1 To 1
                For di As Long = -1 To 1
                    Dim key As Long = ((cx + di) << 32) Xor ((cy + dj) And &HFFFFFFFFL)
                    Dim bucket As List(Of LayoutEdge) = Nothing

                    If Not layoutBuckets.TryGetValue(key, bucket) Then
                        Continue For
                    End If

                    For Each edge As LayoutEdge In bucket
                        Dim lx As Double = edge.B.X - edge.A.X
                        Dim ly As Double = edge.B.Y - edge.A.Y
                        Dim llen As Double = Math.Sqrt(lx * lx + ly * ly)

                        If llen <= 1E-09 Then
                            Continue For
                        End If

                        Dim cosine As Double = Math.Abs(ex * lx + ey * ly) / (elen * llen)
                        sum += cosine + 1.0
                        samples += 1

                        If samples >= Options.MaxCrossingSamples Then
                            Return sum
                        End If
                    Next
                Next
            Next

            Return sum
        End Function

        ''' <summary>计算无向图的连通分量。</summary>
        Private Shared Function ConnectedComponents(graph As NetworkGraph) As List(Of List(Of String))
            Dim visited As New HashSet(Of String)(StringComparer.Ordinal)
            Dim result As New List(Of List(Of String))()

            For Each node As Node In graph.vertex.SafeQuery
                If visited.Contains(node.label) Then
                    Continue For
                End If

                Dim members As New List(Of String)()
                Dim stack As New Stack(Of Node)()
                stack.Push(node)
                visited.Add(node.label)

                While stack.Count > 0
                    Dim u As Node = stack.Pop()
                    members.Add(u.label)

                    For Each v As Node In u.EnumerateAdjacencies()
                        If visited.Add(v.label) Then
                            stack.Push(v)
                        End If
                    Next
                End While

                result.Add(members)
            Next

            Return result
        End Function

    End Class

End Namespace

#End Region
