Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Vector.Shapes
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Public Module EnrichPlot

    <Extension>
    Public Function BubblePlot(data As IEnumerable(Of EnrichmentTerm),
                               GO_terms As Dictionary(Of Term),
                               Optional size$ = "1600,1200",
                               Optional padding$ = g.DefaultPadding,
                               Optional bg$ = "white",
                               Optional unenrichColor$ = "gray",
                               Optional enrichColorSchema$ = "darkblue,lime,red",
                               Optional pvalue# = 0.01,
                               Optional legendFont$ = CSSFont.PlotSmallTitle,
                               Optional geneIDFont$ = CSSFont.Win10Normal,
                               Optional geneIDDisplayPvalue# = 0.0001) As Image

        Dim enrichResult = data.EnrichResult(GO_terms)
        Dim colors As Color() = Designer.GetColors(enrichColorSchema).Alpha(240)
        Dim unenrich As Color = unenrichColor.TranslateColor

        Return g.GraphicsPlots(
            size.SizeParser, padding,
            bg,
            Sub(ByRef g, region)
                Call g.__plotInternal(region, enrichResult, unenrich, colors, pvalue, legendFont)
            End Sub)
    End Function

    <Extension>
    Public Function EnrichResult(data As IEnumerable(Of EnrichmentTerm), GO_terms As Dictionary(Of Term)) As Dictionary(Of String, EnrichmentTerm())
        Dim result As New Dictionary(Of String, List(Of EnrichmentTerm))

        For Each term As EnrichmentTerm In data.Where(Function(t) GO_terms.ContainsKey(t.ID))
            Dim goTerm As Term = GO_terms(term.ID)

            If Not result.ContainsKey(goTerm.namespace) Then
                Call result.Add(goTerm.namespace, New List(Of EnrichmentTerm))
            End If

            Call result(goTerm.namespace).Add(term)
        Next

        Dim out = result.ToDictionary(Function(k) k.Key, Function(v) v.Value.ToArray)
        Return out
    End Function

    <Extension>
    Private Sub __plotInternal(g As Graphics,
                               region As GraphicsRegion,
                               result As Dictionary(Of String, EnrichmentTerm()),
                               unenrich As Color,
                               enrichColors As Color(), pvalue#, legendFontStyle$)
        Dim serials As SerialData() = result _
            .SeqIterator _
            .Select(Function(cat) (+cat).Value.__createModel(
                ns:=(+cat).Key,
                color:=enrichColors(cat),
                pvalue:=pvalue)) _
            .Join(
            {
                result.Values _
                    .IteratesALL _
                    .__unenrichSerial(pvalue, color:=unenrich)
            }) _
            .ToArray  ' 这些都是经过筛选的，pvalue阈值符合条件的，剩下的pvalue阈值不符合条件的都被当作为同一个serials
        Dim plot As Bitmap = Bubble.Plot(
            serials,
            size:=New Size(region.Size.Width * 0.85, region.Size.Height),
            legend:=False)

        Call g.DrawImageUnscaled(plot, New Point)

        Dim legends As Legend() = serials _
            .Select(Function(s) New Legend With {
                .color = s.color.RGBExpression,
                .fontstyle = legendFontStyle,
                .style = LegendStyles.Circle,
                .title = s.title
            }).ToArray
        Dim ltopLeft As New Point(
            plot.Width - region.Padding.Horizontal,
            region.Size.Height * 0.3)

        Call g.DrawLegends(
            ltopLeft,
            legends,
            graphicSize:=New Size(60, 35),
            regionBorder:=New Border With {
                .color = Color.Black,
                .style = DashStyle.Solid,
                .width = 2
            })
    End Sub

    <Extension>
    Private Function __unenrichSerial(catalog As IEnumerable(Of EnrichmentTerm), pvalue#, color As Color) As SerialData
        Dim unenrichs = catalog.Where(Function(term) term.CorrectedPvalue > pvalue)
        Return New SerialData With {
            .color = color,
            .title = "Unenrich terms",
            .pts = unenrichs _
                .Select(Function(gene) New PointData With {
                    .value = Math.Log(gene.number, 1.25) + 1,
                    .pt = New PointF(x:=gene.number / gene.Backgrounds, y:=gene.P)
                }).ToArray
        }
    End Function

    ''' <summary>
    ''' 返回来的是经过cutoff的数据
    ''' </summary>
    ''' <param name="catalog"></param>
    ''' <param name="ns$"></param>
    ''' <param name="color"></param>
    ''' <param name="pvalue#"></param>
    ''' <returns></returns>
    <Extension>
    Private Function __createModel(catalog As EnrichmentTerm(), ns$, color As Color, pvalue#) As SerialData
        Return New SerialData With {
            .color = color,
            .title = ns,
            .pts = catalog _
                .Where(Function(gene) gene.CorrectedPvalue <= pvalue) _
                .Select(Function(gene) New PointData With {
                    .value = Math.Log(gene.number, 1.25) + 1,
                    .pt = New PointF(x:=gene.number / gene.Backgrounds, y:=gene.P)
                }).ToArray
        }
    End Function
End Module
