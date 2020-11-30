Imports System.Xml.Serialization

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

Public Class Interaction

    <XmlAttribute>
    Public Property GraphId As String
    Public Property BiopaxRef As String
    Public Property Graphics As Graphics
    Public Property Xref As Xref

End Class

Public Class Anchor
    <XmlAttribute> Public Property Position As Double
    <XmlAttribute> Public Property Shape As String
    <XmlAttribute> Public Property GraphId As String
End Class

Public Class Graphics
    <XmlAttribute> Public Property BoardWidth As Double
    <XmlAttribute> Public Property BoardHeight As Double
    <XmlAttribute> Public Property CenterX As Double
    <XmlAttribute> Public Property CenterY As Double
    <XmlAttribute> Public Property Width As Double
    <XmlAttribute> Public Property Height As Double
    <XmlAttribute> Public Property ZOrder As Integer
    <XmlAttribute> Public Property FontSize As Double
    <XmlAttribute> Public Property Valign As String
    <XmlAttribute> Public Property Color As String
    <XmlAttribute> Public Property ConnectorType As String
    <XmlAttribute> Public Property LineThickness As Double

    <XmlElement>
    Public Property Points As Point()
    Public Property Anchor As Anchor
End Class

Public Class Label
    <XmlAttribute> Public Property TextLabel As String
    <XmlAttribute> Public Property GraphId As String
    Public Property Graphics As Graphics
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

Public Class Point

    <XmlAttribute> Public Property X As Double
    <XmlAttribute> Public Property Y As Double
    <XmlAttribute> Public Property GraphRef As String
    <XmlAttribute> Public Property RelX As Double
    <XmlAttribute> Public Property RelY As Double
    <XmlAttribute> Public Property ArrowHead As String

End Class

Public Class DataNode

    <XmlAttribute> Public Property TextLabel As String
    <XmlAttribute> Public Property GraphId As String
    <XmlAttribute> Public Property Type As String
    <XmlAttribute> Public Property GroupRef As String

    Public Property Comment As Comment
    Public Property Graphics As Graphics
    Public Property Xref As Xref

End Class

Public Class Xref

    <XmlAttribute> Public Property Database As String
    <XmlAttribute> Public Property ID As String

End Class