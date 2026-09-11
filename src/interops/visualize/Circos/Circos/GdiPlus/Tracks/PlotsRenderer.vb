Imports System.Drawing
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.Visualize.Circos.Configurations.ComponentModel
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots.Lines
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas

Namespace GdiPlus.Tracks

    ''' <summary>
    ''' 各类 2D 绘图轨道（histogram / heatmap / line / scatter / tile / connector / text / highlight）
    ''' 以及 ``&lt;rules>``、``&lt;axes>``、``&lt;backgrounds>`` 子块的绘制。
    ''' </summary>
    Public Module PlotsRenderer

        ''' <summary>
        ''' 渲染 ``&lt;plots>`` 块之中的所有绘图元素
        ''' </summary>
        Public Sub RenderPlots(ctx As GdiRenderContext)
            If ctx.Circos.Plots Is Nothing Then
                Return
            End If

            For Each track As ITrackPlot In ctx.Circos.TracksInBlock(CircosBlocks.plots)
                Call RenderTrack(ctx, track)
            Next
        End Sub

        ''' <summary>
        ''' 渲染顶层 ``&lt;highlights>`` 块之中的所有高亮元素
        ''' </summary>
        Public Sub RenderHighlights(ctx As GdiRenderContext)
            If ctx.Circos.Plots Is Nothing Then
                Return
            End If

            For Each track As ITrackPlot In ctx.Circos.TracksInBlock(CircosBlocks.highlights)
                Call RenderTrack(ctx, track)
            Next
        End Sub

        Private Sub RenderTrack(ctx As GdiRenderContext, track As ITrackPlot)
            Dim geo As New TrackGeometry(track, ctx)

            Call RenderBackgrounds(ctx, track, geo)

            If TypeOf track Is HeatMap Then
                Call RenderHeatMap(ctx, DirectCast(track, HeatMap), geo)
            ElseIf TypeOf track Is Highlight Then
                Call RenderHighlight(ctx, track, geo)
            ElseIf TypeOf track Is Histogram Then
                ' SeparatorCircle 继承自 Histogram，同样在这里处理
                Call RenderHistogram(ctx, DirectCast(track, Histogram), geo)
            ElseIf TypeOf track Is LinePlot Then
                Call RenderLine(ctx, DirectCast(track, LinePlot), geo)
            ElseIf TypeOf track Is ScatterPlot Then
                Call RenderScatter(ctx, DirectCast(track, ScatterPlot), geo)
            ElseIf TypeOf track Is TilePlot Then
                Call RenderTile(ctx, DirectCast(track, TilePlot), geo)
            ElseIf TypeOf track Is Connector Then
                Call RenderConnector(ctx, DirectCast(track, Connector), geo)
            ElseIf TypeOf track Is TextLabel Then
                Call RenderText(ctx, DirectCast(track, TextLabel), geo)
            End If

            Call RenderAxes(ctx, track, geo)
        End Sub

