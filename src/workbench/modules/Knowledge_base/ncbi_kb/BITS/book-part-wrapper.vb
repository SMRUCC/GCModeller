Imports System.Xml.Serialization

Namespace BITS

    <XmlType("book-part-wrapper")>
    Public Class BookPartWrapper

        <XmlAttribute("id")> Public Property id As String
        <XmlAttribute("content-type")> Public Property content_type As String
        <XmlAttribute("from-where")> Public Property from_where As String
        <XmlAttribute("dtd-version")> Public Property dtd_version As String

        <XmlElement("book-part")> Public Property book_part As BookPart

        Public Overrides Function ToString() As String
            Return id
        End Function

    End Class
End Namespace