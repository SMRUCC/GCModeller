#Region "Microsoft.VisualBasic::803984b129c60f5efabeb91596d40859, ..\visualize\visualizeTools\GeneticClockDiagram\DrawingDevice.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Drawing
Imports SMRUCC.genomics.InteractionModel
Imports Microsoft.VisualBasic.Imaging

Namespace GeneticClock

    Public Class DrawingDevice

        Public Shared Function DrawingSerialsLine(data As ColorRender.ColorProfile, Height As Integer, Scale As Integer) As System.Drawing.Image
            Dim Bitmap As Bitmap = New Bitmap(width:=data.Profiles.Count * Scale, height:=Height)
            Using Gr As Graphics = Graphics.FromImage(Bitmap)
                Dim Region As Rectangle = New Rectangle(New Point, Bitmap.Size)
                Dim x As Integer = 1

                Call Gr.FillRectangle(Brushes.White, Region)

                If Scale = 1 Then
                    For Each item In data.Profiles
                        Call Gr.DrawLine(New Pen(item.Value, 1), New Point(x, 0), New Point(x, Height))
                        x += 1
                    Next
                Else
                    For Each item In data.Profiles
                        Dim RECT As Rectangle = New Rectangle(New Point(x, 0), New Size(Scale, Height))
                        Call Gr.FillRectangle(New SolidBrush(item.Value), RECT)
                        x += Scale
                    Next
                End If
            End Using

            Return Bitmap
        End Function

        Public Shared Function Draw(SerialsData As SerialsData(), Scale As Integer) As Image
            Dim ColorRendering = New ColorRender(SerialsData.Skip(1).ToArray)
            Dim DataChunk = ColorRendering.GetColorRenderingProfiles
            Dim DrawingFont As Font = New Font("Ubuntu", 12)
            Dim MaxSize As Size = (From item In DataChunk Select item.TagValue Order By Len(TagValue) Ascending).Last.MeasureString(DrawingFont)
            Dim Height As Integer = MaxSize.Height
            Dim ImageOffSet As Integer = MaxSize.Width + 20
            Dim Bitmap As Bitmap = New Bitmap(CInt(DataChunk.First.Profiles.Count * Scale + ImageOffSet + 0.5 * MaxSize.Width), Height * DataChunk.Count + Height * 4)

            Using Gr As Graphics = Graphics.FromImage(Bitmap)
                Dim Region As Rectangle = New Rectangle(New Point, Bitmap.Size)
                Dim y As Integer = 0

                Gr.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                Gr.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                Call Gr.FillRectangle(Brushes.White, Region)

                For Each Line In DataChunk
                    Call Gr.DrawImage(DrawingSerialsLine(Line, Height - 1, Scale), New Point(ImageOffSet, y))
                    Dim Left As Integer = ImageOffSet - Gr.MeasureString(Line.TagValue, DrawingFont).Width
                    Call Gr.DrawString(Line.TagValue, DrawingFont, Brushes.Black, New Point(Left, y - 2))
                    y += Height
                Next

                y += 10
                Dim ImageWidth = DataChunk.First.Profiles.Count * Scale
                Dim RuleEndPoint As New Point(ImageOffSet + ImageWidth, y)
                Dim DrawingPen As Pen = New Pen(Brushes.Black, 1)
                'Dim ArrowLeft = RuleEndPoint.X - 5, ArrowHeightOffSet = 5

                Call Gr.DrawLine(DrawingPen, New Point(ImageOffSet, y), RuleEndPoint)
                'Call Gr.DrawLine(DrawingPen, New Point(ArrowLeft, y - ArrowHeightOffSet), RuleEndPoint)
                'Call Gr.DrawLine(DrawingPen, New Point(ArrowLeft, y + ArrowHeightOffSet), RuleEndPoint)

                Dim TimePoints = SerialsData.First.ChunkBuffer
                Dim [step] As Integer = 6
                Dim TimeStep As Integer = TimePoints.Last / [step]
                Dim DrawStep = ImageWidth / [step]
                Dim x = ImageOffSet
                Dim TimeValue = 0
                Dim Rule_Y As Integer = y

                y += 3

                DrawingFont = New Font("Ubuntu", 10)

                For i As Integer = 0 To [step]  '绘制时间标尺
                    Dim size = Gr.MeasureString(TimeValue.ToString, DrawingFont)

                    Call Gr.DrawString(TimeValue, DrawingFont, Brushes.Black, New Point(x - size.Width / 2, y))
                    Call Gr.DrawLine(DrawingPen, New Point(x, Rule_Y - 2), New Point(x, Rule_Y))

                    TimeValue += TimeStep
                    x += DrawStep
                Next

                MaxSize = SerialsData.First.Tag.MeasureString(DrawingFont)
                Call Gr.DrawString(SerialsData.First.Tag, DrawingFont, Brushes.Black, New Point(x - 0.5 * DrawStep, y - 3 - MaxSize.Height / 2))
                Call ColorRendering.GetDesityRule(50).DrawingDensityRule(Gr, New Point(ImageOffSet, y), DrawingFont, ImageWidth)
            End Using

            Return Bitmap
        End Function
    End Class
End Namespace
