#Region "Microsoft.VisualBasic::04be3e8e76f0bddb904ac38f9107d199, GCModeller\engine\IO\Raw\vcXML\XML.vb"

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

    '   Total Lines: 92
    '    Code Lines: 68
    ' Comment Lines: 0
    '   Blank Lines: 24
    '     File Size: 2.68 KB


    '     Class vcXMLFile
    ' 
    '         Properties: index, indexOffset, sha1, vcRun
    ' 
    '     Class parameters
    ' 
    '         Properties: args
    ' 
    '         Function: getCollection, getSize
    ' 
    '     Class vcRun
    ' 
    '         Properties: omics, parameters
    ' 
    '     Class omicsDataEntities
    ' 
    '         Properties: [module], content_type, entities
    ' 
    '     Class frame
    ' 
    '         Properties: [module], frameTime, num, tick, vector
    ' 
    '     Class vector
    ' 
    '         Properties: byteOrder, compressedLen, compressionType, contentType, data
    '                     precision
    ' 
    '     Class index
    ' 
    '         Properties: name, offsets, size
    ' 
    '     Class offset
    ' 
    '         Properties: [module], content_type, id, offset, tick
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace vcXML.XML

    Public Class vcXMLFile
        Public Property vcRun As vcRun
        Public Property index As index
        Public Property indexOffset As Long
        Public Property sha1 As String
    End Class

    Public Class parameters : Inherits ListOf(Of NamedValue)

        <XmlElement("argument")>
        Public Property args As NamedValue()

        Protected Overrides Function getSize() As Integer
            Return args.Length
        End Function

        Protected Overrides Function getCollection() As IEnumerable(Of NamedValue)
            Return args
        End Function
    End Class

    Public Class vcRun

        Public Property parameters As parameters

        <XmlElement>
        Public Property omics As omicsDataEntities()

    End Class

    Public Class omicsDataEntities

        <XmlAttribute>
        Public Property [module] As String
        <XmlAttribute>
        Public Property content_type As String

        <XmlText>
        Public Property entities As String

    End Class

    Public Class frame
        <XmlAttribute> Public Property num As Integer
        <XmlAttribute> Public Property frameTime As Double
        <XmlAttribute> Public Property tick As Integer
        <XmlAttribute> Public Property [module] As String

        Public Property vector As vector
    End Class

    Public Class vector
        <XmlAttribute> Public Property compressionType As String = "zlib"
        <XmlAttribute> Public Property compressedLen As Integer
        <XmlAttribute> Public Property precision As Integer = 64
        <XmlAttribute> Public Property byteOrder As String = "network"
        <XmlAttribute> Public Property contentType As String = "mass_profile"

        <XmlText>
        Public Property data As String
    End Class

    Public Class index

        <XmlAttribute>
        Public Property name As String
        <XmlAttribute>
        Public Property size As Integer
        <XmlElement>
        Public Property offsets As offset()
    End Class

    Public Class offset

        <XmlAttribute> Public Property id As Integer
        <XmlAttribute> Public Property tick As Integer
        <XmlAttribute> Public Property [module] As String
        <XmlAttribute> Public Property content_type As String

        <XmlText>
        Public Property offset As Long

        Public Overrides Function ToString() As String
            Return $"[{id}] Dim {[module]} As {content_type} = &{offset}"
        End Function
    End Class
End Namespace
