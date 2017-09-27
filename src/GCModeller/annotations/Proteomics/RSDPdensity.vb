Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Statistics.Heatmap
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports RDotNET.Extensions.VisualBasic.API

Public Module RSDPdensity

    <Extension>
    Public Function RSDP(data As IEnumerable(Of DataSet), n%) As PointF()
        Dim ZERO#() = 0#.Replicate(n).ToArray
        Dim points As PointF() = data _
            .Select(Function(x)
                        Dim sample#() = x.Properties.Values.ToArray
                        Dim pvalue# = stats.Ttest(sample, ZERO, varEqual:=True).pvalue
                        Dim P# = -Math.Log10(pvalue)
                        Return New PointF(sample.RSD, P)
                    End Function) _
            .ToArray
        Return points
    End Function

    ReadOnly P_threshold# = -Math.Log10(0.05)

    <Extension>
    Public Function RSDPdensity(points As IEnumerable(Of PointF),
                                Optional size$ = "1600,1200",
                                Optional padding$ = g.DefaultPadding,
                                Optional bg$ = "white",
                                Optional schema$ = "Jet",
                                Optional RSD# = 1,
                                Optional lineStroke$ = Stroke.StrongHighlightStroke) As GraphicsData

        With points _
            .Where(Function(pt)
                       Return Not New Double() {
                           pt.X, pt.Y
                       }.Any(Function(x)
                                 Return x.IsNaNImaginary
                             End Function)
                   End Function) _
            .ToArray

            ' 分别绘制出P值和RSD值得临界值线
            Using g = DensityPlot _
                .Plot(points, size, padding, bg, schema, levels:=100) _
                .CreateGraphics

                Dim region As New GraphicsRegion With {
                    .Padding = padding,
                    .Size = g.Size
                }
                Dim canvas = region.PlotRegion
                Dim ticksX = .Select(Function(pt) CDbl(pt.X)).CreateAxisTicks.AsVector
                Dim ticksY = .Select(Function(pt) CDbl(pt.Y)).CreateAxisTicks.AsVector
                Dim X = d3js.scale.linear().domain(ticksX).range(New Integer() {canvas.Left, canvas.Right})
                Dim Y = d3js.scale.linear().domain(ticksY).range(New Integer() {0, canvas.Bottom})
                Dim scaler As New DataScaler With {
                    .X = X,
                    .Y = Y,
                    .AxisTicks = (ticksX, ticksY),
                    .ChartRegion = canvas
                }

                ' P直线是横向的，即(0,P) (maxX,P)
                ' RSD线是竖向的，即(RSD,minY) (RSD, maxY)
                Dim P = scaler.TranslateY(P_threshold)
                Dim line As Pen = Stroke.TryParse(lineStroke).GDIObject

                RSD = scaler.TranslateX(RSD)

                Call g.DrawLine(line, 0!, CSng(P), CSng(scaler.TranslateX(ticksX.Max)), CSng(P))
                Call g.DrawLine(line, CSng(RSD), CSng(scaler.TranslateY(ticksY.Min)), CSng(RSD), CSng(scaler.TranslateY(ticksY.Max)))

                If TypeOf g Is Graphics2D Then
                    Return New ImageData(DirectCast(g, Graphics2D).ImageResource, g.Size)
                Else
                    Return New SVGData(g, g.Size)
                End If
            End Using
        End With
    End Function
End Module
