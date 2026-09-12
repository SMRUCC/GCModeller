#Region "Metabopolis: pipeline"

' ============================================================================
' Metabopolis 布局流水线（顶层编排）
' ----------------------------------------------------------------------------
' 把论文的五步流水线串起来：
'     数据模型 -> 预处理 -> 图骨架 -> 启发式 floor-planning
'              -> 块内布局 + 局部流网络 -> 块间流网络 -> 渲染
'
' 每一步都可以单独调用（各阶段类型均为 Public），便于调试与回归测试；
' 这里提供的是把它们串成一次完整布局的便捷入口。
' ============================================================================

Imports System.Diagnostics
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports Metabopolis.Floorplan
Imports Metabopolis.Model
Imports Metabopolis.Preprocess
Imports Metabopolis.Routing
Imports Metabopolis.Skeleton

''' <summary>
''' 整条流水线的参数。
''' </summary>
Public Class MetabopolisOptions

    ''' <summary>输出画布宽度（像素）。</summary>
    Public Property CanvasWidth As Double = 2400

    ''' <summary>输出画布高度（像素）。</summary>
    Public Property CanvasHeight As Double = 1800

    ''' <summary>画布边距（像素）。</summary>
    Public Property Margin As Double = 40

    ''' <summary>预处理参数。</summary>
    Public Property Preprocess As PreprocessOptions

    ''' <summary>骨架参数。</summary>
    Public Property Skeleton As New GraphSkeletonBuilder()

    ''' <summary>floor-planning 参数。</summary>
    Public Property Floorplan As FloorplanOptions

    ''' <summary>块内路由参数。</summary>
    Public Property Routing As RoutingOptions

    ''' <summary>块间路由参数。</summary>
    Public Property InterBlock As InterBlockOptions

    ''' <summary>是否输出阶段日志。</summary>
    Public Property Verbose As Boolean = True

    ''' <summary>布局结果的 JSON 快照输出路径（为空则不输出）。</summary>
    Public Property SnapshotPath As String

End Class

''' <summary>
''' Metabopolis 布局流水线。
''' </summary>
Public Class MetabopolisLayout

    ''' <summary>
    ''' 对一张代谢网络执行完整布局，返回渲染层可直接使用的布局结果。
    ''' </summary>
    Public Shared Function Run(network As MetabolicNetwork, Optional options As MetabopolisOptions = Nothing) As NetworkLayout
        If network Is Nothing Then
            Throw New ArgumentNullException(NameOf(network))
        End If

        Dim opts As MetabopolisOptions = If(options, New MetabopolisOptions())
        Dim meta As New Dictionary(Of String, String)(StringComparer.Ordinal)
        Dim watch As Stopwatch = Stopwatch.StartNew()

        Log(opts, $"network: {network}")

        If network.TotalCategoryCount = 0 Then
            Return New NetworkLayout With {
                .CanvasWidth = opts.CanvasWidth,
                .CanvasHeight = opts.CanvasHeight,
                .Blocks = New BlockLayout() {},
                .Buildings = New BuildingBlock() {},
                .Routes = New RoutePolyline() {},
                .Junctions = New Junction() {},
                .Meta = meta
            }
        End If

        ' ---- 1. 预处理：节点复制 / 枢纽判定 / 九色角色 ----
        watch.Restart()
        Dim preprocessor As New NetworkPreprocessor(If(opts.Preprocess, New PreprocessOptions()))
        Dim preprocessed As PreprocessedNetwork = preprocessor.Process(network)
        meta("preprocess.ms") = watch.ElapsedMilliseconds.ToString()
        Log(opts, $"preprocess: {preprocessed}")

        ' ---- 2. 图骨架 ----
        watch.Restart()
        Dim skeleton As SkeletonResult = opts.Skeleton.Build(network)
        meta("skeleton.ms") = watch.ElapsedMilliseconds.ToString()
        Log(opts, $"skeleton: {If(skeleton.Edges Is Nothing, 0, skeleton.Edges.Length)} edges, {skeleton.Components?.Length} components")

        ' ---- 3. 启发式 floor-planning ----
        watch.Restart()
        Dim planner As New HeuristicFloorplanner(network, skeleton, If(opts.Floorplan, New FloorplanOptions()))
        Dim floorplan As FloorplanResult = planner.Run()
        meta("floorplan.ms") = watch.ElapsedMilliseconds.ToString()
        meta("floorplan.overlaps") = floorplan.Overlaps.ToString()
        Log(opts, $"floorplan: {floorplan}")

        Dim blockRects As Dictionary(Of String, Rect) = floorplan.Blocks

        ' ---- 4. 块内布局 + 局部流网络 ----
        watch.Restart()
        Dim intra As New IntraBlockRouter(preprocessed, blockRects, If(opts.Routing, New RoutingOptions()))
        Dim buildings As New List(Of BuildingBlock)()
        Dim junctions As New List(Of Junction)()
        Dim routes As New List(Of RoutePolyline)()
        Dim junctionIndex As New Dictionary(Of String, Junction)(StringComparer.Ordinal)

        For Each category As Category In network.Categories.SafeQuery
            If opts.Verbose Then
                Console.Out.WriteLine($"[metabopolis] intra-block: {category.Id} ({category.ReactionCount} reactions)")
                Console.Out.Flush()
            End If

            Dim result As BlockRoutingResult = intra.Route(category.Id)

            buildings.AddRange(result.Buildings.SafeQuery)
            junctions.AddRange(result.Junctions.SafeQuery)
            routes.AddRange(result.Routes.SafeQuery)

            For Each junction As Junction In result.Junctions.SafeQuery
                If Not junctionIndex.ContainsKey(junction.Id) Then
                    junctionIndex.Add(junction.Id, junction)
                End If
            Next
        Next

        meta("intra.ms") = watch.ElapsedMilliseconds.ToString()
        Log(opts, $"intra-block: {buildings.Count} buildings, {junctions.Count} junctions, {routes.Count} lanes")

        ' ---- 5. 块间流网络 ----
        watch.Restart()
        Dim inter As New InterBlockRouter(preprocessed, blockRects, junctionIndex, If(opts.InterBlock, New InterBlockOptions()))
        Dim globalRoutes As RoutePolyline() = inter.Route()
        routes.AddRange(globalRoutes.SafeQuery)
        meta("inter.ms") = watch.ElapsedMilliseconds.ToString()
        Log(opts, $"inter-block: {globalRoutes.Length} routes")

        ' ---- 6. 组装布局结果 ----
        Dim blocks As BlockLayout() = BuildBlocks(network, blockRects)

        Dim layout As New NetworkLayout With {
            .CanvasWidth = opts.CanvasWidth,
            .CanvasHeight = opts.CanvasHeight,
            .Blocks = blocks,
            .Buildings = buildings.ToArray,
            .Routes = routes.ToArray,
            .Junctions = junctions.ToArray,
            .Meta = meta
        }

        ' 统一把布局几何归一化到画布内
        layout.Normalize(opts.Margin)

        meta("total.ms") = watch.ElapsedMilliseconds.ToString()
        meta("metabolites") = network.TotalCompoundCount.ToString()
        meta("reactions") = network.TotalReactionCount.ToString()
        meta("categories") = network.TotalCategoryCount.ToString()

        If Not String.IsNullOrEmpty(opts.SnapshotPath) Then
            Call layout.ToJson(opts.SnapshotPath)
            Log(opts, $"snapshot: {opts.SnapshotPath}")
        End If

        Log(opts, layout.Statistics())
        Log(opts, $"total: {watch.ElapsedMilliseconds} ms")

        Return layout
    End Function

    Private Shared Function BuildBlocks(network As MetabolicNetwork, rects As Dictionary(Of String, Rect)) As BlockLayout()
        Dim result As New List(Of BlockLayout)()
        Dim index As Integer = 0

        For Each category As Category In network.Categories.SafeQuery
            Dim rect As Rect = Nothing

            If Not rects.TryGetValue(category.Id, rect) Then
                index += 1
                Continue For
            End If

            result.Add(New BlockLayout With {
                .CategoryId = category.Id,
                .Label = If(category.Name, category.Id),
                .X = rect.X,
                .Y = rect.Y,
                .Width = rect.Width,
                .Height = rect.Height,
                .Weight = category.Weight,
                .ColorIndex = index
            })

            index += 1
        Next

        Return result.ToArray
    End Function

    Private Shared Sub Log(options As MetabopolisOptions, message As String)
        If options.Verbose Then
            Console.WriteLine($"[metabopolis] {message}")
        End If
    End Sub

End Class

#End Region
