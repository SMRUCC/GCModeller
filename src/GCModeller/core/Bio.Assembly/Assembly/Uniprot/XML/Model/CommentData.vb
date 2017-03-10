Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.Uniprot.XML

    Public Class comment
        <XmlAttribute> Public Property type As String
        <XmlAttribute> Public Property evidence As String
        Public Property text As value
        <XmlElement("subcellularLocation")>
        Public Property subcellularLocations As subcellularLocation()
        Public Property [event] As value
        <XmlElement("isoform")>
        Public Property isoforms As isoform()
    End Class

    Public Class subcellularLocation

        <XmlElement("location")> Public Property locations As String()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class isoform
        Public Property id As String
        Public Property name As String
        Public Property sequence As value
        Public Property text As value
    End Class

    Public Class disease

        <XmlAttribute>
        Public Property id As String
        Public Property name As String
        Public Property acronym As String
        Public Property description As String
        Public Property dbReference As value

    End Class
End Namespace