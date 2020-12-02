
Imports System.Xml.Serialization

Namespace GPML

    Public Class DataNode

        <XmlAttribute> Public Property TextLabel As String
        <XmlAttribute> Public Property GraphId As String
        <XmlAttribute> Public Property Type As String
        <XmlAttribute> Public Property GroupRef As String

        Public Property Comment As Comment
        Public Property Graphics As Graphics
        Public Property Xref As Xref

    End Class

    Public Class Interaction

        <XmlAttribute>
        Public Property GraphId As String
        Public Property BiopaxRef As String
        Public Property Graphics As Graphics
        Public Property Xref As Xref

    End Class
End Namespace