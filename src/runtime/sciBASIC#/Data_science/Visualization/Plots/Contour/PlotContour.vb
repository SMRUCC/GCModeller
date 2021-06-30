Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.MarchingSquares
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Scripting.Runtime

Namespace Contour

    Public Module PlotContour

        <Extension>
        Public Function Plot(sample As IEnumerable(Of MeasureData),
                             Optional threshold As Double = 0.2,
                             Optional size$ = "2700,2000",
                             Optional gridSize$ = "50,50") As GraphicsData

            Dim matrix As New MapMatrix(sample, size.SizeParser, gridSize.SizeParser)
            Dim contour As New MarchingSquares(matrix)
            Dim q As QuantileEstimationGK = matrix.GetLevelQuantile

            For Each polygon In contour.CreateMapData(threshold:=q.Query(threshold))

            Next
        End Function
    End Module
End Namespace