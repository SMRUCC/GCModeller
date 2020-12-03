Imports System.Xml.Serialization

Namespace GPML

    Public Class Pathway

        <XmlAttribute> Public Property Name As String
        <XmlAttribute> Public Property Version As String
        <XmlAttribute> Public Property Organism As String

        Public Property Comment As Comment

        <XmlElement>
        Public Property BiopaxRef As String()

    End Class

    Public Class Comment

        <XmlAttribute> Public Property Source As String
        <XmlText> Public Property Text As String

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