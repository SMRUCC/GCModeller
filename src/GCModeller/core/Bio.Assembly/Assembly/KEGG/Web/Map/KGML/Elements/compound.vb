Imports System.Xml.Serialization

Namespace Assembly.KEGG.WebServices.KGML

    Public Class compound

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property name As String

        Public Overrides Function ToString() As String
            Return name
        End Function
    End Class
End Namespace