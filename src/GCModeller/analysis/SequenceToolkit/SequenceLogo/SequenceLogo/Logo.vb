#Region "Microsoft.VisualBasic::7cec6ed7bc0d6d0bd541ac1f0020d38a, analysis\SequenceToolkit\SequenceLogo\SequenceLogo\Logo.vb"

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

    '   Total Lines: 186
    '    Code Lines: 136 (73.12%)
    ' Comment Lines: 16 (8.60%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 34 (18.28%)
    '     File Size: 7.70 KB


    '     Class Logo
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: PlotInternal
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo

#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
#End If

Namespace SequenceLogo

    Public Class Logo : Inherits Plot

        ReadOnly model As DrawingModel
        ReadOnly reverse As Boolean
        ReadOnly frequencyOrder As Boolean

        Public Sub New(model As DrawingModel, reverse As Boolean, frequencyOrder As Boolean, theme As Theme)
            MyBase.New(theme)

            Me.model = model
            Me.reverse = reverse
            Me.frequencyOrder = frequencyOrder

            If main.StringEmpty Then
                main = model.ModelsId
            End If
        End Sub

        Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
            Dim css As CSSEnvirnment = g.LoadEnvironment
            Dim size As SizeF
            Dim region As Rectangle = canvas.PlotRegion(css)
            Dim X, Y As Single
            Dim font As Font = css.GetFont(theme.tagCSS)
            Dim wordSize As Single = font.Size
            Dim margin As Padding = canvas.Padding
            Dim n As Integer = model.Alphabets
            Dim gfx = g
            Dim height As Single = region.Height / n

            Call DrawMainTitle(g, region, 1.25)

            font = New Font(FontFace.MicrosoftYaHei, CInt(wordSize * 0.4))

#Region "画坐标轴"
            ' 坐标轴原点
            ' x0, y0
            X = css.GetWidth(margin.Left)
            Y = region.Height + css.GetHeight(margin.Top)

            Dim maxBits As Double = Math.Log(n, newBase:=2)
            Dim yHeight As Single = n * height

            ' y axis
            Call g.DrawLine(Pens.Black, New PointF(X, Y - yHeight), New PointF(X, Y))
            ' x axis
            Call g.DrawLine(Pens.Black, New PointF(X, Y), New PointF(X + region.Width, y:=Y))

            ' nt 2 steps,  aa 5 steps
            Dim departs As Integer = If(maxBits = 2, 2, 5)
            Dim d As Double = maxBits / departs

            ' 步进
            yHeight = d / maxBits * (height * n)
            d = Math.Round(d, 1)

            Dim yBits As Double = 0
            Dim y1!
            Dim str As String
            Dim pad As Integer = 5
            Dim maxStrW As Double = Enumerable.Range(0, departs + 1) _
                .Select(Function(a)
                            Return gfx.MeasureString(a * d, font).Width
                        End Function) _
                .Max
            Dim bitStrLeft As Single = X - maxStrW - pad

            For j As Integer = 0 To departs
                str = yBits.ToString
                size = g.MeasureString(str, font:=font)

                y1 = Y - size.Height / 2
                g.DrawString(str, font, Brushes.Black, New PointF(x:=bitStrLeft, y:=y1))

                y1 = Y '- sz.Height / 8
                g.DrawLine(Pens.Black, New PointF(x:=X, y:=y1), New PointF(x:=X + 10, y:=y1))

                yBits += d
                Y -= yHeight
            Next

            '绘制bits字符串
            Dim bitsLabelFont As New Font(font.Name, font.Size / 2)

            size = g.MeasureString("Bits", bitsLabelFont)
            g.DrawString("Bits", bitsLabelFont, Brushes.Black, css.GetWidth(margin.Left) / 3, region.Top + (region.Height - size.Width) / 2, -90)
#End Region
            Dim source As IEnumerable(Of Residue) = If(reverse, model.Residues.Reverse, model.Residues)
            Dim colorSchema As Dictionary(Of Char, Image) = model.CharColorImages(theme.background.TranslateColor)
            Dim order As Alphabet()

            Call VBDebugger.WriteLine(New String("-"c, model.Residues.Length), ConsoleColor.Green)

            ' start from x0
            X = css.GetWidth(margin.Left)
            font = New Font(font.Name, font.Size * 0.65)
            wordSize = region.Width / model.Residues.Length

            For Each residue As Residue In source
                If Not frequencyOrder Then
                    order = residue.Alphabets
                Else
                    order = (From rsd As Alphabet
                             In residue.Alphabets
                             Order By rsd.RelativeFrequency Ascending).ToArray
                End If

                Y = region.Height + css.GetHeight(margin.Top)
                ' YHeight is the max height of current residue,
                ' and its value is calculate from its Bits value
                yHeight = region.Height * (If(residue.Bits > maxBits, maxBits, residue.Bits) / maxBits)

                Dim idx As String = CStr(residue.Position)
                Dim loci As PointF

                size = g.MeasureString(idx, font)
                loci = New PointF(X + (wordSize - size.Width) / 2, Y + 10)
                g.DrawString(idx, font, Brushes.Black, loci)

                For Each alphabet As Alphabet In order
                    If alphabet.RelativeFrequency <= 0.0 Then
                        Continue For
                    End If

                    ' H is the drawing height of the current drawing alphabet, 
                    ' this height value can be calculate from the formula that show above. 
                    ' As the YHeight variable is transform from the current residue Bits value, so that from this statement
                    ' The drawing height of the alphabet can be calculated out. 
                    Dim H As Single = alphabet.RelativeFrequency * yHeight

                    ' Due to the reason of the Y Axis in gdi+ is up side down, so that we needs Subtraction operation, 
                    ' and then this makes the next alphabet move up direction 
                    Y -= H

                    g.DrawImage(
                        colorSchema(alphabet.Alphabet),   ' Drawing alphabet
                        X, Y,                             ' position
                        CSng(wordSize), H)  ' Size and relative height
                Next

                X += wordSize
                Call residue.AsChar.Echo
            Next

            Call Console.WriteLine()
        End Sub
    End Class
End Namespace