#Region "数据访问辅助"

        Private Function DataOf(Of T As ITrackData)(track As ITrackPlot) As T()
            If track.tracksData Is Nothing Then
                Return New T() {}
            End If

            Return track.tracksData.GetEnumerator().OfType(Of T)().ToArray()
        End Function

        Private Function ChromosomeOf(ctx As GdiRenderContext, chr As String) As ChromosomeBand
            Dim band As ChromosomeBand = Nothing

            Call ctx.Layout.TryGetBand(chr, band)

            Return band
        End Function

        ''' <summary>
        ''' 计算区段的角度区间，并对过窄的区段设置一个最小可视宽度
        ''' </summary>
        Private Sub AngleSpanOf(ctx As GdiRenderContext,
                               band As ChromosomeBand,
                               start As Integer,
                               [end] As Integer,
                               ByRef angle As Double,
                               ByRef sweep As Double)

            Dim a0 As Double = ctx.Layout.AngleOf(band, start)
            Dim a1 As Double = ctx.Layout.AngleOf(band, [end])

            angle = Math.Min(a0, a1)
            sweep = Math.Abs(a1 - a0)

            ' 1nt 所对应的最小角度
            Dim minimal As Double = Math.Max(0.05, band.SweepAngle / Math.Max(1, band.Length))

            If sweep < minimal Then
                sweep = Math.Min(band.SweepAngle, minimal)
            End If
        End Sub

        Private Function ThicknessOf(ctx As GdiRenderContext, track As ITrackPlot, Optional fallback As Double = 0) As Double
            Return CircosUnits.ParseNumber(track.thickness, fallback)
        End Function

        Private Function StrokeThicknessOf(track As ITrackPlot) As Double
            ' stroke_thickness 支持 "0" / "1" / "1p" 等形式
            Return CircosUnits.ParseNumber((track.stroke_thickness OrElse "").TrimEnd("p"c), 0)
        End Function

#End Region

#Region "规则（rules）"

        ''' <summary>
        ''' 依据 ``&lt;rules>`` 对当前的数值求出一个覆盖的填充颜色；没有规则命中的时候返回 Nothing
        ''' </summary>
        Private Function RuleColorOf(ctx As GdiRenderContext, track As ITrackPlot, value As Double) As Color?
            Dim rules As List(Of ConditionalRule) = track.rules

            If rules Is Nothing OrElse rules.Count = 0 Then
                Return Nothing
            End If

            For Each rule As ConditionalRule In rules
                If RuleEvaluator.Matches(rule.condition, value) Then
                    Dim colorExpr$ = If(rule.fill_color, rule.color)

                    If Not String.IsNullOrEmpty(colorExpr) Then
                        Return ctx.ColorOf(colorExpr)
                    End If

                    If CircosUnits.IsYes(rule.flow) Then
                        Exit For
                    End If
                End If
            Next

            Return Nothing
        End Function

#End Region

#Region "histogram"

        Private Sub RenderHistogram(ctx As GdiRenderContext, track As Histogram, geo As TrackGeometry)
            Dim points As ValueTrackData() = DataOf(Of ValueTrackData)(track)
            Dim fillDefault As Color = ctx.ColorOf(track.fill_color, Color.Gray)
            Dim strokeThickness As Double = StrokeThicknessOf(track)
            Dim strokeColor As Color = ctx.ColorOf(track.stroke_color, Color.Black)

            For Each pt As ValueTrackData In points
                Dim band = ChromosomeOf(ctx, pt.chr)

                If band Is Nothing Then
                    Continue For
                End If

                Dim angle As Double
                Dim sweep As Double

                Call AngleSpanOf(ctx, band, pt.start, pt.end, angle, sweep)

                Dim valueRadius As Double = geo.RadiusOf(pt.value)
                Dim fill As Color = RuleColorOf(ctx, track, pt.value)

                If Not String.IsNullOrEmpty(pt.formatting.fill_color) Then
                    fill = ctx.ColorOf(pt.formatting.fill_color)
                End If

                Call ctx.Canvas.FillAnnularSector(geo.Baseline, valueRadius, angle, sweep, fill)

                If strokeThickness > 0 Then
                    Call ctx.Canvas.DrawArc(Math.Max(geo.Baseline, valueRadius), angle, sweep, strokeColor, CSng(strokeThickness))
                End If
            Next
        End Sub

#End Region

