Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization

Namespace BITS

    Public Class body

        <XmlElement("sec")> Public Property sections As section()

    End Class

    <XmlType("sec")>
    Public Class section

        <XmlAttribute("id")> Public Property id As String
        <XmlAttribute("sec-type")> Public Property sec_type As String

        Public Property title As String

        <XmlElement("p")>
        Public Property p As Paragraph()

        <XmlElement("sec")> Public Property sections As section()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetContentText() As String
            Return p.Select(Function(pi) pi.text).JoinBy(vbCrLf & vbCrLf)
        End Function

    End Class

    Public Class Paragraph

        <XmlText> Public Property text As String

        <XmlElement("related-object")>
        Public Property related_object As RelatedObject()

    End Class

    <XmlType("related-object")>
    Public Class RelatedObject

        <XmlAttribute("link-type")> Public Property link_type As String
        <XmlAttribute("source-id")> Public Property source_id As String
        <XmlAttribute("document-id")> Public Property document_id As String
        <XmlAttribute("document-type")> Public Property document_type As String

    End Class
End Namespace