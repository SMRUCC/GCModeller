﻿#Region "Microsoft.VisualBasic::fac5c5c296144c482206b7c11466e787, Data_science\Mathematica\Plot\test\ColorLegendTest.vb"

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

    ' Module ColorLegendTest
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors

Module ColorLegendTest

    Sub Main()

        Dim colors As SolidBrush() = Designer.GetColors(ColorBrewer.DivergingSchemes.RdBu9, 200).GetBrushes
        Dim ticks = AxisScalling.CreateAxisTicks({-10.3301, 13.7566}, 20)
        Dim range = ticks.Range

        Using g As Graphics2D = New Size(1200, 200).CreateGDIDevice

            Call colors.ColorLegendHorizontal(ticks, g, New Rectangle(New Point, g.Size))
            Call g.Save("./test.legend.png", ImageFormats.Png)

        End Using
    End Sub
End Module
