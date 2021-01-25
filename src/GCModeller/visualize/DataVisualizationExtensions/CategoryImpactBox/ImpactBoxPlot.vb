Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

Public Class ImpactBoxPlot : Inherits Plot

    ReadOnly impacts As Impact()

    Public Sub New(impacts As Impact(), theme As Theme)
        MyBase.New(theme)

        Me.impacts = impacts
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Throw New NotImplementedException()
    End Sub
End Class
