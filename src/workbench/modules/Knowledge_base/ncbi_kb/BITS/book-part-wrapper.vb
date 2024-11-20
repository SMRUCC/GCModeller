Imports System.Xml.Serialization

Namespace BITS

    <XmlType("book-part-wrapper")>
    Public Class BookPartWrapper

        <XmlAttribute("id")> Public Property id As String
        <XmlAttribute("content-type")> Public Property content_type As String
        <XmlAttribute("from-where")> Public Property from_where As String
        <XmlAttribute("dtd-version")> Public Property dtd_version As String

    End Class
End Namespace