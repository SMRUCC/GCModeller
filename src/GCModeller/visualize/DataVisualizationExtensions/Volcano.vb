﻿#Region "Microsoft.VisualBasic::42d69ed3e6bbf20aaac1384337fa4a36, GCModeller.DataVisualization\Volcano.vb"

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

' Module Volcano
' 
'     Properties: PValueThreshold
' 
'     Function: CreateModel, GetLegends, Plot, (+2 Overloads) PlotDEGs
'     Structure DEGModel
' 
'         Properties: label, logFC, pvalue
' 
'         Function: ToString
' 
'     Enum LabelTypes
' 
'         ALL, Custom, DEG, None
' 
' 
' 
'  
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.d3js
Imports Microsoft.VisualBasic.Imaging.d3js.Layout
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Visualize

''' <summary>
''' 用来可视化差异表达基因
''' </summary>
Public Module Volcano

    Public ReadOnly Property PValueThreshold# = -Math.Log10(0.05)

    ReadOnly DEG_diff# = Math.Log(2, 2)
    ReadOnly DEP_diff# = Math.Log(1.5, 2)

    Const UP$ = "Up"
    Const DOWN$ = "Down"

    <Extension>
    Public Function PlotDEGs(genes As IEnumerable(Of EntityObject),
                             Optional size$ = "2000,1850",
                             Optional padding$ = g.DefaultPadding,
                             Optional bg$ = "white",
                             Optional logFC$ = "logFC",
                             Optional pvalue$ = "p.value",
                             Optional displayLabel As LabelTypes = LabelTypes.None,
                             Optional labelFontStyle$ = CSSFont.PlotTitle,
                             Optional ylayout As YAxisLayoutStyles = YAxisLayoutStyles.ZERO) As GraphicsData

        Return genes.PlotDEGs(
            x:=Function(gene)
                   Return Val(gene(logFC))
               End Function,
            y:=Function(gene)
                   Return Val(gene(pvalue))
               End Function,
            label:=Function(gene) gene.ID,
            size:=size,
            padding:=padding,
            bg:=bg,
            displayLabel:=displayLabel,
            labelFontStyle:=labelFontStyle,
            ylayout:=ylayout)
    End Function

    <Extension>
    Public Function PlotDEGs(Of T)(genes As IEnumerable(Of T),
                                   x As Func(Of T, Double),
                                   y As Func(Of T, Double),
                                   label As Func(Of T, String),
                                   Optional size$ = "2000,1850",
                                   Optional padding$ = g.DefaultPadding,
                                   Optional bg$ = "white",
                                   Optional displayLabel As LabelTypes = LabelTypes.None,
                                   Optional labelFontStyle$ = CSSFont.Win10Normal,
                                   Optional ylayout As YAxisLayoutStyles = YAxisLayoutStyles.ZERO) As GraphicsData

        Dim factor As Func(Of DEGModel, Integer) =
            Function(DEG)
                If DEG.pvalue < PValueThreshold Then
                    Return 0
                End If

                If DEG.logFC >= DEG_diff Then
                    Return 1
                Else
                    Return -1
                End If
            End Function
        Dim colors As New Dictionary(Of Integer, Color) From {
            {1, Color.Red},    ' 上调
            {-1, Color.Lime}, ' 下调
            {0, Color.Gray}    ' 没有变化
        }
        Return genes _
            .Select(Function(g)
                        Return New DEGModel With {
                            .label = label(g),
                            .logFC = x(g),
                            .pvalue = y(g)
                        }
                    End Function).Plot(factor, colors,
                size, padding, bg,
                displayLabel:=displayLabel,
                labelFontStyle:=labelFontStyle,
                axisLayout:=ylayout
            )
    End Function

    ReadOnly black As Brush = Brushes.Black
    ReadOnly P As DefaultValue(Of Func(Of Double, Double)) = New Func(Of Double, Double)(Function(pvalue) -Math.Log10(pvalue))

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Private Function CreateModel(Of T As IDeg)(source As IEnumerable(Of T), pvalueTranslate As Func(Of Double, Double), labelP#) As IEnumerable(Of DEGModel)
        Return source _
            .Select(Function(g)
                        Dim P = pvalueTranslate(g.pvalue)

                        Return New DEGModel With {
                            .label = If(P >= labelP, g.label, Nothing),
                            .logFC = g.log2FC,
                            .pvalue = P
                        }
                    End Function)
    End Function

    ''' <summary>
    ''' 绘制差异表达基因的火山图
    ''' </summary>
    ''' <param name="genes"></param>
    ''' <param name="colors"></param>
    ''' <param name="factors">
    ''' 这个函数描述了如何从<paramref name="colors"/>参数之中取出差异表达基因自己所对应的颜色值
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function Plot(Of T As IDeg)(genes As IEnumerable(Of T),
                                       factors As Func(Of DEGModel, Integer),
                                       colors As Dictionary(Of Integer, Color),
                                       Optional size$ = "2000,2250",
                                       Optional padding$ = g.DefaultPadding,
                                       Optional bg$ = "white",
                                       Optional xlab$ = "log<sub>2</sub>(Fold Change)",
                                       Optional ylab$ = "-log<sub>10</sub>(p-value)",
                                       Optional title$ = "Volcano plot",
                                       Optional log2Threshold# = 2,
                                       Optional pvalueThreshold# = 0.05,
                                       Optional thresholdStroke$ = Stroke.AxisGridStroke,
                                       Optional ptSize! = 5,
                                       Optional translate As Func(Of Double, Double) = Nothing,
                                       Optional displayLabel As LabelTypes = LabelTypes.None,
                                       Optional labelFontStyle$ = CSSFont.Win7Normal,
                                       Optional legendFont$ = CSSFont.Win7LargeBold,
                                       Optional titleFontStyle$ = CSSFont.Win7Large,
                                       Optional ticksFontStyle$ = CSSFont.Win7LargerBold,
                                       Optional axisLayout As YAxisLayoutStyles = YAxisLayoutStyles.ZERO,
                                       Optional displayCount As Boolean = True,
                                       Optional labelP As Double = -1) As GraphicsData

        Dim DEG_matrix As DEGModel() = genes _
            .CreateModel(translate Or P, labelP) _
            .Where(Function(g) Not g.pvalue.IsNaNImaginary) _
            .ToArray

        ' 下面分别得到了log2fc的对称range，以及pvalue范围
        Dim xRange As DoubleRange = DEG_matrix _
            .Select(Function(d) Math.Abs(d.logFC)) _
            .Where(Function(n) Not n.IsNaNImaginary) _
            .Max _
            .SymmetricalRange
        Dim yRange As DoubleRange = {
            0, DEG_matrix _
                .Select(Function(d) d.pvalue) _
                .Where(Function(n) Not n.IsNaNImaginary) _
                .Max
        }
        Dim xTicks = xRange.CreateAxisTicks
        Dim yTicks = yRange.CreateAxisTicks

        Dim brushes As Dictionary(Of Integer, Brush) = colors _
            .ToDictionary(Function(k) k.Key,
                          Function(br)
                              Return DirectCast(New SolidBrush(br.Value), Brush)
                          End Function)
        Dim labelFont As Font = CSSFont.TryParse(labelFontStyle)
        Dim titleFont As Font = CSSFont.TryParse(titleFontStyle)
        Dim ticksFont As Font = CSSFont.TryParse(ticksFontStyle)
        Dim thresholdPen As Pen = Stroke.TryParse(thresholdStroke).GDIObject
        Dim point As PointF
        Dim px!, py!
        Dim up%, down%

        Return g.Allocate(size.SizeParser, padding, bg) <=
 _
            Sub(ByRef g As IGraphics, region As GraphicsRegion)

                ' 布局如下：
                '
                '          title
                '   +----------------+
                '   |         legends|
                ' y |                |
                '   |  scatter plots |
                '   +----------------+
                '           x

                ' 先计算出title文件的大小
                Dim titleSize As SizeF = g.MeasureString(title, titleFont)
                Dim top! = titleSize.Height * 1.5 + ticksFont.Height + 10
                Dim left! = g.MeasureString("00.0", ticksFont).Width + 10
                Dim plotRegion As New Rectangle With {
                    .X = region.Padding.Left + left,
                    .Y = region.Padding.Top + titleSize.Height * 1.5,
                    .Width = region.PlotRegion.Width - left,
                    .Height = region.PlotRegion.Height - top
                }   ' 得到最终剩余的绘图区域

                Dim x, y As d3js.scale.LinearScale

                With plotRegion
                    x = d3js.scale.linear.domain(xTicks).range(integers:={ .Left, .Right})
                    y = d3js.scale.linear.domain(yTicks).range(integers:={plotRegion.Top, plotRegion.Bottom})
                End With

                Dim scaler As New DataScaler With {
                    .AxisTicks = (xTicks, yTicks),
                    .region = plotRegion,
                    .X = x,
                    .Y = y
                }

                ' 必须要首先绘制出坐标轴，否则背景填充会将下面的几条阈值虚线给覆盖掉的
                Call g.DrawAxis(region, scaler, True, xlabel:=xlab, ylabel:=ylab, ylayout:=axisLayout)
                Call g.DrawRectangle(Pens.Black, plotRegion)

                ' 绘制出顶部的大标题
                point = New PointF With {
                    .X = region.Padding.Left + (region.PlotRegion.Width - titleSize.Width) / 2,
                    .Y = region.Padding.Top
                }
                Call g.DrawString(title, titleFont, New SolidBrush(Color.Black), point)

                ' 分别绘制出log2(level)和pvalue的4条threshold虚线条
                log2Threshold = Log2(Math.Abs(log2Threshold))

                left = x(log2Threshold)
                Call g.DrawLine(thresholdPen, New Point(left, plotRegion.Top), New Point(left, plotRegion.Bottom))

                left = x(-log2Threshold)
                Call g.DrawLine(thresholdPen, New Point(left, plotRegion.Top), New Point(left, plotRegion.Bottom))

                ' 在绘制出pvalue的临界值虚线
                top = scaler.TranslateY(-Math.Log10(pvalueThreshold))
                Call g.DrawLine(thresholdPen, New Point(plotRegion.Left, top), New Point(plotRegion.Right, top))

                Dim labels As New List(Of Label)
                Dim anchors As New List(Of PointF)

                For Each gene As DEGModel In DEG_matrix
                    Dim factor% = factors(gene)
                    Dim color As Brush = brushes(factor)

                    point = scaler.Translate(gene.logFC, gene.pvalue)
                    g.DrawCircle(point, ptSize, color)

                    If factor > 0 Then
                        up += 1
                    ElseIf factor < 0 Then
                        down += 1
                    End If

                    Select Case displayLabel
                        Case LabelTypes.None
                            ' 不进行任何操作
                        Case LabelTypes.DEG
                            If factor <> 0 AndAlso Not gene.label.StringEmpty Then
                                labels.Add(New Label(gene.label, point, g.MeasureString(gene.label, labelFont)))
                                anchors.Add(point)
                            End If
                        Case LabelTypes.ALL
                            If Not gene.label.StringEmpty Then
                                labels.Add(New Label(gene.label, point, g.MeasureString(gene.label, labelFont)))
                                anchors.Add(point)
                            End If
                        Case Else  ' 自定义
                            If Not gene.label.StringEmpty Then
                                labels.Add(New Label(gene.label, point, g.MeasureString(gene.label, labelFont)))
                                anchors.Add(point)
                            End If
                    End Select
                Next

                If labels > 0 Then
                    Dim black As SolidBrush = System.Drawing.Brushes.Black

                    Call d3js.labeler(maxMove:=20) _
                        .Labels(labels) _
                        .Anchors(labels.GetLabelAnchors(ptSize)) _
                        .Width(plotRegion.Width) _
                        .Height(plotRegion.Height) _
                        .Start(showProgress:=True, nsweeps:=1000)

                    For Each label As SeqValue(Of Label) In labels.SeqIterator
                        With label.value
                            Dim textAnchor = .Rectangle _
                                             .GetTextAnchor(anchors(label))

                            Call g.DrawLine(Pens.Black, textAnchor, anchors(label))
                            Call g.DrawString(.text, labelFont, black, .ByRef)
                        End With
                    Next
                End If

                With region
                    Dim legends = colors.GetLegends(legendFont, (up, down), displayCount)
                    Dim lsize As SizeF = legends.MaxLegendSize(g)

                    px = .Size.Width - .Padding.Left - (lsize.Width + 50)
                    py = plotRegion.Top + .Padding.Top / 2
                    point = New PointF(px, py)

                    Call g.DrawLegends(point.ToPoint, legends, gSize:="40,40")
                End With
            End Sub
    End Function

    <Extension>
    Private Function GetLegends(colors As Dictionary(Of Integer, Color), font$, count As (up%, down%), displayCount As Boolean) As Legend()
        Dim up As New Legend With {
            .color = colors(1).RGBExpression,
            .fontstyle = font,
            .style = LegendStyles.Circle,
            .title = "log2FC >= UP" Or $"({count.up}) log2FC >= UP".AsDefault(Function() displayCount)
        }
        Dim down As New Legend With {
            .color = colors(-1).RGBExpression,
            .fontstyle = font,
            .style = LegendStyles.Circle,
            .title = "log2FC <= DOWN" Or $"({count.down}) log2FC <= DOWN".AsDefault(Function() displayCount)
        }
        Dim normal As New Legend With {
            .color = colors(0).RGBExpression,
            .fontstyle = font,
            .style = LegendStyles.Circle,
            .title = "unchange"
        }

        Return {normal, up, down}
    End Function

    Public Structure DEGModel
        Implements IDeg

        Public Property label$ Implements IDeg.label
        Public Property logFC# Implements IDeg.log2FC
        Public Property pvalue# Implements IDeg.pvalue

        Public Overrides Function ToString() As String
            Return $"[{label}] log2FC={logFC}, pvalue={pvalue}"
        End Function
    End Structure

    Public Enum LabelTypes
        None
        ''' <summary>
        ''' <see cref="DEGModel.label"/>不为空字符串的时候就会被显示出来
        ''' </summary>
        Custom
        ALL
        DEG
    End Enum
End Module
