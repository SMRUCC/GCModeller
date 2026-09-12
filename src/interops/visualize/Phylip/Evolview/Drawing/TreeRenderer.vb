Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging

Namespace Evolview.Drawing

    ''' <summary>
    ''' 基于 GDI+（<see cref="IGraphics"/>）的进化树渲染器：只消费 <see cref="TreeLayoutResult"/>，
    ''' 负责绘制分支、节点标记、叶标签、bootstrap 支持度、分支长度、叶背景色块、标题与比例尺。
    ''' </summary>
    ''' <remarks>
    ''' 字体 / 画笔 / 画刷在实例内集中创建并按颜色缓存，实例释放时统一销毁，
    ''' 因此本类型应配合 <c>Using</c> 使用；外部传入的字体不会被释放。
    ''' </remarks>
    Public Class TreeRenderer : Implements IDisposable

        Private ReadOnly _tree As PhyloTree
        Private ReadOnly _options As TreeDrawingOptions

        Private ReadOnly _ownedFonts As New List(Of Font)
        Private ReadOnly _penCache As New Dictionary(Of Integer, Pen)
        Private ReadOnly _brushCache As New Dictionary(Of Integer, SolidBrush)

        Private ReadOnly _leafFont As Font
        Private ReadOnly _bootstrapFont As Font
        Private ReadOnly _branchLengthFont As Font
        Private ReadOnly _titleFont As Font

        Private _bootstrapData As Boolean?
        Private _disposed As Boolean

        Public Sub New(tree As PhyloTree, options As TreeDrawingOptions)
            If tree Is Nothing Then
                Throw New ArgumentNullException(NameOf(tree))
            End If

            Me._tree = tree
            Me._options = If(options, TreeDrawingOptions.Defaults())

            Me._leafFont = ResolveFont(_options.LeafFont, 10)
            Me._bootstrapFont = ResolveFont(_options.BootstrapFont, 8)
            Me._branchLengthFont = ResolveFont(_options.BranchLengthFont, 8)
            Me._titleFont = ResolveFont(_options.TitleFont, 14)
        End Sub

        Public ReadOnly Property Options As TreeDrawingOptions
            Get
                Return _options
            End Get
        End Property

        ''' <summary>
        ''' 渲染整棵树。
        ''' </summary>
        Public Sub Render(g As IGraphics, layout As TreeLayoutResult)
            If g Is Nothing Then
                Throw New ArgumentNullException(NameOf(g))
            End If
            If layout Is Nothing Then
                Throw New ArgumentNullException(NameOf(layout))
            End If

            If _options.Background.A > 0 Then
                Call g.Clear(_options.Background)
            End If

            ' 叶背景先绘制，避免遮盖骨架
            Call DrawLeafBackgrounds(g, layout)

            Call DrawEdges(g, layout)

            If _options.ShowNodeMarkers Then
                Call DrawNodeMarkers(g, layout)
            End If

            If _options.ShowBranchLength AndAlso _tree.HasBranchLength() Then
                Call DrawBranchLengths(g, layout)
            End If

            If _options.ShowBootstrap AndAlso HasBootstrapData() Then
                Call DrawBootstrap(g, layout)
            End If

            If _options.ShowLeafLabels Then
                Call DrawLeafLabels(g, layout)
            End If

            Call DrawTitle(g, layout)
            Call DrawScaleBar(g, layout)

            Call g.Flush()
        End Sub

#Region "分支与节点"

        Private Sub DrawEdges(g As IGraphics, layout As TreeLayoutResult)
            For Each pair As KeyValuePair(Of PhyloNode, NodeLayout) In layout.Nodes
                Dim node As PhyloNode = pair.Key

                If node.IsRoot OrElse node.Parent Is Nothing Then
                    Continue For
                End If

                Dim item As NodeLayout = pair.Value
                Dim pen As Pen = GetPen(ResolveBranchColor(node))

                Select Case layout.EdgeStyle
                    Case TreeEdgeStyle.Elbow
                        ' 直角折线：先竖直后水平
                        If Math.Abs(item.ParentPosition.X - item.Position.X) > 0.001F Then
                            Call g.DrawLine(pen, item.ParentPosition.X, item.ParentPosition.Y, item.ParentPosition.X, item.Position.Y)
                        End If

                        Call g.DrawLine(pen, item.ParentPosition.X, item.Position.Y, item.Position.X, item.Position.Y)

                    Case TreeEdgeStyle.Arc
                        If item.ParentRadius > 1.0F Then
                            Dim rect As New RectangleF(
                                layout.Center.X - item.ParentRadius,
                                layout.Center.Y - item.ParentRadius,
                                item.ParentRadius * 2,
                                item.ParentRadius * 2)
                            Dim startAngle As Single = CSng(-item.ParentTheta * 180.0 / Math.PI)
                            Dim sweepAngle As Single = CSng(-(item.Theta - item.ParentTheta) * 180.0 / Math.PI)

                            Call g.DrawArc(pen, rect, startAngle, sweepAngle)

                            Dim arcEnd As PointF = DrawingHelper.PolarToCartesian(layout.Center, item.ParentRadius, item.Theta)
                            Call g.DrawLine(pen, arcEnd, item.Position)
                        Else
                            Call g.DrawLine(pen, item.ParentPosition, item.Position)
                        End If

                    Case Else
                        Call g.DrawLine(pen, item.ParentPosition, item.Position)
                End Select
            Next
        End Sub

        Private Sub DrawNodeMarkers(g As IGraphics, layout As TreeLayoutResult)
            Dim radius As Single = _options.NodeRadius

            If radius <= 0 Then
                Return
            End If

            Dim brush As SolidBrush = GetBrush(_options.NodeColor)

            For Each item As NodeLayout In layout.Nodes.Values
                Call g.FillEllipse(brush,
                                   item.Position.X - radius,
                                   item.Position.Y - radius,
                                   radius * 2,
                                   radius * 2)
            Next
        End Sub

#End Region

#Region "叶背景"

        Private Sub DrawLeafBackgrounds(g As IGraphics, layout As TreeLayoutResult)
            If Not _options.UseNodeColors Then
                Return
            End If

            For Each node As PhyloNode In _tree.LeafNodes
                Dim item As NodeLayout = layout.GetLayout(node)

                If item Is Nothing Then
                    Continue For
                End If

                Dim color As Color = ResolveLeafBackgroundColor(node)

                ' 默认白色背景视为「不绘制」
                If color.A = 0 OrElse color.ToArgb() = Color.White.ToArgb() Then
                    Continue For
                End If

                Dim brush As SolidBrush = GetBrush(Color.FromArgb(200, color))

                If item.BackgroundFan IsNot Nothing AndAlso item.BackgroundFan.Length >= 3 Then
                    Call g.FillPolygon(brush, item.BackgroundFan)
                ElseIf item.BackgroundRect.HasValue Then
                    Call g.FillRectangle(brush, item.BackgroundRect.Value)
                End If
            Next
        End Sub

#End Region

#Region "文本标注"

        Private Sub DrawLeafLabels(g As IGraphics, layout As TreeLayoutResult)
            For Each node As PhyloNode In _tree.LeafNodes
                Dim item As NodeLayout = layout.GetLayout(node)

                If item Is Nothing Then
                    Continue For
                End If

                Call DrawAnnotation(g,
                                    TreeLayoutEngine.FormatLeafText(node),
                                    _leafFont,
                                    ResolveLeafColor(node),
                                    item.LabelPosition,
                                    item.LabelAngle,
                                    item.LabelRightAligned,
                                    centerVertically:=False)
            Next
        End Sub

        Private Sub DrawBootstrap(g As IGraphics, layout As TreeLayoutResult)
            For Each pair As KeyValuePair(Of PhyloNode, NodeLayout) In layout.Nodes
                Dim node As PhyloNode = pair.Key

                If node.IsRoot OrElse node.Descendents.Count = 0 Then
                    Continue For
                End If

                Call DrawAnnotation(g,
                                    node.BootStrap.ToString("0.##"),
                                    _bootstrapFont,
                                    _options.BranchColor,
                                    pair.Value.BootstrapPosition,
                                    pair.Value.BootstrapAngle,
                                    alignRight:=False,
                                    centerVertically:=True)
            Next
        End Sub

        Private Sub DrawBranchLengths(g As IGraphics, layout As TreeLayoutResult)
            For Each pair As KeyValuePair(Of PhyloNode, NodeLayout) In layout.Nodes
                Dim node As PhyloNode = pair.Key

                If node.IsRoot OrElse node.Parent Is Nothing Then
                    Continue For
                End If

                Call DrawAnnotation(g,
                                    node.BranchLength.ToString("0.####"),
                                    _branchLengthFont,
                                    _options.BranchColor,
                                    pair.Value.BranchLengthPosition,
                                    pair.Value.BranchLengthAngle,
                                    alignRight:=False,
                                    centerVertically:=True)
            Next
        End Sub

        ''' <summary>
        ''' 绘制一段（可旋转的）文本标注。
        ''' </summary>
        Private Sub DrawAnnotation(g As IGraphics,
                                   text As String,
                                   font As Font,
                                   color As Color,
                                   position As PointF,
                                   angle As Single,
                                   alignRight As Boolean,
                                   centerVertically As Boolean,
                                   Optional measure As TextMeasureCache = Nothing)

            If String.IsNullOrEmpty(text) OrElse font Is Nothing Then
                Return
            End If

            measure = If(measure, New TextMeasureCache(g))

            Dim size As SizeF = measure.Measure(text, font)
            Dim radians As Double = angle * Math.PI / 180.0
            Dim x As Single = position.X
            Dim y As Single = position.Y

            If centerVertically Then
                ' 沿文本方向的法线方向回退半个字高，使文本垂直居中于锚点
                x += CSng(Math.Sin(radians) * size.Height / 2)
                y -= CSng(Math.Cos(radians) * size.Height / 2)
            End If

            If alignRight Then
                ' 使文本的结束端落在锚点上
                x -= CSng(Math.Cos(radians) * size.Width)
                y -= CSng(Math.Sin(radians) * size.Width)
            End If

            If angle = 0 Then
                Call g.DrawString(text, font, GetBrush(color), x, y)
            Else
                Dim rotateX As Single = x
                Dim rotateY As Single = y

                Call g.DrawString(text, font, GetBrush(color), rotateX, rotateY, angle)
            End If
        End Sub

