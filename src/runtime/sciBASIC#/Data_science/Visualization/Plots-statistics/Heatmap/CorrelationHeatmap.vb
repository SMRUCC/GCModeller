
Imports System.Drawing
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors

Namespace Heatmap

    Public Class CorrelationHeatmap : Inherits Plot

        Dim cor As CorrelationData
        Dim hist As Cluster
        Dim levels As Integer

        Public Sub New(cor As CorrelationData, theme As Theme, Optional levels As Integer = 20)
            MyBase.New(theme)

            Me.cor = cor
            Me.hist = New DefaultClusteringAlgorithm().performClustering(
                distances:=cor.GetMatrix,
                clusterNames:=cor.data.keys,
                linkageStrategy:=New AverageLinkageStrategy
            )
            Me.levels = levels
        End Sub

        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
            ' left
            Dim hor As New DendrogramPanelV2(hist, theme, showRuler:=False, showLeafLabels:=False)
            ' top
            Dim ver As New Horizon(hist, theme, showRuler:=False, showLeafLabels:=False)
            Dim region As Rectangle = canvas.PlotRegion
            Dim labelOrders As String() = hist.OrderLeafs

            Call hor.Plot(g, New Rectangle(New Point(region.Left, region.Top), New Size(0.1 * g.Size.Width, region.Height)))
            Call ver.Plot(g, New Rectangle(New Point(region.Left, region.Top), New Size(region.Width, 0.1 * g.Size.Height)))

            cor = cor _
                .SetLevels(levels) _
                .SetKeyOrders(labelOrders)

            Dim rectSize As New SizeF(region.Width / labelOrders.Length, region.Height / labelOrders.Length)
            Dim rect As RectangleF
            Dim left As Integer = region.Left
            Dim top As Integer = region.Top
            Dim colors As SolidBrush() = Designer _
                .GetColors(theme.colorSet, cor.levelRange.Max) _
                .Select(Function(cl) New SolidBrush(cl)) _
                .ToArray
            Dim level As Integer

            For i As Integer = 0 To labelOrders.Length - 1
                For j As Integer = 0 To labelOrders.Length - 1
                    rect = New RectangleF(New PointF(left + i * rectSize.Width, top + j * rectSize.Height), rectSize)
                    level = cor.GetLevel(i, j)
                    g.FillRectangle(colors(level), rect)
                Next
            Next
        End Sub
    End Class
End Namespace