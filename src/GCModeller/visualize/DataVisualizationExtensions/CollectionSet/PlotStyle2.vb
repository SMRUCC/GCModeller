﻿Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

Namespace CollectionSet

    ''' <summary>
    ''' upset plot with alternative plot style
    ''' </summary>
    Public Class PlotStyle2 : Inherits Plot

        Public Sub New(theme As Theme)
            MyBase.New(theme)
        End Sub

        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
            Throw New NotImplementedException()
        End Sub
    End Class
End Namespace