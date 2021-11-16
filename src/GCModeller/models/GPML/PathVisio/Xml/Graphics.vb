#Region "Microsoft.VisualBasic::c1ce27b5411c90acbe535e0c86e339ad, models\GPML\PathVisio\Xml\Graphics.vb"

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

    '     Class Anchor
    ' 
    '         Properties: GraphId, Position, Shape
    ' 
    '     Class Graphics
    ' 
    '         Properties: Anchor, BoardHeight, BoardWidth, CenterX, CenterY
    '                     Color, ConnectorType, FontSize, Height, LineThickness
    '                     Points, Valign, Width, ZOrder
    ' 
    '         Function: ToString
    ' 
    '     Class Label
    ' 
    '         Properties: Graphics, GraphId, TextLabel
    ' 
    '         Function: ToString
    ' 
    '     Class Point
    ' 
    '         Properties: ArrowHead, GraphRef, RelX, RelY, X
    '                     Y
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace GPML

    Public Class Anchor
        <XmlAttribute> Public Property Position As Double
        <XmlAttribute> Public Property Shape As String
        <XmlAttribute> Public Property GraphId As String
    End Class

    Public Class Graphics
        <XmlAttribute> Public Property BoardWidth As Double
        <XmlAttribute> Public Property BoardHeight As Double
        <XmlAttribute> Public Property CenterX As Double
        <XmlAttribute> Public Property CenterY As Double
        <XmlAttribute> Public Property Width As Double
        <XmlAttribute> Public Property Height As Double
        <XmlAttribute> Public Property ZOrder As Integer
        <XmlAttribute> Public Property FontSize As Double
        <XmlAttribute> Public Property Valign As String
        <XmlAttribute> Public Property Color As String
        <XmlAttribute> Public Property ConnectorType As String
        <XmlAttribute> Public Property LineThickness As Double

        <XmlElement("Point")>
        Public Property Points As Point()
        Public Property Anchor As Anchor

        Public Overrides Function ToString() As String
            Return $"[{CenterX},{CenterY},{ZOrder}]"
        End Function

    End Class

    Public Class Label

        <XmlAttribute> Public Property TextLabel As String
        <XmlAttribute> Public Property GraphId As String
        Public Property Graphics As Graphics

        Public Overrides Function ToString() As String
            Return $"[{GraphId}] {TextLabel}"
        End Function

    End Class

    Public Class Point

        <XmlAttribute> Public Property X As Double
        <XmlAttribute> Public Property Y As Double
        <XmlAttribute> Public Property GraphRef As String
        <XmlAttribute> Public Property RelX As Double
        <XmlAttribute> Public Property RelY As Double
        <XmlAttribute> Public Property ArrowHead As String

        Public Overrides Function ToString() As String
            Return $"[{GraphRef}] [X={X}, Y={Y}]"
        End Function

    End Class
End Namespace
