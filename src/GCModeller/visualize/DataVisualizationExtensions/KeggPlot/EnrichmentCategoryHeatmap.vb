Imports System.Drawing
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Math.DataFrame
Imports Microsoft.VisualBasic.MIME.Html.CSS

Public Class EnrichmentCategoryHeatmap : Inherits HeatMapPlot

    ReadOnly data As DataFrame

    Public Sub New(data As DataFrame, theme As Theme)
        MyBase.New(theme)
        Me.data = data
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


    End Sub
End Class
