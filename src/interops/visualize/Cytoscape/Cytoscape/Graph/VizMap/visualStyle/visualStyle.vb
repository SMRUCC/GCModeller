Imports System.Xml.Serialization

Namespace Visualization

    Public Class visualStyle

        <XmlAttribute("name")> Public Property Name As String
        <XmlElement("network")> Public Property Network As network
        <XmlElement("node")> Public Property Node As node
        <XmlElement("edge")> Public Property Edge As edge

        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class
End Namespace