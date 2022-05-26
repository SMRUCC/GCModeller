Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports FontStyle = System.Drawing.FontStyle
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

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
        ReadOnly alpha As Double = 1

        Sub New(multiples As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm()))),
                radius As DoubleRange,
                alpha As Double,
                theme As Theme)

            Call MyBase.New(theme)

            Me.radius = radius
            Me.multiples = multiples.ToArray
            Me.alpha = alpha
        End Sub

        Public Shared Iterator Function TopBubbles(multiples As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm()))), topN As Integer) As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm())))
            Dim all = multiples.ToArray
            Dim categories As String() = all _
               .Select(Function(t) t.Value.Keys) _
               .IteratesALL _
               .Distinct _
               .ToArray
            Dim nsamples As Double = all.Length
            Dim takes = all _
                .Select(Function(d)
                            Return d.Value _
                                .Select(Function(b)
                                            Return b.Value.Select(Function(i) (i, category:=b.Key, sampleName:=d.Name))
                                        End Function)
                        End Function) _
                .IteratesALL _
                .IteratesALL _
                .Where(Function(d)
                           Dim i = d.i
                           Dim test1 = Not (i.data.IsNaNImaginary OrElse i.Factor.IsNaNImaginary OrElse i.PValue.IsNaNImaginary)
                           Dim test2 = i.data * i.PValue > 0 AndAlso i.Factor > 0

                           Return test1 AndAlso test2
                       End Function) _
                .GroupBy(Function(i) i.i.termId) _
                .Select(Function(t)
                            Dim category As String = t.First.category
                            Dim rsd As Double = t _
                                .Select(Function(xi) xi.i.PValue * xi.i.Factor) _
                                .JoinIterates(0.0.Repeats(nsamples - t.Count).Select(Function(any) randf.NextDouble * 99999)) _
                                .RSD()

                            Return (t, Category, rsd)
                        End Function) _
                .GroupBy(Function(i) i.category) _
                .Select(Function(i)
                            Return i _
                                .OrderByDescending(Function(a) a.rsd) _
                                .Take(topN) _
                                .ToArray
                        End Function) _
                .ToArray

            For Each sample In takes _
                .IteratesALL _
                .Select(Function(a) a.t) _
                .IteratesALL _
                .GroupBy(Function(a)
                             Return a.sampleName
                         End Function) _
                .OrderByDescending(Function(d) d.Count) _
                .Take(12)

                Dim cats = sample _
                    .GroupBy(Function(a) a.category) _
                    .ToDictionary(Function(a) a.Key,
                                  Function(a)
                                      Return a.Select(Function(i) i.i).ToArray
                                  End Function)

                Yield New NamedValue(Of Dictionary(Of String, BubbleTerm())) With {
                    .Name = sample.Key,
                    .Value = cats
                }
            Next
        End Function

        Private Sub drawRadiusLegend(ByRef g As IGraphics, impacts As DoubleRange, canvas As GraphicsRegion)
            Dim values As Double() = impacts.Enumerate(4)
            Dim x As Double = canvas.PlotRegion.Right + canvas.Padding.Right / 5
            Dim y As Double = canvas.Padding.Top * 1.125
            Dim r As Double
            Dim paint As SolidBrush = Brushes.Black
            Dim pos As PointF
            Dim tickFont As Font = CSSFont.TryParse(theme.axisTickCSS).GDIObject(g.Dpi)
            Dim labelFont As Font = CSSFont.TryParse(theme.legendTitleCSS).GDIObject(g.Dpi)

            g.DrawString("Enrichment Factor", labelFont, Brushes.Black, New PointF(x - impacts.ScaleMapping(values.Max, Me.radius) * 2, y))
            y += g.MeasureString("A", labelFont).Height * 1.5

            Dim ymin As Double = y
            Dim ymax As Double
            Dim nsize As SizeF = g.MeasureString("0", tickFont)

            For Each ip As Double In values
                r = impacts.ScaleMapping(ip, Me.radius)
                pos = New PointF(x, y)
                ymax = y
                y = y + r * 2.5 + 30

                Call g.DrawCircle(pos, r, paint)
            Next

            x = x + r * 1.5

            Call g.DrawString(values.Min.ToString("F4"), tickFont, Brushes.Black, New PointF(x + 5, ymin - nsize.Height / 2))
            Call g.DrawLine(New Pen(Color.Black, 2), New PointF(x, ymin), New PointF(x, ymax))
            Call g.DrawString(values.Max.ToString("F4"), tickFont, Brushes.Black, New PointF(x + 5, ymax - nsize.Height / 2))
        End Sub

        Private Function getSampleColors() As Dictionary(Of String, SolidBrush)
            Dim colors As Color() = Designer.GetColors("paper", n:=multiples.Length)
            Dim list As New Dictionary(Of String, SolidBrush)

            For Each sample In multiples.SeqIterator
                Call list.Add(sample.value.Name, New SolidBrush(colors(sample).Alpha(alpha * 255)))
            Next

            Return list
        End Function

        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
            Dim allScores As Double() = multiples _
                .Select(Function(t) t.Value.Values) _
                .IteratesALL _
                .IteratesALL _
                .Select(Function(b) b.PValue * b.data) _
                .OrderBy(Function(xi) xi) _
                .ToArray
            Dim pvalueTicks As Double() = allScores.CreateAxisTicks
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
            Dim dh As Double = region.Height / (pathways.Length + categories.Length + 1)
            Dim y As Double = region.Top - g.MeasureString("A", categoryFont).Height / 2
            Dim x As Double
            Dim impacts As DoubleRange = multiples _
                .Select(Function(t) t.Value.Values) _
                .IteratesALL _
                .IteratesALL _
                .Select(Function(b) b.Factor) _
                .Range
            Dim r As Double
            Dim colorSet As LoopArray(Of Color) = Designer.GetColors(theme.colorSet)
            Dim paint As SolidBrush
            Dim sampleColors As Dictionary(Of String, SolidBrush) = getSampleColors()

            Call g.DrawRectangle(Stroke.TryParse(theme.axisStroke).GDIObject, region)

            ' draw axis
            Call Axis.DrawX(
                g:=g,
                pen:=Stroke.TryParse(theme.axisStroke).GDIObject,
                label:=xlabel,
                scaler:=New DataScaler With {.AxisTicks = (pvalueTicks.AsVector, Nothing), .region = region, .X = xscale, .Y = Nothing},
                layout:=XAxisLayoutStyles.Bottom,
                Y0:=0,
                offset:=Nothing,
                labelFont:=theme.axisLabelCSS,
                labelColor:=Brushes.Black,
                tickFont:=CSSFont.TryParse(theme.axisTickCSS).GDIObject(g.Dpi),
                tickColor:=Brushes.Black,
                htmlLabel:=False
            )

            For Each catName As String In categories
                fontsize = g.MeasureString(catName, categoryFont)
                x = canvas.Padding.Left
                paint = New SolidBrush(++colorSet)

                Call Console.WriteLine(catName)
                Call g.DrawString(catName, categoryFont, Brushes.Black, New PointF(x, y))

                Dim categoryData = multiples _
                    .Where(Function(group) group.Value.ContainsKey(catName)) _
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

                y = y + fontsize.Height
                fontsize = g.MeasureString("A", pathwayLabelFont)
                y = y + (dh - fontsize.Height) / 2

                ' draw bubbles in multiple groups
                For Each name As String In pathwayNames
                    Call g.DrawString(name, pathwayLabelFont, paint, New PointF(x + fontsize.Width / 2, y))
                    Call g.DrawLine(New Pen(paint, 3) With {.DashStyle = DashStyle.Dash}, New PointF(region.Left, y), New PointF(region.Right, y))

                    For Each group As NamedValue(Of Dictionary(Of String, BubbleTerm)) In categoryData
                        Dim bubble As BubbleTerm = group.Value.TryGetValue(name)
                        Dim fill As SolidBrush = sampleColors(group.Name)

                        If Not bubble Is Nothing Then
                            x = xscale(bubble.PValue * bubble.data)
                            r = impacts.ScaleMapping(bubble.Factor, Me.radius)

                            Call g.DrawCircle(New PointF(x, y), r, color:=fill)
                        End If
                    Next

                    y += dh
                    x = canvas.Padding.Left
                Next
            Next

            Call drawRadiusLegend(g, impacts, canvas)
            Call drawSampleLegends(sampleColors, g, canvas)
            Call DrawMainTitle(g, region)
        End Sub

        Private Sub drawSampleLegends(sampleColors As Dictionary(Of String, SolidBrush), g As IGraphics, canvas As GraphicsRegion)
            Dim legends As LegendObject() = sampleColors _
                .Select(Function(sample)
                            Return New LegendObject With {
                                .color = sample.Value.Color.ToHtmlColor,
                                .fontstyle = theme.legendLabelCSS,
                                .style = LegendStyles.Circle,
                                .title = sample.Key
                            }
                        End Function) _
                .ToArray

            theme.legendLayout = New Absolute() With {
                .x = canvas.PlotRegion.Right + 100,
                .y = canvas.PlotRegion.Top + canvas.PlotRegion.Height / 3
            }

            Call DrawLegends(g, legends, showBorder:=False, canvas)
        End Sub
    End Class
End Namespace