#Region "heatmap"

        Private ReadOnly colorSplitter As New Regex("[\s,]+")

        Private Sub RenderHeatMap(ctx As GdiRenderContext, track As HeatMap, geo As TrackGeometry)
            Dim points As ValueTrackData() = DataOf(Of ValueTrackData)(track)
            Dim colors As String() = colorSplitter _
                .Split(If(track.color, "")).Where(Function(s) s.Length > 0).ToArray()

            If colors.Length = 0 Then
                colors = {"red"}
            End If

            Dim strokeThickness As Double = StrokeThicknessOf(track)

            For Each pt As ValueTrackData In points
                Dim band = ChromosomeOf(ctx, pt.chr)

                If band Is Nothing Then
                    Continue For
                End If

                Dim angle As Double
                Dim sweep As Double

                Call AngleSpanOf(ctx, band, pt.start, pt.end, angle, sweep)

                Dim f As Double = geo.FractionOf(pt.value)
                Dim idx As Integer = CInt(Math.Floor(f * colors.Length))

                If idx >= colors.Length Then idx = colors.Length - 1
                If idx < 0 Then idx = 0

                Dim fill As Color = RuleColorOf(ctx, track, pt.value)

                If Not String.IsNullOrEmpty(pt.formatting.fill_color) Then
                    fill = ctx.ColorOf(pt.formatting.fill_color)
                End If
                If fill Is Nothing Then
                    fill = ctx.ColorOf(colors(idx), Color.Gray)
                End If

                Call ctx.Canvas.FillAnnularSector(geo.InnerRadius, geo.OuterRadius, angle, sweep, fill)

                If strokeThickness > 0 Then
                    Call ctx.Canvas.DrawArc(geo.OuterRadius, angle, sweep, ctx.ColorOf(track.stroke_color, Color.Gray), CSng(strokeThickness))
                End If
            Next
        End Sub

#End Region

#Region "line"

        Private Sub RenderLine(ctx As GdiRenderContext, track As LinePlot, geo As TrackGeometry)
            Dim points As ValueTrackData() = DataOf(Of ValueTrackData)(track)
            Dim color As Color = ctx.ColorOf(track.color, Color.Black)
            Dim width As Double = ThicknessOf(ctx, track, 1)

            If width <= 0 Then width = 1

            For Each chrGroup In points.GroupBy(Function(p) p.chr)
                Dim band = ChromosomeOf(ctx, chrGroup.Key)

                If band Is Nothing Then
                    Continue For
                End If

                Dim pts As New List(Of PointF)

                For Each pt In chrGroup.OrderBy(Function(p) p.start)
                    Dim angle As Double = ctx.Layout.AngleOf(band, pt.start)
                    Dim radius As Double = geo.RadiusOf(pt.value)

                    pts.Add(ctx.Canvas.PointAt(angle, radius))
                Next

                If pts.Count = 1 Then
                    Call ctx.Canvas.FillCircle(pts(0), Math.Max(1, width), color)
                Else
                    Call ctx.Canvas.DrawPolyline(pts, color, CSng(width))
                End If
            Next
        End Sub

#End Region

#Region "scatter"

        Private Sub RenderScatter(ctx As GdiRenderContext, track As ScatterPlot, geo As TrackGeometry)
            Dim points As ValueTrackData() = DataOf(Of ValueTrackData)(track)
            Dim defaultColor As Color = ctx.ColorOf(If(track.color, track.fill_color), Color.Gray)
            Dim glyphSize As Double = CircosUnits.ParseNumber(track.glyph_size, 10)

            If glyphSize <= 0 Then glyphSize = 10

            Dim radius As Double = glyphSize / 2

            For Each pt As ValueTrackData In points
                Dim band = ChromosomeOf(ctx, pt.chr)

                If band Is Nothing Then
                    Continue For
                End If

                Dim angle As Double = ctx.Layout.AngleOf(band, pt.start)
                Dim valueRadius As Double = geo.RadiusOf(pt.value)
                Dim color As Color = defaultColor

                If Not String.IsNullOrEmpty(pt.formatting.fill_color) Then
                    color = ctx.ColorOf(pt.formatting.fill_color)
                End If

                Call ctx.Canvas.FillCircle(ctx.Canvas.PointAt(angle, valueRadius), radius, color)
            Next
        End Sub

#End Region

