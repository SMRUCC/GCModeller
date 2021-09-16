Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

Namespace Plots

    Public Class PolygonPlot2D : Inherits Plot

        ReadOnly polygons As SerialData()

        Public Sub New(data As IEnumerable(Of SerialData), theme As Theme)
            MyBase.New(theme)

            polygons = data.ToArray
        End Sub

        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
            Throw New NotImplementedException()
        End Sub
    End Class
End Namespace