#Region "Microsoft.VisualBasic::c4ab466c8f97786eb499c719a2e4b0d8, ..\GCModeller\annotations\Proteomics\RSDPdensity.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

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
            Dim xMax# = {1, ticksX.Max}.Max

            Dim Pa As New PointF(0!, P)
            Dim Pb As New PointF(xMax, P)
            Dim Ra As New PointF(CSng(RSD), 0)
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
                xMax:=xMax,
                yMin:=0,
                xMin:=-0.25) ' 做出来的图不从零开始可能会比较好一些

        End With
    End Function
End Module

