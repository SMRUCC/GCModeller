Imports System.Xml.Serialization

Namespace SBGN

    Public Class arc

        <XmlAttribute> Public Property [class] As String
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property source As String
        <XmlAttribute> Public Property target As String

        Public Property glyph As glyph

        Public Property start As point

        <XmlElement>
        Public Property [next] As point()

        Public Property [end] As point

        Public Overrides Function ToString() As String
            Return $"{source} -> {target}"
        End Function

    End Class

    Public Class point

        <XmlAttribute> Public Property x As Double
        <XmlAttribute> Public Property y As Double

        Public Overrides Function ToString() As String
            Return $"[{x},{y}]"
        End Function

    End Class
End Namespace