Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Language

Public Class MingleRender

    ReadOnly options As RenderOptions

    Sub New()

    End Sub

    Public Sub renderLine(ctx As RenderContext, edges As Edge())
        Dim lineWidth = options.lineWidth
        Dim fillStyle = options.fillStyle Or "gray".AsDefault
        Dim pos As Double()

        ctx.fillStyle = fillStyle
        ctx.lineWidth = lineWidth

        For Each e As Edge In edges
            ctx.beginPath()

            'For j As Integer = 0 To e.length - 1
            '    pos = e(j).unbundledPos

            '    If (j = 0) Then
            '        ctx.moveTo(pos(0), pos(1))
            '    Else
            '        ctx.lineTo(pos(0), pos(1))
            '    End If
            'Next

            ctx.stroke()
            ctx.closePath()
        Next
    End Sub

End Class
