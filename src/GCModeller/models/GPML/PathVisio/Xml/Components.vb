Imports System.Xml.Serialization

Namespace GPML

    Public Class InfoBox
        <XmlAttribute> Public Property CenterX  As  Double
        <XmlAttribute> Public Property CenterY As Double
    End Class

    Public Class Comment

        <XmlAttribute>
        Public Property Source As String

        <XmlText>
        Public Property Text As String

    End Class

    Public Class PublicationXref

        <XmlAttribute>
        Public Property id As String
    End Class

    Public Class Group

        <XmlAttribute> Public Property GroupId As String
        <XmlAttribute> Public Property GraphId As String
        <XmlAttribute> Public Property Style As String

    End Class

    Public Class Xref

        <XmlAttribute> Public Property Database As String
        <XmlAttribute> Public Property ID As String

    End Class
End Namespace