#Region "Microsoft.VisualBasic::d774485ba36633dd3554bc6a2556e495, ChromosomeMap\PlasmidMap\DrawingDevice.vb"

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

'     Class DrawingDevice
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: InvokeDrawing
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.Visualize.ChromosomeMap.PlasmidMap.DrawingModels

Namespace PlasmidMap

    Public Class DrawingDevice

        Sub New()

        End Sub

        Public Function InvokeDrawing(plasmid As PlasmidMapDrawingModel) As Bitmap
            Using g = (New Size(1000, 1000)).CreateGDIDevice(filled:=Color.White)

                Dim center As Point = New Point(g.Width / 2, g.Height / 2)
                Dim r1 As Double = Math.Min(g.Width, g.Height) * 0.95
                Dim r2 As Double = Math.Min(g.Width, g.Height) * 0.85

#Const DEBUG = 1
#If DEBUG Then
                Call g.Graphics.DrawPie(Pens.Black, New Rectangle(New Point(center.X - 5, center.Y - 5), New Size(10, 10)), 0, 360)
#End If
                For i As Integer = 0 To plasmid.GeneObjects.Count - 1
                    Dim gene = plasmid.GeneObjects(i)
                    Dim size = DrawGene.Draw(g, center, gene, plasmid.genomeSize, r1, r2)
                Next

                Return g.ImageResource
            End Using
        End Function
    End Class
End Namespace
