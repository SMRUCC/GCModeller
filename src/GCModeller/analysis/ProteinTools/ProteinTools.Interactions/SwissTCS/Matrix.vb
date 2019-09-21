#Region "Microsoft.VisualBasic::2e27d02659a29c37910ea4779112a606, analysis\ProteinTools\ProteinTools.Interactions\SwissTCS\Matrix.vb"

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

    ' Class Matrix
    ' 
    '     Function: Generate
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Data.csv

Public Class Matrix

    Const BLOCK_WIDTH As Integer = 120

    Public Function Generate(Matrix As IO.File, titl As String) As System.Drawing.Image
        Dim Colors As System.Drawing.Color() = (From [property] In GetType(Drawing.Color).GetProperties() Where [property].PropertyType Is GetType(System.Drawing.Color) Select CType([property].GetValue(Nothing), System.Drawing.Color)).ToArray
        Colors = (From cl In Colors Select cl Order By (System.Convert.ToInt32(cl.R) * 128 + System.Convert.ToInt32(cl.G) * 32 + System.Convert.ToInt32(cl.B)) Descending).Skip(20).ToArray

        Dim SolidBrushs As System.Drawing.Brush() = New System.Drawing.Brush(Colors.Count - 2) {}
        For i As Integer = 0 To SolidBrushs.Count - 1
            SolidBrushs(i) = New System.Drawing.SolidBrush(Colors(i))
        Next

        Dim Picture As System.Drawing.Image = New System.Drawing.Bitmap(Matrix.Width * (BLOCK_WIDTH + 40), (Matrix.Count + 6) * (BLOCK_WIDTH + 5))
        Call Picture.FillBlank(Drawing.Brushes.White)

        Dim skId = (From row In Matrix.Skip(1) Select row.First).ToArray
        Dim rrId = Matrix.First.Skip(1).ToArray

        Dim txtFont As System.Drawing.Font = New Drawing.Font("Microsoft YaHei", 18, FontStyle.Bold)

        Dim OffSet_Height = Picture.Height * 0.075
        Dim OffSet_Width = Picture.Width * 0.035

        Using gr = System.Drawing.Graphics.FromImage(Picture)
            Dim titleFont = New System.Drawing.Font("Microsoft YaHei", 40, Drawing.FontStyle.Bold)
            Dim StringSize = gr.MeasureString(titl, titleFont)
            Call gr.DrawString(titl, titleFont, Drawing.Brushes.Black, New System.Drawing.Point((Picture.Width - StringSize.Width) / 2, Picture.Height * 0.05))

            Dim BlockSize As Size = New Size(15, 160) '绘制标尺
            Dim LocationHeight = Picture.Height * 0.92
            Dim LineLocationHeight = LocationHeight + BlockSize.Height + 20
            Dim ruleOffset As Integer = 150

            For i As Integer = 0 To SolidBrushs.Count - 1
                Dim Left As Integer = BlockSize.Width * (i + 1) + OffSet_Width + ruleOffset
                Call gr.FillRectangle(SolidBrushs(i), New Rectangle(New Point(Left, LocationHeight), BlockSize))
                Call gr.DrawLine(Pens.Black, New Point(Left, LineLocationHeight), New Point(Left, LineLocationHeight - 10)) '绘制标尺上面的小刻度
            Next
            Dim WLeft = BlockSize.Width + OffSet_Width + ruleOffset, WRight = OffSet_Width + BlockSize.Width * (SolidBrushs.Count + 1) + ruleOffset
            Dim NLEFT = BlockSize.Width * (SolidBrushs.Count + 1) + OffSet_Width + ruleOffset
            Call gr.DrawLine(Pens.Black, New Point(NLEFT, LineLocationHeight), New Point(NLEFT, LineLocationHeight - 10))
            Call gr.DrawLine(Pens.Black, New Point(WLeft, LineLocationHeight), New Point(WRight, LineLocationHeight))
            LineLocationHeight += 5

            Call gr.DrawString("0.0", txtFont, Brushes.Black, New Point(WLeft - 15, LineLocationHeight))
            Call gr.DrawString("1.0", txtFont, Brushes.Black, New Point(WRight - 15, LineLocationHeight))

            BlockSize = New Size(BLOCK_WIDTH, BLOCK_WIDTH)

            Dim txtFontSize = gr.MeasureString(rrId.First, txtFont)

            Dim clCounts = SolidBrushs.Count
            Dim drawRR As Boolean = True
            For rowIndex As Integer = 1 To Matrix.Count - 1
                Dim row = Matrix(rowIndex)
                Dim height = (rowIndex + 1) * (BLOCK_WIDTH + 4) - 20 + OffSet_Height

                For colIndex As Integer = 1 To row.Count - 1
                    Dim cl_idx As Integer = Val(row(colIndex)) * clCounts - 1
                    If cl_idx = -1 Then
                        cl_idx = 0
                    End If

                    Dim LeftBBBB = (BlockSize.Width + 5) * colIndex + OffSet_Width + 10
                    Call gr.FillRectangle(SolidBrushs(cl_idx), New Rectangle(New Point(LeftBBBB, height), BlockSize))

                    If drawRR Then
                        Call gr.DrawString(rrId(colIndex - 1), txtFont, Drawing.Brushes.Black, New Point(LeftBBBB + (BLOCK_WIDTH - txtFontSize.Width) / 2, height - 20 - txtFontSize.Height))
                    End If
                Next

                drawRR = False
                Call gr.DrawString(row.First, txtFont, Drawing.Brushes.Black, New System.Drawing.Point(OffSet_Width - 10, height + (BLOCK_WIDTH - txtFontSize.Height) / 2))
            Next
        End Using

        Return Picture
    End Function
End Class
