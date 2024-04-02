Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports SMRUCC.genomics.Analysis.HTS.GSEA

Public Class EnrichmentCategoryBubble : Inherits Plot

    ReadOnly enrich As EnrichmentResult()

    Public Sub New(enrich As IEnumerable(Of EnrichmentResult), theme As Theme)
        MyBase.New(theme)

        Me.enrich = enrich.ToArray
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Throw New NotImplementedException()
    End Sub
End Class
