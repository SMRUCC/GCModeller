#Region "Metabopolis: inter-block edge routing"

' ============================================================================
' 块间边路由（论文第五节）
' ----------------------------------------------------------------------------
' 目标：把「出现在不同街区中的同一代谢物副本」连接起来，形成跨街区的彩色边。
'
' 做法（对应论文的全局流网络 G_N）：
'   * 在整张地图上构建一套网格道路；落在街区内部的网格点被标记为占用，
'     只有在街区之间空隙 / 街区边界上的网格点才是可通行的道路点；
'   * 每个枢纽代谢物在多个街区中的副本按照空间就近原则串成一条链，
'     相邻副本之间构成一次路由需求；
'   * 用最小费用最大流（successive shortest path）求全局路由；
'     每条道路的容量 = MAX_CAPACITY，从而像城市规划中「分散交通密度」
'     那样限制单条道路上的边密度；
'   * 边的颜色由九色角色编码决定（源/目标街区中该代谢物是底物还是产物）。
' ============================================================================

Imports System.Drawing
Imports Microsoft.VisualBasic.Linq
Imports Metabopolis.Model
Imports Metabopolis.Preprocess

Namespace Routing

    ''' <summary>
    ''' 块间路由参数。
    ''' </summary>
    Public Class InterBlockOptions

        ''' <summary>全局道路网格步长（像素）。</summary>
        Public Property GridStep As Double = 26

        ''' <summary>全局网格节点上限（超出则自动放大步长）。</summary>
        Public Property MaxGridNodes As Integer = 12000

        ''' <summary>每条全局道路的容量上限（论文中的 MAX_CAPACITY）。</summary>
        Public Property MaxCapacity As Integer = 6

        ''' <summary>参与块间路由的枢纽代谢物数量上限（性能保护）。</summary>
        Public Property MaxHubMetabolites As Integer = 200

        ''' <summary>块间路由的配对数量上限（性能保护）。</summary>
        Public Property MaxPairs As Integer = 400

        ''' <summary>网格道路的长度权重。</summary>
        Public Property LengthWeight As Double = 1.0

        ''' <summary>街区内部向外扩张多少像素之后仍视为「不可通行」。</summary>
        Public Property WallInset As Double = 1.0

        Public Overrides Function ToString() As String
            Return $"grid={GridStep:0.##}, capacity={MaxCapacity}, hubs<={MaxHubMetabolites}, pairs<={MaxPairs}"
        End Function

    End Class

    ''' <summary>
    ''' 块间边路由器。
    ''' </summary>
    Public Class InterBlockRouter

        ''' <summary>一次跨街区连接的端点。</summary>
        Private Class HubPair

            Public Property FromCopy As String
            Public Property ToCopy As String
            Public Property MetaboliteId As String

        End Class

        Public Property Options As InterBlockOptions

        Private ReadOnly preprocessed As PreprocessedNetwork
        Private ReadOnly blocks As Dictionary(Of String, Rect)
        Private ReadOnly junctions As Dictionary(Of String, Junction)

        Public Sub New(preprocessed As PreprocessedNetwork,
                       blocks As Dictionary(Of String, Rect),
                       junctions As Dictionary(Of String, Junction),
                       Optional options As InterBlockOptions = Nothing)

            Me.preprocessed = preprocessed
            Me.blocks = blocks
            Me.junctions = junctions
            Me.Options = If(options, New InterBlockOptions())
        End Sub

        ''' <summary>执行块间路由。</summary>
        Public Function Route() As RoutePolyline()
            If junctions Is Nothing OrElse junctions.Count < 2 Then
                Return New RoutePolyline() {}
            End If

            Dim pairs As List(Of HubPair) = BuildPairs()

            If pairs.Count = 0 Then
                Return New RoutePolyline() {}
            End If

            Dim extent As Rect = Bounds()

            If extent Is Nothing OrElse extent.Width <= 0 OrElse extent.Height <= 0 Then
                Return New RoutePolyline() {}
            End If

            ' 几何异常保护：非有限或超大尺寸的布局直接跳过块间路由
            If Not IsFinite(extent) OrElse extent.Width > MaxDimension OrElse extent.Height > MaxDimension Then
                Return New RoutePolyline() {}
            End If

            ' 1. 全局道路网格：由目标节点数直接解析出步长，避免迭代放大导致整数溢出
            Dim step_ As Double = Math.Max(Options.GridStep,
                                           Math.Sqrt(extent.Width * extent.Height / Math.Max(1, Options.MaxGridNodes)))
            Dim cols As Integer = Math.Max(1, Math.Min(MaxGridDimension, CInt(Math.Floor(extent.Width / step_))))
            Dim rows As Integer = Math.Max(1, Math.Min(MaxGridDimension, CInt(Math.Floor(extent.Height / step_))))
            Dim cellW As Double = extent.Width / cols
            Dim cellH As Double = extent.Height / rows

            Dim blocked(rows, cols) As Boolean
            MarkBlocks(blocked, extent, rows, cols, cellW, cellH)

            ' 2. 流网络
            Dim flow As New MinCostFlow()
            Dim source As Integer = flow.AddNode()
            Dim sink As Integer = flow.AddNode()

            Dim gridNode(rows, cols) As Integer
            Dim gridPoint(rows, cols) As PointF
            Dim gridLookup As New Dictionary(Of Integer, PointF)()

            For j As Integer = 0 To rows
                For i As Integer = 0 To cols
                    gridNode(j, i) = flow.AddNode()
                    gridPoint(j, i) = New PointF(CSng(extent.X + i * cellW), CSng(extent.Y + j * cellH))
                    gridLookup(gridNode(j, i)) = gridPoint(j, i)
                Next
            Next

            ' 3. 每个枢纽副本一个节点
            Dim copyNode As New Dictionary(Of String, Integer)(StringComparer.Ordinal)
            Dim copySupply As New Dictionary(Of Integer, Integer)()
            Dim copyDemand As New Dictionary(Of Integer, Integer)()

            For Each pair As HubPair In pairs
                If Not copyNode.ContainsKey(pair.FromCopy) Then
                    copyNode(pair.FromCopy) = flow.AddNode()
                End If
                If Not copyNode.ContainsKey(pair.ToCopy) Then
                    copyNode(pair.ToCopy) = flow.AddNode()
                End If

                Dim fromNode As Integer = copyNode(pair.FromCopy)
                Dim toNode As Integer = copyNode(pair.ToCopy)
                Bump(copySupply, fromNode)
                Bump(copyDemand, toNode)
            Next

            ' 4. 超级源 / 超级汇 <-> 枢纽副本
            For Each item As KeyValuePair(Of Integer, Integer) In copySupply
                flow.AddEdge(source, item.Key, item.Value, 0)
            Next

            For Each item As KeyValuePair(Of Integer, Integer) In copyDemand
                flow.AddEdge(item.Key, sink, item.Value, 0)
            Next

            ' 5. 枢纽副本 -> 最近的可通行道路点
            For Each item As KeyValuePair(Of String, Integer) In copyNode
                Dim junction As Junction = Nothing

                If Not junctions.TryGetValue(item.Key, junction) Then
                    Continue For
                End If

                For Each corner As ValueTuple(Of Integer, Double) In NearestFreeCorners(junction.Point, extent, blocked, gridNode, gridPoint, rows, cols, cellW, cellH)
                    flow.AddEdge(item.Value, corner.Item1, Options.MaxCapacity, corner.Item2 * Options.LengthWeight)
                Next
            Next

            ' 6. 全局道路双向边（容量 = MAX_CAPACITY）
            For j As Integer = 0 To rows
                For i As Integer = 0 To cols
                    If i < cols AndAlso Not blocked(j, i) AndAlso Not blocked(j, i + 1) Then
                        flow.AddEdge(gridNode(j, i), gridNode(j, i + 1), Options.MaxCapacity, Options.LengthWeight * cellW)
                        flow.AddEdge(gridNode(j, i + 1), gridNode(j, i), Options.MaxCapacity, Options.LengthWeight * cellW)
                    End If

                    If j < rows AndAlso Not blocked(j, i) AndAlso Not blocked(j + 1, i) Then
                        flow.AddEdge(gridNode(j, i), gridNode(j + 1, i), Options.MaxCapacity, Options.LengthWeight * cellH)
                        flow.AddEdge(gridNode(j + 1, i), gridNode(j, i), Options.MaxCapacity, Options.LengthWeight * cellH)
                    End If
                Next
            Next

            ' 7. 求解并生成折线
            Dim solution As FlowSolution = flow.Solve(source, sink)
            Dim copyLabel As Dictionary(Of Integer, String) = Invert(copyNode)
            Dim routes As New List(Of RoutePolyline)()
            Dim counter As Integer = 0

            For Each path As List(Of Integer) In solution.ExtractPaths()
                If path.Count < 3 Then
                    Continue For
                End If

                Dim fromLabel As String = Nothing
                Dim toLabel As String = Nothing

                If Not copyLabel.TryGetValue(path(1), fromLabel) Then
                    Continue For
                End If

                If Not copyLabel.TryGetValue(path(path.Count - 2), toLabel) Then
                    Continue For
                End If

                Dim fromJunction As Junction = Nothing
                Dim toJunction As Junction = Nothing

                If Not junctions.TryGetValue(fromLabel, fromJunction) Then
                    Continue For
                End If

                If Not junctions.TryGetValue(toLabel, toJunction) Then
                    Continue For
                End If

                Dim polyline As New List(Of PointF)()
                polyline.Add(fromJunction.Point)

                For k As Integer = 2 To path.Count - 3
                    Dim point As PointF = Nothing

                    If gridLookup.TryGetValue(path(k), point) Then
                        polyline.Add(point)
                    End If
                Next

                polyline.Add(toJunction.Point)

                Dim metaboliteId As String = preprocessed.GetCopy(fromLabel)?.MetaboliteId
                Dim role As EdgeRole = preprocessed.RoleBetween(metaboliteId, fromJunction.CategoryId, toJunction.CategoryId)
                counter += 1

                Dim globalRoute As RoutePolyline = RoutePolyline.Create(
                    id:=$"G:{counter}",
                    sourceId:=fromLabel,
                    targetId:=toLabel,
                    metaboliteId:=metaboliteId,
                    role:=role,
                    directed:=False,
                    points:=polyline,
                    label:=fromJunction.Label)

                globalRoute.IsInterBlock = True
                routes.Add(globalRoute)
            Next

            Return routes.ToArray
        End Function

        ''' <summary>
        ''' 把每个枢纽代谢物在各个街区中的副本按空间就近串成链，相邻副本构成一次连接。
        ''' </summary>
        Private Function BuildPairs() As List(Of HubPair)
            Dim result As New List(Of HubPair)()
            Dim hubs As New List(Of String)()

            For Each metaboliteId As String In preprocessed.HubMetabolites.SafeQuery
                hubs.Add(metaboliteId)
            Next

            ' 优先连接度数更高的枢纽（连接更多街区），控制规模
            hubs = hubs _
                .OrderByDescending(Function(id) preprocessed.CopiesOf(id).Length) _
                .ThenBy(Function(id) id, StringComparer.Ordinal) _
                .Take(Options.MaxHubMetabolites) _
                .ToList

            For Each metaboliteId As String In hubs
                If result.Count >= Options.MaxPairs Then
                    Exit For
                End If

                Dim points As New List(Of ValueTuple(Of String, PointF))()

                For Each copy As MetaboliteCopy In preprocessed.CopiesOf(metaboliteId)
                    Dim junction As Junction = Nothing

                    If junctions.TryGetValue(copy.NodeId, junction) Then
                        points.Add((copy.NodeId, junction.Point))
                    End If
                Next

                If points.Count < 2 Then
                    Continue For
                End If

                ' 贪心最近邻成链：保证跨街区连接沿地图走向展开，而不是交叉绕行
                Dim remaining As New List(Of ValueTuple(Of String, PointF))(points)
                Dim current As ValueTuple(Of String, PointF) = remaining(0)
                remaining.RemoveAt(0)

                While remaining.Count > 0 AndAlso result.Count < Options.MaxPairs
                    Dim bestIndex As Integer = 0
                    Dim bestDistance As Double = Double.MaxValue

                    For i As Integer = 0 To remaining.Count - 1
                        Dim dx As Double = remaining(i).Item2.X - current.Item2.X
                        Dim dy As Double = remaining(i).Item2.Y - current.Item2.Y
                        Dim distance As Double = dx * dx + dy * dy

                        If distance < bestDistance Then
                            bestDistance = distance
                            bestIndex = i
                        End If
                    Next

                    Dim next_ As ValueTuple(Of String, PointF) = remaining(bestIndex)
                    remaining.RemoveAt(bestIndex)

                    result.Add(New HubPair With {
                        .FromCopy = current.Item1,
                        .ToCopy = next_.Item1,
                        .MetaboliteId = metaboliteId
                    })

                    current = next_
                End While
            Next

            Return result
        End Function

        ''' <summary>把落在街区内部的网格单元标记为不可通行。</summary>
        Private Sub MarkBlocks(blocked As Boolean(,),
                               extent As Rect,
                               rows As Integer,
                               cols As Integer,
                               cellW As Double,
                               cellH As Double)

            For Each block As Rect In blocks.Values
                If block Is Nothing OrElse block.Width <= 0 OrElse block.Height <= 0 Then
                    Continue For
                End If

                Dim wall As Rect = block.Inflate(-Options.WallInset)
                Dim i0 As Integer = Math.Max(0, CInt(Math.Floor((wall.X - extent.X) / Math.Max(1E-06, cellW))))
                Dim i1 As Integer = Math.Min(cols, CInt(Math.Ceiling((wall.P - extent.X) / Math.Max(1E-06, cellW))))
                Dim j0 As Integer = Math.Max(0, CInt(Math.Floor((wall.Y - extent.Y) / Math.Max(1E-06, cellH))))
                Dim j1 As Integer = Math.Min(rows, CInt(Math.Ceiling((wall.Q - extent.Y) / Math.Max(1E-06, cellH))))

                For j As Integer = j0 To j1
                    For i As Integer = i0 To i1
                        ' 网格点落在街区内部才视为墙；正好压在边界上的点仍可作为道路
                        Dim px As Double = extent.X + i * cellW
                        Dim py As Double = extent.Y + j * cellH

                        If wall.X < px AndAlso px < wall.P AndAlso wall.Y < py AndAlso py < wall.Q Then
                            blocked(j, i) = True
                        End If
                    Next
                Next
            Next
        End Sub

        ''' <summary>取给定点附近最近的可通行网格角点。</summary>
        Private Shared Function NearestFreeCorners(point As PointF,
                                                   extent As Rect,
                                                   blocked As Boolean(,),
                                                   gridNode As Integer(,),
                                                   gridPoint As PointF(,),
                                                   rows As Integer,
                                                   cols As Integer,
                                                   cellW As Double,
                                                   cellH As Double) As List(Of ValueTuple(Of Integer, Double))

            Dim result As New List(Of ValueTuple(Of Integer, Double))()
            Dim ci As Integer = Math.Max(0, Math.Min(cols, CInt(Math.Round((point.X - extent.X) / Math.Max(1E-06, cellW)))))
            Dim cj As Integer = Math.Max(0, Math.Min(rows, CInt(Math.Round((point.Y - extent.Y) / Math.Max(1E-06, cellH)))))

            ' 从 1 环扩到 3 环，保证总能找到可通行点
            For radius As Integer = 1 To 3
                For j As Integer = Math.Max(0, cj - radius) To Math.Min(rows, cj + radius)
                    For i As Integer = Math.Max(0, ci - radius) To Math.Min(cols, ci + radius)
                        If blocked(j, i) Then
                            Continue For
                        End If

                        Dim gp As PointF = gridPoint(j, i)
                        Dim dx As Double = point.X - gp.X
                        Dim dy As Double = point.Y - gp.Y
                        result.Add((gridNode(j, i), Math.Sqrt(dx * dx + dy * dy)))
                    Next
                Next

                If result.Count >= 4 Then
                    Exit For
                End If
            Next

            Return result _
                .OrderBy(Function(t) t.Item2) _
                .Take(4) _
                .ToList
        End Function

        Private Function Bounds() As Rect
            Dim rect As Rect = Nothing

            For Each block As Rect In blocks.Values
                If rect Is Nothing Then
                    rect = block.Clone()
                Else
                    rect = rect.Union(block)
                End If
            Next

            If rect Is Nothing Then
                Return Nothing
            End If

            ' 预留一圈空白作为外环道路
            Return rect.Inflate(Math.Max(24.0, Options.GridStep * 1.5))
        End Function

        Private Shared Sub Bump(table As Dictionary(Of Integer, Integer), key As Integer)
            Dim value As Integer = 0
            table.TryGetValue(key, value)
            table(key) = value + 1
        End Sub

        Private Shared Function Invert(table As Dictionary(Of String, Integer)) As Dictionary(Of Integer, String)
            Dim result As New Dictionary(Of Integer, String)()

            For Each item As KeyValuePair(Of String, Integer) In table
                result(item.Value) = item.Key
            Next

            Return result
        End Function

        ''' <summary>几何尺寸的合理上限（像素）。</summary>
        Private Const MaxDimension As Double = 1.0E+08

        Private Const MaxGridDimension As Integer = 4096

        Private Shared Function IsFinite(rect As Rect) As Boolean
            If rect Is Nothing Then
                Return False
            End If

            Return IsFinite(rect.X) AndAlso IsFinite(rect.Y) AndAlso
                   IsFinite(rect.Width) AndAlso IsFinite(rect.Height)
        End Function

        Private Shared Function IsFinite(value As Double) As Boolean
            Return Not Double.IsNaN(value) AndAlso Not Double.IsInfinity(value)
        End Function

    End Class

End Namespace

#End Region
