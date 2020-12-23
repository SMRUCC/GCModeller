Imports System.Xml.Serialization

Namespace SVG.CSS

    Public Class radialGradient : Inherits Gradient

        <XmlAttribute> Public Property cx As String
        <XmlAttribute> Public Property cy As String
        <XmlAttribute> Public Property r As String
        <XmlAttribute> Public Property fx As String
        <XmlAttribute> Public Property fy As String
        <XmlAttribute> Public Property spreadMethod As String
        <XmlAttribute> Public Property gradientUnits As String

    End Class
End Namespace