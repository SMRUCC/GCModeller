Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Distributions

Namespace Graphic

    Public MustInherit Class HeatMapPlot : Inherits Plot

        Public Property mapLevel As Integer

        Protected Sub New(theme As Theme)
            MyBase.New(theme)
        End Sub

        Public Shared Iterator Function Z(scatter As IEnumerable(Of PixelData)) As IEnumerable(Of PixelData)
            Dim allSpots As PixelData() = scatter.ToArray
            Dim v As Double() = allSpots _
                .Select(Function(p) p.Scale) _
                .AsVector _
                .Z _
                .ToArray

            For i As Integer = 0 To v.Length - 1
                Yield New PixelData With {
                    .Scale = v(i),
                    .X = allSpots(i).X,
                    .Y = allSpots(i).Y
                }
            Next
        End Function
    End Class
End Namespace