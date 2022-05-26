Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports FontStyle = System.Drawing.FontStyle

Namespace CatalogProfiling

    ''' <summary>
    ''' kegg enrichment bubble in multiple groups
    ''' 
    ''' 1. x axis is related to the -log10(pvalue)
    ''' 2. y axis is the category of the kegg pathway maps
    ''' 3. bubble size is the impact factor or pathway hit score
    ''' 4. color of the bubble can be related to the another score
    ''' </summary>
    Public Class MultipleBubble : Inherits Plot

        ''' <summary>
        ''' the multiple groups data
        ''' </summary>
        ReadOnly multiples As NamedValue(Of Dictionary(Of String, BubbleTerm()))()
        ReadOnly radius As DoubleRange

        Sub New(multiples As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm()))), theme As Theme)
            Call MyBase.New(theme)

            Me.multiples = multiples.ToArray
        End Sub

        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
            Dim pvalueTicks As Double() = multiples _
                .Select(Function(t) t.Value.Values) _
                .IteratesALL _
                .IteratesALL _
                .Select(Function(b) b.PValue) _
                .Range _
                .CreateAxisTicks
            Dim categories As String() = multiples _
                .Select(Function(t) t.Value.Keys) _
                .IteratesALL _
                .Distinct _
                .ToArray
            Dim pathways As String() = multiples _
                .Select(Function(t) t.Value.Values) _
                .IteratesALL _
                .IteratesALL _
                .Select(Function(b) b.termId) _
                .Distinct _
                .ToArray
            Dim fontsize As SizeF
            Dim pathwayLabelFont As Font = CSSFont.TryParse(theme.axisLabelCSS).GDIObject(g.Dpi)
            Dim categoryFont As New Font(pathwayLabelFont.Name, CSng(pathwayLabelFont.Size * 1.25), FontStyle.Bold)
            Dim viz As IGraphics = g
            Dim maxLabel As SizeF = categories _
                .Select(Function(str) viz.MeasureString(str, categoryFont)) _
                .JoinIterates(pathways.Select(Function(str)
                                                  Return viz.MeasureString(str, pathwayLabelFont)
                                              End Function)) _
                .OrderByDescending(Function(t) t.Width) _
                .First
            Dim region As New Rectangle With {
                .X = canvas.Padding.Left + maxLabel.Width,
                .Y = canvas.Padding.Top,
                .Width = canvas.PlotRegion.Width - maxLabel.Width,
                .Height = canvas.PlotRegion.Height
            }
            Dim xscale = d3js.scale _
                .linear() _
                .domain(values:=pvalueTicks) _
                .range(values:={region.Left, region.Right})

            Dim y As Double = region.Top + 5
            Dim x As Double
            Dim impacts = multiples.Select(Function(t) t.Value.Values).IteratesALL.IteratesALL.Select(Function(b) b.data).Range
            Dim r As Double

            Call g.DrawRectangle(Stroke.TryParse(theme.axisStroke).GDIObject, region)

            For Each catName As String In categories
                fontsize = g.MeasureString(catName, categoryFont)
                x = canvas.Padding.Left

                Call g.DrawString(catName, categoryFont, Brushes.Black, New PointF(x, y))

                Dim categoryData = multiples _
                    .Select(Function(group)
                                Return New NamedValue(Of Dictionary(Of String, BubbleTerm)) With {
                                    .Name = group.Name,
                                    .Value = group.Value(catName) _
                                        .ToDictionary(Function(t)
                                                          Return t.termId
                                                      End Function)
                                }
                            End Function) _
                    .ToArray
                Dim pathwayNames As String() = categoryData _
                    .Select(Function(t) t.Value.Keys) _
                    .IteratesALL _
                    .Distinct _
                    .ToArray

                fontsize = g.MeasureString("A", pathwayLabelFont)
                y += fontsize.Height / 2

                ' draw bubbles in multiple groups
                For Each name As String In pathwayNames
                    Call g.DrawString(name, pathwayLabelFont, Brushes.Black, New PointF(x + fontsize.Width, y))

                    For Each group As NamedValue(Of Dictionary(Of String, BubbleTerm)) In categoryData
                        Dim bubble As BubbleTerm = group.Value.TryGetValue(name)

                        If Not bubble Is Nothing Then
                            x = xscale(bubble.PValue)
                            r = impacts.ScaleMapping(bubble.data, Me.radius)

                            Call g.DrawCircle(New PointF(x, y), r, Brushes.Black)
                        End If
                    Next

                    y += fontsize.Height + 5
                Next
            Next
        End Sub
    End Class
End Namespace