#Region "Microsoft.VisualBasic::7a1adcb4558da313757510fb55204714, visualize\DataVisualizationExtensions\KeggPlot\EnrichmentCategoryBubble.vb"

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

'   Total Lines: 100
'    Code Lines: 86
' Comment Lines: 1
'   Blank Lines: 13
'     File Size: 5.11 KB


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
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports std = System.Math

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
        Dim plotH As Single = canvas.PlotRegion.Height
        Dim top As Single = canvas.PlotRegion.Top
        Dim termH As Single = plotH / (enrich.Count - 1 + Aggregate ci In enrich Into Sum(ci.Value.Length))
        Dim max_string = enrich.Values.IteratesALL.Select(Function(ti) ti.name).MaxLengthString
        Dim name_label_font As Font = CSSFont.TryParse(theme.axisLabelCSS).GDIObject(g.Dpi)
        Dim max_string_size As SizeF = g.MeasureString(max_string, name_label_font)
        Dim left = canvas.PlotRegion.Left + max_string_size.Width
        Dim bubble_rect_width As Single = canvas.PlotRegion.Width - max_string_size.Width
        Dim labelColor As Brush = theme.tagColor.GetBrush
        Dim y As Single = canvas.PlotRegion.Top
        Dim dashline As Pen = Stroke.TryParse(theme.gridStrokeY)
        Dim plotW As Single = canvas.PlotRegion.Width - g.MeasureString(max_string, name_label_font).Width
        Dim x_Ticks As Vector = (-enrich.Values.IteratesALL.Select(Function(ti) ti.pvalue).AsVector.Log10).CreateAxisTicks
        Dim radius_scaler As DoubleRange = enrich.Values.IteratesALL.Select(Function(ti) Val(ti.enriched)).AsVector
        Dim color_scaler As DoubleRange = enrich.Values.IteratesALL.Select(Function(ti) ti.FDR).AsVector
        Dim radius As New DoubleRange(0.45 * termH, termH)
        Dim colors As Brush() = Designer.GetBrushes(theme.colorSet, mapLevels)
        Dim colorOffset As New DoubleRange(0, colors.Length - 1)
        Dim boxFill As Brush = Brushes.LightGray
        Dim axis_stroke As Pen = Stroke.TryParse(theme.axisStroke)
        Dim scaler As New DataScaler() With {
            .AxisTicks = (x_Ticks, {canvas.PlotRegion.Top, canvas.PlotRegion.Bottom}),
            .region = New Rectangle(left + radius.Min, top, bubble_rect_width - radius.Min, y - top),
            .X = d3js.scale.linear().range(values:={ .region.Left, .region.Right}).domain(x_Ticks),
            .Y = d3js.scale.constant(0)
        }
        Dim categoryBarWidth As Single = 0.05 * plotW
        Dim radiusVal As New List(Of Double)

        Call Array.Reverse(colors)

        For Each category As String In enrich.Keys
            Dim y0 As Single = y

            For Each term As EnrichmentResult In enrich(category)
                Dim label_size As SizeF = g.MeasureString(term.name, name_label_font)
                Dim label_left = left - label_size.Width
                Dim label_pos As New PointF(label_left, y)
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

        Dim tickFont As Font = CSSFont.TryParse(theme.axisTickCSS).GDIObject(g.Dpi)

        ' Call g.DrawLine(Stroke.TryParse(theme.axisStroke).GDIObject, scaler.region.Left, scaler.region.Bottom, scaler.region.Right, scaler.region.Bottom)
        Call Axis.DrawX(g, axis_stroke, "-log10(p)", scaler, XAxisLayoutStyles.ZERO, y, Nothing, theme.axisLabelCSS,
                        Brushes.Black, tickFont,
                        Brushes.Black, htmlLabel:=False)

        Dim legendTitleFont As Font = CSSFont.TryParse(theme.legendTitleCSS).GDIObject(g.Dpi)
        Dim legend_layout As New Rectangle(
            canvas.PlotRegion.Right + categoryBarWidth * 2,
            canvas.PlotRegion.Top + canvas.PlotRegion.Height / 6,
            canvas.Padding.Right * (2 / 3),
            canvas.PlotRegion.Height / 3)

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
            .circleStroke = Stroke.TryParse(theme.axisStroke).GDIObject,
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
