#Region "Microsoft.VisualBasic::0d3d78aabb7314205fe324ba1a560f36, G:/GCModeller/src/GCModeller/visualize/DataVisualizationExtensions//KeggPlot/EnrichmentCategoryBar.vb"

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

    '   Total Lines: 13
    '    Code Lines: 10
    ' Comment Lines: 0
    '   Blank Lines: 3
    '     File Size: 439 B


    ' Class ClassEnrichmentCategoryBar
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: PlotInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas

Public Class ClassEnrichmentCategoryBar : Inherits Plot

    Public Sub New(theme As Theme)
        MyBase.New(theme)
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As Imaging.IGraphics, canvas As Imaging.Drawing2D.GraphicsRegion)
        Throw New NotImplementedException()
    End Sub
End Class
