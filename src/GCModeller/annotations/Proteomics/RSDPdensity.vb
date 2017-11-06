Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Statistics.Heatmap
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Shapes
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports RDotNET.Extensions.VisualBasic.API

''' <summary>
''' iTraq质谱实验结果的质量检验分析
''' </summary>
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
                                Optional size$ = "1200,1000",
                                Optional padding$ = g.DefaultPadding,
                                Optional bg$ = "white",
                                Optional schema$ = "Jet",
                                Optional RSD# = 1,
                                Optional lineStroke$ = Stroke.AxisGridStroke) As GraphicsData

        Call $"{NameOf(P_threshold)}={P_threshold}".__DEBUG_ECHO

        With points _
            .Where(Function(pt)
                       Return Not New Double() {
                           pt.X, pt.Y
                       }.Any(Function(x)
                                 Return x.IsNaNImaginary
                             End Function)
                   End Function) _
            .ToArray

            Dim ticksX = .Select(Function(pt) CDbl(pt.X)).CreateAxisTicks.AsVector
            Dim ticksY = .Select(Function(pt) CDbl(pt.Y)).CreateAxisTicks.AsVector

            ' 分别绘制出P值和RSD值得临界值线
            ' P直线是横向的，即(0,P) (maxX,P)
            ' RSD线是竖向的，即(RSD,minY) (RSD, maxY)
            Dim P = P_threshold
            Dim line As Pen = Stroke.TryParse(lineStroke).GDIObject

            Dim Pa As New PointF(0!, P)
            Dim Pb As New PointF(CSng(ticksX.Max), P)
            Dim Ra As New PointF(CSng(RSD), CSng(ticksY.Min))
            Dim Rb As New PointF(CSng(RSD), CSng(ticksY.Max))
            Dim ablines = {
                New Line(Pa, Pb, line), New Line(Ra, Rb, line)
            }

            Return DensityPlot.Plot(
                .ref,
                size, padding, bg, schema, levels:=100,
                ablines:=ablines,
                labX:="RSD", labY:="-log10(P.value)",
                htmlLabel:=False,
                xMax:={1.25, ticksX.Max}.Max,
                yMin:=0,
                xMin:=0)

        End With
    End Function
End Module
