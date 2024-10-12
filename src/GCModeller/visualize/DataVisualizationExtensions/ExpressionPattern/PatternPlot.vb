﻿#Region "Microsoft.VisualBasic::47ab848e23bccea6219c0b4f17a0a2ba, visualize\DataVisualizationExtensions\ExpressionPattern\PatternPlot.vb"

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

    '   Total Lines: 203
    '    Code Lines: 169 (83.25%)
    ' Comment Lines: 13 (6.40%)
    '    - Xml Docs: 69.23%
    ' 
    '   Blank Lines: 21 (10.34%)
    '     File Size: 9.04 KB


    '     Class PatternPlot
    ' 
    '         Properties: clusterLabelStyle, legendTickStyle, legendTitleStyle, matrix, Prefix
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
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.DataMining.FuzzyCMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
#End If

Namespace ExpressionPattern

    Public Class PatternPlot : Inherits Plot

        Public ReadOnly Property matrix As ExpressionPattern

        ReadOnly patternsIndex As Dictionary(Of String, FuzzyCMeansEntity)
        ReadOnly colors As Color()
        ReadOnly membershipCutoff As Double
        ReadOnly topMembers As Double

        Public Property clusterLabelStyle As String = CSSFont.PlotSubTitle
        Public Property legendTitleStyle As String = CSSFont.Win7Small
        Public Property legendTickStyle As String = CSSFont.Win7Small
        Public Property Prefix As String = "Pattern"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="matrix"></param>
        ''' <param name="membershipCutoff">
        ''' the cluster members is filter via the membership cutoff
        ''' </param>
        ''' <param name="theme"></param>
        ''' <param name="colorSet$"></param>
        ''' <param name="levels%"></param>
        Public Sub New(matrix As ExpressionPattern,
                       membershipCutoff As Double,
                       topMembers As Double,
                       theme As Theme,
                       colorSet$,
                       levels%)

            Call MyBase.New(theme)

            Me.matrix = matrix
            Me.patternsIndex = matrix.Patterns.ToDictionary(Function(a) a.uid)
            Me.colors = Designer.GetColors(colorSet, levels)
            Me.membershipCutoff = membershipCutoff
            Me.topMembers = topMembers
        End Sub

        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
            ' 下面得到作图子区域的大小
            ' 用于计算布局信息
            Dim plot As Rectangle = canvas.PlotRegion
            Dim intervalTotalWidth! = plot.Width * 0.3
            Dim intervalTotalHeight! = plot.Height * 0.3
            Dim w = (plot.Width - intervalTotalWidth) / matrix.dim(1)
            Dim h = (plot.Height - intervalTotalHeight) / matrix.dim(Scan0)
            Dim iw = intervalTotalWidth / matrix.dim(1)
            Dim ih = intervalTotalHeight / matrix.dim(Scan0)

            Dim scatterData As SerialData()
            Dim i As i32 = 1
            Dim layout As GraphicsRegion
            Dim x!
            Dim y! = canvas.PlotRegion.Top + ih / 2
            Dim padding As String
            Dim clusterTagId As Integer
            Dim css As CSSEnvirnment = g.LoadEnvironment
            Dim clusterTagFont As Font = css.GetFont(CSSFont.TryParse(clusterLabelStyle))
            Dim tagPos As PointF
            Dim levels As New Value(Of DoubleRange)
            Dim legendLayout As Rectangle
            Dim designer As SolidBrush() = colors _
                .Select(Function(c) New SolidBrush(c)) _
                .ToArray
            Dim legendTitleFont As Font = css.GetFont(CSSFont.TryParse(legendTitleStyle))
            Dim legendTickFont As Font = css.GetFont(CSSFont.TryParse(legendTickStyle))
            Dim tickFormat As String
            Dim left As Double = canvas.PlotRegion.Left + iw / 6
            Dim topMembers As Integer = If(
                Me.topMembers > 1,
                Me.topMembers,                         ' is a integer number(real member size) 
                Me.topMembers * matrix.Patterns.Length ' is a percentage number(needs multiply with the data size)
            )
            Dim label As String

            For Each row As Matrix() In matrix.GetPartitionMatrix(
                membershipCutoff:=membershipCutoff,
                topMembers:=topMembers
            )
                x = left + iw / 5

                For Each col As Matrix In row
                    tagPos = New PointF(x, y - g.MeasureString("0", clusterTagFont).Height)
                    padding = $"padding: {y}px {canvas.Width - (x + w)}px {canvas.Height - (y + h)}px {x}"
                    legendLayout = New Rectangle With {
                        .X = x + w,
                        .Y = y,
                        .Width = iw * 0.65,
                        .Height = h * 0.75
                    }
                    layout = New GraphicsRegion(canvas.Size, padding)
                    x += w + iw
                    clusterTagId = Integer.Parse(col.tag)
                    scatterData = createLines(col, levels) _
                        .OrderBy(Function(gene)
                                     Return patternsIndex(gene.title).memberships(clusterTagId)
                                 End Function) _
                        .ToArray
                    label = $"{Prefix} #{Integer.Parse(col.tag) + 1}"

                    If scatterData.Select(Function(l) l.pts.Select(Function(a) a.pt.Y).Max).Max > 3000 Then
                        tickFormat = "G2"
                    Else
                        tickFormat = "F2"
                    End If

                    Call g.DrawString(label, clusterTagFont, Brushes.Black, tagPos)
                    Call Console.WriteLine(label & "...")

                    Call Scatter.Plot(
                        c:=scatterData,
                        g:=g,
                        rect:=layout,
                        Xlabel:=xlabel,
                        Ylabel:=ylabel,
                        tickFontStyle:=theme.axisTickCSS,
                        axisLabelCSS:=theme.axisLabelCSS,
                        showLegend:=False,
                        YtickFormat:=tickFormat,
                        xAxisLabelRotate:=theme.xAxisRotate
                    )
                    Call g.ColorMapLegend(
                        layout:=legendLayout,
                        designer:=designer,
                        ticks:=levels.Value.CreateAxisTicks,
                        titleFont:=legendTitleFont,
                        title:="membership",
                        tickFont:=legendTickFont,
                        tickAxisStroke:=Pens.Black,
                        legendOffsetLeft:=iw / 20
                    )
                Next

                y += h + ih
            Next
        End Sub

        Private Iterator Function createLines(col As Matrix, levels As Value(Of DoubleRange)) As IEnumerable(Of SerialData)
            Dim rawSampleId As String() = matrix.sampleNames
            Dim clusterTagId As Integer = Integer.Parse(col.tag)

            levels.Value = col.expression _
                .Keys _
                .Select(Function(a)
                            Return patternsIndex(a).memberships(clusterTagId)
                        End Function) _
                .ToArray

            For Each gene As DataFrameRow In col.expression
                Dim i As Integer

                If col.expression.Length = 1 OrElse levels.Value.Length = 0.0 Then
                    ' 聚类有时会出现一个成员元素的结果？
                    i = colors.Length - 1
                Else
                    i = CInt(levels.Value.ScaleMapping(patternsIndex(gene.geneID).memberships(clusterTagId), New Double() {0, colors.Length - 1}))
                End If

                Yield New SerialData With {
                    .title = gene.geneID,
                    .color = colors(i),
                    .lineType = DashStyle.Solid,
                    .pointSize = 5,
                    .width = 30,
                    .pts = gene.experiments _
                        .Select(Function(exp, idx)
                                    Return New PointData With {
                                        .tag = rawSampleId(idx),
                                        .axisLabel = rawSampleId(idx),
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
