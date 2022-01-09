Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

Public Class GSVADiffBar : Inherits Plot

    ReadOnly diff As GSVADiff()

    Public Sub New(diff As GSVADiff(), theme As Theme)
        MyBase.New(theme)

        Me.diff = diff
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)

    End Sub
End Class
