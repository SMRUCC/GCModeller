Imports System.IO

Public Module XmlParser

    Public Function ParseXml(xml As String) As XmlElement
        Dim doc As XDocument = XDocument.Load(New StringReader(xml))

    End Function
End Module