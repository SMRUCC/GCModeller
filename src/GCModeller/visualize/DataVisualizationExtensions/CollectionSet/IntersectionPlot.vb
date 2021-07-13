Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
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
            Dim barData As New List(Of NamedValue(Of Integer))

            Call drawBottomIntersctionVisualize(g, collectionSetLabels, barData, layout:=bottomIntersection)
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
            Dim dotsPerGroup As Integer = collectionSetLabels.Length + allCompares.Length  ' unique + combinations
            Dim widthPerGroup As Double = layout.Width / collections.size
            Dim boxWidth As Double = widthPerGroup / dotsPerGroup
            Dim boxHeight As Double = layout.Height / collectionSetLabels.Length
            Dim pointSize As Double = stdnum.min(boxWidth, boxHeight) / 2
            Dim gray As New SolidBrush(Color.Gray)

            For Each factor As FactorGroup In collections.groups
                Dim allData = factor.GetAllUniques.ToDictionary(Function(i) i.name)
                Dim labelIndex As Index(Of String) = collectionSetLabels
                Dim y As Double = layout.Top + pointSize
                Dim x As Double = layout.Left + pointSize
                Dim color As New SolidBrush(factor.color)

                ' single & unique
                For Each label As String In collectionSetLabels
                    ' get all data only appears in current collection
                    Dim unique As Integer = allData(label).Length

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

                Next
            Next


        End Sub

        Private Sub drawLeftBarSet(g As IGraphics, rect As Rectangle)

        End Sub

        Private Sub drawTopBarPlot()

        End Sub
    End Class
End Namespace