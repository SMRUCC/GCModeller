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

        <XmlElement("table-wrap")> Public Property table_wrap As TableWrap

        <XmlElement("sec")> Public Property sections As section()

        <XmlElement("ref-list")> Public Property ref_list As RefList

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetContentText() As String
            Return p.Select(Function(pi) pi.text).JoinBy(vbCrLf & vbCrLf)
        End Function

    End Class

    Public Class RefList

        <XmlAttribute("id")> Public Property id As String
        <XmlElement> Public Property ref As ref()

    End Class

    Public Class ref

        <XmlAttribute> Public Property id As String

        Public Property mixed_citation As MixedCitation

    End Class

    Public Class MixedCitation : Inherits Paragraph

        <XmlAttribute("publication-type")> Public Property publication_type As String
        <XmlElement> Public Property annotation As Annotation()

    End Class

    Public Class Annotation

        <XmlElement> Public Property p As Paragraph()

    End Class

    Public Class TableWrap

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property orientation As String
        <XmlAttribute> Public Property position As String

    End Class

    Public Class Table

        <XmlAttribute> Public Property frame As String
        <XmlAttribute> Public Property rules As String

        Public Property thead As THead
        Public Property tbody As tbody

    End Class

    Public Class TBody

        <XmlElement("tr")> Public Property tr As BodyRow()

    End Class

    Public Class THead

        Public Property tr As HeaderRow

    End Class

    Public Class HeaderRow

        <XmlElement("th")> Public Property header_cells As Cell()

    End Class

    Public Class BodyRow

        <XmlElement("td")> Public Property row_cells As Cell()

    End Class

    Public Class Cell

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property valign As String
        <XmlAttribute> Public Property align As String
        <XmlAttribute> Public Property scope As String
        <XmlAttribute> Public Property rowspan As String
        <XmlAttribute> Public Property colspan As String
        <XmlAttribute> Public Property headers As String
        <XmlText> Public Property text As Paragraph

    End Class

    Public Class Paragraph

        <XmlText> Public Property text As String

        <XmlElement("related-object")>
        Public Property related_object As RelatedObject()

        <XmlElement> Public Property bold As String()

        <XmlElement("ext-link")> Public Property links As ExtLink()
        <XmlElement("italic")> Public Property italic As Italic()

    End Class

    Public Class Italic

        <XmlAttribute> Public Property toggle As String
        <XmlText> Public Property text As String

    End Class

    <XmlType("ext-link", [Namespace]:=ExtLink.xlink)>
    Public Class ExtLink

        <XmlAttribute("ext-link-type")> Public Property ext_link_type As String
        <XmlAttribute("href", [Namespace]:=xlink)> Public Property href As String

        Public Const xlink As String = "http://www.w3.org/1999/xlink"

        <XmlText> Public Property text As String

    End Class

    <XmlType("related-object")>
    Public Class RelatedObject

        <XmlAttribute("link-type")> Public Property link_type As String
        <XmlAttribute("source-id")> Public Property source_id As String
        <XmlAttribute("document-id")> Public Property document_id As String
        <XmlAttribute("document-type")> Public Property document_type As String

    End Class
End Namespace