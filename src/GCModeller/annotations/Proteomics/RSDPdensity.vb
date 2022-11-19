#Region "Microsoft.VisualBasic::66d1728b46524645100d2cbfea065b54, GCModeller\annotations\Proteomics\RSDPdensity.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 110
    '    Code Lines: 76
    ' Comment Lines: 24
    '   Blank Lines: 10
    '     File Size: 4.29 KB


    ' Module RSDPdensity
    ' 
    '     Function: RSDP, RSDPdensity
    ' 
    ' /********************************************************************************/

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
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports Microsoft.VisualBasic.MIME.Html.CSS
#If netcore5 = 0 Then
Imports RDotNet.Extensions.VisualBasic.API
#End If

''' <summary>
''' iTraq质谱实验结果的质量检验分析
''' </summary>
Public Module RSDPdensity

    ''' <summary>
    ''' 计算得到每一个蛋白的质量点
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="n%"></param>
    ''' <returns></returns>
    <Extension>
    Public Function RSDP(data As IEnumerable(Of DataSet), n%) As PointF()
        Dim ZERO#() = 0#.Replicate(n).ToArray
        Dim points As PointF() = data _
            .Select(Function(x)
                        Dim sample#() = x.Properties.Values.ToArray
                        Dim pvalue# = t.Test(sample, ZERO, varEqual:=True).Pvalue
                        Dim P# = -Math.Log10(pvalue)
                        Return New PointF(sample.RSD, P)
                    End Function) _
            .ToArray
        Return points
    End Function

    ReadOnly P_threshold# = -Math.Log10(0.05)

    ''' <summary>
    ''' iTraq/TMT数据质谱质量曲线作图
    ''' </summary>
    ''' <param name="points">每一个蛋白的质量点</param>
    ''' <param name="size">图片大小</param>
    ''' <param name="padding">页边距</param>
    ''' <param name="bg">背景色</param>
    ''' <param name="schema">质量点密度映射的颜色谱名称</param>
    ''' <param name="RSD">RSD参考值</param>
    ''' <param name="lineStroke">线条的样式</param>
    ''' <returns></returns>
    <Extension>
    Public Function RSDPdensity(points As IEnumerable(Of PointF),
                                Optional size$ = "1800,1600",
                                Optional padding$ = g.DefaultPadding,
                                Optional bg$ = "white",
                                Optional schema$ = "Jet",
                                Optional RSD# = 1,
                                Optional lineStroke$ = "stroke: lightgray; stroke-width: 5px; stroke-dash: dash;") As GraphicsData

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

            ' 做出来的图不从零开始可能会比较好一些
            Return DensityPlot.Plot(
                .ByRef,
                size, padding, bg, schema, levels:=100,
                ptSize:=10,
                ablines:=ablines,
                labX:="RSD", labY:="-log10(P.value)",
                legendTitleFontCSS:=CSSFont.Win7Large,
                htmlLabel:=False,
                legendWidth:=200,
                xMax:=xMax,
                yMin:=0,
                xMin:=-0.25
            )
        End With
    End Function
End Module
