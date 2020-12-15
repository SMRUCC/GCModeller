Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS

Public Class ClassChanges : Inherits Plot

    ReadOnly degClass As NamedCollection(Of DEGModel)()

    Public Sub New(deg As IEnumerable(Of DEGModel), theme As Theme)
        MyBase.New(theme)

        Me.degClass = deg _
            .GroupBy(Function(a) a.class) _
            .Select(Function(group)
                        Return New NamedCollection(Of DEGModel)(group.Key, group.ToArray)
                    End Function) _
            .ToArray
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim xTicks As Double() = degClass _
            .Select(Function(d) d.Select(Function(gi) gi.logFC)) _
            .IteratesALL _
            .Range _
            .CreateAxisTicks
        Dim plotregion As Rectangle = canvas.PlotRegion
        Dim y As Double = degClass.Length
        Dim x As Double
        Dim axisStroke As Pen = Stroke.TryParse(theme.axisStroke)
        Dim tickStroke As Pen = Stroke.TryParse(theme.axisTickStroke)
        Dim a As PointF
        Dim b As PointF
        Dim xscale = d3js.scale.linear.domain(xTicks).range(integers:={plotregion.Left, plotregion.Right})
        Dim labelText As String
        Dim labelSize As SizeF
        Dim labelFont As Font = CSSFont.TryParse(theme.axisTickCSS)
        Dim tickPadding As Double = g.MeasureString("0", labelFont).Height / 2
        Dim dh As Double = plotregion.Height / degClass.Length
        Dim colors As SolidBrush() = Designer _
            .GetColors(theme.colorSet, degClass.Length) _
            .Select(Function(c) New SolidBrush(c)) _
            .ToArray

        ' X
        a = New PointF(plotregion.Left, plotregion.Bottom)
        b = New PointF(plotregion.Right, plotregion.Bottom)

        Call g.DrawLine(axisStroke, a, b)

        For Each tick As Double In xTicks
            x = xscale(tick)
            a = New PointF(x, plotregion.Bottom)
            b = New PointF(x, plotregion.Bottom - tickPadding)
            labelText = tick.ToString(theme.axisTickFormat)
            labelSize = g.MeasureString(labelText, labelFont)

            Call g.DrawLine(tickStroke, a, b)
            Call g.DrawString(tick, labelFont, Brushes.Black, x - labelSize.Width / 2, plotregion.Bottom - tickPadding * 2)
        Next

        labelFont = CSSFont.TryParse(theme.axisLabelCSS)
        labelSize = g.MeasureString(theme.xlabel, labelFont)
        g.DrawString(theme.xlabel, labelFont, Brushes.Black, New PointF((plotregion.Width - labelSize.Width) / 2, plotregion.Bottom + tickPadding * 4))

        ' Y
        a = New PointF(plotregion.Left, plotregion.Bottom)
        b = New PointF(plotregion.Left, plotregion.Top)

        Call g.DrawLine(axisStroke, a, b)

        Dim i As Integer = 1
        Dim radius As Double
        Dim color As SolidBrush
        Dim tagFont As Font = CSSFont.TryParse(theme.tagCSS)

        labelFont = CSSFont.TryParse(theme.axisTickCSS)

        For Each [class] As NamedCollection(Of DEGModel) In degClass
            labelText = [class].name
            labelSize = g.MeasureString(labelText, labelFont)
            y = plotregion.Top + i * dh
            a = New PointF(plotregion.Left, y)
            b = New PointF(plotregion.Left - tickPadding, y)
            color = colors(i - 1)

            Call g.DrawLine(tickStroke, a, b)
            Call g.DrawString(labelText, labelFont, Brushes.Black, plotregion.Left - tickPadding - labelSize.Width, y - labelSize.Height / 2)

            For Each deg As DEGModel In [class]
                x = xscale(deg.logFC)
                radius = -Math.Log10(deg.pvalue)

                Call g.DrawCircle(New PointF(x, y), radius, color)

                If Not deg.label.StringEmpty Then
                    Call g.DrawString(deg.label, tagFont, Brushes.Black, x, y)
                End If
            Next

            i += 1
        Next
    End Sub
End Class
