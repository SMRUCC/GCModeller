Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports SMRUCC.genomics.Visualize.ChromosomeMap.FootprintMap

Namespace DrawingModels

    ''' <summary>
    ''' 基因组之中的一个位点
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class Site

        Public Property SiteName As String
        Public Property Comments As String
        Public Property Left As Integer
        Public Property Right As Integer

        Public Overridable ReadOnly Property Width As Integer
            Get
                Return Math.Abs(Left - Right)
            End Get
        End Property

        Public MustOverride Sub Draw(Device As Graphics, Location As Point, FlagLength As Integer, FLAG_HEIGHT As Integer)

        Public Overrides Function ToString() As String
            Return SiteName
        End Function
    End Class
End Namespace