#Region "tile"

        Private Sub RenderTile(ctx As GdiRenderContext, track As TilePlot, geo As TrackGeometry)
            Dim regions As RegionTrackData() = DataOf(Of RegionTrackData)(track)
            Dim fill As Color = ctx.ColorOf(track.fill_color, Color.Gray)
            Dim strokeThickness As Double = StrokeThicknessOf(track)
            Dim strokeColor As Color = ctx.ColorOf(track.stroke_color, Color.LightGray)

            For Each region As RegionTrackData In regions
                Dim band = ChromosomeOf(ctx, region.chr)

                If band Is Nothing Then
                    Continue For
                End If

                Dim angle As Double
                Dim sweep As Double

                Call AngleSpanOf(ctx, band, region.start, region.end, angle, sweep)

                Dim fillColor As Color = fill

                If Not String.IsNullOrEmpty(region.formatting.fill_color) Then
                    fillColor = ctx.ColorOf(region.formatting.fill_color)
                End If

                Call ctx.Canvas.FillAnnularSector(geo.InnerRadius, geo.OuterRadius, angle, sweep, fillColor)

                If strokeThickness > 0 Then
                    Call ctx.Canvas.DrawArc(geo.OuterRadius, angle, sweep, strokeColor, CSng(strokeThickness))
                End If
            Next
        End Sub

#End Region

#Region "connector"

        Private Sub RenderConnector(ctx As GdiRenderContext, track As Connector, geo As TrackGeometry)
            Dim regions As RegionTrackData() = DataOf(Of RegionTrackData)(track)
            Dim color As Color = ctx.ColorOf(track.fill_color, Color.Black)
            Dim width As Double = ThicknessOf(ctx, track, 1)

            If width <= 0 Then width = 1

            ' connector_dims = radius1_start,pad1,radius2_start,pad2,pad3
            Dim dims As Double() = If(track.connector_dims, "") _
                .Split(","c) _
                .Select(Function(s) CircosUnits.ParseNumber(s, 0)) _
                .ToArray()
            Dim pad As Double = If(dims.Length > 1, dims(1), 0.3)
            Dim midRadius As Double = geo.Baseline + (geo.Full - geo.Baseline) * Math.Max(0.1, 1 - pad)

            For Each region As RegionTrackData In regions
                Dim band = ChromosomeOf(ctx, region.chr)

                If band Is Nothing Then
                    Continue For
                End If

                Dim a0 As Double = ctx.Layout.AngleOf(band, region.start)
                Dim a1 As Double = ctx.Layout.AngleOf(band, region.end)

                Dim pts As New List(Of PointF) From {
                    ctx.Canvas.PointAt(a0, geo.Baseline),
                    ctx.Canvas.PointAt(a0, midRadius),
                    ctx.Canvas.PointAt(a1, midRadius),
                    ctx.Canvas.PointAt(a1, geo.Baseline)
                }

                Call ctx.Canvas.DrawPolyline(pts, color, CSng(width))
            Next
        End Sub

#End Region

#Region "text"

        Private Sub RenderText(ctx As GdiRenderContext, track As TextLabel, geo As TrackGeometry)
            Dim labels As TextTrackData() = DataOf(Of TextTrackData)(track)
            Dim color As Color = ctx.ColorOf(track.color, Color.Black)
            Dim size As Double = CircosUnits.ParseNumber(track.label_size, 16)

            If size <= 0 Then size = 16

            Dim font = ctx.CreateFont(size * 0.75)

            For Each label As TextTrackData In labels
                Dim band = ChromosomeOf(ctx, label.chr)

                If band Is Nothing Then
                    Continue For
                End If

                Dim angle As Double = ctx.Layout.AngleOf(band, label.start)
                Dim radius As Double = geo.Full

                Call ctx.Canvas.DrawRadialText(label.text, angle, radius, font, color)
            Next
        End Sub

#End Region

