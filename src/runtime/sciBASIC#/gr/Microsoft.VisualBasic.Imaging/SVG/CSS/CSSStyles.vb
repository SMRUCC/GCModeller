Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.Markup.HTML
Imports Microsoft.VisualBasic.MIME.Markup.HTML.XmlMeta

Namespace SVG.CSS

    ''' <summary>
    ''' 在这个SVG对象之中所定义的CSS样式数据
    ''' </summary>
    Public Class CSSStyles : Inherits Node

        <XmlElement("linearGradient")>
        Public Property linearGradients As linearGradient()
        <XmlElement("radialGradient")>
        Public Property radialGradients As radialGradient()
        <XmlElement("style")>
        Public Property styles As XmlMeta.CSS()
        <XmlElement("filter")>
        Public Property filters As Filter()

    End Class
End Namespace