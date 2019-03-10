﻿#Region "Microsoft.VisualBasic::b5dae829fce6cebbdd48028b63d3b194, gr\Microsoft.VisualBasic.Imaging\Drawing3D\Models\Line.vb"

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

    '     Structure Line3D
    ' 
    '         Function: Copy, GetEnumerator, IEnumerable_GetEnumerator
    ' 
    '         Sub: Draw
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging.Drawing3D.Math3D

Namespace Drawing3D.Models

    Public Structure Line3D
        Implements IEnumerable(Of Point3D)
        Implements I3DModel

        Public a, b As Point3D
        Public pen As Pen

        Public Sub Draw(ByRef canvas As Graphics, camera As Camera) Implements I3DModel.Draw
            Dim pts As Point() = camera.Project(Me).Select(Function(pt) pt.PointXY(camera.screen)).ToArray
            Call canvas.DrawLine(pen, pts(0), pts(1))
        End Sub

        Public Function Copy(data As IEnumerable(Of Point3D)) As I3DModel Implements I3DModel.Copy
            Dim array As Point3D() = data.ToArray
            Return New Line3D With {
                .a = data(0),
                .b = data(1),
                .pen = pen
            }
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of Point3D) Implements IEnumerable(Of Point3D).GetEnumerator
            Yield a
            Yield b
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Structure
End Namespace
