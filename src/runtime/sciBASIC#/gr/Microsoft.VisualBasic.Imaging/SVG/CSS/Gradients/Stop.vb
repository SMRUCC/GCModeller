Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.Markup.HTML.XmlMeta

Namespace SVG.CSS

    Public Class [stop] : Inherits Node

        <XmlAttribute("offset")> Public Property offset As String
        <XmlAttribute("stop-color")>
        Public Property stopColor As String
        <XmlAttribute("stop-opacity")>
        Public Property stopOpacity As String

        Public Overrides Function ToString() As String
            Return Me.GetXml
        End Function
    End Class
End Namespace