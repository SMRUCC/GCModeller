Imports System.Drawing

Namespace Drawing2D.Colors

    Public Class ColorMapLegend

        Public Property designer As SolidBrush()
        Public Property title As String
        Public Property titleFont As Font
        Public Property ticks As Double()
        Public Property tickFont As Font
        Public Property tickAxisStroke As Pen
        Public Property unmapColor As String = Nothing
        Public Property ruleOffset As Single = 10
        Public Property format As String = "F2"
        Public Property legendOffsetLeft As Single = -99999

        Public Sub Draw(ByRef g As IGraphics, layout As Rectangle)
            Call g.ColorMapLegend(
                layout:=layout,
                designer:=designer,
                ticks:=ticks,
                titleFont:=titleFont,
                title:=title,
                tickFont:=tickFont,
                tickAxisStroke:=tickAxisStroke,
                unmapColor:=unmapColor,
                ruleOffset:=ruleOffset,
                format:=format,
                legendOffsetLeft:=legendOffsetLeft
            )
        End Sub
    End Class
End Namespace