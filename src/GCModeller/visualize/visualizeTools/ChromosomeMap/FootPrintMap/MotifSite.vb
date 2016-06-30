Imports System.Drawing
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.ChromosomeMap.FootprintMap
Imports Microsoft.VisualBasic.Imaging

Namespace ChromosomeMap.DrawingModels

    ''' <summary>
    ''' Motif位点
    ''' </summary>
    Public Class MotifSite : Inherits Site

        Public Property Color As Color
        ''' <summary>
        ''' +/-
        ''' </summary>
        ''' <returns></returns>
        Public Property Strand As Char
        Public Property MotifName As String
        ''' <summary>
        ''' 这个位点的调控因子的名称，可以为空
        ''' </summary>
        ''' <returns></returns>
        Public Property Regulators As String()

        Public Overrides Sub Draw(Device As Graphics,
                                  Location As Point,
                                  WidthLength As Integer,
                                  Height As Integer)

            Dim GraphModel = RegulationMotifSite.TriangleModel(Position:=Location,
                                                               Height:=WidthLength,
                                                               Width:=Height,
                                                               UpSideDown:=If(Strand = "+"c, 1, 0))
            Dim infoLabel As String = __getLabel()
            Dim LabelFont = New Font(FontFace.MicrosoftYaHei, 8)
            Dim size = Device.MeasureString(infoLabel, LabelFont)
            Dim loci As New Point(Location.X + (size.Width - WidthLength) / 2, Location.Y - size.Height * 1.3)

            Call Device.DrawString(infoLabel, LabelFont, Brushes.DarkGreen, loci)
            Call Device.DrawPath(New Pen(Color, 8), GraphModel)
            Call Device.FillPath(New SolidBrush(Color), GraphModel)
        End Sub

        Private Function __getLabel() As String
            If Regulators.IsNullOrEmpty Then
                Return MotifName
            Else
                Return MotifName & ": " & String.Join(vbCrLf & New String(" "c, Len(MotifName) + 2), (From s As String In Regulators Select s & ";").ToArray)
            End If
        End Function
    End Class

    ''' <summary>
    ''' 长方形
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Loci : Inherits Site

        Public Property Color As Color
        Public Property SequenceData As String
        Public Property Scale As Double

        Public Overrides ReadOnly Property Width As Integer
            Get
                Return Scale * MyBase.Width
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Device"></param>
        ''' <param name="Location">左上角的位置</param>
        ''' <param name="FlagLength">无用参数</param>
        ''' <param name="FLAG_HEIGHT">高度</param>
        ''' <remarks></remarks>
        Public Overrides Sub Draw(Device As Graphics, Location As Point, FlagLength As Integer, FLAG_HEIGHT As Integer)
            Dim GraphModel = Me.CreateLociModel(ref:=Location, Height:=FLAG_HEIGHT)
            Dim infoLabel As String = Me.SequenceData
            Dim LabelFont = New Font("Microsoft YaHei", 8)
            Dim size = Device.MeasureString(infoLabel, LabelFont)

            Call Device.DrawString(infoLabel, LabelFont, Brushes.DarkGreen, New Point(Location.X + (size.Width - Me.Width) / 2, Location.Y - size.Height * 1.3))
            Call Device.DrawPath(New Pen(Color, 8), GraphModel)
            Call Device.FillPath(New SolidBrush(Color), GraphModel)
        End Sub

        Private Function CreateLociModel(ref As Point, Height As Integer) As System.Drawing.Drawing2D.GraphicsPath
            Dim Model As System.Drawing.Drawing2D.GraphicsPath = New Drawing2D.GraphicsPath
            Dim TopLeft = ref
            Dim TopRight As Point = New Point(TopLeft.X + Width, TopLeft.Y)
            Dim BottomLeft = New Point(TopLeft.X, TopLeft.Y + Height)
            Dim BottomRight = New Point(TopRight.X, TopRight.Y + Height)

            Call Model.AddLine(TopLeft, TopRight)
            Call Model.AddLine(TopRight, BottomRight)
            Call Model.AddLine(BottomRight, BottomLeft)
            Call Model.AddLine(BottomLeft, TopLeft)

            Return Model
        End Function
    End Class
End Namespace