Public Class XmlElement

    Public Property name As String
    Public Property [namespace] As String
    Public Property attributes As Dictionary(Of String, String)
    Public Property elements As XmlElement()
    Public Property text As String

    Public Overrides Function ToString() As String
        Return $"{[namespace]}::{name}"
    End Function

    Public Shared Function ParseXmlText(xmlText As String) As XmlElement
        Return XmlParser.ParseXml(xmlText)
    End Function

End Class
