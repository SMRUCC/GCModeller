Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging.Drawing2D

Namespace Graphic.Canvas

    Public MustInherit Class Layout

        Public MustOverride Function GetLocation(canvas As GraphicsRegion) As PointF

    End Class

    ''' <summary>
    ''' 绝对位置定位
    ''' </summary>
    Public Class Absolute : Inherits Layout

        Public Property x As Double
        Public Property y As Double

        Public Overrides Function GetLocation(canvas As GraphicsRegion) As PointF
            Return New PointF(x, y)
        End Function
    End Class

    ''' <summary>
    ''' 相对位置定位
    ''' </summary>
    Public Class Relative : Inherits Layout

        Public Overrides Function GetLocation(canvas As GraphicsRegion) As PointF
            Throw New NotImplementedException()
        End Function
    End Class

    ''' <summary>
    ''' 百分比相对定位
    ''' </summary>
    Public Class PercentageRelative : Inherits Layout

        Public Property x As Double
        Public Property y As Double

        Public Overrides Function GetLocation(canvas As GraphicsRegion) As PointF
            Dim x = Me.x * canvas.PlotRegion.Width
            Dim y = canvas.PlotRegion.Height - Me.y * canvas.PlotRegion.Height

            Return New PointF(x, y)
        End Function
    End Class
End Namespace