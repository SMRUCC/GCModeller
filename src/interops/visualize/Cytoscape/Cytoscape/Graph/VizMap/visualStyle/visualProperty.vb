#Region "Microsoft.VisualBasic::6ea1a62b6f070ce2736029e8641216ce, visualize\Cytoscape\Cytoscape\Graph\VizMap\visualStyle\visualProperty.vb"

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

    '     Class visualProperty
    ' 
    '         Properties: [Default], continuousMapping, discreteMapping, name, passthroughMapping
    ' 
    '     Class dependency
    ' 
    '         Properties: name, value
    ' 
    '     Class passthroughMapping
    ' 
    '         Properties: attributeName, attributeType
    ' 
    '     Class discreteMapping
    ' 
    '         Properties: attributeName, attributeType, discreteMappingEntrys
    ' 
    '     Class discreteMappingEntry
    ' 
    '         Properties: attributeValue, value
    ' 
    '     Class continuousMapping
    ' 
    '         Properties: attributeName, attributeType, continuousMappingPoints
    ' 
    '     Class continuousMappingPoint
    ' 
    '         Properties: attrValue, equalValue, greaterValue, lesserValue
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace Visualization.visualProperty

    Public Class visualProperty
        <XmlAttribute("name")> Public Property name As String
        <XmlAttribute("default")> Public Property [Default] As String
        Public Property discreteMapping As discreteMapping
        Public Property passthroughMapping As passthroughMapping
        Public Property continuousMapping As continuousMapping
    End Class

    Public Class dependency
        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property value As String
    End Class

    Public Class passthroughMapping
        <XmlAttribute("attributeType")> Public Property attributeType As String = "string"
        <XmlAttribute("attributeName")> Public Property attributeName As String
    End Class

    Public Class discreteMapping
        <XmlAttribute> Public Property attributeType As String = "string"
        <XmlAttribute> Public Property attributeName As String
        <XmlElement("discreteMappingEntry")> Public Property discreteMappingEntrys As discreteMappingEntry()
    End Class

    Public Class discreteMappingEntry
        Public Property value As String
        Public Property attributeValue As String
    End Class

    Public Class continuousMapping
        <XmlAttribute> Public Property attributeType As String
        <XmlAttribute> Public Property attributeName As String
        <XmlElement("continuousMappingPoint")> Public Property continuousMappingPoints As continuousMappingPoint()
    End Class

    Public Class continuousMappingPoint
        Public Property lesserValue As String
        Public Property greaterValue As String
        Public Property equalValue As String
        Public Property attrValue As String
    End Class
End Namespace
