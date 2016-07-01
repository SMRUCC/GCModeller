Imports System.Drawing
Imports SMRUCC.genomics.AnalysisTools.DataVisualization.ChromosomeMap.FootprintMap
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace ChromosomeMap.DrawingModels

    Public Class TSSs : Inherits Site

        ''' <summary>
        ''' 链的方向决定箭头的方向，正向链箭头向右，反向链则箭头向左
        ''' </summary>
        ''' <returns></returns>
        Public Property Strand As Strands
        Public Property Synonym As String

        ''' <summary>
        ''' 绘制有左右方向的折线，上面的终端有小箭头
        ''' </summary>
        ''' <param name="Device"></param>
        ''' <param name="Location"></param>
        ''' <param name="FlagLength"></param>
        ''' <param name="FLAG_HEIGHT"></param>
        Public Overrides Sub Draw(Device As Graphics, Location As Point, FlagLength As Integer, FLAG_HEIGHT As Integer)
            Dim Arrow = New Pen(Color.Black, 3)
            Arrow.EndCap = Drawing2D.LineCap.ArrowAnchor

            Call Device.DrawLine(Pens.Black, Location, New Point(Location.X, Location.Y - FLAG_HEIGHT))
            Call Device.DrawLine(Arrow, New Point(Location.X, Location.Y - FLAG_HEIGHT), New Point(Location.X + Strand * FlagLength, Location.Y - FLAG_HEIGHT))
        End Sub
    End Class
End Namespace