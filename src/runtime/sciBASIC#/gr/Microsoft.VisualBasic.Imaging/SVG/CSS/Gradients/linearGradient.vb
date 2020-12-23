Imports System.Xml.Serialization

Namespace SVG.CSS

    Public Class linearGradient : Inherits Gradient

        <XmlAttribute> Public Property x1 As String
        <XmlAttribute> Public Property x2 As String
        <XmlAttribute> Public Property y1 As String
        <XmlAttribute> Public Property y2 As String

    End Class
End Namespace