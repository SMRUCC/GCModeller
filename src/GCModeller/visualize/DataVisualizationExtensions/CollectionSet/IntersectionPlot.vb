Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.MIME.Html.CSS

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

        Private Sub drawBottomIntersctionVisualize(g As IGraphics, collectionSetLabels As String(), barData As List(Of NamedValue(Of Integer)), layout As Rectangle)

        End Sub

        Private Sub drawLeftBarSet(g As IGraphics, rect As Rectangle)

        End Sub

        Private Sub drawTopBarPlot()

        End Sub
    End Class
End Namespace