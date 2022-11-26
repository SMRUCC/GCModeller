#Region "Microsoft.VisualBasic::a2d419a0a43707aa29d61d2af664d093, GCModeller\visualize\DataVisualizationExtensions\DEGPlot\Volcano2.vb"

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

    '   Total Lines: 30
    '    Code Lines: 15
    ' Comment Lines: 9
    '   Blank Lines: 6
    '     File Size: 808 B


    ' Class Volcano2
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: PlotInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D

''' <summary>
''' volcano of two comparision result
''' </summary>
Public Class Volcano2 : Inherits Plot

    ''' <summary>
    ''' x
    ''' </summary>
    ReadOnly compares1 As DEGModel()
    ''' <summary>
    ''' y
    ''' </summary>
    ReadOnly compares2 As DEGModel()

    Public Sub New(compares1 As DEGModel(), compares2 As DEGModel(), theme As Theme)
        MyBase.New(theme)

        Me.compares1 = compares1
        Me.compares2 = compares2
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)

    End Sub
End Class
