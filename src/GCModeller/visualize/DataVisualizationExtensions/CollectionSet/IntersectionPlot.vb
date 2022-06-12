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
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports stdNum = System.Math

Namespace CollectionSet

    ''' <summary>
    ''' show venn intersection plot for more than 6 category 
    ''' </summary>
    Public Class IntersectionPlot : Inherits Plot

        ReadOnly collections As UpsetData
        ReadOnly setSizeLabel As String = "Set Size"
        ReadOnly setSizeBarColor As String = "gray"
        ReadOnly classSet As Dictionary(Of String, String())
        ReadOnly classColors As New Dictionary(Of String, Brush)

        Public Sub New(data As UpsetData,
                       setSizeBarColor As String,
                       classSet As Dictionary(Of String, String()),
                       theme As Theme)

            Call MyBase.New(theme)

            Me.classSet = classSet
            Me.setSizeBarColor = setSizeBarColor
            Me.collections = data

            If Not classSet.IsNullOrEmpty Then
                Dim colors As Brush() = Designer _
                    .GetColors(theme.colorSet, n:=classSet.Count) _
                    .Select(Function(c) New SolidBrush(c)) _
                    .ToArray
                Dim i As i32 = Scan0

                For Each [class] In classSet
                    Call classColors.Add([class].Key, colors(++i))
                Next
            End If
        End Sub

        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
            Dim plotRect As Rectangle = canvas.PlotRegion
            Dim labelFont As Font = CSSFont.TryParse(theme.axisLabelCSS).GDIObject(g.Dpi)
            Dim maxLabelSize As SizeF = g.MeasureString(collections.collectionSetLabels.MaxLengthString, labelFont)
            Dim hideLabels As Boolean = False
            Dim totalHeight As Double = collections.collectionSetLabels.Length * (maxLabelSize.Height * 1.125)
            Dim topBarPlot As New Rectangle(plotRect.Left, plotRect.Top, plotRect.Width, plotRect.Height - totalHeight)
            Dim bottomIntersection As New Rectangle(plotRect.Left, plotRect.Bottom - totalHeight + 50, plotRect.Width, totalHeight)
            Dim leftSetSizeBar As New Rectangle(canvas.Padding.Left / 20, bottomIntersection.Top, plotRect.Left, totalHeight)
            Dim topbarLayout As New Rectangle(
                x:=bottomIntersection.Left,
                y:=plotRect.Top,
                width:=bottomIntersection.Width,
                height:=plotRect.Height - bottomIntersection.Height
            )
            Dim boxWidth As Double = -1
            Dim boxHeight As Double = -1
            Dim barPlotLayout As New Rectangle(
                x:=leftSetSizeBar.X,
                y:=leftSetSizeBar.Y,
                width:=leftSetSizeBar.Width - boxWidth * 5,
                height:=leftSetSizeBar.Height
            )

            Call drawBottomIntersctionVisualize(
                g,
                boxWidth:=boxWidth,
                boxHeight:=boxHeight,
                layout:=bottomIntersection
            )
            Call drawLeftBarSet(g, labelFont, layout:=barPlotLayout)

            If maxLabelSize.Width / boxWidth > 3 Then
                hideLabels = True
            End If

            Call drawTopBarPlot(
                g:=g,
                hideLabels:=hideLabels,
                boxWidth:=boxWidth,
                layout:=topbarLayout,
                boxHeight:=boxHeight
            )

            If Not classSet.IsNullOrEmpty Then
                Call drawClassLegends(g, canvas)
            End If
        End Sub

        Private Sub drawClassLegends(g As IGraphics, canvas As GraphicsRegion)
            Dim legends As New List(Of LegendObject)
            Dim maxwidth As Integer = -1
            Dim font As Font = CSSFont.TryParse(theme.legendLabelCSS).GDIObject(g.Dpi)
            Dim defaultColor As String = collections.compares.color.ToHtmlColor

            For Each classKey As String In classColors.Keys
                Call New LegendObject With {
                    .color = DirectCast(classColors(classKey), SolidBrush).Color.ToHtmlColor,
                    .fontstyle = theme.legendLabelCSS,
                    .style = LegendStyles.Rectangle,
                    .title = classKey
                }.DoCall(AddressOf legends.Add)

                maxwidth = stdNum.Max(g.MeasureString(classKey, font).Width, maxwidth)
            Next

            Call New LegendObject With {
                .color = defaultColor,
                .fontstyle = theme.legendLabelCSS,
                .style = LegendStyles.Rectangle,
                .title = "No Class"
            }.DoCall(AddressOf legends.Add)

            maxwidth = stdNum.Max(g.MeasureString(legends.Last.title, font).Width, maxwidth)

            theme.legendLayout = New Absolute With {
                .x = canvas.Width - maxwidth * 2,
                .y = stdNum.Max(200, canvas.Padding.Top)
            }

            Call DrawLegends(g, legends.ToArray, showBorder:=True, canvas:=canvas)
        End Sub

        ''' <summary>
        ''' draw the dot intersection summary in the bottom region
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="boxWidth"></param>
        ''' <param name="boxHeight"></param>
        ''' <param name="layout"></param>
        Private Sub drawBottomIntersctionVisualize(g As IGraphics,
                                                   ByRef boxWidth As Double,
                                                   ByRef boxHeight As Double,
                                                   layout As Rectangle)

            Dim collectionSetLabels As String() = collections.collectionSetLabels
            Dim dh As Double = layout.Height / collectionSetLabels.Length
            ' unique + combinations
            Dim dotsPerGroup As Integer = collectionSetLabels.Length + collections.compares.data.Length
            Dim widthPerGroup As Double = layout.Width / collections.compares.data.Length

            boxWidth = widthPerGroup ' / dotsPerGroup
            boxHeight = layout.Height / collectionSetLabels.Length

            Dim pointSize As Double = stdNum.Min(boxWidth, boxHeight) / 3
            Dim gray As New SolidBrush("LightGray".TranslateColor)
            Dim linkStroke As Pen = Stroke.TryParse(theme.lineStroke)
            Dim x As Double = layout.Left + pointSize
            Dim allData As Dictionary(Of String, NamedCollection(Of String)) = collections.allData
            Dim labelIndex As Index(Of String) = collectionSetLabels
            Dim y As Double = layout.Top + pointSize
            Dim color As New SolidBrush(collections.compares.color)
            Dim htmlColor As String = collections.compares.color.ToHtmlColor

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

            ' draw for each combine group
            For i As Integer = 0 To collections.index.Length - 1
                Dim combineIndex As Index(Of String) = collections.index(i)
                Dim intersect As String() = collections.compares.data(i).value

                y = layout.Top + pointSize

                If intersect.Length > 0 Then
                    ' line between the dots
                    Dim ymin As Double = 999999
                    Dim ymax As Double = -99999

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

                x += boxWidth
            Next
        End Sub

        ''' <summary>
        ''' bar plot for display set size
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="layout"></param>
        Private Sub drawLeftBarSet(g As IGraphics, labelFont As Font, layout As Rectangle)
            Dim collectionSetLabels As String() = collections.collectionSetLabels
            Dim setSize As Dictionary(Of String, Integer) = collections.setSize.ToDictionary(Function(d) d.Name, Function(d) d.Value)
            Dim maxLabelSize As SizeF = collectionSetLabels _
                .Select(Function(lb) g.MeasureString(lb, labelFont)) _
                .OrderByDescending(Function(lb) lb.Width) _
                .FirstOrDefault

            maxLabelSize = New SizeF(maxLabelSize.Width * 1.25, maxLabelSize.Height)

            Dim y As Double = layout.Top
            Dim scale = d3js.scale _
                .linear _
                .domain(0.0.Join(setSize.Select(Function(i) CDbl(i.Value)))) _
                .range(New Double() {0, layout.Width - maxLabelSize.Width})
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

        ''' <summary>
        ''' draw barplot of the intersect data
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="boxWidth"></param>
        ''' <param name="boxHeight"></param>
        ''' <param name="layout"></param>
        Private Sub drawTopBarPlot(g As IGraphics,
                                   boxWidth As Double,
                                   boxHeight As Double,
                                   hideLabels As Boolean,
                                   layout As Rectangle)

            Dim yTick As Double() = collections.compares.data _
                .Select(Function(d) CDbl(d.Length)) _
                .JoinIterates({0.0, 1.0}) _
                .CreateAxisTicks
            Dim scaleY = d3js.scale.linear _
                .domain(values:=yTick) _
                .range(New Double() {0, layout.Height})
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

            For Each bar As NamedCollection(Of String) In collections.compares.data
                Dim barHeight As Double = scaleY(bar.Length)
                Dim y As Double = layout.Bottom - barHeight
                Dim barRect As New Rectangle With {
                    .X = x + offset,
                    .Y = y,
                    .Width = boxWidth * 0.8,
                    .Height = barHeight
                }
                Dim sizeLabel As String = bar.Length.ToString()
                Dim labelSize As SizeF = g.MeasureString(sizeLabel, labelFont)
                Dim defaultColor As Brush = bar.description.GetBrush
                Dim labelPos As New PointF With {
                    .X = x + (barRect.Width - labelSize.Width) / 2,
                    .Y = y - labelSize.Height
                }

                If classSet.IsNullOrEmpty OrElse bar.Length <= classColors.Count Then
                    Call g.FillRectangle(defaultColor, barRect)
                Else
                    Dim blocks As New List(Of (label As String, rect As Rectangle))
                    Dim size = classSet.ToDictionary(Function(v) v.Key, Function(v) CDbl(bar.Intersect(v.Value).Count))
                    Dim missing As Double = bar.value.Intersect(classSet.Values.IteratesALL.Distinct).Count
                    Dim all As Double = size.Values.Sum + missing

                    size = size.ToDictionary(Function(v) v.Key, Function(v) v.Value / all)
                    y = layout.Bottom
                    missing = missing / all

                    Dim [set] As Double = bar.Length * missing
                    Dim h As Integer = scaleY([set])
                    Dim rect As New Rectangle With {
                        .X = x + offset,
                        .Y = y - h,
                        .Width = boxWidth * 0.8,
                        .Height = h
                    }

                    y -= h

                    If h > 0 Then
                        Call g.FillRectangle(defaultColor, rect)
                    End If

                    For Each v In classSet
                        [set] = bar.Length * size(v.Key)
                        h = scaleY([set])
                        rect = New Rectangle With {
                            .X = x + offset,
                            .Y = y - h,
                            .Width = boxWidth * 0.8,
                            .Height = h
                        }

                        y -= h

                        If h > 0 Then
                            Call g.FillRectangle(classColors(v.Key), rect)
                        End If
                    Next
                End If

                If Not hideLabels Then
                    Call g.DrawString(sizeLabel, labelFont, Brushes.Black, labelPos)
                End If

                x += boxWidth
            Next
        End Sub
    End Class
End Namespace
