Imports System.Drawing

Namespace Drawing2D

    ''' <summary>
    ''' 以极坐标为主的作图系统模块
    ''' </summary>
    Public Class PolarGraphics2D

        ''' <summary>
        ''' 通过极坐标计算出二维直角坐标的圆心中心点
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property center As PointF
        ''' <summary>
        ''' the graphics canvas module
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property canvas As IGraphics

        Public ReadOnly Property Size As Size
            Get
                Return canvas.Size
            End Get
        End Property

        Sub New(canvas As IGraphics, center As PointF)
            Me.canvas = canvas
            Me.center = center
        End Sub

    End Class
End Namespace