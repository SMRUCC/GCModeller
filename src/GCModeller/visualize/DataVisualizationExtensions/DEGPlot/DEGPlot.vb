#Region "Microsoft.VisualBasic::2ecc6b18e5c33feb7a540dca5f6d9092, GCModeller\visualize\DataVisualizationExtensions\DEGPlot\DEGPlot.vb"

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

    '   Total Lines: 63
    '    Code Lines: 40
    ' Comment Lines: 19
    '   Blank Lines: 4
    '     File Size: 2.76 KB


    ' Module DEGPlot
    ' 
    '     Function: ClassChangePlot
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.Html.CSS

Public Module DEGPlot

    ''' <summary>
    ''' X坐标轴位置与<see cref="DEGModel.logFC"/>相关
    ''' 气泡大小与<see cref="DEGModel.pvalue"/>相关
    ''' </summary>
    ''' <param name="deg"></param>
    ''' <param name="size$"></param>
    ''' <param name="padding$"></param>
    ''' <param name="bg$"></param>
    ''' <param name="colorSet$"></param>
    ''' <param name="axisTickCSS$"></param>
    ''' <param name="axisStroke$"></param>
    ''' <param name="axisTickStroke$"></param>
    ''' <param name="axisLabelCSS$"></param>
    ''' <param name="labelCSS$"></param>
    ''' <param name="radius"></param>
    ''' <param name="xlab$"></param>
    ''' <param name="orderByClass"></param>
    ''' <param name="dpi%"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ClassChangePlot(deg As IEnumerable(Of DEGModel),
                                    Optional size$ = "2700,2100",
                                    Optional padding$ = g.DefaultPadding,
                                    Optional bg$ = "white",
                                    Optional colorSet$ = "Set1:c9",
                                    Optional axisTickCSS$ = CSSFont.Win10Normal,
                                    Optional axisStroke$ = Stroke.AxisStroke,
                                    Optional axisTickStroke$ = Stroke.AxisStroke,
                                    Optional axisLabelCSS$ = CSSFont.Win10NormalLarger,
                                    Optional labelCSS$ = CSSFont.Win7Normal,
                                    Optional radius As String = "5,30",
                                    Optional xlab$ = "X",
                                    Optional orderByClass As String = "none",
                                    Optional dpi% = 300) As GraphicsData

        Dim theme As New Theme With {
            .padding = padding,
            .colorSet = colorSet,
            .background = bg,
            .axisTickCSS = axisTickCSS,
            .axisStroke = axisStroke,
            .axisTickStroke = axisTickStroke,
            .axisLabelCSS = axisLabelCSS,
            .tagCSS = labelCSS
        }

        Return New ClassChanges(
            deg:=deg,
            radius:=DoubleRange.TryParse(radius),
            theme:=theme,
            orderByClass:=orderByClass
        ) With {.xlabel = xlab}.Plot(size, ppi:=dpi)
    End Function
End Module
