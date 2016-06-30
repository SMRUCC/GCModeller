Imports System.Xml.Serialization

<XmlType("rss")>
Public Class RSSReader

    Public Property version As String
    Public Property channel As Channel

End Class

Public Class Channel
    Public Property title As String
    Public Property link As String
    Public Property description As String
    Public Property lastBuildDate As String
    Public Property language As String
    Public Property image As Image
    <XmlElement("item")> Public Property items As Item()
End Class

Public Class Image
    Public Property url As String
    Public Property title As String
    Public Property link As String
    Public Property width As Integer
    Public Property height As Integer
End Class

Public Class Guid
    <XmlAttribute> Public Property isPermaLink As Boolean
    <XmlText> Public Property link As String
End Class

Public Class Item
    Public Property title As String
    Public Property link As String
    Public Property comments As String
    Public Property pubDate As String
    Public Property dc_creator As String
    <XmlElement> Public Property category As Category()
    Public Property guid As Guid
    Public Property description As String
    Public Property content As String
    Public Property wfw_commentRss As String
    Public Property slash_comments As Integer
End Class

Public Class Category
    <XmlText> Public Property value As String
End Class