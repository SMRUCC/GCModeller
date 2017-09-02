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
        <XmlAttribute> Public Property type As String
        <XmlAttribute> Public Property [date] As String
        <XmlAttribute> Public Property db As String
        ''' <summary>
        ''' 期刊名称
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property volume As String
        <XmlAttribute> Public Property first As String
        <XmlAttribute> Public Property last As String

        ''' <summary>
        ''' ``submission``类型的引用的标题可能是空的
        ''' </summary>
        ''' <returns></returns>
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