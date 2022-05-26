Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
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

        Sub New(multiples As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm()))),
                radius As DoubleRange,
                theme As Theme)

            Call MyBase.New(theme)

            Me.radius = radius
            Me.multiples = multiples.ToArray
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
                            Return d.Value.Select(Function(b) b.Value.Select(Function(i) (i, category:=b.Key, sampleName:=d.Name)))
                        End Function) _
                .IteratesALL _
                .IteratesALL _
                .GroupBy(Function(i) i.i.termId) _
                .Select(Function(t)
                            Return (t, category:=t.First.category, rsd:=t.Select(Function(xi) xi.i.PValue).RSD(maxSize:=nsamples))
                        End Function) _
                .GroupBy(Function(i) i.category) _
                .Select(Function(i)
                            Return i.OrderByDescending(Function(a) a.rsd).Take(topN).ToArray
                        End Function) _
                .ToArray

            For Each sample In takes.IteratesALL.Select(Function(a) a.t).IteratesALL.GroupBy(Function(a) a.sampleName)
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
            Dim x As Double = canvas.PlotRegion.Right + canvas.Padding.Right / 2
            Dim y As Double = canvas.Padding.Top * 1.125
            Dim r As Double
            Dim paint As SolidBrush = Brushes.Black
            Dim pos As PointF
            Dim ymin As Double = y
            Dim ymax As Double
            Dim tickFont As Font = CSSFont.TryParse(theme.axisTickCSS).GDIObject(g.Dpi)

            For Each ip As Double In values
                r = impacts.ScaleMapping(ip, Me.radius)
                pos = New PointF(x, y)
                ymax = y
                y = y + r * 2.5 + 10

                Call g.DrawCircle(pos, r, paint)
            Next

            x = x + r * 1.5

            Call g.DrawString(values.Min.ToString("F4"), tickFont, Brushes.Black, New PointF(x + 5, ymin))
            Call g.DrawLine(New Pen(Color.Black, 2), New PointF(x, ymin), New PointF(x, ymax))
            Call g.DrawString(values.Max.ToString("F4"), tickFont, Brushes.Black, New PointF(x + 5, ymax))
        End Sub

        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
            Dim pvalueTicks As Double() = multiples _
                .Select(Function(t) t.Value.Values) _
                .IteratesALL _
                .IteratesALL _
                .Select(Function(b) b.PValue) _
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
            Dim impacts As DoubleRange = multiples _
                .Select(Function(t) t.Value.Values) _
                .IteratesALL _
                .IteratesALL _
                .Select(Function(b) b.data) _
                .Range
            Dim r As Double
            Dim colorSet As LoopArray(Of Color) = Designer.GetColors(theme.colorSet)
            Dim paint As SolidBrush

            Call g.DrawRectangle(Stroke.TryParse(theme.axisStroke).GDIObject, region)

            ' draw axis
            Call Axis.DrawX(
                g:=g,
                pen:=Stroke.TryParse(theme.axisStroke).GDIObject,
                label:="-log10(pvalue)",
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

                fontsize = g.MeasureString("A", pathwayLabelFont)
                y += fontsize.Height / 2

                ' draw bubbles in multiple groups
                For Each name As String In pathwayNames
                    Call g.DrawString(name, pathwayLabelFont, paint, New PointF(x + fontsize.Width, y))
                    Call g.DrawLine(New Pen(paint, 2) With {.DashStyle = DashStyle.Dot}, New PointF(region.Left, y), New PointF(region.Right, y))

                    For Each group As NamedValue(Of Dictionary(Of String, BubbleTerm)) In categoryData
                        Dim bubble As BubbleTerm = group.Value.TryGetValue(name)

                        If Not bubble Is Nothing Then
                            x = xscale(bubble.PValue)
                            r = impacts.ScaleMapping(bubble.data, Me.radius)

                            Call g.DrawCircle(New PointF(x, y), r, paint)
                        End If
                    Next

                    y += fontsize.Height + 5
                    x = canvas.Padding.Left
                Next
            Next

            Call drawRadiusLegend(g, impacts, canvas)
        End Sub
    End Class
End Namespace