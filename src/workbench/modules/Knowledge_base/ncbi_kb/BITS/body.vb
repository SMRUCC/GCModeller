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

        <XmlElement> Public Property list As list_data()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetContentText() As String
            Return p.Select(Function(pi) pi.text.JoinBy(" ")).JoinBy(vbCrLf & vbCrLf)
        End Function

        Public Overrides Function ToString() As String
            Return title.ToString
        End Function

    End Class

    Public Class list_data

        <XmlAttribute("list-type")> Public Property list_type As String

        <XmlElement("list-item")> Public Property list_item As list_item()

    End Class

    Public Class list_item

        Public Property label As String
        <XmlElement("p")> Public Property p As Paragraph()

    End Class

End Namespace