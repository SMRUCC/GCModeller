﻿#Region "Microsoft.VisualBasic::c4ced4b5b162429ddf33d23d8f5fc6fa, ChromosomeMap\FootPrintMap\TSSs.vb"

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

    '     Class TSSs
    ' 
    '         Properties: Strand, Synonym
    ' 
    '         Sub: Draw
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports SMRUCC.genomics.Visualize.ChromosomeMap.FootprintMap
Imports SMRUCC.genomics.ComponentModel.Loci
Imports Microsoft.VisualBasic.Imaging

Namespace DrawingModels

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
        Public Overrides Sub Draw(Device As IGraphics, Location As Point, FlagLength As Integer, FLAG_HEIGHT As Integer)
            Dim Arrow = New Pen(Color.Black, 3)
            Arrow.EndCap = Drawing2D.LineCap.ArrowAnchor

            Call Device.DrawLine(Pens.Black, Location, New Point(Location.X, Location.Y - FLAG_HEIGHT))
            Call Device.DrawLine(Arrow, New Point(Location.X, Location.Y - FLAG_HEIGHT), New Point(Location.X + Strand * FlagLength, Location.Y - FLAG_HEIGHT))
        End Sub
    End Class
End Namespace
