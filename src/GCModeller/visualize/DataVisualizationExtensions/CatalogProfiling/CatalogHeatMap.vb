
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace CatalogProfiling

    Public Class CatalogHeatMap : Inherits Plot

        ReadOnly profile As CatalogProfiles
        ReadOnly matrix As Matrix
        ReadOnly mapLevels As Integer

        Public Sub New(profile As CatalogProfiles, matrix As Matrix, mapLevels As Integer, theme As Theme)
            MyBase.New(theme)

            Me.profile = profile
            Me.matrix = matrix
            Me.mapLevels = mapLevels
        End Sub

        ''' <summary>
        ''' heatmap是按照代谢途径分块绘制的
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="canvas"></param>
        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
            Throw New NotImplementedException()
        End Sub
    End Class
End Namespace