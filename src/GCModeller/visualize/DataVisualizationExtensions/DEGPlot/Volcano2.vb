Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

''' <summary>
''' volcano of two comparision result
''' </summary>
Public Class Volcano2 : Inherits Plot

    ''' <summary>
    ''' x
    ''' </summary>
    ReadOnly compares1 As DEGModel()
    ''' <summary>
    ''' y
    ''' </summary>
    ReadOnly compares2 As DEGModel()

    Public Sub New(compares1 As DEGModel(), compares2 As DEGModel(), theme As Theme)
        MyBase.New(theme)

        Me.compares1 = compares1
        Me.compares2 = compares2
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)

    End Sub
End Class