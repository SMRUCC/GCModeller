#Region "Microsoft.VisualBasic::6cec0c312204f1ec414e6d83880f8bd2, analysis\RNA-Seq\TSSsTools\ReadsPlot.vb"

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

    ' Module ReadsPlot
    ' 
    '     Function: Plot
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging

Public Module ReadsPlot

    Public Function Plot(reads As Generic.IEnumerable(Of ReadsCount)) As Bitmap
        Dim Gr = New Size(reads.Count, 1000).CreateGDIDevice
        Dim readsArray = (From x In reads Select x Order By x.Index Ascending).ToArray

        Dim px As Integer = 100
        Dim py As Integer = 500

        For Each x As ReadsCount In readsArray
            Call Gr.Graphics.DrawLine(Pens.Brown, New Point(px, py), New Point(px, py - x.ReadsPlus))
            Call Gr.Graphics.DrawLine(Pens.Green, New Point(px, py), New Point(px, py - x.SharedPlus))

            Call Gr.Graphics.DrawLine(Pens.SeaGreen, New Point(px, py), New Point(px, py + x.ReadsMinus))
            Call Gr.Graphics.DrawLine(Pens.Blue, New Point(px, py), New Point(px, py + x.SharedMinus))

            px += 1
        Next

        Call Gr.ImageResource.Save("x:\sdfsdf.bmp")

        Return Gr.ImageResource
    End Function
End Module
