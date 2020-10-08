#Region "Microsoft.VisualBasic::cf33860d5093beffd546ea66a20e3299, visualize\DataVisualizationExtensions\ExpressionPattern\PatternPlot.vb"

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

    '     Class PatternPlot
    ' 
    '         Properties: matrix
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: createLines
    ' 
    '         Sub: PlotInternal
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports DashStyle = System.Drawing.Drawing2D.DashStyle

Namespace ExpressionPattern

    Public Class PatternPlot : Inherits Plot

        Public Property matrix As ExpressionPattern

        Public Sub New(theme As Theme)
            MyBase.New(theme)
        End Sub

        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
            ' 下面得到作图子区域的大小
            ' 用于计算布局信息
            Dim intervalTotalWidth! = canvas.PlotRegion.Width * 0.2
            Dim intervalTotalHeight! = canvas.PlotRegion.Height * 0.2
            Dim w = (canvas.PlotRegion.Width - intervalTotalWidth) / matrix.dim(Scan0)
            Dim h = (canvas.PlotRegion.Height - intervalTotalHeight) / matrix.dim(1)
            Dim iw = intervalTotalWidth / (matrix.dim(Scan0) - 1)
            Dim ih = intervalTotalHeight / (matrix.dim(1) - 1)

            Dim scatterData As SerialData()
            Dim i As i32 = 1
            Dim layout As GraphicsRegion
            Dim x!
            Dim y! = canvas.PlotRegion.Top + h
            Dim padding As String

            For Each row As Matrix() In matrix.GetPartitionMatrix
                x = canvas.PlotRegion.Left

                For Each col As Matrix In row
                    padding = $"padding: {y}px {canvas.Width - (x + w)}px {canvas.Height - (y + h)}px {x}"
                    layout = New GraphicsRegion(canvas.Size, padding)
                    x += w + iw
                    scatterData = col.DoCall(AddressOf createLines).ToArray

                    Call Scatter.Plot(
                        c:=scatterData,
                        g:=g,
                        rect:=layout,
                        Xlabel:=xlabel,
                        Ylabel:=ylabel,
                        tickFontStyle:=theme.axisTickCSS,
                        labelFontStyle:=theme.axisLabelCSS
                    )
                Next

                y += h + ih
            Next
        End Sub

        Private Iterator Function createLines(col As Matrix) As IEnumerable(Of SerialData)
            Dim rawSampleId As String() = matrix.sampleNames

            For Each gene In col.expression
                Yield New SerialData With {
                    .title = gene.geneID,
                    .color = Color.Red,
                    .lineType = DashStyle.Solid,
                    .pointSize = 5,
                    .width = 5,
                    .pts = gene.experiments _
                        .Select(Function(exp, idx)
                                    Return New PointData With {
                                        .tag = rawSampleId(idx),
                                        .pt = New PointF With {
                                            .X = idx,
                                            .Y = exp
                                        }
                                    }
                                End Function) _
                        .ToArray
                }
            Next
        End Function
    End Class
End Namespace
