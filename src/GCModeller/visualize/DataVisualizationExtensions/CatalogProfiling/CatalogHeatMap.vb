
Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS

Namespace CatalogProfiling

    ''' <summary>
    ''' heatmap data of the KEGG enrichment between 
    ''' multiple groups data:
    ''' 
    ''' 1. x axis is the sample id
    ''' 2. y axis is the pathway name and category data
    ''' 3. cell size is the impact value or enrich factor
    ''' 4. cell color is scaled via -log10(pvalue)
    ''' </summary>
    Public Class CatalogHeatMap : Inherits MultipleCategoryProfiles

        ReadOnly mapLevels As Integer

        Public Sub New(profile As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm()))), mapLevels As Integer, theme As Theme)
            Call MyBase.New(profile, theme)

            Me.mapLevels = mapLevels
        End Sub

        ''' <summary>
        ''' heatmap是按照代谢途径分块绘制的
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="canvas"></param>
        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
            Dim pathways As Dictionary(Of String, String()) = getPathways()
            Dim pathwayNameFont As Font = CSSFont.TryParse(theme.axisTickCSS).GDIObject(g.Dpi)
            Dim categoryFont As Font = CSSFont.TryParse(theme.axisLabelCSS).GDIObject(g.Dpi)
            Dim pvalues As DoubleRange = multiples _
                .Select(Function(v) v.Value.Values) _
                .IteratesALL _
                .IteratesALL _
                .Select(Function(b) b.PValue) _
                .Range
            Dim impacts As DoubleRange = multiples _
                .Select(Function(v) v.Value.Values) _
                .IteratesALL _
                .IteratesALL _
                .Select(Function(p) p.data) _
                .Range
            Dim viz As IGraphics = g
            Dim maxTag As SizeF = pathways.Values _
                .IteratesALL _
                .Select(Function(str)
                            Return viz.MeasureString(str, pathwayNameFont)
                        End Function) _
                .JoinIterates(pathways.Keys.Select(Function(str) viz.MeasureString(str, categoryFont))) _
                .OrderByDescending(Function(sz) sz.Width) _
                .First
            Dim region As New Rectangle With {
                .X = canvas.PlotRegion.Left,
                .Y = canvas.PlotRegion.Top,
                .Width = canvas.PlotRegion.Width - maxTag.Width,
                .Height = canvas.PlotRegion.Height
            }
            Dim gap As Double = 50
            Dim dh As Double = (region.Height - gap * (pathways.Count - 1)) / (pathways.Values.IteratesALL.Count)
            Dim dw As Double = region.Width / multiples.Length
            Dim sizeRange As DoubleRange = New Double() {0, dw}
            Dim colors As SolidBrush() = Designer _
                .GetColors(theme.colorSet, mapLevels) _
                .Select(Function(c) New SolidBrush(c)) _
                .ToArray
            Dim indexRange As DoubleRange = New Double() {0, mapLevels - 1}
            Dim y As Double = region.Top
            Dim x As Double

            For Each catName As String In pathways.Keys
                Dim pathIds As String() = pathways(catName)
                Dim samples = multiples _
                    .Select(Function(v)
                                Dim list = v.Value.TryGetValue(catName)
                                Dim maps As Dictionary(Of String, BubbleTerm) = Nothing

                                If Not list Is Nothing Then
                                    maps = list.ToDictionary(Function(p) p.termId)
                                End If

                                Return New NamedValue(Of Dictionary(Of String, BubbleTerm)) With {
                                    .Name = v.Name,
                                    .Value = maps
                                }
                            End Function) _
                    .ToArray

                Call g.DrawString(catName, categoryFont, Brushes.Black, New PointF(region.Right, y))

                Dim top As Double = y

                y += dh

                For Each id As String In pathIds
                    x = region.Left

                    For Each sample In samples
                        If (Not sample.Value.IsNullOrEmpty) AndAlso sample.Value.ContainsKey(id) Then
                            Dim bubble As BubbleTerm = sample.Value(id)
                            Dim index As Integer = pvalues.ScaleMapping(bubble.PValue, indexRange)
                            Dim paint As Brush = colors(index)
                            Dim size As Double = impacts.ScaleMapping(bubble.data, sizeRange)
                            Dim cell As New RectangleF With {
                                .X = x,
                                .Y = y,
                                .Width = dw,
                                .Height = dw
                            }

                            Call g.FillRectangle(paint, cell)
                        End If

                        x += dw
                    Next

                    Call g.DrawString(id, pathwayNameFont, Brushes.Black, New PointF(x, y))
                Next

                Call g.DrawRectangle(Stroke.TryParse(theme.axisStroke).GDIObject, New Rectangle(region.Left, top, region.Width, y - top))

                y += gap
            Next
        End Sub
    End Class
End Namespace