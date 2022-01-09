Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.MIME.Html
Imports stdNum = System.Math

Public Class GSVADiffBar : Inherits Plot

    ReadOnly diff As GSVADiff()

    Public Sub New(diff As GSVADiff(), theme As Theme)
        MyBase.New(theme)

        Me.diff = diff
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
            .domain(t) _
            .range(integers:={rect.Left, rect.Right})
        Dim axisPen As Pen = CSS.Stroke.TryParse(theme.axisStroke).GDIObject
        Dim axisTickStroke As Pen = CSS.Stroke.TryParse(theme.axisTickStroke).GDIObject

        Call g.DrawRectangle(stroke, rect)
        Call g.DrawX(axisPen, $"t-value of GSVA score",)

    End Sub
End Class
