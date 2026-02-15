Imports System.Drawing
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render
Imports SMRUCC.genomics.Analysis.Metagenome.UPGMATree

Public Class UPGMATreeDrawer : Inherits Plot

    ReadOnly tree As Taxa

    Public Sub New(tree As Taxa, theme As Theme)
        MyBase.New(theme)
        Me.tree = tree
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim css As CSSEnvirnment = g.LoadEnvironment
        Dim region As Rectangle = canvas.PlotRegion(css)

        Call TreeDrawer.DrawCircularTree(g, tree, region.Size, region.Location)
    End Sub
End Class
