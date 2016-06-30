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