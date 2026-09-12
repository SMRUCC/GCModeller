#Region "Metabopolis: heuristic floor-planning"

' ============================================================================
' 启发式 Floor-Planning（替代论文的 MIP 求解器）
' ----------------------------------------------------------------------------
' 论文把区块排布建模为混合整数规划（CH1-CH4 硬约束 + CS1-CS3 软目标），并用
' CPLEX/Gurobi 求全局最优。当前代码库中没有任何整数规划求解器，因此这里用
' 「约束驱动的模拟退火 + 迭代松弛」实现同一套约束与目标：
'
'   硬约束
'     CH1 贴附     —— 沿配置空间的四条线段吸附相邻街区（ConfigSpace）
'     CH2 无重叠   —— 任何移动后都执行「最小平移量推出」修复，修复失败则回退该步
'     CH3 相对位置 —— 与骨架初始布局的方位一致（左/右/上/下）保持一致
'     CH4 重心保持 —— 骨架基本环的重心必须落在环内部
'   软目标
'     CS1 紧凑      w = 1000，最小化布局外接矩形的 Bx + By
'     CS2 长宽比    w = 1，  最小化 |Bx - R·By|，默认 R = 4/3
'     CS3 长共享边  w = 10， 最小化相邻街区中心的曼哈顿位移
'
' 收敛后执行第二轮「尺寸微调」：
'     FH1 最小宽高、FH2 已接触边界保持、FS1 面积最大化（向域边界扩张）、
'     FS2 区块自身长宽比不剧烈变化。
' ============================================================================

Imports System.Drawing
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports Metabopolis.Model
Imports Metabopolis.Skeleton

