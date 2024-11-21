Imports System.Xml.Serialization

Namespace BITS
    Public Class Paragraph

        <XmlText> Public Property text As String()

        <XmlElement("related-object")>
        Public Property related_object As RelatedObject()

        <XmlElement> Public Property bold As Bold()

        <XmlElement("ext-link")> Public Property links As ExtLink()
        <XmlElement("italic")> Public Property italic As Italic()

        <XmlElement> Public Property xref As xref()

        Public Function GetTextContent() As String
            Return text.JoinBy(" ")
        End Function

        Public Overrides Function ToString() As String
            Return text.JoinBy(" ")
        End Function

    End Class

    Public Class xref

        <XmlAttribute("ref-type")> Public Property ref_type As String
        <XmlAttribute> Public Property rid As String
        <XmlText> Public Property text As String

    End Class

    Public Class Italic : Inherits Paragraph

        <XmlAttribute> Public Property toggle As String

        Public Overrides Function ToString() As String
            Return GetTextContent()
        End Function

    End Class

    Public Class Bold : Inherits Paragraph

        Public Overrides Function ToString() As String
            Return GetTextContent()
        End Function

    End Class

    <XmlType("ext-link")>
    Public Class ExtLink

        <XmlAttribute("ext-link-type")> Public Property ext_link_type As String
        <XmlAttribute("href", [Namespace]:=xlink)> Public Property href As String

        Public Const xlink As String = "http://www.w3.org/1999/xlink"

        <XmlText> Public Property text As String

        Public Overrides Function ToString() As String
            Return $"[{text}]({href})"
        End Function
    End Class

    <XmlType("related-object")>
    Public Class RelatedObject

        <XmlAttribute("link-type")> Public Property link_type As String
        <XmlAttribute("source-id")> Public Property source_id As String
        <XmlAttribute("document-id")> Public Property document_id As String
        <XmlAttribute("document-type")> Public Property document_type As String

        <XmlText> Public Property text As String

        Public Overrides Function ToString() As String
            Return source_id
        End Function

    End Class
End Namespace