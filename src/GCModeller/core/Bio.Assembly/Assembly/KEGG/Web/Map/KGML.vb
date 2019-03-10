﻿#Region "Microsoft.VisualBasic::55a783e83d902bacb8403d03e4abe71c, Bio.Assembly\Assembly\KEGG\Web\Map\KGML.vb"

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

    '     Class pathway
    ' 
    '         Properties: image, items, link, name, number
    '                     org, reactions, relations, title
    ' 
    '         Function: ResourceURL, ToString
    ' 
    '     Class Link
    ' 
    '         Properties: entry1, entry2, type
    ' 
    '     Class relation
    ' 
    '         Properties: subtype
    ' 
    '     Class subtype
    ' 
    '         Properties: name, value
    ' 
    '     Class compound
    ' 
    '         Properties: id, name
    ' 
    '         Function: ToString
    ' 
    '     Class reaction
    ' 
    '         Properties: products, substrates
    ' 
    '         Function: ToString
    ' 
    '     Class entry
    ' 
    '         Properties: graphics, id, link, name, type
    ' 
    '         Function: ToString
    ' 
    '     Class graphics
    ' 
    '         Properties: bgcolor, fgcolor, height, name, type
    '                     width, x, y
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.KEGG.WebServices.KGML

    ''' <summary>
    ''' + pathway map: http://www.kegg.jp/kegg-bin/download?entry=xcb00280&amp;format=kgml
    ''' </summary>
    Public Class pathway

        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property org As String
        <XmlAttribute> Public Property number As String
        <XmlAttribute> Public Property title As String
        <XmlAttribute> Public Property image As String
        <XmlAttribute> Public Property link As String

#Region "pathway network"
        <XmlElement(NameOf(entry))> Public Property items As entry()
        <XmlElement(NameOf(relation))> Public Property relations As relation()
        <XmlElement(NameOf(reaction))> Public Property reactions As reaction()
#End Region

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ResourceURL(entry As String) As String
            Return $"http://www.kegg.jp/kegg-bin/download?entry={entry}&format=kgml"
        End Function

        Public Overrides Function ToString() As String
            Return $"[{name}] {title}"
        End Function
    End Class

    ''' <summary>
    ''' Network edges
    ''' </summary>
    Public Class Link

        <XmlAttribute> Public Property entry1 As String
        <XmlAttribute> Public Property entry2 As String
        <XmlAttribute> Public Property type As String

    End Class

    Public Class relation : Inherits Link
        Public Property subtype As subtype
    End Class

    Public Class subtype
        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property value As String
    End Class

    Public Class compound
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property name As String

        Public Overrides Function ToString() As String
            Return name
        End Function
    End Class

    Public Class reaction : Inherits Link
        <XmlElement("substrate")>
        Public Property substrates As compound()
        <XmlElement("product")>
        Public Property products As compound()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    ''' <summary>
    ''' Network nodes
    ''' </summary>
    Public Class entry

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property type As String
        <XmlAttribute> Public Property link As String

        Public Property graphics As graphics

        Public Overrides Function ToString() As String
            Return name
        End Function
    End Class

    Public Class graphics

        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property fgcolor As String
        <XmlAttribute> Public Property bgcolor As String
        <XmlAttribute> Public Property type As String
        <XmlAttribute> Public Property x As String
        <XmlAttribute> Public Property y As String
        <XmlAttribute> Public Property width As String
        <XmlAttribute> Public Property height As String

        Public Overrides Function ToString() As String
            Return name
        End Function
    End Class
End Namespace
