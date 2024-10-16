﻿#Region "Microsoft.VisualBasic::9f3ec0cec798005df7bf335c95cf5fed, visualize\DataVisualizationExtensions\KeggPlot\EnrichmentCategoryBubble.vb"

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


    ' Code Statistics:

    '   Total Lines: 159
    '    Code Lines: 136 (85.53%)
    ' Comment Lines: 2 (1.26%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 21 (13.21%)
    '     File Size: 7.86 KB


    ' Class EnrichmentCategoryBubble
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: PlotInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
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
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports std = System.Math

#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
#End If

Public Class EnrichmentCategoryBubble : Inherits HeatMapPlot

    ReadOnly enrich As Dictionary(Of String, EnrichmentResult())

    Public Sub New(enrich As IEnumerable(Of EnrichmentResult), theme As Theme, Optional top_n As Integer = 8)
        Call MyBase.New(theme)

        Dim kegg = htext.br08901.GetEntryDictionary

        Me.enrich = enrich _
            .GroupBy(Function(ti)
                         Dim key As String = ti.term.Match("\d+")

                         If kegg.ContainsKey(key) Then
                             Return kegg(key).class
                         Else
                             Return "NO_CLASS"
                         End If
                     End Function) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a _
                                 .OrderBy(Function(ti) ti.term) _
                                 .Take(top_n) _
                                 .ToArray
                          End Function)
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim css As CSSEnvirnment = g.LoadEnvironment
        Dim plotRect = canvas.PlotRegion(css)
        Dim plotH As Single = plotRect.Height
        Dim top As Single = plotRect.Top
        Dim termH As Single = plotH / (enrich.Count - 1 + Aggregate ci In enrich Into Sum(ci.Value.Length))
        Dim max_string = enrich.Values.IteratesALL.Select(Function(ti) ti.name).MaxLengthString
        Dim name_label_font As Font = CSS.GetFont(CSSFont.TryParse(theme.axisLabelCSS))
        Dim max_string_size As SizeF = g.MeasureString(max_string, name_label_font)
        Dim left = plotRect.Left + max_string_size.Width
        Dim bubble_rect_width As Single = plotRect.Width - max_string_size.Width
        Dim labelColor As Brush = theme.tagColor.GetBrush
        Dim y As Single = plotRect.Top
        Dim dashline As Pen = css.GetPen(Stroke.TryParse(theme.gridStrokeY))
        Dim plotW As Single = plotRect.Width - g.MeasureString(max_string, name_label_font).Width
        Dim x_Ticks As Vector = (-enrich.Values.IteratesALL.Select(Function(ti) ti.pvalue).AsVector.Log10).CreateAxisTicks
        Dim radius_scaler As DoubleRange = enrich.Values.IteratesALL.Select(Function(ti) Val(ti.enriched)).AsVector
        Dim color_scaler As DoubleRange = enrich.Values.IteratesALL.Select(Function(ti) ti.FDR).AsVector
        Dim radius As New DoubleRange(0.45 * termH, termH)
        Dim colors As Brush() = Designer.GetBrushes(theme.colorSet, mapLevels)
        Dim colorOffset As New DoubleRange(0, colors.Length - 1)
        Dim boxFill As Brush = Brushes.LightGray
        Dim axis_stroke As Pen = css.GetPen(Stroke.TryParse(theme.axisStroke))
        Dim scaler As New DataScaler() With {
            .AxisTicks = (x_Ticks, {plotRect.Top, plotRect.Bottom}),
            .region = New Rectangle(left + radius.Min, top, bubble_rect_width - radius.Min, y - top),
            .X = d3js.scale.linear().range(values:={ .region.Left, .region.Right}).domain(x_Ticks),
            .Y = d3js.scale.constant(0)
        }
        Dim categoryBarWidth As Single = 0.05 * plotW
        Dim radiusVal As New List(Of Double)
        Dim label_char_size As SizeF = g.MeasureString("A", name_label_font)

        Call Array.Reverse(colors)

        For Each category As String In enrich.Keys
            Dim y0 As Single = y

            For Each term As EnrichmentResult In enrich(category)
                Dim label_size As SizeF = g.MeasureString(term.name, name_label_font)
                Dim label_left = left - label_size.Width
                Dim label_pos As New PointF(label_left - label_char_size.Width, y)
                Dim r As Single = radius_scaler.ScaleMapping(Val(term.enriched), radius)
                Dim c As Brush = colors(CInt(color_scaler.ScaleMapping(term.FDR, colorOffset)))
                Dim xi As Single = -std.Log10(term.pvalue)

                xi = scaler.TranslateX(xi)
                y += termH

                radiusVal.Add(r)
                g.DrawString(term.name, name_label_font, labelColor, label_pos)
                g.DrawLine(dashline, New PointF(left, label_pos.Y + termH / 2), New PointF(left + plotW, label_pos.Y + termH / 2))
                g.DrawCircle(New PointF(xi - 2, label_pos.Y + termH / 2 - 2), r + 9, Brushes.Black)
                g.DrawCircle(New PointF(xi, label_pos.Y + termH / 2), r, c)
            Next

            ' fill box
            Dim box As New RectangleF(New PointF(left + plotW, y0), New SizeF(categoryBarWidth, y - y0))

            g.DrawLine(axis_stroke, New PointF(left, y0), New PointF(left, y))
            g.FillRectangle(boxFill, box)
            g.DrawString(category.ToUpper.First, name_label_font, Brushes.Black,
                         box.Left + max_string_size.Height,
                         box.Top + box.Height / 2,
                         angle:=90)
            y += termH / 2
        Next

        Dim tickFont As Font = css.GetFont(theme.axisTickCSS)

        ' Call g.DrawLine(Stroke.TryParse(theme.axisStroke).GDIObject, scaler.region.Left, scaler.region.Bottom, scaler.region.Right, scaler.region.Bottom)
        Call Axis.DrawX(g, axis_stroke, "-log10(p)", scaler, XAxisLayoutStyles.ZERO, y, Nothing, theme.axisLabelCSS,
                        Brushes.Black, tickFont,
                        Brushes.Black, htmlLabel:=False)

        Dim legendTitleFont As Font = css.GetFont(theme.legendTitleCSS)
        Dim legend_layout As New Rectangle(
            plotRect.Right + categoryBarWidth * 2,
            plotRect.Top + plotRect.Height / 6,
            css.GetWidth(canvas.Padding.Right) * (2 / 3),
            plotRect.Height / 3)

        Call Array.Reverse(colors)
        Call New ColorMapLegend(colors) With {
            .maxWidth = legend_layout.Width,
            .tickAxisStroke = axis_stroke,
            .tickFont = tickFont,
            .title = "Adjust p with BH",
            .ticks = color_scaler.CreateAxisTicks,
            .titleFont = legendTitleFont,
            .noblank = True
        }.Draw(g, legend_layout)

        Dim circle_layout As New Rectangle(
            legend_layout.Left,
            legend_layout.Bottom + max_string_size.Height * 2,
            legend_layout.Width,
            legend_layout.Height)

        Call New CircleSizeLegend With {
            .circleStroke = css.GetPen(Stroke.TryParse(theme.axisStroke)),
            .radius = radiusVal _
                .CreateAxisTicks(ticks:=5) _
                .Select(Function(d) CInt(d)) _
                .Distinct _
                .OrderBy(Function(a) a) _
                .ToArray,
            .radiusFont = tickFont,
            .title = "Count",
            .titleFont = legendTitleFont
        }.Draw(g, circle_layout)

    End Sub
End Class
