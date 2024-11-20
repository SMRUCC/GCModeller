Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace BITS

    Public Class body

        <XmlElement("sec")> Public Property sections As section()

        Public Overrides Function ToString() As String
            Return sections.Select(Function(sec) sec.ToString).GetJson
        End Function

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

        Public Overrides Function ToString() As String
            Return title
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
        <XmlElement("string-name")> Public Property string_names As StringName()
        Public Property etal As String
        <XmlElement("article-title")> Public Property title As String
        Public Property source As String
        Public Property year As String
        Public Property volume As String
        Public Property fpage As String
        Public Property lpage As String
        <XmlElement("pub-id")> Public Property pub_id As PubId

        Public Property collab As String
        Public Property issue As String

    End Class

    Public Class PubId

        <XmlAttribute("pub-id-type")>
        Public Property pub_id_type As String

        <XmlText> Public Property id As String

        Public Overrides Function ToString() As String
            Return id
        End Function

    End Class

    <XmlType("string-name")>
    Public Class StringName

        <XmlAttribute("name-style")> Public Property name_style As String
        Public Property surname As String
        <XmlElement("given-names")> Public Property given_names As String

        Public Overrides Function ToString() As String
            Return surname & " " & given_names
        End Function

    End Class

    Public Class Annotation

        <XmlElement> Public Property p As Paragraph()

    End Class

    Public Class TableWrap

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property orientation As String
        <XmlAttribute> Public Property position As String

        Public Property table As Table

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

        Public Overrides Function ToString() As String
            Return tr.ToString
        End Function

    End Class

    Public Class HeaderRow

        <XmlElement("th")> Public Property header_cells As Cell()

        Public Overrides Function ToString() As String
            Return header_cells.Select(Function(th) th.ToString).GetJson
        End Function

    End Class

    Public Class BodyRow

        <XmlElement("td")> Public Property row_cells As Cell()

        Public Overrides Function ToString() As String
            Return row_cells.Select(Function(td) td.ToString).GetJson
        End Function

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

        Public Overrides Function ToString() As String
            Return text.ToString
        End Function

    End Class

    Public Class Paragraph

        <XmlText> Public Property text As String

        <XmlElement("related-object")>
        Public Property related_object As RelatedObject()

        <XmlElement> Public Property bold As String()

        <XmlElement("ext-link")> Public Property links As ExtLink()
        <XmlElement("italic")> Public Property italic As Italic()

        Public Overrides Function ToString() As String
            Return text
        End Function

    End Class

    Public Class Italic

        <XmlAttribute> Public Property toggle As String
        <XmlText> Public Property text As String

        Public Overrides Function ToString() As String
            Return text
        End Function

    End Class

    <XmlType("ext-link", [Namespace]:=ExtLink.xlink)>
    Public Class ExtLink

        <XmlAttribute("ext-link-type")> Public Property ext_link_type As String
        <XmlAttribute("href", [Namespace]:=xlink)> Public Property href As String

        Public Const xlink As String = "http://www.w3.org/1999/xlink"

        <XmlText> Public Property text As String

        <XmlNamespaceDeclarations()>
        Public xmlns As New XmlSerializerNamespaces

        Sub New()
            xmlns.Add("xlink", xlink)
        End Sub

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

        Public Overrides Function ToString() As String
            Return source_id
        End Function

    End Class
End Namespace