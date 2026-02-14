Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports SMRUCC.genomics.Analysis.Metagenome.UPGMATree

Public Class UPGMATreeDrawer : Inherits Plot

    ReadOnly tree As Taxa

    Public Sub New(tree As Taxa, theme As Theme)
        MyBase.New(theme)
        Me.tree = tree
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Call TreeDrawer.DrawCircularTree(g, tree, canvas.Size)
    End Sub
End Class
