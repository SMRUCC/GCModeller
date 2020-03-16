Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace vcXML.XML

    Public Class vcXMLFile
        Public Property vcRun As vcRun
        Public Property index As index
        Public Property indexOffset As Long
        Public Property sha1 As String
    End Class

    Public Class parameters : Inherits ListOf(Of NamedValue)

        <XmlElement("argument")>
        Public Property args As NamedValue()

        Protected Overrides Function getSize() As Integer
            Return args.Length
        End Function

        Protected Overrides Function getCollection() As IEnumerable(Of NamedValue)
            Return args
        End Function
    End Class

    Public Class vcRun

        Public Property parameters As parameters

        <XmlElement>
        Public Property omics As omicsDataEntities()

    End Class

    Public Class omicsDataEntities

        <XmlAttribute>
        Public Property [module] As String
        <XmlAttribute>
        Public Property content_type As String

        <XmlText>
        Public Property entities As String

    End Class

    Public Class frame
        <XmlAttribute> Public Property num As Integer
        <XmlAttribute> Public Property frameTime As Double
        <XmlAttribute> Public Property tick As Integer
        <XmlAttribute> Public Property [module] As String

        Public Property vector As vector
    End Class

    Public Class vector
        <XmlAttribute> Public Property compressionType As String = "zlib"
        <XmlAttribute> Public Property compressedLen As Integer
        <XmlAttribute> Public Property precision As Integer = 64
        <XmlAttribute> Public Property byteOrder As String = "network"
        <XmlAttribute> Public Property contentType As String = "mass_profile"

        <XmlText>
        Public Property data As String
    End Class

    Public Class index

        <XmlAttribute>
        Public Property name As String
        <XmlAttribute>
        Public Property size As Integer
        <XmlElement>
        Public Property offsets As offset()
    End Class

    Public Class offset

        <XmlAttribute> Public Property id As Integer
        <XmlAttribute> Public Property tick As Integer
        <XmlAttribute> Public Property [module] As String
        <XmlAttribute> Public Property content_type As String

        <XmlText>
        Public Property offset As Long

        Public Overrides Function ToString() As String
            Return $"[{id}] Dim {[module]} As {content_type} = &{offset}"
        End Function
    End Class
End Namespace