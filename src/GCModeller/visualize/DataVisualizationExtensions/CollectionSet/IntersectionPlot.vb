#Region "Microsoft.VisualBasic::833413fdac8bf5b1b69baecdbab9f232, visualize\DataVisualizationExtensions\CollectionSet\IntersectionPlot.vb"

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

'     Class IntersectionPlot
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: getCombinations
' 
'         Sub: drawBottomIntersctionVisualize, drawLeftBarSet, drawTopBarPlot, PlotInternal
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports stdNum = System.Math

Namespace CollectionSet

    ''' <summary>
    ''' show venn intersection plot for more than 6 category 
    ''' </summary>
    Public Class IntersectionPlot : Inherits Plot

        ReadOnly collections As IntersectionData
        ReadOnly setSizeLabel As String = "Set Size"
        ReadOnly setSizeBarColor As String = "gray"
        ReadOnly desc As Boolean = False

        Public Sub New(data As IntersectionData, desc As Boolean, setSizeBarColor As String, theme As Theme)
            MyBase.New(theme)

            Me.desc = desc
            Me.setSizeBarColor = setSizeBarColor
            Me.collections = data
        End Sub

        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
            Dim plotRect As Rectangle = canvas.PlotRegion
            Dim labelFont As Font = CSSFont.TryParse(theme.axisLabelCSS).GDIObject(g.Dpi)
            Dim collectionSetLabels As String() = collections.GetAllCollectionTags
            Dim maxLabelSize As SizeF = g.MeasureString(collectionSetLabels.MaxLengthString, labelFont)
            Dim totalHeight As Double = collectionSetLabels.Length * (maxLabelSize.Height * 1.125)
            Dim topBarPlot As New Rectangle(plotRect.Left, plotRect.Top, plotRect.Width, plotRect.Height - totalHeight)
            Dim bottomIntersection As New Rectangle(plotRect.Left, plotRect.Bottom - totalHeight + 50, plotRect.Width, totalHeight)
            Dim leftSetSizeBar As New Rectangle(canvas.Padding.Left / 20, bottomIntersection.Top, plotRect.Left, totalHeight)
            Dim topbarLayout As New Rectangle(bottomIntersection.Left, plotRect.Top, bottomIntersection.Width, plotRect.Height - bottomIntersection.Height)
            Dim barData As New List(Of NamedValue(Of Integer))
            Dim boxWidth As Double = -1
            Dim boxHeight As Double = -1

            Call drawBottomIntersctionVisualize(g, collectionSetLabels, barData, boxWidth:=boxWidth, boxHeight:=boxHeight, layout:=bottomIntersection)
            Call drawLeftBarSet(g, labelFont, collectionSetLabels, layout:=New Rectangle(leftSetSizeBar.X, leftSetSizeBar.Y, leftSetSizeBar.Width - boxWidth * 5, leftSetSizeBar.Height))
            Call drawTopBarPlot(g, barData, boxWidth:=boxWidth, layout:=topbarLayout, boxHeight:=boxHeight)
        End Sub

        ''' <summary>
        ''' 2 vs 2 -> a vs b vs c vs ...
        ''' </summary>
        ''' <returns></returns>
        Private Function getCombinations(collectionSetLabels As String()) As String()()
            Return collectionSetLabels _
                .AllCombinations _
                .GroupBy(Function(combine)
                             Return combine.Distinct.OrderBy(Function(str) str).JoinBy("---")
                         End Function) _
                .Select(Function(group)
                            Return group.First.Distinct.ToArray
                        End Function) _
                .ToArray
        End Function

        Private Sub drawBottomIntersctionVisualize(g As IGraphics, collectionSetLabels As String(), barData As List(Of NamedValue(Of Integer)), ByRef boxWidth As Double, ByRef boxHeight As Double, layout As Rectangle)
            Dim dh As Double = layout.Height / collectionSetLabels.Length
            Dim allCompares = getCombinations(collectionSetLabels).ToArray
            ' unique + combinations
            Dim dotsPerGroup As Integer = collectionSetLabels.Length + allCompares.Length
            Dim widthPerGroup As Double = layout.Width / collections.size

            boxWidth = widthPerGroup / dotsPerGroup
            boxHeight = layout.Height / collectionSetLabels.Length

            Dim pointSize As Double = stdNum.Min(boxWidth, boxHeight) / 3
            Dim gray As New SolidBrush(Color.LightGray)
            Dim linkStroke As Pen = Stroke.TryParse(theme.lineStroke)
            Dim x As Double = layout.Left + pointSize

            For Each factor As FactorGroup In collections.groups
                Dim allData = factor.GetAllUniques.ToDictionary(Function(i) i.name)
                Dim labelIndex As Index(Of String) = collectionSetLabels
                Dim y As Double = layout.Top + pointSize
                Dim color As New SolidBrush(factor.color)
                Dim htmlColor As String = factor.color.ToHtmlColor

                '' single & unique
                'For Each label As String In collectionSetLabels
                '    ' get all data only appears in current collection
                '    Dim unique As Integer = allData(label).Length

                '    Call New NamedValue(Of Integer) With {
                '        .Name = label,
                '        .Value = unique,
                '        .Description = htmlColor
                '    }.DoCall(AddressOf barData.Add)

                '    For Each label2 As String In collectionSetLabels
                '        If label <> label2 OrElse unique = 0 Then
                '            ' gray dot
                '            Call g.FillCircles(gray, {New Point(x, y)}, pointSize)
                '        Else
                '            Call g.FillCircles(color, {New Point(x, y)}, pointSize)
                '        End If

                '        y += boxHeight
                '    Next

                '    x += boxWidth
                '    y = layout.Top + pointSize
                'Next

                Dim intersectList As (index As Index(Of String), intersect As String())() = allCompares _
                    .Select(Function(combine)
                                Dim intersect As String() = factor _
                                    .GetIntersection(combine) _
                                    .ToArray

                                Return (index:=combine.Indexing, intersect)
                            End Function) _
                    .Where(Function(d) d.intersect.Length > 0) _
                    .JoinIterates(collectionSetLabels.Select(Function(lbl)
                                                                 Dim unique As String() = factor.GetUniqueId(lbl)
                                                                 Return ({lbl}.Indexing, unique)
                                                             End Function)) _
                    .Sort(Function(d) d.intersect.Length, desc) _
                    .ToArray

                ' draw for each combine group
                For Each combine In intersectList
                    Dim intersect As String() = combine.intersect

                    y = layout.Top + pointSize

                    If intersect.Length > 0 Then
                        ' line between the dots
                        Dim ymin As Double = 999999
                        Dim ymax As Double = -99999
                        Dim combineIndex As Index(Of String) = combine.index

                        For Each tag In collectionSetLabels
                            If tag Like combineIndex Then
                                ' black dot
                                Call g.FillCircles(color, {New Point(x, y)}, pointSize)

                                If y < ymin Then
                                    ymin = y
                                End If
                                If y > ymax Then
                                    ymax = y
                                End If
                            Else
                                ' none
                                Call g.FillCircles(gray, {New Point(x, y)}, pointSize)
                            End If

                            y += boxHeight
                        Next

                        ' draw line
                        Call g.DrawLine(linkStroke, New PointF(x, ymin), New PointF(x, ymax))
                    Else
                        ' all gray dots
                        For Each tag In collectionSetLabels
                            g.FillCircles(gray, {New Point(x, y)}, pointSize)
                            y += boxHeight
                        Next
                    End If

                    Call New NamedValue(Of Integer) With {
                        .Name = combine.index.Objects.JoinBy("--"),
                        .Value = intersect.Length,
                        .Description = htmlColor
                    }.DoCall(AddressOf barData.Add)

                    x += boxWidth
                Next
            Next
        End Sub

        ''' <summary>
        ''' bar plot for display set size
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="layout"></param>
        Private Sub drawLeftBarSet(g As IGraphics, labelFont As Font, collectionSetLabels As String(), layout As Rectangle)
            Dim setSize = collections.GetSetSize.ToDictionary(Function(d) d.Name, Function(d) d.Value)
            Dim maxLabelSize As SizeF = g.MeasureString(collectionSetLabels.MaxLengthString, labelFont)
            Dim y As Double = layout.Top
            Dim scale = d3js.scale.linear.domain(0.0.Join(setSize.Select(Function(i) CDbl(i.Value)))).range(New Double() {0, layout.Width - maxLabelSize.Width})
            Dim labelSize As SizeF
            Dim labelPos As New PointF
            Dim setSizeColor As Brush = Me.setSizeBarColor.GetBrush

            ' label is center alignment?
            For Each label As String In collectionSetLabels
                labelSize = g.MeasureString(label, labelFont)
                labelPos = New PointF With {
                    .X = layout.Right - maxLabelSize.Width + (maxLabelSize.Width - labelSize.Width) / 2,
                    .Y = y
                }

                Dim bar As New Rectangle With {
                    .Width = scale(setSize(label)),
                    .X = layout.Right - maxLabelSize.Width - .Width,
                    .Y = y + maxLabelSize.Height * 0.1,
                    .Height = maxLabelSize.Height * 0.8
                }

                ' draw label
                Call g.DrawString(label, labelFont, Brushes.Black, labelPos)
                ' draw bar plot
                Call g.FillRectangle(setSizeColor, bar)

                y += maxLabelSize.Height
            Next

            y += 5

            ' draw axis
            Dim a As New PointF(layout.Right - maxLabelSize.Width, y)
            Dim b As New PointF(layout.Left, y)
            Dim pen As Pen = Stroke.TryParse(theme.axisStroke)
            Dim width As Double = stdNum.Abs(a.X - b.X)

            labelFont = CSSFont.TryParse(theme.axisTickCSS).GDIObject(g.Dpi)

            g.DrawLine(pen, a, b)

            For Each tick As Double In New DoubleRange(0.Join(setSize.Values)).CreateAxisTicks(ticks:=3, decimalDigits:=0)
                If stdNum.Abs(tick) < 0.01 Then
                    tick = 0
                End If

                Dim tickX As Double = a.X - scale(tick)
                Dim label As String = tick.ToString(theme.XaxisTickFormat)

                labelSize = g.MeasureString(label, labelFont)
                labelPos = New PointF(tickX - labelSize.Width / 2, y)

                If labelPos.X < 10 Then
                    Continue For
                End If

                g.DrawString(label, labelFont, Brushes.Black, labelPos)
            Next

            Dim dTick = labelSize.Height * 1.125

            labelFont = CSSFont.TryParse(theme.axisLabelCSS).GDIObject(g.Dpi)
            labelSize = g.MeasureString(setSizeLabel, labelFont)
            labelPos = New PointF(b.X + (width - labelSize.Width) / 2, y + dTick)

            g.DrawString("Set Size", labelFont, Brushes.Black, labelPos)
        End Sub

        Private Sub drawTopBarPlot(g As IGraphics, barData As List(Of NamedValue(Of Integer)), boxWidth As Double, boxHeight As Double, layout As Rectangle)
            Dim yTick = barData.Select(Function(d) CDbl(d.Value)).JoinIterates({0.0, 1.0}).CreateAxisTicks
            Dim scaleY = d3js.scale.linear.domain(values:=yTick).range(New Double() {0, layout.Height})
            Dim x As Double = layout.Left
            Dim labelFont As Font = CSSFont.TryParse(theme.tagCSS).GDIObject(g.Dpi)
            Dim offset As Double = boxWidth * 0.1
            Dim pen As Pen = Stroke.TryParse(theme.axisStroke)
            Dim yscale As New YScaler(False) With {
                .region = layout,
                .Y = scaleY
            }

            ' draw axis
            Call g.DrawY(pen, "Intersection Size", yscale, 0, yTick, YAxisLayoutStyles.Left,
                         New Point(0, -boxHeight),
                         theme.axisLabelCSS, Brushes.Black,
                         CSSFont.TryParse(theme.axisTickCSS).GDIObject(g.Dpi), Brushes.Black,
                         htmlLabel:=False,
                         tickFormat:="F0"
            )

            For Each bar In barData
                Dim barHeight As Double = scaleY(bar.Value)
                Dim y As Double = layout.Bottom - barHeight
                Dim barRect As New Rectangle With {
                    .X = x + offset,
                    .Y = y,
                    .Width = boxWidth * 0.8,
                    .Height = barHeight
                }
                Dim labelSize As SizeF = g.MeasureString(bar.Value, labelFont)

                Call g.FillRectangle(bar.Description.GetBrush, barRect)
                Call g.DrawString(bar.Value, labelFont, Brushes.Black, New PointF(x + (barRect.Width - labelSize.Width) / 2, y - labelSize.Height))

                x += boxWidth
            Next
        End Sub
    End Class
End Namespace
