#Region "Microsoft.VisualBasic::8582df198d6550bc3011e658e0d9c71f, visualize\DataVisualizationExtensions\UPGMATreeDrawer.vb"

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

    '   Total Lines: 25
    '    Code Lines: 20 (80.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (20.00%)
    '     File Size: 881 B


    ' Class UPGMATreeDrawer
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: PlotInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render
Imports SMRUCC.genomics.Analysis.Metagenome.UPGMATree

Public Class UPGMATreeDrawer : Inherits Plot

    ReadOnly tree As Taxa

    Public Sub New(tree As Taxa, theme As Theme)
        MyBase.New(theme)
        Me.tree = tree
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
        Dim css As CSSEnvirnment = g.LoadEnvironment
        Dim region As Rectangle = canvas.PlotRegion(css)

        Call TreeDrawer.DrawCircularTree(g, tree, region.Size, region.Location)
    End Sub
End Class

