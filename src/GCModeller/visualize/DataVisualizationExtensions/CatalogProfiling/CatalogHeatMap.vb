
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

Namespace CatalogProfiling

    ''' <summary>
    ''' heatmap data of the KEGG enrichment between 
    ''' multiple groups data:
    ''' 
    ''' 1. x axis is the sample id
    ''' 2. y axis is the pathway name and category data
    ''' 3. cell size is the impact value or enrich factor
    ''' 4. cell color is scaled via -log10(pvalue)
    ''' </summary>
    Public Class CatalogHeatMap : Inherits Plot

        ''' <summary>
        ''' the multiple groups data
        ''' </summary>
        ReadOnly multiples As NamedValue(Of Dictionary(Of String, BubbleTerm()))()
        ReadOnly mapLevels As Integer

        Public Sub New(profile As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm()))), mapLevels As Integer, theme As Theme)
            Call MyBase.New(theme)

            Me.multiples = profile.ToArray
            Me.mapLevels = mapLevels
        End Sub

        ''' <summary>
        ''' heatmap是按照代谢途径分块绘制的
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="canvas"></param>
        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)

        End Sub
    End Class
End Namespace