#Region "highlight"

        Private Sub RenderHighlight(ctx As GdiRenderContext, track As ITrackPlot, geo As TrackGeometry)
            Dim regions As ValueTrackData() = DataOf(Of ValueTrackData)(track)
            Dim fill As Color = ctx.ColorOf(track.fill_color, Color.Yellow)

            For Each region As ValueTrackData In regions
                Dim band = ChromosomeOf(ctx, region.chr)

                If band Is Nothing Then
                    Continue For
                End If

                Dim angle As Double
                Dim sweep As Double

                Call AngleSpanOf(ctx, band, region.start, region.end, angle, sweep)

                Dim fillColor As Color = fill

                If Not String.IsNullOrEmpty(region.formatting.fill_color) Then
                    fillColor = ctx.ColorOf(region.formatting.fill_color)
                End If

                Call ctx.Canvas.FillAnnularSector(geo.InnerRadius, geo.OuterRadius, angle, sweep, fillColor)
            Next
        End Sub

#End Region

#Region "backgrounds / axes"

        Private Sub RenderBackgrounds(ctx As GdiRenderContext, track As ITrackPlot, geo As TrackGeometry)
            Dim backgrounds As List(Of Background) = track.backgrounds

            If backgrounds Is Nothing OrElse backgrounds.Count = 0 Then
                Return
            End If

            For Each background As Background In backgrounds
                Dim y0 As Double = CircosUnits.ParseFraction(background.y0, 0)
                Dim y1 As Double = CircosUnits.ParseFraction(background.y1, 1)

                Dim r0 As Double = geo.RadiusOfFraction(y0)
                Dim r1 As Double = geo.RadiusOfFraction(y1)
                Dim color As Color = ctx.ColorOf(background.color, Color.LightGray)

                For Each band As ChromosomeBand In ctx.Layout.Bands
                    Call ctx.Canvas.FillAnnularSector(r0, r1, band.StartAngle, band.SweepAngle, color)
                Next
            Next
        End Sub

        Private Sub RenderAxes(ctx As GdiRenderContext, track As ITrackPlot, geo As TrackGeometry)
            Dim axes As List(Of Axis) = track.axes

            If axes Is Nothing OrElse axes.Count = 0 Then
                Return
            End If

            For Each axis As Axis In axes
                Dim stepRadius As Double = ctx.Radius(axis.spacing, 0)

                If stepRadius <= 0 Then
                    Continue For
                End If

                Dim color As Color = ctx.ColorOf(axis.color, Color.LightGray)
                Dim width As Double = CircosUnits.ParseNumber(axis.thickness, 1)

                If width <= 0 Then width = 1

                Dim r As Double = geo.InnerRadius

                Do While r <= geo.OuterRadius + 0.001
                    Call ctx.Canvas.DrawArc(r, 0, 360, color, CSng(width))
                    r += stepRadius
                Loop
            Next
        End Sub

#End Region

    End Module

    ''' <summary>
    ''' circos ``&lt;rule>`` 的 ``condition`` 表达式求值。
    ''' 
    ''' 支持形如 ``var(value) &gt; 0.6``、``var(value) &lt;= -0.1`` 的简单比较表达式。
    ''' </summary>
    Public Module RuleEvaluator

        Private ReadOnly conditionPattern As New Regex(
            "var\s*\(\s*value\s*\)\s*(?<op>>=|<=|==|!=|>|<)\s*(?<value>-?[\d\.]+)",
            RegexOptions.IgnoreCase)

        ''' <summary>
        ''' 判断给定的数值是否满足规则表达式。无法解析的表达式返回 False
        ''' </summary>
        Public Function Matches(condition As String, value As Double) As Boolean
            If String.IsNullOrWhiteSpace(condition) Then
                Return False
            End If

            Dim m As Match = conditionPattern.Match(condition)

            If Not m.Success Then
                Return False
            End If

            Dim target As Double = CircosUnits.ParseNumber(m.Groups("value").Value, 0)

            Select Case m.Groups("op").Value
                Case ">="
                    Return value >= target
                Case "<="
                    Return value <= target
                Case "=="
                    Return Math.Abs(value - target) < 0.0000001
                Case "!="
                    Return Math.Abs(value - target) >= 0.0000001
                Case ">"
                    Return value > target
                Case "<"
                    Return value < target
                Case Else
                    Return False
            End Select
        End Function
    End Module
End Namespace
