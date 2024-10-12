﻿#Region "Microsoft.VisualBasic::85f383f9faf1d25c0f98f45eb11dae8a, visualize\DataVisualizationExtensions\SampleColorBend.vb"

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

    '   Total Lines: 87
    '    Code Lines: 62 (71.26%)
    ' Comment Lines: 13 (14.94%)
    '    - Xml Docs: 38.46%
    ' 
    '   Blank Lines: 12 (13.79%)
    '     File Size: 3.29 KB


    ' Module SampleColorBend
    ' 
    '     Function: GetColors
    ' 
    '     Sub: Draw
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

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

Public Module SampleColorBend

    ''' <summary>
    ''' 仅对行进行计算
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function GetColors(matrix As Matrix, Optional colorSet$ = "RdYlGn:c8", Optional levels As Integer = 25) As IEnumerable(Of NamedCollection(Of Color))
        Dim colors As Color() = Designer.GetColors(colorSet, n:=levels)
        Dim indexRange As DoubleRange = New Double() {0, colors.Length - 1}

        For Each gene As DataFrameRow In matrix.expression
            Dim levelRange As New DoubleRange(gene.experiments)
            Dim index As Integer() = gene.experiments _
                .Select(Function(a)
                            Return levelRange.ScaleMapping(a, indexRange)
                        End Function) _
                .Select(Function(d) CInt(d)) _
                .ToArray

            Yield New NamedCollection(Of Color) With {
                .name = gene.geneID,
                .value = index _
                    .Select(Function(i) colors(i)) _
                    .ToArray
            }
        Next
    End Function

    ' drawing layout
    '
    '                A  B  C  D
    ' horizontal:   [ ][ ][ ][ ]
    '
    ' vertical:     [ ] A
    '               [ ] B
    '               [ ] C

    Public Sub Draw(g As IGraphics, layout As RectangleF, geneExpression As Color(),
                    Optional horizontal As Boolean = True,
                    Optional sampleNames As String() = Nothing,
                    Optional labelFontCSS$ = CSSFont.PlotSmallTitle）

        Dim boxSize As Single
        Dim css As CSSEnvirnment = g.LoadEnvironment
        Dim labelFont As Font = css.GetFont(CSSFont.TryParse(labelFontCSS))

        If horizontal Then
            boxSize = layout.Width / geneExpression.Length
        Else
            boxSize = layout.Height / geneExpression.Length
        End If

        Dim x = layout.Left
        Dim y = layout.Top
        ' Dim textDraw As New GraphicsText(DirectCast(g, Graphics2D).Graphics)

        For i As Integer = 0 To geneExpression.Length - 1
            Call g.FillRectangle(New SolidBrush(geneExpression(i)), New RectangleF(x, y, boxSize, boxSize))

            If Not sampleNames.IsNullOrEmpty Then
                If horizontal Then
                    Call g.DrawString(sampleNames(i), labelFont, Brushes.Black, x, y - labelFont.Height, -90)
                Else
                    Call g.DrawString(sampleNames(i), labelFont, Brushes.Black, New PointF(x + boxSize + 5, y))
                End If
            End If

            If horizontal Then
                x += boxSize
            Else
                y += boxSize
            End If
        Next
    End Sub
End Module
