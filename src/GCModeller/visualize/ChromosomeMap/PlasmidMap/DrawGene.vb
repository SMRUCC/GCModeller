﻿#Region "Microsoft.VisualBasic::8851236c844e6a47964e928d1c70bf9f, visualize\ChromosomeMap\PlasmidMap\DrawGene.vb"

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

    '   Total Lines: 40
    '    Code Lines: 26 (65.00%)
    ' Comment Lines: 9 (22.50%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (12.50%)
    '     File Size: 1.62 KB


    '     Module DrawGene
    ' 
    '         Function: Draw
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels

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
Imports LineCap = System.Drawing.Drawing2D.LineCap
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
Imports LineCap = Microsoft.VisualBasic.Imaging.LineCap
#End If

Namespace PlasmidMap.DrawingModels

    Public Module DrawGene

        ''' <summary>
        ''' 弧度是通过基因的起始和结束的位置在整个序列的长度的百分比乘上2pi得到的
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="center">图形的左上角的坐标</param>
        ''' <returns>返回绘制的图形的大小</returns>
        ''' <param name="r1">在绘制基因的时候的内圈的半径</param>
        ''' <param name="r2">在绘制基因的时候的外圈的半径</param>
        ''' <remarks></remarks>
        Public Function Draw(g As IGraphics, center As Point, gene As SegmentObject, genomeSize%, r1!, r2!) As Size
            Dim path As New GraphicsPath
            Dim startAngle! = gene.Left / genomeSize * 360
            Dim endAngle! = gene.Right / genomeSize * 360
            Dim sweep = endAngle - startAngle
            Dim line As New Pen(Color.Black, Math.Abs(r1 - r2) / 3) With {
                .EndCap = LineCap.ArrowAnchor
            }
            Dim ref As New PointF With {
                .X = center.X - r2 / 2,
                .Y = center.Y - r2 / 2
            }
            Dim region As New RectangleF(ref, New Size(r2, r2))

#If DEBUG Then
            Call g.DrawRectangle(Pens.Green, region)
#End If

            Call g.DrawArc(line, region, startAngle, sweep)
        End Function
    End Module
End Namespace
