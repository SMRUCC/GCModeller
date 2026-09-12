#Region "Metabopolis: renderer"

' ============================================================================
' GDI+ / SVG 渲染层
' ----------------------------------------------------------------------------
' 复用 imaging 库的统一绘图抽象 IGraphics + g.GraphicsPlots + Drivers：
' 同一份绘制代码既可以通过 GDI 输出位图（PNG），也可以通过 SVG 输出矢量图。
'
' 绘制顺序（自底向上）：
'     背景 -> 街区底色 -> 建筑块 -> 块内车道 -> 块间彩色路由（含方向箭头）
'          -> 边界枢纽 -> 街区标题 / 枢纽标签 -> 九色图例
' ============================================================================

Imports System.Drawing
Imports System.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Metabopolis.Model

Namespace Rendering

    ''' <summary>
    ''' 渲染选项。
    ''' </summary>
    Public Class RenderOptions

        ''' <summary>是否绘制建筑块。</summary>
        Public Property DrawBuildings As Boolean = True

        ''' <summary>是否绘制街区内部的局部车道。</summary>
        Public Property DrawLocalLanes As Boolean = True

        ''' <summary>是否绘制跨街区的彩色路由。</summary>
        Public Property DrawGlobalRoutes As Boolean = True

        ''' <summary>是否绘制方向箭头。</summary>
        Public Property DrawArrows As Boolean = True

        ''' <summary>是否绘制边界枢纽。</summary>
        Public Property DrawJunctions As Boolean = True

        ''' <summary>是否绘制街区标题。</summary>
        Public Property DrawBlockLabels As Boolean = True

        ''' <summary>是否绘制枢纽标签（密集时建议关闭）。</summary>
        Public Property DrawJunctionLabels As Boolean = False

        ''' <summary>是否绘制九色图例。</summary>
        Public Property DrawLegend As Boolean = True

        ''' <summary>是否绘制标题。</summary>
        Public Property DrawTitle As Boolean = True

        ''' <summary>标题文本。</summary>
        Public Property Title As String

        ''' <summary>输出分辨率（DPI）。</summary>
        Public Property Ppi As Integer = 100

    End Class

    ''' <summary>
    ''' Metabopolis 地图渲染器。
    ''' </summary>
    Public Class MetabopolisRenderer

        ''' <summary>视觉主题。</summary>
        Public Property Theme As MapTheme

        ''' <summary>渲染选项。</summary>
        Public Property Options As RenderOptions

        Private _layout As NetworkLayout

        Public Sub New(Optional theme As MapTheme = Nothing, Optional options As RenderOptions = Nothing)
            Me.Theme = If(theme, MapTheme.CreateDefault())
            Me.Options = If(options, New RenderOptions())
        End Sub

        ''' <summary>
        ''' 渲染为图形数据；<paramref name="driver"/> 决定输出位图还是矢量图。
        ''' </summary>
        Public Function Render(layout As NetworkLayout, Optional driver As Drivers = Drivers.GDI) As GraphicsData
            If layout Is Nothing Then
                Throw New ArgumentNullException(NameOf(layout))
            End If

            _layout = layout

            Dim size As New Size(CInt(Math.Max(64, layout.CanvasWidth)), CInt(Math.Max(64, layout.CanvasHeight)))
            Dim padding As New Padding(0)
            Dim background As String = ToHex(Theme.BackgroundColor)

            Return g.GraphicsPlots(size, padding, background, AddressOf Plot, driver, Options.Ppi)
        End Function

        ''' <summary>按扩展名自动选择 PNG 或 SVG 输出。</summary>
        Public Function RenderToFile(layout As NetworkLayout, path As String) As Boolean
            Dim extension As String = System.IO.Path.GetExtension(path)

            If String.Equals(extension, ".svg", StringComparison.OrdinalIgnoreCase) Then
                Return SaveSvg(layout, path)
            End If

            Return SavePng(layout, path)
        End Function

        ''' <summary>输出 PNG 位图。</summary>
        Public Function SavePng(layout As NetworkLayout, path As String) As Boolean
            EnsureDirectory(path)

            Using data As GraphicsData = Render(layout, Drivers.GDI)
                Return data.AsGDIImage.SaveAs(path, ImageFormats.Png)
            End Using
        End Function

        ''' <summary>输出 SVG 矢量图。</summary>
        Public Function SaveSvg(layout As NetworkLayout, path As String) As Boolean
            EnsureDirectory(path)

            Using data As GraphicsData = Render(layout, Drivers.SVG)
                Return DirectCast(data, SVGData).Save(path)
            End Using
        End Function

        ''' <summary>
        ''' IGraphics 绘图回调。<see cref="IPlot"/> 的签名带 ByRef 参数，
        ''' 因此这里使用方法组而不是 lambda。
        ''' </summary>
        Private Sub Plot(ByRef ig As IGraphics, region As GraphicsRegion)
            Draw(ig, _layout)
        End Sub

        Private Sub Draw(ig As IGraphics, layout As NetworkLayout)
            If layout Is Nothing Then
                Return
            End If

            ig.Clear(Theme.BackgroundColor)

            DrawBlocks(ig, layout)

            If Options.DrawBuildings Then
                DrawBuildings(ig, layout)
            End If

            If Options.DrawLocalLanes Then
                DrawLanes(ig, layout)
            End If

            If Options.DrawGlobalRoutes Then
                DrawGlobalRoutes(ig, layout)
            End If

            If Options.DrawJunctions Then
                DrawJunctions(ig, layout)
            End If

            If Options.DrawBlockLabels Then
                DrawBlockLabels(ig, layout)
            End If

            If Options.DrawLegend Then
                DrawLegend(ig, layout)
            End If

            If Options.DrawTitle AndAlso Not String.IsNullOrEmpty(Options.Title) Then
                ig.DrawString(Options.Title, Theme.TitleFont, New SolidBrush(Theme.LabelColor), 18.0F, 12.0F)
            End If
        End Sub

        Private Sub DrawBlocks(ig As IGraphics, layout As NetworkLayout)
            For Each block As BlockLayout In layout.Blocks.SafeQuery
                Dim color As Color = Theme.CategoryColor(block.ColorIndex)

                ig.FillRectangle(
                    New SolidBrush(Color.FromArgb(Theme.CategoryFillAlpha, color)),
                    block.Box.Bounds)

                ig.DrawRectangle(
                    New Pen(Color.FromArgb(Theme.CategoryStrokeAlpha, color), Theme.BlockStrokeWidth),
                    block.Box.Bounds)
            Next
        End Sub

        Private Sub DrawBuildings(ig As IGraphics, layout As NetworkLayout)
            Dim fill As New SolidBrush(Theme.BuildingFill)
            Dim stroke As New Pen(Theme.BuildingBorder, 1.0F)

            For Each building As BuildingBlock In layout.Buildings.SafeQuery
                Dim box As RectangleF = building.Box.Bounds

                If box.Width < 1 OrElse box.Height < 1 Then
                    Continue For
                End If

                ig.FillRectangle(fill, box)
                ig.DrawRectangle(stroke, box)
            Next
        End Sub

        Private Sub DrawLanes(ig As IGraphics, layout As NetworkLayout)
            Dim pen As New Pen(Theme.LaneColor, Theme.LaneWidth)

            For Each route As RoutePolyline In layout.Routes.SafeQuery
                If route.IsInterBlock Then
                    Continue For
                End If

                Dim polyline As PointF() = route.Polyline()

                If polyline.Length >= 2 Then
                    ig.DrawLines(pen, polyline)
                End If
            Next
        End Sub

        Private Sub DrawGlobalRoutes(ig As IGraphics, layout As NetworkLayout)
            For Each route As RoutePolyline In layout.Routes.SafeQuery
                If Not route.IsInterBlock Then
                    Continue For
                End If

                Dim polyline As PointF() = route.Polyline()

                If polyline.Length < 2 Then
                    Continue For
                End If

                Dim color As Color = Theme.RoleColor(route.Role)
                Dim pen As New Pen(Color.FromArgb(220, color), Theme.RouteWidth)

                ig.DrawLines(pen, polyline)

                If Options.DrawArrows AndAlso route.IsDirected Then
                    DrawArrow(ig, pen, polyline)
                End If
            Next
        End Sub

        Private Sub DrawJunctions(ig As IGraphics, layout As NetworkLayout)
            Dim stroke As New Pen(Color.FromArgb(200, 120, 70, 20), 0.9F)

            For Each junction As Junction In layout.Junctions.SafeQuery
                Dim radius As Single = CSng(Math.Max(2.2, 2.0 + junction.Degree * 0.35))
                ig.DrawCircle(junction.Point, Theme.JunctionColor, stroke, radius)

                If Options.DrawJunctionLabels Then
                    ig.DrawString(
                        junction.Label,
                        Theme.SmallFont,
                        New SolidBrush(Theme.SubLabelColor),
                        CSng(junction.X + radius + 2),
                        CSng(junction.Y - 5))
                End If
            Next
        End Sub

        Private Sub DrawBlockLabels(ig As IGraphics, layout As NetworkLayout)
            Dim brush As New SolidBrush(Theme.LabelColor)

            For Each block As BlockLayout In layout.Blocks.SafeQuery
                Dim box As RectangleF = block.Box.Bounds

                If box.Width < 30 OrElse box.Height < 20 Then
                    Continue For
                End If

                ' 标题放在街区左上角的内侧
                ig.FillRectangle(
                    New SolidBrush(Color.FromArgb(210, Theme.BackgroundColor)),
                    New RectangleF(box.X + 2, box.Y + 2, box.Width - 4, 16))

                ig.DrawString(
                    Trim(block.Label, box.Width - 8, ig, Theme.BlockFont),
                    Theme.BlockFont,
                    brush,
                    box.X + 4,
                    box.Y + 3)
            Next
        End Sub

        Private Sub DrawLegend(ig As IGraphics, layout As NetworkLayout)
            Const rowHeight As Single = 14
            Const swatchWidth As Single = 20
            Const swatchHeight As Single = 8

            Dim rows As Integer = 9
            Dim height As Single = rows * rowHeight + 18
            Dim width As Single = 190
            Dim x As Single = 16
            Dim y As Single = CSng(Math.Max(16, layout.CanvasHeight - height - 16))

            ig.FillRectangle(
                New SolidBrush(Color.FromArgb(225, Theme.BackgroundColor)),
                New RectangleF(x - 6, y - 6, width, height))

            ig.DrawRectangle(
                New Pen(Color.FromArgb(160, Theme.BuildingBorder), 1.0F),
                New RectangleF(x - 6, y - 6, width, height))

            ig.DrawString("edge role", Theme.SmallFont, New SolidBrush(Theme.LabelColor), x, y)
            y += rowHeight

            For i As Integer = 0 To rows - 1
                Dim role As EdgeRole = CType(i, EdgeRole)
                Dim color As Color = Theme.RoleColor(role)

                ig.FillRectangle(New SolidBrush(color), New RectangleF(x, y + 3, swatchWidth, swatchHeight))

                ig.DrawString(
                    Theme.RoleName(role),
                    Theme.SmallFont,
                    New SolidBrush(Theme.SubLabelColor),
                    x + swatchWidth + 6,
                    y)

                y += rowHeight
            Next
        End Sub

        Private Sub DrawArrow(ig As IGraphics, pen As Pen, polyline As PointF())
            Dim last As PointF = polyline(polyline.Length - 1)
            Dim previous As PointF = polyline(polyline.Length - 2)
            Dim dx As Double = last.X - previous.X
            Dim dy As Double = last.Y - previous.Y
            Dim length As Double = Math.Sqrt(dx * dx + dy * dy)

            If length < 1E-06 Then
                Return
            End If

            Const size As Double = 6

            dx /= length
            dy /= length

            Dim tip As New PointF(CSng(last.X), CSng(last.Y))
            Dim left As New PointF(CSng(last.X - dx * size - dy * size * 0.5), CSng(last.Y - dy * size + dx * size * 0.5))
            Dim right As New PointF(CSng(last.X - dx * size + dy * size * 0.5), CSng(last.Y - dy * size - dx * size * 0.5))

            ig.FillPolygon(New SolidBrush(pen.Color), New PointF() {tip, left, right})
        End Sub

        ''' <summary>按可用宽度截断过长的标签。</summary>
        Private Shared Function Trim(text As String, width As Single, ig As IGraphics, font As Font) As String
            If String.IsNullOrEmpty(text) Then
                Return ""
            End If

            If width <= 0 Then
                Return ""
            End If

            Dim measured As SizeF = ig.MeasureString(text, font)

            If measured.Width <= width Then
                Return text
            End If

            Dim result As String = text

            While result.Length > 1 AndAlso ig.MeasureString(result & "...", font).Width > width
                result = result.Substring(0, result.Length - 1)
            End While

            Return result & "..."
        End Function

        Private Shared Function ToHex(color As Color) As String
            Return $"#{color.R:X2}{color.G:X2}{color.B:X2}"
        End Function

        Private Shared Sub EnsureDirectory(path As String)
            Dim folder As String = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path))

            If Not String.IsNullOrEmpty(folder) AndAlso Not Directory.Exists(folder) Then
                Directory.CreateDirectory(folder)
            End If
        End Sub

    End Class

End Namespace

#End Region
