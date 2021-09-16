Imports System.Drawing
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Contour
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Data.ChartPlots.Plots
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.MarchingSquares

Public Module OutlineTest

    Sub Main()

        Dim matrix As DataSet() = DataSet.LoadMatrix("D:\GCModeller\src\runtime\sciBASIC#\Data_science\Visualization\data\contour_outlines\region_9.csv").ToArray
        Dim x As Double() = matrix.Vector("X")
        Dim y As Double() = matrix.Vector("Y")
        Dim scatter As New SerialData With {
            .color = Color.Red,
            .pointSize = 30,
            .pts = x _
                .Select(Function(xi, i)
                            Return New PointData With {
                                .pt = New PointF(xi, y(i))
                            }
                        End Function) _
                .ToArray,
            .shape = LegendStyles.Circle,
            .title = "region 9"
        }
        Dim theme As New Theme With {.padding = "padding: 200px 200px 300px 300px;", .drawLegend = False}
        Dim app As New Scatter2D({scatter}, theme, scatterReorder:=True, fillPie:=True)

        ' raw scatter
        Call app.Plot.Save("D:\GCModeller\src\runtime\sciBASIC#\Data_science\Visualization\data\contour_outlines\region_9.raw.png")

        Dim outline = ContourLayer.GetOutline(x, y, fillSize:=5)
        Dim contour As New ContourPlot({outline}, New Theme With {.padding = "padding: 200px 800px 200px 200px;"})

        Call contour.Plot.Save("D:\GCModeller\src\runtime\sciBASIC#\Data_science\Visualization\data\contour_outlines\region_9.outline.png")

        Pause()

    End Sub


End Module