#End Region

#Region "标题与比例尺"

        Private Sub DrawTitle(g As IGraphics, layout As TreeLayoutResult)
            If Not _options.ShowTitle OrElse String.IsNullOrEmpty(_options.Title) Then
                Return
            End If

            Dim measure As New TextMeasureCache(g)
            Dim size As SizeF = measure.Measure(_options.Title, _titleFont)
            Dim x As Single = (_options.CanvasSize.Width - size.Width) / 2
            Dim y As Single = Math.Max(4, _options.MarginTop / 3)

            Call g.DrawString(_options.Title, _titleFont, GetBrush(_options.TitleColor), x, y)
        End Sub

        Private Sub DrawScaleBar(g As IGraphics, layout As TreeLayoutResult)
            If Not _options.ShowScaleBar OrElse layout.BranchLengthScale <= 0 Then
                Return
            End If

            ' 长度：优先使用用户指定值，否则取「100 像素」对应的分支长度并规整为 1/2/5 × 10^n
            Dim length As Double = _options.ScaleBarLength

            If length <= 0 Then
                length = NiceNumber(100.0 / layout.BranchLengthScale)
            End If

            Dim pixels As Single = CSng(length * layout.BranchLengthScale)

            If pixels < 5 Then
                Return
            End If

            Dim x0 As Single = _options.MarginLeft
            Dim y0 As Single = _options.CanvasSize.Height - Math.Max(6, _options.MarginBottom / 2)

            Call g.DrawLine(GetPen(_options.BranchColor), x0, y0, x0 + pixels, y0)

            Dim measure As New TextMeasureCache(g)
            Dim text As String = length.ToString("0.####")
            Dim size As SizeF = measure.Measure(text, _branchLengthFont)

            Call g.DrawString(text, _branchLengthFont, GetBrush(_options.BranchColor), (x0 + pixels - size.Width) / 2, y0 - size.Height - 2)
        End Sub

        ''' <summary>
        ''' 把数值规整为 1/2/5 × 10^n 形式的「整数刻度」。
        ''' </summary>
        Friend Shared Function NiceNumber(value As Double) As Double
            If value <= 0 OrElse Double.IsNaN(value) OrElse Double.IsInfinity(value) Then
                Return 1
            End If

            Dim exponent As Double = Math.Floor(Math.Log10(value))
            Dim fraction As Double = value / Math.Pow(10, exponent)
            Dim nice As Double

            If fraction < 1.5 Then
                nice = 1
            ElseIf fraction < 3.5 Then
                nice = 2
            ElseIf fraction < 7.5 Then
                nice = 5
            Else
                nice = 10
            End If

            Return nice * Math.Pow(10, exponent)
        End Function

#End Region

#Region "颜色解析"

        Private Function ResolveBranchColor(node As PhyloNode) As Color
            If _options.UseNodeColors AndAlso Not String.IsNullOrEmpty(_options.ActiveColorSetID) Then
                Return DrawingHelper.ResolveColor(node.getBranchColorByColorsetID(_options.ActiveColorSetID), _options.BranchColor)
            End If

            Return _options.BranchColor
        End Function

        Private Function ResolveLeafColor(node As PhyloNode) As Color
            If _options.UseNodeColors AndAlso Not String.IsNullOrEmpty(_options.ActiveColorSetID) Then
                Return DrawingHelper.ResolveColor(node.getLeafColorByColorsetID(_options.ActiveColorSetID), _options.LeafColor)
            End If

            Return _options.LeafColor
        End Function

        Private Function ResolveLeafBackgroundColor(node As PhyloNode) As Color
            If _options.UseNodeColors AndAlso Not String.IsNullOrEmpty(_options.ActiveColorSetID) Then
                Return DrawingHelper.ResolveColor(node.getLeafBKColorByColorsetID(_options.ActiveColorSetID), Color.White)
            End If

            Return Color.White
        End Function

        Private Function HasBootstrapData() As Boolean
            If _bootstrapData.HasValue Then
                Return _bootstrapData.Value
            End If

            Dim any As Boolean = _tree.hasBootstrapScores() OrElse _tree.AllNodes.Any(Function(n) n.BootStrap > 0)

            _bootstrapData = any

            Return any
        End Function

#End Region

#Region "资源管理"

        Private Function ResolveFont(font As Font, size As Single) As Font
            If font IsNot Nothing Then
                Return font
            End If

            Dim created As Font = DrawingHelper.CreateFont(FontFace.SegoeUI, size)
            _ownedFonts.Add(created)

            Return created
        End Function

        Private Function GetPen(color As Color) As Pen
            Dim key As Integer = color.ToArgb()
            Dim pen As Pen = Nothing

            If _penCache.TryGetValue(key, pen) Then
                Return pen
            End If

            pen = New Pen(color, Math.Max(0.1F, _options.BranchWidth))
            _penCache(key) = pen

            Return pen
        End Function

        Private Function GetBrush(color As Color) As SolidBrush
            Dim key As Integer = color.ToArgb()
            Dim brush As SolidBrush = Nothing

            If _brushCache.TryGetValue(key, brush) Then
                Return brush
            End If

            brush = New SolidBrush(color)
            _brushCache(key) = brush

            Return brush
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            If _disposed Then
                Return
            End If

            For Each font As Font In _ownedFonts
                Try
                    font.Dispose()
                Catch
                End Try
            Next

            For Each pen As Pen In _penCache.Values
                Try
                    pen.Dispose()
                Catch
                End Try
            Next

            For Each brush As SolidBrush In _brushCache.Values
                Try
                    brush.Dispose()
                Catch
                End Try
            Next

            _ownedFonts.Clear()
            _penCache.Clear()
            _brushCache.Clear()
            _disposed = True
        End Sub

#End Region

    End Class

End Namespace
