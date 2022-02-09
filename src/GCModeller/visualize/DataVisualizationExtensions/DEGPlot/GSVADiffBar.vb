Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.MIME.Html
Imports stdNum = System.Math

Public Class GSVADiffBar : Inherits Plot

    ReadOnly diff As GSVADiff()
    ReadOnly posColor As Brush = "#104e8b".GetBrush
    ReadOnly negColor As Brush = "#7ccd7c".GetBrush
    ReadOnly notsigColor As Brush = "#cdc9c9".GetBrush
    ''' <summary>
    ''' abs cutoff value of the t-static value
    ''' </summary>
    ReadOnly cut As Double = 2

    Public Sub New(diff As GSVADiff(), theme As Theme)
        MyBase.New(theme)

        Me.diff = diff.OrderByDescending(Function(t) t.t).ToArray
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim rect As Rectangle = canvas.PlotRegion
        Dim stroke As New Pen(Brushes.Black, 3)
        Dim t As Double() = diff _
            .Select(Function(d) stdNum.Abs(d.t)) _
            .Where(Function(n) Not n.IsNaNImaginary) _
            .Max _
            .SymmetricalRange _
            .CreateAxisTicks(ticks:=5)
        Dim scaleX = d3js.scale _
            .linear() _
            .domain(values:=t) _
            .range(integers:={rect.Left, rect.Right})
        Dim axisPen As Pen = CSS.Stroke.TryParse(theme.axisStroke).GDIObject
        Dim axisTickStroke As Pen = CSS.Stroke.TryParse(theme.axisTickStroke).GDIObject
        Dim axisTickFont As Font = CSS.CSSFont.TryParse(theme.axisTickCSS).GDIObject(g.Dpi)
        Dim axis As New XAxis(
            plotRegion:=rect,
            scaler:=scaleX,
            ticks:=t.AsVector,
            pen:=axisPen,
            overridesTickLine:=-1,
            noTicks:=False,
            tickFormat:="F2",
            tickfont:=axisTickFont,
            tickColor:=Brushes.Black,
            label:="t-value of GSVA score",
            labelFont:=theme.axisLabelCSS,
            labelColor:=Brushes.Black,
            htmlLabel:=False,
            xRotate:=0
        )

        Call g.DrawRectangle(stroke, rect)
        Call axis.Draw(g, XAxisLayoutStyles.Bottom, rect.Bottom, Nothing)

        Dim dbar As Double = 5
        Dim dy As Double = (rect.Height - diff.Length * dbar) / diff.Length
        Dim x As Double
        Dim y As Double = rect.Top - dy
        Dim zeroX As Double = scaleX(0)
        Dim bar As Rectangle
        Dim color As Brush
        Dim labelFont As Font = CSS.CSSFont.TryParse(theme.tagCSS).GDIObject(g.Dpi)
        Dim labelPos As PointF
        Dim labelSize As SizeF
        Dim labelColor As Brush

        For Each line As GSVADiff In diff
            x = scaleX(line.t)
            y += dy + dbar
            labelSize = g.MeasureString(line.pathName, labelFont)

            If x > zeroX Then
                ' pos
                bar = New Rectangle(zeroX, y, x - zeroX, dy)
                color = posColor
                ' right align
                labelPos = New PointF(zeroX - labelSize.Width, y - labelSize.Height / 2)
            Else
                ' neg
                bar = New Rectangle(x, y, zeroX - x, dy)
                color = negColor
                ' left align
                labelPos = New PointF(zeroX, y - labelSize.Height / 2)
            End If

            If stdNum.Abs(line.t) < cut Then
                color = notsigColor
                labelColor = notsigColor
            Else
                labelColor = Brushes.Black
            End If

            Call g.FillRectangle(color, bar)
            Call g.DrawString(line.pathName, labelFont, labelColor, labelPos)
        Next

        Dim cutline As New Pen(Brushes.White, 2) With {
            .DashStyle = DashStyle.Dash
        }

        Call g.DrawLine(cutline, New PointF(scaleX(-cut), rect.Top), New PointF(scaleX(-cut), rect.Bottom))
        Call g.DrawLine(cutline, New PointF(scaleX(cut), rect.Top), New PointF(scaleX(cut), rect.Bottom))

    End Sub
End Class
