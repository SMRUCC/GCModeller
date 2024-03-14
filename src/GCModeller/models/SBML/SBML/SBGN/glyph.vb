Imports System.Xml.Serialization

Namespace SBGN

    Public Class glyph

        <XmlAttribute> Public Property [class] As String
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property compartmentOrder As String

        Public Property label As label
        Public Property bbox As bbox

        <XmlElement>
        Public Property port As port()

    End Class

    Public Class port

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property x As Double
        <XmlAttribute> Public Property y As Double

    End Class

    Public Class label

        <XmlAttribute>
        Public Property text As String
        Public Property bbox As bbox

    End Class

    Public Class bbox

        <XmlAttribute> Public Property w As Double
        <XmlAttribute> Public Property h As Double
        <XmlAttribute> Public Property x As Double
        <XmlAttribute> Public Property y As Double

    End Class
End Namespace