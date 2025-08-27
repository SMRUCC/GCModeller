#Region "Microsoft.VisualBasic::e4b50b1a30c9a3e59631e077e3d50c3b, visualize\DataVisualizationExtensions\DEGPlot\Volcano2.vb"

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

'   Total Lines: 31
'    Code Lines: 16 (51.61%)
' Comment Lines: 9 (29.03%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 6 (19.35%)
'     File Size: 873 B


' Class Volcano2
' 
'     Constructor: (+1 Overloads) Sub New
'     Sub: PlotInternal
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions
Imports std = System.Math
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend


#If NET48 Then
Imports SolidBrush = System.Drawing.SolidBrush
#Else
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
#End If

''' <summary>
''' volcano of multiple comparision analysis result
''' </summary>
Public Class VolcanoMultiple : Inherits Plot

    ''' <summary>
    ''' x
    ''' </summary>
    ReadOnly compares As NamedCollection(Of DEGModel)()
    ReadOnly deg_class As String
    ReadOnly non_deg As New SolidBrush(Color.LightGray)
    ReadOnly up_deg As New SolidBrush(Color.Red)
    ReadOnly down_deg As New SolidBrush(Color.Blue)
    ReadOnly countWidth As Boolean = False
    ReadOnly topN As Integer = 6
    ReadOnly rect_color As New SolidBrush(Color.FromArgb(245, 245, 245))

    Public Sub New(compares As IEnumerable(Of NamedCollection(Of DEGModel)), deg_class As String, theme As Theme)
        MyBase.New(theme)

        Me.deg_class = deg_class
        Me.compares = compares.ToArray
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim css As CSSEnvirnment = g.LoadEnvironment
        Dim plotRect = canvas.PlotRegion(css)
        Dim sumAll As Double = compares _
            .Select(Function(group) std.Log(group.Count(Function(e) e.class = deg_class) + 1)) _
            .Sum
        Dim maxLogFC As Double = compares _
            .Select(Function(group) group.Max(Function(e) e.logFC)) _
            .Max
        Dim minLogFC As Double = compares _
            .Select(Function(group) group.Min(Function(e) e.logFC)) _
            .Min
        Dim upTicks = {0, maxLogFC}.CreateAxisTicks
        Dim downTicks = {0, -minLogFC}.CreateAxisTicks
        Dim left As Double = plotRect.Left
        Dim zero As Double = plotRect.Top + plotRect.Height / 2
        Dim height As Double = plotRect.Height / 2
        Dim upAxis = d3js.scale.linear.domain(upTicks).range(0, height)
        Dim downAxis = d3js.scale.linear.domain(downTicks).range(0, height)
        Dim radius As Single = theme.pointSize
        Dim lineEdge As Pen = css.GetPen(Stroke.TryParse(theme.gridStrokeX))
        Dim xAxisLine As Pen = css.GetPen(Stroke.TryParse(theme.axisStroke))
        Dim meanWidth As Double = plotRect.Width / compares.Length
        Dim axisFont As Font = css.GetFont(theme.axisLabelCSS)
        Dim fheight As Single = g.MeasureString("A", axisFont).Height
        Dim labelFont As Font = css.GetFont(theme.tagCSS)

        ' draw zero
        Call g.DrawLine(xAxisLine, New PointF(plotRect.Left, zero), New PointF(plotRect.Right, zero))
        Call g.DrawLine(xAxisLine, New PointF(plotRect.Left, plotRect.Top), New PointF(plotRect.Left, plotRect.Bottom))
        Call g.DrawString("Average log2FC FoldChange", axisFont, Brushes.Black, plotRect.Left, zero, -90)

        For Each group As NamedCollection(Of DEGModel) In TqdmWrapper.Wrap(compares)
            Dim delta_with = If(countWidth, plotRect.Width * std.Log(group.Count(Function(e) e.class = deg_class) + 1) / sumAll, meanWidth)
            Dim width As Double = delta_with * 0.8
            Dim maxlogp As Double = std.Log(group.Max(Function(e) e.nLog10p) + 1)
            Dim halfWidth As Double = width / 2
            Dim top_degs = group _
                .Where(Function(gi) gi.class = deg_class) _
                .OrderByDescending(Function(a) std.Abs(a.logFC)) _
                .Take(topN) _
                .Keys _
                .Indexing
            Dim maxLogfcUp As Double = group.Max(Function(a) a.logFC)
            Dim maxlogfcDown As Double = std.Abs(group.Min(Function(a) a.logFC))

            Call g.DrawLine(lineEdge, New PointF(left, plotRect.Top), New PointF(left, plotRect.Bottom))

            Call g.FillRectangle(rect_color, New RectangleF(left - halfWidth, zero - upAxis(maxLogfcUp), width, upAxis(maxLogfcUp)))
            Call g.FillRectangle(rect_color, New RectangleF(left - halfWidth, zero, width, downAxis(maxlogfcDown)))

            ' draw non-deg first
            For Each gene As DEGModel In group.OrderBy(Function(gi) If(gi.class = deg_class, 1, 0))
                Dim maxoffset As Double = randf.NextDouble(0, std.Log((gene.nLog10p + 1) / maxlogp)) * halfWidth
                Dim x As Double = If(randf.NextDouble > 0.5, 1, -1) * maxoffset + left
                Dim y As Double = If(gene.logFC > 0, upAxis, downAxis)(std.Abs(gene.logFC))
                Dim sign As Double = If(gene.logFC > 0, 1, -1)
                Dim yi As Double = zero + sign * y

                If gene.class <> deg_class Then
                    Call g.DrawCircle(New PointF(x, yi), radius, non_deg)
                ElseIf gene.logFC > 0 Then
                    Call g.DrawCircle(New PointF(x, yi), radius, up_deg)
                Else
                    Call g.DrawCircle(New PointF(x, yi), radius, down_deg)
                End If

                If theme.drawLabels AndAlso gene.label Like top_degs Then
                    Call g.DrawString(gene.label, labelFont, Brushes.Black, New PointF(x, yi + If(gene.logFC > 0, fheight, -fheight)))
                End If
            Next

            If theme.drawLabels Then
                Call g.DrawString(group.name, axisFont, Brushes.Black, New PointF(left, plotRect.Bottom + fheight))
            End If

            left += delta_with
        Next

        If theme.drawLegend Then
            Dim legendUp As New LegendObject With {.color = "red", .fontstyle = theme.legendLabelCSS, .style = LegendStyles.Circle, .title = "Sig.up"}
            Dim legendDown As New LegendObject With {.color = "blue", .fontstyle = theme.legendLabelCSS, .style = LegendStyles.Circle, .title = "Sig.down"}

            Call DrawLegends(g, {legendUp, legendDown}, False, canvas)
        End If
    End Sub
End Class
