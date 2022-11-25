#Region "Microsoft.VisualBasic::6dbb305b6bec23ac4321e7835dfaee3d, GCModeller\visualize\ChromosomeMap\PlasmidMap\DrawingDevice.vb"

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

    '   Total Lines: 41
    '    Code Lines: 35
    ' Comment Lines: 1
    '   Blank Lines: 5
    '     File Size: 1.88 KB


    '     Module DrawingDevice
    ' 
    '         Function: DrawMap
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Visualize.ChromosomeMap.PlasmidMap.DrawingModels

Namespace PlasmidMap

    Public Module DrawingDevice

        <Extension>
        Public Function DrawMap(plasmid As PlasmidMapDrawingModel,
                                Optional size$ = "1200,1200",
                                Optional margin$ = g.DefaultPadding,
                                Optional bg$ = "white",
                                Optional r1Scale# = 0.9,
                                Optional r2Scale# = 0.8) As GraphicsData
            Dim plotInternal =
                Sub(ByRef g As IGraphics, region As GraphicsRegion)
                    Dim canvasSize As Size = region.PlotRegion.Size
                    Dim center As New Point(canvasSize.Width / 2, canvasSize.Height / 2)
                    Dim r! = Math.Min(canvasSize.Width, canvasSize.Height)
                    Dim r1 As Double = r * r1Scale
                    Dim r2 As Double = r * r2Scale
#If DEBUG Then
                    ' 在调试模式下，会首先将参考用的圆心绘制出来
                    Call g.FillPie(Brushes.Red, New Rectangle(New Point(center.X - 5, center.Y - 5), New Size(10, 10)), 0, 360)
#End If
                    For i As Integer = 0 To plasmid.GeneObjects.Count - 1
                        Dim gene = plasmid.GeneObjects(i)

                        Call DrawGene.Draw(g, center, gene, plasmid.genomeSize, r1, r2)
                    Next
                End Sub

            Return g.GraphicsPlots(size.SizeParser, margin, bg, plotInternal)
        End Function
    End Module
End Namespace
