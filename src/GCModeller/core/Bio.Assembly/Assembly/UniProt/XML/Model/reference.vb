Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.Uniprot.XML

    Public Class reference

        <XmlAttribute>
        Public Property key As String
        Public Property citation As citation
        <XmlElement>
        Public Property scope As String()
        Public Property source As source

    End Class

    Public Class citation
        Public Property type As String
        Public Property [date] As String
        Public Property name As String
        Public Property volume As String
        Public Property first As Integer
        Public Property last As Integer
        Public Property title As String
        Public Property authorList As person()
        <XmlElement("dbReference")>
        Public Property dbReferences As dbReference()

        Public Overrides Function ToString() As String
            Return title
        End Function
    End Class

    Public Class person
        <XmlAttribute> Public Property name As String

        Public Overrides Function ToString() As String
            Return name
        End Function
    End Class

    Public Class source

        <XmlElement("tissue")>
        Public Property tissues As String()

        Public Overrides Function ToString() As String
            Return tissues.GetJson
        End Function
    End Class
End Namespace