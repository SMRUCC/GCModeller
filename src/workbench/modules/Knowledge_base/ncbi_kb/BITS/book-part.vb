Imports System.Xml.Serialization

Namespace BITS

    <XmlType("book-part")>
    Public Class BookPart

        Public Property body As body

        <XmlElement("book-part-meta")>
        Public Property book_part_meta As bookPartMeta

        Public Overrides Function ToString() As String
            Return body.ToString
        End Function

    End Class

    Public Class bookPartMeta

        <XmlElement("book-part-id")> Public Property book_part_id As bookPartId
        <XmlElement("title-group")> Public Property title_group As TitleGroup
        <XmlElement("related-object")> Public Property related_object As RelatedObject()
        <XmlElement("pub-history")> Public Property pub_history As pubHistory

    End Class

    Public Class pubHistory

        Public Property [date] As [date]

    End Class

    Public Class [date]

        <XmlAttribute("date-type")> Public Property date_type As String

        Public Property day As String
        Public Property month As String
        Public Property year As String

    End Class

    Public Class TitleGroup

        Public Property title As String

    End Class

    Public Class bookPartId

        <XmlAttribute("book-part-id-type")> Public Property book_part_id_type As String

        <XmlText> Public Property id As String

    End Class

End Namespace