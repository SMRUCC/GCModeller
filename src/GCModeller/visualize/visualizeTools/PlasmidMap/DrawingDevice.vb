#Region "Microsoft.VisualBasic::ed064360a3882100cfa0130b804fad2f, ..\GCModeller\visualize\visualizeTools\PlasmidMap\DrawingDevice.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.Imaging

Namespace PlasmidMap

    Public Class DrawingDevice

        Sub New()

        End Sub

        Public Function InvokeDrawing(ObjectModel As PlasmidMapDrawingModel) As Bitmap
            Dim GRHandle = (New Size(1000, 1000)).CreateGDIDevice(filled:=Color.White)
            Dim Center As Point = New Point(GRHandle.ImageResource.Size.Width / 2, GRHandle.ImageResource.Size.Height / 2)

            Dim r1 As Double = Math.Min(GRHandle.Width, GRHandle.Height) * 0.95
            Dim r2 As Double = Math.Min(GRHandle.Width, GRHandle.Height) * 0.85

#Const DEBUG = 1
#If DEBUG Then
            Call GRHandle.Graphics.DrawPie(Pens.Black, New Rectangle(New Point(Center.X - 5, Center.Y - 5), New Size(10, 10)), 0, 360)
#End If
            For i As Integer = 0 To ObjectModel.GeneObjects.Count - 1
                Dim Gene = ObjectModel.GeneObjects(i)
                Dim size = Gene.Draw(GRHandle.Graphics, Center, r1, r2)
            Next

            Return GRHandle.ImageResource
        End Function
    End Class
End Namespace
