﻿#Region "Microsoft.VisualBasic::1a5bd5afa4215fb60773ba10f8bb68be, gr\Microsoft.VisualBasic.Imaging\PostScript\PSElements\ImageData.vb"

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

    '   Total Lines: 54
    '    Code Lines: 34 (62.96%)
    ' Comment Lines: 12 (22.22%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 8 (14.81%)
    '     File Size: 1.86 KB


    '     Class ImageData
    ' 
    '         Properties: image, location, scale, size
    ' 
    '         Function: GetSize, GetXy, ScaleTo
    ' 
    '         Sub: Paint, WriteAscii
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Net.Http

Namespace PostScript.Elements

    Public Class ImageData : Inherits PSElement

        ''' <summary>
        ''' image data is encoded as base64 data uri
        ''' </summary>
        ''' <returns></returns>
        Public Property image As DataURI
        ''' <summary>
        ''' the image dimension size
        ''' </summary>
        ''' <returns></returns>
        Public Property size As Size
        ''' <summary>
        ''' the image drawing size
        ''' </summary>
        ''' <returns></returns>
        Public Property scale As SizeF
        Public Property location As PointF

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Friend Overrides Sub WriteAscii(ps As Writer)
            Call ps.image(image, location.X, location.Y, size.Width, size.Height, scale.Width, scale.Height)
        End Sub

        Friend Overrides Sub Paint(g As IGraphics)
            Call g.DrawImage(DriverLoad.LoadFromStream(image.ToStream), location.X, location.Y, scale.Width, scale.Height)
        End Sub

        Friend Overrides Function ScaleTo(scaleX As d3js.scale.LinearScale, scaleY As d3js.scale.LinearScale) As PSElement
            Return New ImageData With {
                .image = image,
                .location = New PointF(scaleX(location.X), scaleY(location.Y)),
                .scale = scale,
                .size = size,
                .comment = comment
            }
        End Function

        Friend Overrides Function GetXy() As PointF
            Return location
        End Function

        Friend Overrides Function GetSize() As SizeF
            Return scale
        End Function
    End Class
End Namespace
