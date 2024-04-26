Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Scaler
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.DataFrame
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Public Class EnrichmentCategoryHeatmap : Inherits HeatMapPlot

    ReadOnly data As DataFrame
    ReadOnly groupd As Dictionary(Of String, SampleInfo)
    ReadOnly metadata As DataFrame
    ReadOnly kegg_class As String

    Public Sub New(data As DataFrame, metadata As DataFrame, groupd As SampleInfo(), theme As Theme, Optional kegg_class As String = "class")
        MyBase.New(theme)

        Me.metadata = metadata
        Me.data = data.ZScale(byrow:=True)
        Me.groupd = groupd.ToDictionary(Function(s) s.ID)
        ' re-order column of samples by groups 
        Me.data = Me.data(groupd _
            .GroupBy(Function(s) s.sample_info) _
            .Select(Function(group)
                        Return group _
                            .OrderBy(Function(s) s.sample_name) _
                            .Select(Function(s)
                                        Return s.ID
                                    End Function)
                    End Function) _
            .IteratesALL)
        Me.kegg_class = kegg_class
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
        Dim [class] As String() = metadata(kegg_class).TryCast(Of String)
        Dim class_colors As New CategoryColorProfile([class], "paper")

        x = heatmap_region.Left
        y = heatmap_region.Top

        For Each col As String In data.featureNames
            boxCell = New RectangleF(x, y - dy - 10, dx, dy)
            vec = data(col).NumericGetter

            ' draw group color bar
            Call g.FillRectangle(groupd(col).color.GetBrush, boxCell)

            For i = 0 To data.rownames.Length - 1
                color = range.ScaleMapping(vec(i), index)
                boxCell = New RectangleF(x, y, dx, dy)
                g.FillRectangle(heatmap(color), boxCell)
                y += dy
            Next

            Call g.DrawString(col, label_font, Brushes.Black, x, y + 20, 60)

            x += dx
            y = heatmap_region.Top
        Next

        ' draw class HC-tree
        x = tree_region.Left
        dx = tree_region.Width * 0.25

        For i = 0 To data.rownames.Length - 1
            boxCell = New RectangleF(x, y, dx, dy)
            g.FillRectangle(New SolidBrush(class_colors.GetColor([class](i))), boxCell)
            y += dy
        Next
    End Sub
End Class
