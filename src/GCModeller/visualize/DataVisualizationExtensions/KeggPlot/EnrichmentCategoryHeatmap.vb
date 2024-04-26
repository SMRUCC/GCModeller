Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.DataFrame
Imports Microsoft.VisualBasic.MIME.Html.CSS

Public Class EnrichmentCategoryHeatmap : Inherits HeatMapPlot

    ReadOnly data As DataFrame

    Public Sub New(data As DataFrame, theme As Theme)
        MyBase.New(theme)
        Me.data = data.ZScale(byrow:=True)
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim rect As Rectangle = canvas.PlotRegion
        Dim label_region As New Rectangle(rect.Left, rect.Top, rect.Width * 0.2, rect.Height)
        Dim heatmap_region As New Rectangle(label_region.Right, rect.Top, rect.Width * 0.4, rect.Height)
        Dim tree_region As New Rectangle(heatmap_region.Right, rect.Top, rect.Width * 0.1, rect.Height)
        Dim right_region As New Rectangle(tree_region.Right, rect.Top, rect.Width - label_region.Width - heatmap_region.Width - tree_region.Width, rect.Height)
        Dim label_font As Font = CSSFont.TryParse(theme.axisTickCSS).GDIObject(g.Dpi)
        Dim label_maxh As Single = label_region.Height / data.nsamples

        ' draw labels on left
        Dim y As Double = label_region.Top
        Dim x As Double
        Dim size As SizeF
        Dim i As Integer = 0

        For Each name As String In data.rownames
            size = g.MeasureString(name, label_font)
            x = label_region.Right - size.Width
            y = label_maxh * i + label_region.Top - size.Height / 2
            i += 1
            g.DrawString(name, label_font, Brushes.Black, New PointF(x, y))
        Next

        ' draw heatmap
        Dim dx = heatmap_region.Width / data.featureNames.Length
        Dim dy = heatmap_region.Height / data.rownames.Length
        Dim boxCell As RectangleF
        Dim heatmap As Brush() = Designer.GetColors(theme.colorSet, mapLevels) _
            .Select(Function(c) New SolidBrush(c)) _
            .ToArray
        Dim range As New DoubleRange(data.features.Values.Select(Function(v) DirectCast(v.vector, Double())).IteratesALL)
        Dim index As New DoubleRange(0, mapLevels - 1)
        Dim vec As Func(Of Integer, Double)
        Dim color As Integer

        x = heatmap_region.Left
        y = heatmap_region.Top

        For Each col As String In data.featureNames
            boxCell = New RectangleF(x, y, dx, dy)
            vec = data(col).NumericGetter

            For i = 0 To data.rownames.Length - 1
                color = range.ScaleMapping(vec(i), index)
                boxCell = New RectangleF(x, y, dx, dy)
                g.FillRectangle(heatmap(color), boxCell)
                y += dy
            Next

            x += dx
        Next
    End Sub
End Class
