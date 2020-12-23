#Region "Microsoft.VisualBasic::5b632fc0f4ec6442333b80c67163c3cc, gr\Microsoft.VisualBasic.Imaging\SVG\CSS\Gradients.vb"

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

    '     Class [stop]
    ' 
    '         Properties: offset, stopColor, stopOpacity
    ' 
    '         Function: ToString
    ' 
    '     Class Gradient
    ' 
    '         Properties: stops
    ' 
    '     Class linearGradient
    ' 
    '         Properties: x1, x2, y1, y2
    ' 
    '     Class radialGradient
    ' 
    '         Properties: cx, cy, fx, fy, gradientUnits
    '                     r, spreadMethod
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.Markup.HTML.XmlMeta

Namespace SVG.CSS

    ' https://developer.mozilla.org/en-US/docs/Web/SVG/Tutorial/Gradients

    Public Class Gradient : Inherits Node

        <XmlElement("stop")>
        Public Property stops As [stop]()

    End Class
End Namespace
