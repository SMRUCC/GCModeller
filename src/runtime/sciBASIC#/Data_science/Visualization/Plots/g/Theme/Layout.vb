Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

Namespace Graphic.Canvas

    Public MustInherit Class Layout

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="canvas"></param>
        ''' <param name="dependency">
        ''' a list of object with its ``[top-left]`` layout information. 
        ''' this objects table should always contains an ``canvas`` 
        ''' object layout information.
        ''' </param>
        ''' <returns></returns>
        Public MustOverride Function GetLocation(canvas As GraphicsRegion, dependency As Dictionary(Of String, RectangleF)) As PointF

        Public Shared Function BlankDependency(canvas As GraphicsRegion) As Dictionary(Of String, RectangleF)
            Dim rect As Rectangle = canvas.PlotRegion
            Dim rectf As New RectangleF(rect.Location.PointF, rect.Size.SizeF)

            Return New Dictionary(Of String, RectangleF) From {
                {NameOf(canvas), rectf}
            }
        End Function

    End Class

    ''' <summary>
    ''' 绝对位置定位
    ''' </summary>
    Public Class Absolute : Inherits Layout

        Public Property x As Double
        Public Property y As Double

        Public Overrides Function GetLocation(canvas As GraphicsRegion, objects As Dictionary(Of String, RectangleF)) As PointF
            Return New PointF(x, y)
        End Function
    End Class

    ''' <summary>
    ''' 相对位置定位
    ''' </summary>
    Public Class Relative : Inherits Layout

        Public Overrides Function GetLocation(canvas As GraphicsRegion, objects As Dictionary(Of String, RectangleF)) As PointF
            Throw New NotImplementedException()
        End Function
    End Class

    ''' <summary>
    ''' 百分比相对定位
    ''' </summary>
    Public Class PercentageRelative : Inherits Layout

        Public Property x As Double
        Public Property y As Double
        Public Property target As String = "canvas"

        Public Overrides Function GetLocation(canvas As GraphicsRegion, objects As Dictionary(Of String, RectangleF)) As PointF
            Dim rect As RectangleF = objects.TryGetValue(target)

            If rect.IsEmpty Then
                Throw New MissingPrimaryKeyException("missing layout dependency: " & target)
            End If

            Dim x = Me.x * rect.Width
            Dim y = rect.Height - Me.y * rect.Height

            Return New PointF(x, y)
        End Function
    End Class
End Namespace