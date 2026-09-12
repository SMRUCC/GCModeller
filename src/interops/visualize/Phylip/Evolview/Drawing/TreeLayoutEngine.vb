Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging

Namespace Evolview.Drawing

    ''' <summary>
    ''' 进化树布局引擎：把 <see cref="PhyloTree"/> 的拓扑与几何量转换为画布坐标。
    ''' </summary>
    ''' <remarks>
    ''' 实现分两步：先以「单位坐标」计算（矩形布局为水平单位 + 垂直层；圆形布局为半径单位 + 极角），
    ''' 再统一映射到画布（自适应缩放或用户指定的单位像素）。这样可以保证线宽、字号等
    ''' 装饰参数不随几何缩放而变化，并支持内容自适应。
    '''
    ''' 8 种布局的几何公式迁移自原始 SVG 实现（TreeSkeleton），退化情形（无分支长度、
    ''' 单叶/两叶、几何量缺失）均安全降级。
    ''' </remarks>
    Public Module TreeLayoutEngine

        ''' <summary>
        ''' 计算给定树的布局。
        ''' </summary>
        ''' <param name="tree">待绘制的树</param>
        ''' <param name="options">绘制配置</param>
        ''' <param name="graphics">用于文本尺寸测量的画布</param>
        Public Function Compute(tree As PhyloTree,
                                options As TreeDrawingOptions,
                                graphics As IGraphics) As TreeLayoutResult

            If tree Is Nothing OrElse tree.RootNode Is Nothing Then
                Throw New ArgumentNullException(NameOf(tree), "待绘制的进化树为空！")
            End If
            If graphics Is Nothing Then
                Throw New ArgumentNullException(NameOf(graphics))
            End If

            If options Is Nothing Then
                options = TreeDrawingOptions.Defaults()
            End If

            Dim root As PhyloNode = tree.RootNode
            Dim nodes As List(Of PhyloNode) = DrawingHelper.EnumerateNodes(root)
            Dim leaves As List(Of PhyloNode) = nodes.Where(Function(n) n.Descendents.Count = 0).AsList
            Dim maxVertical As Integer = Math.Max(1, tree.maxVerticalLevel)
            Dim hasBranchLength As Boolean = tree.HasBranchLength()
            Dim phylogram As Boolean = IsPhylogramMode(options.Mode) AndAlso hasBranchLength

            If IsPhylogramMode(options.Mode) AndAlso Not hasBranchLength Then
                Console.WriteLine($"[TreeDrawing] 树没有分支长度信息，{options.Mode} 自动降级为分支图（cladogram）样式。")
            End If

            Dim measure As New TextMeasureCache(graphics)
            Dim leafFont As Font = ResolveFont(options.LeafFont, 10)
            Dim fontHeight As Single = measure.Measure("Ag", leafFont).Height
            Dim maxLabelWidth As Single = 0

            If options.ShowLeafLabels Then
                For Each leaf As PhyloNode In leaves
                    Dim width As Single = measure.Measure(FormatLeafText(leaf), leafFont).Width

                    If width > maxLabelWidth Then
                        maxLabelWidth = width
                    End If
                Next
            End If

            If IsCircularMode(options.Mode) Then
                Return ComputeCircular(root, nodes, leaves, maxVertical, phylogram, options, measure, fontHeight, maxLabelWidth)
            Else
                Return ComputeRectangular(root, nodes, leaves, maxVertical, phylogram, options, measure, fontHeight, maxLabelWidth)
            End If
        End Function

        Public Function IsCircularMode(mode As TreePlotMode) As Boolean
            Return mode = TreePlotMode.CIRCULAR_CLADOGRAM OrElse
                mode = TreePlotMode.CIRCULAR_PHYLOGRAM OrElse
                mode = TreePlotMode.RADIAL_CLADOGRAM
        End Function

        Public Function IsSlantedMode(mode As TreePlotMode) As Boolean
            Return mode = TreePlotMode.SLANTED_CLADOGRAM_RECT OrElse
                mode = TreePlotMode.SLANTED_CLADOGRAM_MIDDLE OrElse
                mode = TreePlotMode.SLANTED_CLADOGRAM_NORMAL
        End Function

        Public Function IsPhylogramMode(mode As TreePlotMode) As Boolean
            Return mode = TreePlotMode.RECT_PHYLOGRAM OrElse mode = TreePlotMode.CIRCULAR_PHYLOGRAM
        End Function

#Region "矩形 / 斜线布局"

        Private Function ComputeRectangular(root As PhyloNode,
                                            nodes As List(Of PhyloNode),
                                            leaves As List(Of PhyloNode),
                                            maxVertical As Integer,
                                            phylogram As Boolean,
                                            options As TreeDrawingOptions,
                                            measure As TextMeasureCache,
                                            fontHeight As Single,
                                            maxLabelWidth As Single) As TreeLayoutResult

            Dim result As New TreeLayoutResult With {
                .Mode = options.Mode,
                .EdgeStyle = If(IsSlantedMode(options.Mode), TreeEdgeStyle.Straight, TreeEdgeStyle.Elbow)
            }

            ' ---- 1. 单位坐标 ----
            Dim unitX As New Dictionary(Of PhyloNode, Double)
            Dim unitY As New Dictionary(Of PhyloNode, Double)
            Dim depth As New Dictionary(Of PhyloNode, Double)

            ' 先序遍历保证父节点先于子节点被处理，从而可以增量累计深度
            For Each node As PhyloNode In nodes
                Dim d As Double = 0

                If node.Parent IsNot Nothing AndAlso depth.ContainsKey(node.Parent) Then
                    d = depth(node.Parent) + Math.Max(0, node.BranchLength)
                End If

                depth(node) = d
            Next

            Dim xExtent As Double = If(phylogram, depth.Values.Max(), Math.Max(1.0, root.LevelHorizontal - 1))
            xExtent = Math.Max(xExtent, 1.0E-09)

            For Each node As PhyloNode In nodes
                Dim uy As Double

                Select Case options.Mode
                    Case TreePlotMode.SLANTED_CLADOGRAM_MIDDLE
                        uy = node.LevelVerticalSlanted
                    Case TreePlotMode.SLANTED_CLADOGRAM_NORMAL
                        uy = If(node.Descendents.Count = 0,
                                node.LevelVertical,
                                (node.minLeafVerticalLevel + node.maxLeafVerticalLevel) / 2)
                    Case Else
                        uy = node.LevelVertical
                End Select

                Dim ux As Double

                If options.Mode = TreePlotMode.SLANTED_CLADOGRAM_NORMAL Then
                    ' 叶节点统一落在最右侧，内部节点稍后依据倾角修正
                    ux = If(node.Descendents.Count = 0, xExtent, 0)
                ElseIf phylogram Then
                    ux = depth(node)
                Else
                    ux = root.LevelHorizontal - node.LevelHorizontal
                End If

                unitX(node) = ux
                unitY(node) = uy
            Next

            ' ---- 2. 缩放 ----
            Dim labelSpace As Single = If(options.ShowLeafLabels, maxLabelWidth + options.LabelGap, 0)
            Dim availableWidth As Double = options.CanvasSize.Width - options.MarginHorizontal - labelSpace
            Dim availableHeight As Double = options.CanvasSize.Height - options.MarginVertical

            Dim pxPerX As Double
            Dim pxPerY As Double

            If options.UnitSize.HasValue Then
                pxPerX = options.UnitSize.Value.Width
                pxPerY = options.UnitSize.Value.Height
            Else
                pxPerX = availableWidth / xExtent
                pxPerY = availableHeight / maxVertical
            End If

            If pxPerX <= 0 Then
                pxPerX = 1
            End If
            If pxPerY <= 0 Then
                pxPerY = 1
            End If

            ' SLANTED_CLADOGRAM_NORMAL：内部节点的 X 由其首末叶的垂直跨度与全局倾角决定
            If options.Mode = TreePlotMode.SLANTED_CLADOGRAM_NORMAL Then
                Dim leafX As Double = options.MarginLeft + xExtent * pxPerX
                Dim rootY As Double = options.MarginTop + (root.LevelVertical - 0.5) * pxPerY
                Dim firstLeafY As Double = options.MarginTop + 0.5 * pxPerY
                Dim run As Double = Math.Max(1.0E-06, leafX - options.MarginLeft)
                Dim inclination As Double = Math.Atan(Math.Abs(rootY - firstLeafY) / run)

                ' 倾角过小时 tan 趋于 0，会导致内部节点 X 发散，这里给出下限
                If inclination < 0.05 Then
                    inclination = 0.05
                End If

                For Each node As PhyloNode In nodes
                    If node IsNot root AndAlso node.Descendents.Count > 0 Then
                        Dim span As Double = node.maxLeafVerticalLevel - node.minLeafVerticalLevel
                        Dim offset As Double = (span / 2 * pxPerY) / (Math.Tan(inclination) * pxPerX)

                        unitX(node) = Math.Max(0, xExtent - offset)
                    End If
                Next
            End If

            ' ---- 3. 映射到画布 ----
            For Each node As PhyloNode In nodes
                Dim x As Single = CSng(options.MarginLeft + unitX(node) * pxPerX)
                Dim y As Single = CSng(options.MarginTop + (unitY(node) - 0.5) * pxPerY)

                result.Nodes(node) = New NodeLayout With {
                    .Node = node,
                    .Position = New PointF(x, y)
                }
            Next

            For Each node As PhyloNode In nodes
                Dim layout As NodeLayout = result.Nodes(node)
                Dim parentLayout As NodeLayout = result.GetLayout(node.Parent)

                layout.ParentPosition = If(parentLayout IsNot Nothing, parentLayout.Position, layout.Position)
            Next

            ' ---- 4. 叶标签与装饰位置 ----
            Dim maxLeafX As Single = 0

            For Each leaf As PhyloNode In leaves
                If result.Nodes(leaf).Position.X > maxLeafX Then
                    maxLeafX = result.Nodes(leaf).Position.X
                End If
            Next

            Dim leafFont As Font = ResolveFont(options.LeafFont, 10)
            Dim background As RectangleF? = Nothing

            For Each leaf As PhyloNode In leaves
                Dim layout As NodeLayout = result.Nodes(leaf)
                Dim labelX As Single = If(options.AlignLeafLabels, maxLeafX + options.LabelGap, layout.Position.X + options.LabelGap)

                layout.LabelPosition = New PointF(labelX, layout.Position.Y - fontHeight / 2)
                layout.LabelAngle = 0
                layout.LabelRightAligned = False

                If options.ShowLeafLabels Then
                    Dim textWidth As Single = measure.Measure(FormatLeafText(leaf), leafFont).Width
                    Dim rect As New RectangleF(
                        labelX - options.LabelGap / 2,
                        layout.Position.Y - CSng(pxPerY) / 2,
                        textWidth + options.LabelGap,
                        CSng(pxPerY))

                    layout.BackgroundRect = rect
                    background = DrawingHelper.Union(background, rect)
                End If
            Next

            For Each node As PhyloNode In nodes
                If node Is root OrElse node.Descendents.Count = 0 Then
                    Continue For
                End If

                Dim layout As NodeLayout = result.Nodes(node)

                layout.BootstrapPosition = New PointF(layout.Position.X + 3, layout.Position.Y - fontHeight / 2)
                layout.BootstrapAngle = 0

                If IsSlantedMode(options.Mode) Then
                    layout.BranchLengthPosition = New PointF(
                        (layout.ParentPosition.X + layout.Position.X) / 2,
                        (layout.ParentPosition.Y + layout.Position.Y) / 2)
                Else
                    layout.BranchLengthPosition = New PointF(
                        (layout.ParentPosition.X + layout.Position.X) / 2,
                        layout.Position.Y)
                End If

                layout.BranchLengthAngle = 0
            Next

            ' ---- 5. 包围盒 ----
            Dim bounds As RectangleF? = Nothing

            For Each node As PhyloNode In nodes
                Dim p As PointF = result.Nodes(node).Position
                bounds = DrawingHelper.Union(bounds, New RectangleF(p, New SizeF(0, 0)))
            Next

            If background.HasValue Then
                bounds = DrawingHelper.Union(bounds, background.Value)
            End If

            result.Bounds = bounds.GetValueOrDefault()
            result.RootPosition = result.Nodes(root).Position
            result.Center = result.RootPosition
            result.PixelsPerUnitX = CSng(pxPerX)
            result.PixelsPerUnitY = CSng(pxPerY)
            result.PixelsPerRadius = 0
            result.BranchLengthScale = If(phylogram, CSng(pxPerX), 0)

            Return result
        End Function

#End Region

#Region "圆形 / 辐射布局"

        Private Function ComputeCircular(root As PhyloNode,
                                         nodes As List(Of PhyloNode),
                                         leaves As List(Of PhyloNode),
                                         maxVertical As Integer,
                                         phylogram As Boolean,
                                         options As TreeDrawingOptions,
                                         measure As TextMeasureCache,
                                         fontHeight As Single,
                                         maxLabelWidth As Single) As TreeLayoutResult

            Dim result As New TreeLayoutResult With {
                .Mode = options.Mode,
                .EdgeStyle = If(options.Mode = TreePlotMode.RADIAL_CLADOGRAM, TreeEdgeStyle.Straight, TreeEdgeStyle.Arc)
            }

            ' ---- 1. 半径单位 ----
            Dim radiusUnit As New Dictionary(Of PhyloNode, Double)
            Dim maxRadiusUnit As Double = 0

            For Each node As PhyloNode In nodes
                Dim r As Double

                If phylogram Then
                    r = DrawingHelper.NodeDepth(node)
                Else
                    r = Math.Max(0, root.LevelHorizontal - node.LevelHorizontal)
                End If

                radiusUnit(node) = r

                If r > maxRadiusUnit Then
                    maxRadiusUnit = r
                End If
            Next

            maxRadiusUnit = Math.Max(maxRadiusUnit, 1.0E-09)

            ' ---- 2. 缩放与圆心 ----
            Dim labelSpace As Single = If(options.ShowLeafLabels, maxLabelWidth + options.LabelGap, 0) + fontHeight
            Dim contentWidth As Double = options.CanvasSize.Width - options.MarginHorizontal
            Dim contentHeight As Double = options.CanvasSize.Height - options.MarginVertical
            Dim center As New PointF(
                CSng(options.MarginLeft + contentWidth / 2),
                CSng(options.MarginTop + contentHeight / 2))

            Dim availableRadius As Double = Math.Min(contentWidth, contentHeight) / 2 - labelSpace

            If availableRadius < 10 Then
                availableRadius = Math.Max(10, Math.Min(contentWidth, contentHeight) / 2)
            End If

            Dim pxPerRadius As Double

            If options.UnitSize.HasValue Then
                pxPerRadius = (options.UnitSize.Value.Width + options.UnitSize.Value.Height) / 2
            Else
                pxPerRadius = availableRadius / maxRadiusUnit
            End If

            If pxPerRadius <= 0 Then
                pxPerRadius = 1
            End If

            Dim angleSpan As Double = options.EffectiveAngleSpan()
            Dim anglePerLevel As Double = angleSpan / Math.Max(1, maxVertical)
            Dim direction As Double = If(options.Clockwise, -1.0, 1.0)
            Dim maxRadiusPixels As Double = 0

            ' ---- 3. 极坐标映射 ----
            For Each node As PhyloNode In nodes
                Dim radius As Double = radiusUnit(node) * pxPerRadius
                Dim thetaDeg As Double = options.AngleStart + direction * (node.LevelVertical - 1) * anglePerLevel
                Dim theta As Double = thetaDeg * Math.PI / 180.0
                Dim position As PointF = DrawingHelper.PolarToCartesian(center, CSng(radius), CSng(theta))

                result.Nodes(node) = New NodeLayout With {
                    .Node = node,
                    .Position = position,
                    .Radius = CSng(radius),
                    .Theta = CSng(theta)
                }

                If radius > maxRadiusPixels Then
                    maxRadiusPixels = radius
                End If
            Next

            For Each node As PhyloNode In nodes
                Dim layout As NodeLayout = result.Nodes(node)
                Dim parentLayout As NodeLayout = result.GetLayout(node.Parent)

                If parentLayout IsNot Nothing Then
                    layout.ParentPosition = parentLayout.Position
                    layout.ParentRadius = parentLayout.Radius
                    layout.ParentTheta = parentLayout.Theta
                Else
                    layout.ParentPosition = layout.Position
                    layout.ParentRadius = layout.Radius
                    layout.ParentTheta = layout.Theta
                End If
            Next

            ' ---- 4. 标签与装饰位置 ----
            Dim leafFont As Font = ResolveFont(options.LeafFont, 10)

            For Each leaf As PhyloNode In leaves
                Dim layout As NodeLayout = result.Nodes(leaf)
                Dim rightAligned As Boolean = Math.Cos(layout.Theta) < 0
                Dim angleDeg As Single = CSng(-layout.Theta * 180.0 / Math.PI)

                If rightAligned Then
                    angleDeg += 180.0F
                End If

                Dim offset As Double = layout.Radius + options.LabelGap
                layout.LabelPosition = New PointF(
                    CSng(center.X + offset * Math.Cos(layout.Theta)),
                    CSng(center.Y - offset * Math.Sin(layout.Theta)))
                layout.LabelAngle = DrawingHelper.NormalizeDegrees(angleDeg)
                layout.LabelRightAligned = rightAligned

                If options.ShowLeafLabels Then
                    Dim textWidth As Single = measure.Measure(FormatLeafText(leaf), leafFont).Width
                    Dim halfSlot As Double = (anglePerLevel / 2) * Math.PI / 180.0
                    Dim innerRadius As Single = layout.ParentRadius
                    Dim outerRadius As Single = layout.Radius + options.LabelGap + textWidth

                    layout.BackgroundFan = DrawingHelper.BuildAnnularSector(
                        center, innerRadius, outerRadius,
                        CSng(layout.Theta - halfSlot), CSng(layout.Theta + halfSlot), 6)
                End If
            Next

            Dim fontHeightHalf As Single = fontHeight / 2

            For Each node As PhyloNode In nodes
                If node Is root OrElse node.Descendents.Count = 0 Then
                    Continue For
                End If

                Dim layout As NodeLayout = result.Nodes(node)
                Dim rightAligned As Boolean = Math.Cos(layout.Theta) < 0
                Dim angleDeg As Single = CSng(-layout.Theta * 180.0 / Math.PI)

                If rightAligned Then
                    angleDeg += 180.0F
                End If

                layout.BootstrapPosition = New PointF(layout.Position.X + 3, layout.Position.Y - fontHeightHalf)
                layout.BootstrapAngle = DrawingHelper.NormalizeDegrees(angleDeg)

                Dim midRadius As Single = (layout.Radius + layout.ParentRadius) / 2

                layout.BranchLengthPosition = DrawingHelper.PolarToCartesian(center, midRadius, layout.Theta)
                layout.BranchLengthAngle = DrawingHelper.NormalizeDegrees(angleDeg)
            Next

            ' ---- 5. 包围盒 ----
            Dim extent As Single = CSng(maxRadiusPixels + labelSpace)

            result.Bounds = New RectangleF(center.X - extent, center.Y - extent, extent * 2, extent * 2)
            result.RootPosition = result.Nodes(root).Position
            result.Center = center
            result.PixelsPerUnitX = 0
            result.PixelsPerUnitY = 0
            result.PixelsPerRadius = CSng(pxPerRadius)
            result.BranchLengthScale = If(phylogram, CSng(pxPerRadius), 0)

            Return result
        End Function

#End Region

        Private Function ResolveFont(font As Font, Optional defaultSize As Single = 10) As Font
            If font IsNot Nothing Then
                Return font
            End If

            Return DrawingHelper.CreateFont(FontFace.SegoeUI, defaultSize)
        End Function

        ''' <summary>
        ''' 叶标签显示文本：优先使用 <see cref="PhyloNode.ID"/>，为空时退回内部编号。
        ''' </summary>
        Public Function FormatLeafText(node As PhyloNode) As String
            If Not String.IsNullOrEmpty(node.ID) Then
                Return node.ID
            ElseIf Not String.IsNullOrEmpty(node.InternalID) Then
                Return node.InternalID
            Else
                Return ""
            End If
        End Function
    End Module

End Namespace
