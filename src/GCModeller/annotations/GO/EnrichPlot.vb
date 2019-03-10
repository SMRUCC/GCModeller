﻿#Region "Microsoft.VisualBasic::f0d352c2085e78e7acba051c19308456, annotations\GO\EnrichPlot.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Module EnrichPlot
    ' 
    '     Function: __createModel, __unenrichSerial, BubblePlot, EnrichResult
    ' 
    '     Sub: __plotInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Scripting
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Public Module EnrichPlot

    ''' <summary>
    ''' GO富集结果可视化
    ''' </summary>
    ''' <param name="data">KOBAS富集计算分析的结果数据</param>
    ''' <param name="GO_terms">GO数据库</param>
    ''' <param name="size$">输出的图像的大小</param>
    ''' <param name="padding$">留白的大小</param>
    ''' <param name="bg$">背景色</param>
    ''' <param name="unenrichColor$">未被富集的go term的颜色，即那些pvalue值大于<paramref name="pvalue"/>参数值的go term的颜色，默认为浅灰色</param>
    ''' <param name="pvalue#">pvalue阈值</param>
    ''' <param name="legendFont$">legend的字体CSS</param>
    ''' <param name="geneIDFont$">term标签的显示字体CSS</param>
    ''' <param name="R$">term的半径大小的计算表达式</param>
    ''' <param name="displays%">每一个GO的命名空间分类之下的显示标签label的term的数量，默认为每个命名空间显示10个term的标签</param>
    ''' <param name="titleFontCSS$">标题字体的CSS字体</param>
    ''' <param name="title$">Plot绘图的标题</param>
    ''' <returns></returns>
    <Extension>
    Public Function BubblePlot(data As IEnumerable(Of EnrichmentTerm),
                               GO_terms As Dictionary(Of Term),
                               Optional size$ = "1600,1200",
                               Optional padding$ = g.DefaultPadding,
                               Optional bg$ = "white",
                               Optional unenrichColor$ = "gray", ' Optional enrichColorSchema$ = "Set1:c6",
                               Optional pvalue# = 0.01,
                               Optional legendFont$ = CSSFont.PlotSmallTitle,
                               Optional geneIDFont$ = CSSFont.Win10Normal,
                               Optional R$ = "log(x)",
                               Optional displays% = 10,
                               Optional titleFontCSS$ = CSSFont.Win7Large,
                               Optional title$ = "GO enrichment",
                               Optional bubbleBorder As Boolean = True) As GraphicsData

        Dim enrichResult = data.EnrichResult(GO_terms)
        Dim unenrich As Color = unenrichColor.TranslateColor
        Dim math As New Expression
        Dim calcR = Function(x#)
                        Call math.SetVariable("x", x#)
                        Return math.Evaluation(R)
                    End Function

        With New Dictionary(Of String, Color())

            !cellular_component = Designer.GetColors("OrRd:c9", alpha:=225)
            !molecular_function = Designer.GetColors("Blues:c9", alpha:=225)
            !biological_process = Designer.GetColors("Greens:c9", alpha:=225)

            Return g.GraphicsPlots(
                size.SizeParser, padding,
                bg,
                Sub(ByRef g, region)
                    Call g.__plotInternal(
                        region, enrichResult, unenrich,
                        .ByRef, pvalue,
                        legendFont,
                        r:=calcR,
                        displays:=displays,
                        showBubbleBorder:=bubbleBorder)

                    Dim titleFont As Font = CSSFont.TryParse(titleFontCSS).GDIObject
                    Dim fsize As SizeF = g.MeasureString(title, titleFont)
                    Dim tloc As New PointF(
                        (region.Size.Width - fsize.Width) / 2,
                        (region.Padding.Top - fsize.Height) / 2)

                    Call g.DrawString(title, titleFont, Brushes.Black, tloc)
                End Sub)
        End With
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

        ' Dim reorders = result.ToArray.OrderByDescending(Function(x) x.Value.Count)
        Dim out As New Dictionary(Of String, EnrichmentTerm())
        For Each ns In result
            With ns
                Call out.Add(.Key, .Value)
            End With
        Next
        Return out
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="region"></param>
    ''' <param name="result"></param>
    ''' <param name="unenrich"></param>
    ''' <param name="enrichColors"></param>
    ''' <param name="pvalue#"></param>
    ''' <param name="legendFontStyle$"></param>
    ''' <param name="r">点的半径大小的计算公式</param>
    <Extension>
    Private Sub __plotInternal(g As IGraphics,
                               region As GraphicsRegion,
                               result As Dictionary(Of String, EnrichmentTerm()),
                               unenrich As Color,
                               enrichColors As Dictionary(Of String, Color()),
                               pvalue#,
                               legendFontStyle$,
                               r As Func(Of Double, Double),
                               displays%,
                               showBubbleBorder As Boolean)

        Dim serials As SerialData() = result _
            .Keys _
            .Select(Function(category)
                        Dim color As Color() = enrichColors(category) _
                            .Skip(20) _
                            .Alpha(250) _
                            .ToArray
                        Dim terms = result(category).AsList
                        Return terms.__createModel(category, color, pvalue, r, displays)
                    End Function) _
            .Join({
                 result.Values _
                     .IteratesALL _
                     .__unenrichSerial(pvalue, color:=unenrich, r:=r)}) _
            .ToArray  ' 这些都是经过筛选的，pvalue阈值符合条件的，剩下的pvalue阈值不符合条件的都被当作为同一个serials
        Dim bubbleBorder As Stroke = Nothing

        If showBubbleBorder Then
            bubbleBorder = New Stroke With {
                .dash = DashStyle.Solid,
                .fill = "lightgray",
                .width = 1.5
            }
        End If

        Dim plot As GraphicsData = Bubble.Plot(
            serials,
            padding:="padding: 100 100 150 150",
            size:=New Size(region.Size.Width * 0.85, region.Size.Height),
            legend:=False,
            xAxis:="(0,1),tick=0.2",
            xlabel:="richFactor=(n/background)",
            ylabel:="-log<sub>10</sub>(p.value)",
            bubbleBorder:=bubbleBorder)

        Call g.DrawImageUnscaled(plot, New Point)

        Dim legends As Legend() = serials _
            .Select(Function(s) New Legend With {
                .color = s.color.RGBExpression,
                .fontstyle = legendFontStyle,
                .style = LegendStyles.Circle,
                .title = s.title
            }).ToArray
        Dim ltopLeft As New Point(
            plot.Width - 65,
            region.Size.Height * 0.3)

        Call g.DrawLegends(
            ltopLeft,
            legends,
            gSize:="60,35",
            regionBorder:=New Stroke With {
                .fill = "Black",
                .dash = DashStyle.Solid,
                .width = 2
            })
    End Sub

    <Extension>
    Private Function __unenrichSerial(catalog As IEnumerable(Of EnrichmentTerm), pvalue#, color As Color, r As Func(Of Double, Double)) As SerialData
        Dim unenrichs = catalog.Where(Function(term) term.CorrectedPvalue > pvalue)
        Return New SerialData With {
            .color = color,
            .title = "Unenrich terms",
            .pts = unenrichs _
                .Select(Function(gene) New PointData With {
                    .value = r(gene.number) + 1,
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
    Private Function __createModel(catalog As List(Of EnrichmentTerm), ns$, color As Color(), pvalue#, r As Func(Of Double, Double), displays%) As SerialData
        Dim pv = catalog.Select(Function(gene) gene.CorrectedPvalue).AsVector
        Dim enrichResults = catalog(Which.IsTrue(pv <= pvalue))
        Dim colorIndex%() = enrichResults _
            .Select(Function(gene) gene.P(correctedPvalue:=False)) _
            .RangeTransform($"0,{color.Length - 1}") _
            .Select(Function(i) CInt(i)) _
            .ToArray
        Dim s As New SerialData With {
            .color = color.Last,
            .title = ns,
            .pts = enrichResults _
                .SeqIterator _
                .Select(Function(obj)
                            Dim gene As EnrichmentTerm = obj
                            Dim c As Color = color(colorIndex(obj))

                            Return New PointData With {
                                .value = r(gene.number) + 1,
                                .pt = New PointF(x:=gene.number / gene.Backgrounds, y:=gene.P),
                                .Tag = gene.Term,
                                .color = c.ARGBExpression
                            }
                        End Function) _
                .OrderByDescending(Function(bubble)
                                       ' 按照y也就是pvalue倒序排序
                                       Return bubble.pt.Y
                                   End Function) _
                .ToArray
        }

        For i As Integer = displays To s.pts.Length - 1
            Dim pt = s.pts(i)
            s.pts(i) = New PointData With {
                .pt = pt.pt,
                .Tag = Nothing,  ' 只显示前五个term的标签字符串，其余的term的标签字符串都设置为空值，就不会被显示出来了
                .value = pt.value,
                .color = pt.color
            }
        Next

        Return s
    End Function
End Module
