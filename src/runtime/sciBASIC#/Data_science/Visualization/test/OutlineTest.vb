Imports System.Drawing
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Data.ChartPlots.Plots
Imports Microsoft.VisualBasic.Data.csv.IO

Public Module OutlineTest

    Sub Main()

        Dim matrix As DataSet() = DataSet.LoadMatrix("D:\GCModeller\src\runtime\sciBASIC#\Data_science\Visualization\data\contour_outlines\region_2.csv").ToArray
        Dim x As Double() = matrix.Vector("X")
        Dim y As Double() = matrix.Vector("Y")
        Dim scatter As New SerialData With {
            .color = Color.Red,
            .pointSize = 5,
            .pts = x _
                .Select(Function(xi, i)
                            Return New PointData With {
                                .pt = New PointF(xi, y(i))
                            }
                        End Function) _
                .ToArray,
            .shape = LegendStyles.Circle,
            .title = "region 2"
        }
        Dim theme As New Theme
        Dim app As New Scatter2D({scatter}, theme, scatterReorder:=True, fillPie:=True)

        Call app.Plot.Save("D:\GCModeller\src\runtime\sciBASIC#\Data_science\Visualization\data\contour_outlines\region_2.raw.png")

        Pause()

    End Sub
End Module
