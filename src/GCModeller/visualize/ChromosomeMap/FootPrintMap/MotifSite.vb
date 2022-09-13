#Region "Microsoft.VisualBasic::8c923ed3d19ff2fc40b13973e4c55081, GCModeller\visualize\ChromosomeMap\FootPrintMap\MotifSite.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 109
    '    Code Lines: 70
    ' Comment Lines: 23
    '   Blank Lines: 16
    '     File Size: 4.36 KB


    '     Class MotifSite
    ' 
    '         Properties: Color, MotifName, Regulators, Strand
    ' 
    '         Function: __getLabel
    ' 
    '         Sub: Draw
    ' 
    '     Class Loci
    ' 
    '         Properties: Color, Scale, SequenceData, Width
    ' 
    '         Function: CreateLociModel
    ' 
    '         Sub: Draw
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.Visualize.ChromosomeMap.FootprintMap

Namespace DrawingModels

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

        Public Overrides Sub Draw(g As IGraphics,
                                  Location As Point,
                                  WidthLength As Integer,
                                  Height As Integer)

            Dim shape As GraphicsPath = RegulationMotifSite _
                .TriangleModel(Position:=Location,
                               Height:=WidthLength,
                               Width:=Height,
                               UpSideDown:=If(Strand = "+"c, 1, 0))

            Dim infoLabel As String = __getLabel()
            Dim labelFont = New Font(FontFace.MicrosoftYaHei, 8)
            Dim size = g.MeasureString(infoLabel, LabelFont)
            Dim loci As New Point With {
                .X = Location.X + (size.Width - WidthLength) / 2,
                .Y = Location.Y - size.Height * 1.3
            }

            Call g.DrawString(infoLabel, LabelFont, Brushes.DarkGreen, loci)
            Call g.DrawPath(New Pen(Color, 8), shape)
            Call g.FillPath(New SolidBrush(Color), shape)
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
        Public Overrides Sub Draw(Device As IGraphics, Location As Point, FlagLength As Integer, FLAG_HEIGHT As Integer)
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
