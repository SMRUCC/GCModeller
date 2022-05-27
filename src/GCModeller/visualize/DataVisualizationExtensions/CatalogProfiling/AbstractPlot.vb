Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
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
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Namespace CatalogProfiling


    Public MustInherit Class MultipleCatalogHeatmap : Inherits MultipleCategoryProfiles

        Protected ReadOnly mapLevels As Integer
        Protected ReadOnly colorMissing As String

        Protected Sub New(multiples As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm()))),
                          mapLevels As Integer,
                          colorMissing As String,
                          theme As Theme
            )

            Call MyBase.New(multiples, theme)

            Me.mapLevels = mapLevels
            Me.colorMissing = colorMissing
        End Sub

        Protected Sub drawColorLegends(pvalues As DoubleRange, right As Double, ByRef g As IGraphics, canvas As GraphicsRegion, Optional y As Double = Double.NaN)
            Dim maps As New ColorMapLegend(palette:=theme.colorSet, mapLevels) With {
                .format = "F2",
                .noblank = False,
                .tickAxisStroke = Stroke.TryParse(theme.legendTickAxisStroke).GDIObject,
                .tickFont = CSSFont.TryParse(theme.legendTickCSS).GDIObject(g.Dpi),
                .ticks = pvalues.CreateAxisTicks,
                .title = "-log10(pvalue)",
                .titleFont = CSSFont.TryParse(theme.legendTitleCSS).GDIObject(g.Dpi),
                .unmapColor = colorMissing,
                .ruleOffset = 5,
                .legendOffsetLeft = 5
            }
            Dim layout As New Rectangle With {
                .X = right,
                .Width = canvas.Padding.Right * (2 / 3),
                .Height = canvas.PlotRegion.Height / 3,
                .Y = If(y.IsNaNImaginary, canvas.Padding.Top, y)
            }

            Call maps.Draw(g, layout)
        End Sub
    End Class

    Public MustInherit Class MultipleCategoryProfiles : Inherits Plot

        ''' <summary>
        ''' the multiple groups data
        ''' </summary>
        Protected ReadOnly multiples As NamedValue(Of Dictionary(Of String, BubbleTerm()))()

        Protected Sub New(multiples As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm()))), theme As Theme)
            Call MyBase.New(theme)

            Me.multiples = multiples.ToArray
        End Sub

        Protected Function getCategories() As String()
            Return multiples _
                .Select(Function(t) t.Value.Keys) _
                .IteratesALL _
                .Distinct _
                .ToArray
        End Function

        Protected Function getPathways() As Dictionary(Of String, String())
            Return multiples _
                .Select(Function(t) t.Value.Select(Function(b) b.Value.Select(Function(a) (a.termId, b.Key)))) _
                .IteratesALL _
                .IteratesALL _
                .GroupBy(Function(t) t.Key) _
                .ToDictionary(Function(a) a.Key,
                              Function(b)
                                  Return b _
                                      .Select(Function(t) t.termId) _
                                      .Distinct _
                                      .ToArray
                              End Function)
        End Function

        Public Shared Iterator Function TopBubbles(multiples As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm()))), 
                                                   topN As Integer, 
                                                   eval As Func(Of BubbleTerm, Double)) As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm())))
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
                                .Select(Function(xi) eval(xi.i)) _
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

    End Class
End Namespace