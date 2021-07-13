Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
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

        Public Sub New(data As IntersectionData, theme As Theme)
            MyBase.New(theme)

            Me.collections = data
        End Sub

        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
            Dim plotRect As Rectangle = canvas.PlotRegion
            Dim labelFont As Font = CSSFont.TryParse(theme.axisLabelCSS)
            Dim collectionSetLabels As String() = collections.GetAllCollectionTags
            Dim maxLabelSize As SizeF = g.MeasureString(collectionSetLabels.MaxLengthString, labelFont)
            Dim totalHeight As Double = collectionSetLabels.Length * (maxLabelSize.Height * 1.125)
            Dim topBarPlot As New Rectangle(plotRect.Left, plotRect.Top, plotRect.Width, plotRect.Height - totalHeight)
            Dim bottomIntersection As New Rectangle(plotRect.Left, plotRect.Bottom - totalHeight, plotRect.Width, totalHeight)
            Dim leftSetSizeBar As New Rectangle(0, bottomIntersection.Top, plotRect.Left, totalHeight)
            Dim barData As New List(Of NamedValue(Of Integer))

            Call drawBottomIntersctionVisualize(g, collectionSetLabels, barData, layout:=bottomIntersection)
            Call drawLeftBarSet(g, labelFont, collectionSetLabels, layout:=leftSetSizeBar)
        End Sub

        ''' <summary>
        ''' 2 vs 2 -> a vs b vs c vs ...
        ''' </summary>
        ''' <returns></returns>
        Private Function getCombinations(collectionSetLabels As String()) As String()()
            Return collectionSetLabels _
                .AllCombinations _
                .GroupBy(Function(combine)
                             Return combine.OrderBy(Function(str) str).JoinBy("---")
                         End Function) _
                .Select(Function(group)
                            Return group.First
                        End Function) _
                .ToArray
        End Function

        Private Sub drawBottomIntersctionVisualize(g As IGraphics, collectionSetLabels As String(), barData As List(Of NamedValue(Of Integer)), layout As Rectangle)
            Dim dh As Double = layout.Height / collectionSetLabels.Length
            Dim allCompares = getCombinations(collectionSetLabels).ToArray
            ' unique + combinations
            Dim dotsPerGroup As Integer = collectionSetLabels.Length + allCompares.Length
            Dim widthPerGroup As Double = layout.Width / collections.size
            Dim boxWidth As Double = widthPerGroup / dotsPerGroup
            Dim boxHeight As Double = layout.Height / collectionSetLabels.Length
            Dim pointSize As Double = stdNum.Min(boxWidth, boxHeight) / 2
            Dim gray As New SolidBrush(Color.Gray)
            Dim linkStroke As Pen = Stroke.TryParse(theme.lineStroke)
            Dim x As Double = layout.Left + pointSize

            For Each factor As FactorGroup In collections.groups
                Dim allData = factor.GetAllUniques.ToDictionary(Function(i) i.name)
                Dim labelIndex As Index(Of String) = collectionSetLabels
                Dim y As Double = layout.Top + pointSize
                Dim color As New SolidBrush(factor.color)

                ' single & unique
                For Each label As String In collectionSetLabels
                    ' get all data only appears in current collection
                    Dim unique As Integer = allData(label).Length

                    Call New NamedValue(Of Integer) With {
                        .Name = label,
                        .Value = unique
                    }.DoCall(AddressOf barData.Add)

                    For Each label2 As String In collectionSetLabels
                        If label <> label2 OrElse unique = 0 Then
                            ' gray dot
                            Call g.FillCircles(gray, {New Point(x, y)}, pointSize)
                        Else
                            Call g.FillCircles(color, {New Point(x, y)}, pointSize)
                        End If

                        y += boxHeight
                    Next

                    x += boxWidth
                Next

                ' draw for each combine group
                For Each combine As String() In allCompares
                    Dim intersect As String() = factor.GetIntersection(combine).ToArray

                    y = layout.Top + pointSize

                    If intersect.Length > 0 Then
                        ' line between the dots
                        Dim ymin As Double = 999999
                        Dim ymax As Double = -99999
                        Dim combineIndex As Index(Of String) = combine

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
                        .Name = combine.JoinBy("--"),
                        .Value = intersect.Length
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
            Dim scale = d3js.scale.linear.domain(setSize.Select(Function(i) CDbl(i.Value))).range(New Double() {0, layout.Width - maxLabelSize.Width})

            ' label is center alignment?
            For Each label As String In collectionSetLabels
                Dim labelPos As New PointF With {
                    .X = layout.Right - (maxLabelSize.Width - g.MeasureString(label, labelFont).Width) / 2,
                    .Y = y
                }
                Dim bar As New Rectangle With {
                    .Width = scale(setSize(label)),
                    .X = layout.Right - maxLabelSize.Width - .Width,
                    .Y = y,
                    .Height = maxLabelSize.Height
                }

                ' draw label
                Call g.DrawString(label, labelFont, Brushes.Black, labelPos)
                ' draw bar plot
                Call g.FillRectangle(Brushes.Black, bar)

                y += maxLabelSize.Height
            Next
        End Sub

        Private Sub drawTopBarPlot()

        End Sub
    End Class
End Namespace