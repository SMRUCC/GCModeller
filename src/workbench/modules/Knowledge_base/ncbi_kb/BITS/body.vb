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

        Public Property title As Paragraph

        <XmlElement("p")>
        Public Property p As Paragraph()

        <XmlElement("table-wrap")> Public Property table_wrap As TableWrap

        <XmlElement("sec")> Public Property sections As section()

        <XmlElement("ref-list")> Public Property ref_list As RefList

        <XmlElement> Public Property list As list()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetContentText() As String
            Return p.Select(Function(pi) pi.text).JoinBy(vbCrLf & vbCrLf)
        End Function

        Public Overrides Function ToString() As String
            Return title.ToString
        End Function

    End Class

    Public Class list

        <XmlAttribute("list-type")> Public Property list_type As String

        <XmlElement("list-item")> Public Property list_item As list_item()

    End Class

    Public Class list_item

        Public Property label As String
        <XmlElement("p")> Public Property p As Paragraph()

    End Class

    Public Class RefList

        <XmlAttribute("id")> Public Property id As String
        <XmlElement> Public Property ref As ref()

    End Class

    Public Class ref

        <XmlAttribute> Public Property id As String

        <XmlElement("mixed-citation")>
        Public Property mixed_citation As MixedCitation

        <XmlElement("element-citation")>
        Public Property element_citation As MixedCitation

        Public Overrides Function ToString() As String
            Return id
        End Function

    End Class

    Public Class personGroup

        <XmlElement("name")> Public Property name As StringName()

    End Class

    Public Class MixedCitation : Inherits Paragraph

        <XmlAttribute("publication-type")> Public Property publication_type As String
        <XmlElement> Public Property annotation As Annotation()
        <XmlElement("string-name")> Public Property string_names As StringName()
        <XmlElement("person-group")> Public Property person_group As personGroup
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

End Namespace