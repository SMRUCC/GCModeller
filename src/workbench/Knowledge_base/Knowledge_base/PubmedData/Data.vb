Imports System.Xml.Serialization

Public Class History

    <XmlElement("PubMedPubDate")> Public Property PubMedPubDate As PubDate()
End Class

Public Class ArticleId
    <XmlAttribute>
    Public Property IdType As String
    <XmlText>
    Public Property ID As String

    Public Overrides Function ToString() As String
        Return $"{IdType}: {ID}"
    End Function
End Class