Namespace Floorplan

    ''' <summary>
    ''' Floor-planning 参数。
    ''' </summary>
    Public Class FloorplanOptions

        ''' <summary>期望的长宽比 R（论文默认 4/3）。</summary>
        Public Property TargetAspectRatio As Double = 4.0 / 3.0

        ''' <summary>CS1 紧凑布局权重（论文 w_compact = 1000）。</summary>
        Public Property WeightCompact As Double = 1000

        ''' <summary>CS2 期望长宽比权重（论文 w_ratio = 1）。</summary>
        Public Property WeightRatio As Double = 1

        ''' <summary>CS3 长共享边界权重（论文 w_overlay = 10）。</summary>
        Public Property WeightOverlay As Double = 10

        ''' <summary>CH3 相对位置违例的惩罚权重。</summary>
        Public Property WeightRelative As Double = 50000

        ''' <summary>CH4 重心违例的惩罚权重。</summary>
        Public Property WeightBarycenter As Double = 50000

        ''' <summary>每个「类别权重单位」分配的初始面积（平方像素）。</summary>
        Public Property AreaPerWeight As Double = 1400

        ''' <summary>街区最小宽度。</summary>
        Public Property MinBlockWidth As Double = 70

        ''' <summary>街区最小高度。</summary>
        Public Property MinBlockHeight As Double = 52

        ''' <summary>模拟退火迭代次数。</summary>
        Public Property AnnealIterations As Integer = 6000

        ''' <summary>重叠修复的最大迭代次数。</summary>
        Public Property RepairIterations As Integer = 150

        ''' <summary>扰动步长相对于平均街区尺寸的比例。</summary>
        Public Property JitterScale As Double = 0.35

        ''' <summary>尺寸微调时允许的长宽比偏离倍数。</summary>
        Public Property GrowthLimitRatio As Double = 2.0

        ''' <summary>尺寸微调阶段地图域的扩张边距（相对于布局对角线）。</summary>
        Public Property DomainMarginRatio As Double = 0.12

        ''' <summary>随机种子；0 表示按时间自动生成。</summary>
        Public Property RandomSeed As Integer = 0

        ''' <summary>是否输出阶段日志。</summary>
        Public Property Verbose As Boolean = True

    End Class

    ''' <summary>
    ''' Floor-planning 结果。
    ''' </summary>
    Public Class FloorplanResult

        ''' <summary>每个类别的街区几何。</summary>
        Public Property Blocks As Dictionary(Of String, Rect)

        ''' <summary>最终残留的重叠对数（正常应为 0）。</summary>
        Public Property Overlaps As Integer

        ''' <summary>最终目标函数值。</summary>
        Public Property Cost As Double

        ''' <summary>实际执行的退火迭代次数。</summary>
        Public Property Iterations As Integer

        ''' <summary>全部街区的外接矩形。</summary>
        Public Function Bounds() As Rect
            Dim rect As Rect = Nothing

            For Each block As Rect In Blocks.Values
                If rect Is Nothing Then
                    rect = block.Clone()
                Else
                    rect = rect.Union(block)
                End If
            Next

            Return rect
        End Function

        Public Function Statistics() As String
            Dim rect As Rect = Bounds()
            Dim sb As New StringBuilder()

            sb.AppendLine($"blocks       : {Blocks.Count}")
            sb.AppendLine($"extent       : {rect}")
            sb.AppendLine($"overlaps     : {Overlaps}")
            sb.AppendLine($"cost         : {Cost:0.##}")
            sb.AppendLine($"iterations   : {Iterations}")

            Return sb.ToString()
        End Function

        Public Overrides Function ToString() As String
            Return $"floorplan: {Blocks.Count} blocks, {Overlaps} overlaps, cost={Cost:0.##}"
        End Function

    End Class

    ''' <summary>
    ''' 启发式 floor-planner。
    ''' </summary>
    Public Class HeuristicFloorplanner

        ''' <summary>求解参数。</summary>
        Public Property Options As FloorplanOptions

        Private ReadOnly network As MetabolicNetwork
        Private ReadOnly skeleton As SkeletonResult
        Private ReadOnly rand As Random

        Private blocks As Dictionary(Of String, Rect)
        Private order As String()
        Private cycles As List(Of String())
        Private initialSides As Dictionary(Of String, AttachSide)
        Private skeletonAdjacency As Dictionary(Of String, List(Of String))
        Private averageSize As Double

        Public Sub New(network As MetabolicNetwork, skeleton As SkeletonResult, Optional options As FloorplanOptions = Nothing)
            Me.network = network
            Me.skeleton = skeleton
            Me.Options = If(options, New FloorplanOptions())
            Me.rand = If(Options.RandomSeed = 0, New Random(), New Random(Options.RandomSeed))
        End Sub

        ''' <summary>
        ''' 执行求解。
        ''' </summary>
        Public Function Run() As FloorplanResult
            BuildInitialLayout()
            ResolveAllOverlaps()

            Dim cost As Double = EvaluateCost()
            Dim bestCost As Double = cost
            Dim best As Dictionary(Of String, Rect) = CloneBlocks()

            Dim iterations As Integer = Math.Max(1, Options.AnnealIterations)
            Dim t0 As Double = Math.Max(1.0, cost * 0.02)
            Dim t1 As Double = Math.Max(1E-06, cost * 0.0002)
            Dim applied As Integer = 0

            For iteration As Integer = 1 To iterations
                Dim temperature As Double = t0 * Math.Pow(t1 / t0, iteration / iterations)
                Dim accepted As Boolean = False

                If rand.NextDouble() < 0.75 Then
                    accepted = TryAttachMove(temperature, cost)
                Else
                    accepted = TryJitterMove(temperature, cost)
                End If

                If accepted Then
                    applied += 1
                    cost = EvaluateCost()

                    If cost < bestCost Then
                        bestCost = cost
                        best = CloneBlocks()
                    End If
                End If

                If Options.Verbose AndAlso iteration Mod Math.Max(1, iterations \ 6) = 0 Then
                    Console.WriteLine($"[floorplan] iter {iteration}/{iterations} cost={cost:0.##} best={bestCost:0.##}")
                End If
            Next

            ' 回退到历史最优解
            blocks = best
            cost = EvaluateCost()

            ' 第二轮：尺寸微调（FH1/FH2/FS1/FS2）
            AdjustSizes()

            Dim overlaps As Integer = CountOverlaps()

            Return New FloorplanResult With {
                .Blocks = blocks,
                .Overlaps = overlaps,
                .Cost = cost,
                .Iterations = applied
            }
        End Function

        ''' <summary>
        ''' 按类别权重计算初始尺寸，并以骨架初始布局为参照放置。
        ''' </summary>
        Private Sub BuildInitialLayout()
            blocks = New Dictionary(Of String, Rect)(StringComparer.Ordinal)
            order = network.Categories.SafeQuery.Select(Function(c) c.Id).ToArray()

            Dim ratio As Double = Math.Max(0.1, Options.TargetAspectRatio)
            Dim rawSizes As New Dictionary(Of String, SizeF)(StringComparer.Ordinal)
            Dim totalSize As Double = 0

            For Each cat As Category In network.Categories.SafeQuery
                Dim weight As Double = Math.Max(1.0, cat.Weight)
                Dim area As Double = weight * Options.AreaPerWeight
                Dim height As Double = Math.Sqrt(area / ratio)
                Dim width As Double = height * ratio

                width = Math.Max(width, Options.MinBlockWidth)
                height = Math.Max(height, Options.MinBlockHeight)

                rawSizes(cat.Id) = New SizeF(CSng(width), CSng(height))
                totalSize += Math.Max(width, height)
            Next

            averageSize = If(order.Length = 0, 100, totalSize / order.Length)

            For Each cat As Category In network.Categories.SafeQuery
                Dim size As SizeF = rawSizes(cat.Id)
                blocks(cat.Id) = Rect.FromSize(0, 0, size.Width, size.Height)
            Next

            ' 骨架初始布局与区块尺寸之间做一个缩放，使两者尺度可比
            Dim positions As Dictionary(Of String, PointF) = skeleton.Positions
            Dim scale As Double = 1

            If positions IsNot Nothing AndAlso positions.Count > 1 Then
                Dim distances As New List(Of Double)()
                Dim ids As String() = order

                For i As Integer = 0 To ids.Length - 1
                    For j As Integer = i + 1 To ids.Length - 1
                        Dim a As PointF = skeleton.PositionOf(ids(i))
                        Dim b As PointF = skeleton.PositionOf(ids(j))
                        distances.Add(Math.Sqrt((a.X - b.X) ^ 2 + (a.Y - b.Y) ^ 2))
                    Next
                Next

                Dim positive As Double() = distances.Where(Function(d) d > 1E-06).ToArray()

                If positive.Length > 0 Then
                    Dim mean As Double = positive.Average()
                    Dim want As Double = averageSize * 1.35
                    scale = want / mean
                End If
            End If

            For Each id As String In order
                Dim pt As PointF = skeleton.PositionOf(id)
                Dim rect As Rect = blocks(id)
                rect.X = pt.X * scale
                rect.Y = pt.Y * scale
            Next

            ' 预计算 CH3 的参考方位（按方向键存储，保证 Source/Target 语义稳定）
            initialSides = New Dictionary(Of String, AttachSide)(StringComparer.Ordinal)

            For Each edge As SkeletonEdge In skeleton.Edges.SafeQuery
                Dim a As Rect = blocks(edge.Source)
                Dim b As Rect = blocks(edge.Target)
                initialSides(DirectedKey(edge.Source, edge.Target)) = ConfigSpaceHelper.DetermineSide(a, b)
            Next

            ' 预计算骨架邻接（用于提取基本环）
            skeletonAdjacency = New Dictionary(Of String, List(Of String))(StringComparer.Ordinal)

            For Each id As String In order
                skeletonAdjacency(id) = New List(Of String)()
            Next

            For Each edge As SkeletonEdge In skeleton.Edges.SafeQuery
                skeletonAdjacency(edge.Source).Add(edge.Target)
                skeletonAdjacency(edge.Target).Add(edge.Source)
            Next

            cycles = BuildFundamentalCycles()
        End Sub

        Private Shared Function EdgeKey(a As String, b As String) As String
            If String.CompareOrdinal(a, b) <= 0 Then
                Return $"{a}|{b}"
            Else
                Return $"{b}|{a}"
            End If
        End Function

        ''' <summary>有方向的边键：Source -&gt; Target。</summary>
        Private Shared Function DirectedKey(source As String, target As String) As String
            Return $"{source}->{target}"
        End Function

        ''' <summary>
        ''' 用树边路径 + 一条非树边构造基本环（CH4 的判定对象）。
        ''' </summary>
        Private Function BuildFundamentalCycles() As List(Of String())
            Dim result As New List(Of String())()
            Dim treeIds As New HashSet(Of String)(StringComparer.Ordinal)

            For Each edge As SkeletonEdge In skeleton.Edges.SafeQuery
                If edge.IsTreeEdge Then
                    treeIds.Add(EdgeKey(edge.Source, edge.Target))
                End If
            Next

            Dim treeAdjacency As New Dictionary(Of String, List(Of String))(StringComparer.Ordinal)

            For Each id As String In order
                treeAdjacency(id) = New List(Of String)()
            Next

            For Each edge As SkeletonEdge In skeleton.Edges.SafeQuery
                If edge.IsTreeEdge Then
                    treeAdjacency(edge.Source).Add(edge.Target)
                    treeAdjacency(edge.Target).Add(edge.Source)
                End If
            Next

            For Each edge As SkeletonEdge In skeleton.Edges.SafeQuery
                If edge.IsTreeEdge Then
                    Continue For
                End If

                Dim path As List(Of String) = FindPath(treeAdjacency, edge.Source, edge.Target)

                If path Is Nothing OrElse path.Count < 3 Then
                    Continue For
                End If

                result.Add(path.ToArray)
            Next

            Return result
        End Function

        Private Shared Function FindPath(adjacency As Dictionary(Of String, List(Of String)),
                                         start As String,
                                         [end] As String) As List(Of String)

            Dim parent As New Dictionary(Of String, String)(StringComparer.Ordinal)
            Dim queue As New Queue(Of String)()

            queue.Enqueue(start)
            parent(start) = Nothing

            While queue.Count > 0
                Dim u As String = queue.Dequeue()

                If String.Equals(u, [end], StringComparison.Ordinal) Then
                    Exit While
                End If

                For Each v As String In adjacency(u)
                    If parent.ContainsKey(v) Then
                        Continue For
                    End If

                    parent(v) = u
                    queue.Enqueue(v)
                Next
            End While

            If Not parent.ContainsKey([end]) Then
                Return Nothing
            End If

            Dim path As New List(Of String)()
            Dim cursor As String = [end]

            While cursor IsNot Nothing
                path.Add(cursor)

                Dim p As String = Nothing
                parent.TryGetValue(cursor, p)
                cursor = p
            End While

            path.Reverse()
            Return path
        End Function

        ''' <summary>
        ''' 全局重叠修复：反复把「较轻」的街区从重叠中推出。
        ''' </summary>
        Private Sub ResolveAllOverlaps()
            For pass As Integer = 1 To Math.Max(1, Options.RepairIterations)
                Dim moved As Boolean = False

                For i As Integer = 0 To order.Length - 1
                    For j As Integer = i + 1 To order.Length - 1
                        Dim a As Rect = blocks(order(i))
                        Dim b As Rect = blocks(order(j))

                        If a.OverlapArea(b) <= 0 Then
                            Continue For
                        End If

                        ' 让权重较小的街区让位，保持大区块稳定
                        Dim wa As Double = WeightOf(order(i))
                        Dim wb As Double = WeightOf(order(j))

                        If wa <= wb Then
                            PushApart(a, b)
                        Else
                            PushApart(b, a)
                        End If

                        moved = True
                    Next
                Next

                If Not moved Then
                    Return
                End If
            Next
        End Sub

        ''' <summary>
        ''' 把 mover 沿「最小重叠轴」推出，使其离开 anchor。
        ''' </summary>
        Private Shared Sub PushApart(mover As Rect, anchor As Rect)
            Dim ox As Double = Math.Min(mover.P, anchor.P) - Math.Max(mover.X, anchor.X)
            Dim oy As Double = Math.Min(mover.Q, anchor.Q) - Math.Max(mover.Y, anchor.Y)

            If ox <= 0 OrElse oy <= 0 Then
                Return
            End If

            Const epsilon As Double = 0.5

            If ox <= oy Then
                ' 水平方向推出
                If mover.Center.X >= anchor.Center.X Then
                    mover.X = anchor.P + epsilon
                Else
                    mover.X = anchor.X - mover.Width - epsilon
                End If
            Else
                If mover.Center.Y >= anchor.Center.Y Then
                    mover.Y = anchor.Q + epsilon
                Else
                    mover.Y = anchor.Y - mover.Height - epsilon
                End If
            End If
        End Sub

        ''' <summary>
        ''' 尝试把 blockId 移动到 proposed，并修复重叠；失败则回退。
        ''' </summary>
        Private Function TryApplyMove(blockId As String, proposed As Rect) As Boolean
            Dim original As Rect = blocks(blockId).Clone()
            blocks(blockId) = proposed

            If RepairBlock(blockId) Then
                Return True
            End If

            blocks(blockId) = original
            Return False
        End Function

        ''' <summary>
        ''' 只推动单个街区，直到它与其它街区都不重叠。其它街区保持不动，
        ''' 因此「无重叠」这一不变式得以维持。
        ''' </summary>
        Private Function RepairBlock(blockId As String) As Boolean
            For attempt As Integer = 1 To 64
                Dim target As Rect = blocks(blockId)
                Dim collided As Rect = Nothing

                For Each id As String In order
                    If String.Equals(id, blockId, StringComparison.Ordinal) Then
                        Continue For
                    End If

                    Dim other As Rect = blocks(id)

                    If target.OverlapArea(other) > 0 Then
                        collided = other
                        Exit For
                    End If
                Next

                If collided Is Nothing Then
                    Return True
                End If

                PushApart(target, collided)
            Next

            Return CountOverlapsFor(blockId) = 0
        End Function

        ''' <summary>
        ''' 贴附移动：把 bc(j) 吸附到 bc(i) 配置空间的某条线段上（CH1）。
        ''' </summary>
        Private Function TryAttachMove(temperature As Double, cost As Double) As Boolean
            If skeleton.Edges Is Nothing OrElse skeleton.Edges.Length = 0 Then
                Return False
            End If

            Dim edge As SkeletonEdge = skeleton.Edges(rand.Next(skeleton.Edges.Length))
            Dim anchor As Rect = blocks(edge.Source)
            Dim mover As Rect = blocks(edge.Target)

            Dim space As ConfigSpace = ConfigSpace.Create(anchor, mover.Width, mover.Height)

            ' 一半概率保持骨架初始方位（尊重 CH3），一半概率随机换边以探索
            Dim side As AttachSide

            If rand.NextDouble() < 0.5 Then
                Dim reference As AttachSide = AttachSide.Right
                initialSides.TryGetValue(DirectedKey(edge.Source, edge.Target), reference)
                side = reference
            Else
                side = CType(rand.Next(4), AttachSide)
            End If

            ' 沿可变轴对齐到两条街区中心，保持视觉整齐（CS3 长共享边）
            Dim anchorCenter As PointF = anchor.Center
            Dim snapped As PointF = space.Snap(side, anchorCenter.X, anchorCenter.Y)

            Dim proposed As New Rect With {
                .X = snapped.X,
                .Y = snapped.Y,
                .Width = mover.Width,
                .Height = mover.Height
            }

            If Not TryApplyMove(edge.Target, proposed) Then
                Return False
            End If

            Return AcceptByTemperature(cost, temperature)
        End Function

        ''' <summary>
        ''' 抖动移动：小幅平移一个街区，用于跳出局部最优。
        ''' </summary>
        Private Function TryJitterMove(temperature As Double, cost As Double) As Boolean
            If order.Length = 0 Then
                Return False
            End If

            Dim id As String = order(rand.Next(order.Length))
            Dim rect As Rect = blocks(id)
            Dim step_ As Double = averageSize * Options.JitterScale

            Dim proposed As New Rect With {
                .X = rect.X + (rand.NextDouble() - 0.5) * step_ * 2,
                .Y = rect.Y + (rand.NextDouble() - 0.5) * step_ * 2,
                .Width = rect.Width,
                .Height = rect.Height
            }

            If Not TryApplyMove(id, proposed) Then
                Return False
            End If

            Return AcceptByTemperature(cost, temperature)
        End Function

        ''' <summary>
        ''' Metropolis 接受准则：若新解更优则接受，否则按温度概率接受。
        ''' </summary>
        Private Function AcceptByTemperature(previousCost As Double, temperature As Double) As Boolean
            Dim current As Double = EvaluateCost()
            Dim delta As Double = current - previousCost

            If delta <= 0 Then
                Return True
            End If

            If temperature <= 0 Then
                Return False
            End If

            Return rand.NextDouble() < Math.Exp(-delta / temperature)
        End Function

        ''' <summary>
        ''' 目标函数：CH3/CH4 违例惩罚 + CS1/CS2/CS3 加权软目标。
        ''' </summary>
        Private Function EvaluateCost() As Double
            Dim minX As Double = Double.MaxValue
            Dim minY As Double = Double.MaxValue
            Dim maxX As Double = Double.MinValue
            Dim maxY As Double = Double.MinValue

            For Each rect As Rect In blocks.Values
                minX = Math.Min(minX, rect.X)
                minY = Math.Min(minY, rect.Y)
                maxX = Math.Max(maxX, rect.P)
                maxY = Math.Max(maxY, rect.Q)
            Next

            Dim bx As Double = maxX - minX
            Dim by As Double = maxY - minY

            ' CS1 紧凑 + CS2 期望长宽比
            Dim cost As Double = Options.WeightCompact * (bx + by)
            cost += Options.WeightRatio * Math.Abs(bx - Options.TargetAspectRatio * by)

            ' CS3 长共享边界：相邻街区中心位移越小，共享边越长
            For Each edge As SkeletonEdge In skeleton.Edges.SafeQuery
                Dim a As Rect = blocks(edge.Source)
                Dim b As Rect = blocks(edge.Target)
                cost += Options.WeightOverlay * (Math.Abs(a.Center.X - b.Center.X) + Math.Abs(a.Center.Y - b.Center.Y))
            Next

            ' CH3 成对相对位置
            For Each edge As SkeletonEdge In skeleton.Edges.SafeQuery
                Dim reference As AttachSide = AttachSide.Right
                initialSides.TryGetValue(DirectedKey(edge.Source, edge.Target), reference)

                Dim current As AttachSide = ConfigSpaceHelper.DetermineSide(blocks(edge.Source), blocks(edge.Target))

                If current <> reference Then
                    cost += Options.WeightRelative
                End If
            Next

            ' CH4 环重心保持
            For Each cycle As String() In cycles
                If Not BarycenterInside(cycle) Then
                    cost += Options.WeightBarycenter
                End If
            Next

            Return cost
        End Function

        ''' <summary>
        ''' 判断环的重心是否落在环内部（射线法）。
        ''' </summary>
        Private Function BarycenterInside(cycle As String()) As Boolean
            Dim polygon As New List(Of PointF)(cycle.Length)
            Dim cx As Double = 0
            Dim cy As Double = 0

            For Each id As String In cycle
                Dim center As PointF = blocks(id).Center
                polygon.Add(center)
                cx += center.X
                cy += center.Y
            Next

            cx /= cycle.Length
            cy /= cycle.Length

            Return PointInPolygon(polygon, cx, cy)
        End Function

        Private Shared Function PointInPolygon(polygon As List(Of PointF), x As Double, y As Double) As Boolean
            Dim inside As Boolean = False
            Dim count As Integer = polygon.Count

            For i As Integer = 0 To count - 1
                Dim pi As PointF = polygon(i)
                Dim j As Integer = If(i = 0, count - 1, i - 1)
                Dim pj As PointF = polygon(j)

                If ((pi.Y > y) <> (pj.Y > y)) AndAlso
                   (x < (pj.X - pi.X) * (y - pi.Y) / (pj.Y - pi.Y) + pi.X) Then
                    inside = Not inside
                End If
            Next

            Return inside
        End Function

        ''' <summary>
        ''' 第二轮：尺寸微调（FH1 / FH2 / FS1 / FS2）。
        ''' </summary>
        Private Sub AdjustSizes()
            If blocks.Count = 0 Then
                Return
            End If

            Dim extent As Rect = Nothing

            For Each rect As Rect In blocks.Values
                If extent Is Nothing Then
                    extent = rect.Clone()
                Else
                    extent = extent.Union(rect)
                End If
            Next

            Dim diagonal As Double = Math.Sqrt(extent.Width ^ 2 + extent.Height ^ 2)
            Dim margin As Double = diagonal * Options.DomainMarginRatio

            Dim domain As New Rect With {
                .X = extent.X - margin,
                .Y = extent.Y - margin,
                .Width = extent.Width + 2 * margin,
                .Height = extent.Height + 2 * margin
            }

            ' 先扩张大区块（面积权重高），符合 FS1「提高屏幕利用率」的意图
            Dim sequence As String() = order _
                .OrderByDescending(Function(id) WeightOf(id)) _
                .ToArray

            For Each id As String In sequence
                GrowBlock(id, domain)
            Next
        End Sub

        ''' <summary>
        ''' 向四个方向贪心扩张一个街区，直到碰到其它街区或地图域边界。
        ''' </summary>
        Private Sub GrowBlock(id As String, domain As Rect)
            Dim rect As Rect = blocks(id)
            Dim ratio0 As Double = rect.Width / Math.Max(1E-06, rect.Height)
            Dim limit As Double = Math.Max(1.0, Options.GrowthLimitRatio)

            ' FS1 面积最大化必须与 FS2「区块自身长宽比不剧烈变化」共同约束：
            ' 两个轴各最多放大 limit 倍，即面积最多放大 limit² 倍。
            ' 否则权重极小的街区会被一路扩张到地图边界，破坏「面积正比于类别规模」。
            Dim maxArea As Double = rect.Area * limit * limit

            For pass As Integer = 1 To 2
                ' 向右
                If rect.Width / Math.Max(1E-06, rect.Height) <= ratio0 * limit Then
                    Dim bound As Double = domain.P

                    For Each otherId As String In order
                        If String.Equals(otherId, id, StringComparison.Ordinal) Then
                            Continue For
                        End If

                        Dim other As Rect = blocks(otherId)

                        If other.Y < rect.Q AndAlso rect.Y < other.Q AndAlso other.X >= rect.P - 0.5 Then
                            bound = Math.Min(bound, other.X)
                        End If
                    Next

                    Dim delta As Double = ClampGrowth(bound - rect.P, maxArea / Math.Max(1E-06, rect.Height) - rect.Width)

                    If delta > 0 Then
                        rect.Width += delta
                    End If
                End If

                ' 向上
                If rect.Height / Math.Max(1E-06, rect.Width) <= (1.0 / ratio0) * limit Then
                    Dim bound As Double = domain.Q

                    For Each otherId As String In order
                        If String.Equals(otherId, id, StringComparison.Ordinal) Then
                            Continue For
                        End If

                        Dim other As Rect = blocks(otherId)

                        If other.X < rect.P AndAlso rect.X < other.P AndAlso other.Y >= rect.Q - 0.5 Then
                            bound = Math.Min(bound, other.Y)
                        End If
                    Next

                    Dim delta As Double = ClampGrowth(bound - rect.Q, maxArea / Math.Max(1E-06, rect.Width) - rect.Height)

                    If delta > 0 Then
                        rect.Height += delta
                    End If
                End If

                ' 向左
                If rect.Width / Math.Max(1E-06, rect.Height) <= ratio0 * limit Then
                    Dim bound As Double = domain.X

                    For Each otherId As String In order
                        If String.Equals(otherId, id, StringComparison.Ordinal) Then
                            Continue For
                        End If

                        Dim other As Rect = blocks(otherId)

                        If other.Y < rect.Q AndAlso rect.Y < other.Q AndAlso other.P <= rect.X + 0.5 Then
                            bound = Math.Max(bound, other.P)
                        End If
                    Next

                    Dim delta As Double = ClampGrowth(rect.X - bound, maxArea / Math.Max(1E-06, rect.Height) - rect.Width)

                    If delta > 0 Then
                        rect.X -= delta
                        rect.Width += delta
                    End If
                End If

                ' 向下
                If rect.Height / Math.Max(1E-06, rect.Width) <= (1.0 / ratio0) * limit Then
                    Dim bound As Double = domain.Y

                    For Each otherId As String In order
                        If String.Equals(otherId, id, StringComparison.Ordinal) Then
                            Continue For
                        End If

                        Dim other As Rect = blocks(otherId)

                        If other.X < rect.P AndAlso rect.X < other.P AndAlso other.Q <= rect.Y + 0.5 Then
                            bound = Math.Max(bound, other.Q)
                        End If
                    Next

                    Dim delta As Double = ClampGrowth(rect.Y - bound, maxArea / Math.Max(1E-06, rect.Width) - rect.Height)

                    If delta > 0 Then
                        rect.Y -= delta
                        rect.Height += delta
                    End If
                End If
            Next
        End Sub

        ''' <summary>把扩张量同时限制在「障碍物边界」与「面积上限」之内。</summary>
        Private Shared Function ClampGrowth(obstacleDelta As Double, areaDelta As Double) As Double
            If obstacleDelta <= 0 Then
                Return 0
            End If

            Return Math.Min(obstacleDelta, Math.Max(0, areaDelta))
        End Function

        Private Function WeightOf(id As String) As Double
            Dim cat As Category = network.GetCategory(id)
            Return If(cat Is Nothing, 1.0, Math.Max(1.0, cat.Weight))
        End Function

        Private Function CountOverlaps() As Integer
            Dim count As Integer = 0

            For i As Integer = 0 To order.Length - 1
                For j As Integer = i + 1 To order.Length - 1
                    If blocks(order(i)).OverlapArea(blocks(order(j))) > 0 Then
                        count += 1
                    End If
                Next
            Next

            Return count
        End Function

        Private Function CountOverlapsFor(id As String) As Integer
            Dim count As Integer = 0

            For Each other As String In order
                If String.Equals(other, id, StringComparison.Ordinal) Then
                    Continue For
                End If

                If blocks(id).OverlapArea(blocks(other)) > 0 Then
                    count += 1
                End If
            Next

            Return count
        End Function

        Private Function CloneBlocks() As Dictionary(Of String, Rect)
            Dim copy As New Dictionary(Of String, Rect)(StringComparer.Ordinal)

            For Each item As KeyValuePair(Of String, Rect) In blocks
                copy(item.Key) = item.Value.Clone()
            Next

            Return copy
        End Function

    End Class

End Namespace

#End Region
