﻿#Region "Microsoft.VisualBasic::a73f9d73d1f05901b6ec3a248043a1aa, visualize\ChromosomeMap\FootPrintMap\RegulationMotifSite.vb"

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

'   Total Lines: 64
'    Code Lines: 37 (57.81%)
' Comment Lines: 12 (18.75%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 15 (23.44%)
'     File Size: 2.70 KB


'     Class RegulationMotifSite
' 
'         Function: __topVertex, __vertexDown, TriangleModel
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing

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

Namespace FootprintMap

    ''' <summary>
    ''' Drawing model and method for the footprint motifs.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RegulationMotifSite

        Dim Device As ComponentModel.DrawingDevice

        ''' <summary>
        ''' 生成一个三角形的绘图模型
        ''' </summary>
        ''' <param name="Position"></param>
        ''' <param name="Height"></param>
        ''' <param name="Width"></param>
        ''' <param name="UpSideDown"></param>
        ''' <returns></returns>
        Public Shared Function TriangleModel(Position As Point,
                                             Height As Integer,
                                             Width As Integer,
                                             UpSideDown As Integer) As GraphicsPath

            Dim Model = If(UpSideDown <> 0,
                           __topVertex(Position, Height, Width),  ' 顶点在上面
                           __vertexDown(Position, Height, Width)) ' 顶点朝下
            Call Model.CloseFigure()

            Return Model
        End Function

        Private Shared Function __vertexDown(Position As Point,
                                     Height As Integer,
                                     Width As Integer) As GraphicsPath

            Dim Model As New GraphicsPath

            Dim RightTop = New Point(Position.X + 0.5 * Width, Position.Y - Height)
            Dim LeftTop = New Point(RightTop.X - Width, RightTop.Y)

            Call Model.AddLine(Position, RightTop)
            Call Model.AddLine(RightTop, LeftTop)
            Call Model.AddLine(LeftTop, Position)

            Return Model
        End Function

        Private Shared Function __topVertex(Position As Point,
                                     Height As Integer,
                                     Width As Integer) As GraphicsPath

            Dim Model As New GraphicsPath

            Dim RightButtom = New Point(Position.X + 0.5 * Width, Position.Y + Height)
            Dim LeftButtom = New Point(RightButtom.X - Width, RightButtom.Y)

            Call Model.AddLine(Position, RightButtom)
            Call Model.AddLine(RightButtom, LeftButtom)
            Call Model.AddLine(LeftButtom, Position)

            Return Model
        End Function
    End Class
End Namespace
