﻿#Region "Microsoft.VisualBasic::b2d5c1568780c72c1872306658c794d1, gr\Microsoft.VisualBasic.Imaging\SVG\CSS\Marker.vb"

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

    '     Class marker
    ' 
    '         Properties: markerHeight, markerWidth, orient, pathList, refX
    '                     refY, viewBox
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Imaging.SVG.XML
Imports htmlNode = Microsoft.VisualBasic.MIME.Markup.HTML.XmlMeta.Node

Namespace SVG.CSS

    Public Class marker : Inherits htmlNode

        <XmlAttribute> Public Property viewBox As String
        <XmlAttribute> Public Property refX As String
        <XmlAttribute> Public Property refY As String
        <XmlAttribute> Public Property markerWidth As String
        <XmlAttribute> Public Property markerHeight As String
        <XmlAttribute> Public Property orient As String

        <XmlElement("path")>
        Public Property pathList As path()

    End Class
End Namespace
