Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace StringDB.MIF25.XmlCommon

    Public MustInherit Class DataItem
        <XmlAttribute("id")> Public Overridable Property Id As Integer

        Public Overrides Function ToString() As String
            Return Id
        End Function
    End Class

    Public Class Names
        <XmlElement("shortLabel")> Public Property ShortLabel As String
        <XmlElement("fullName")> Public Property FullName As String

        Public Overrides Function ToString() As String
            Return String.Format("({0}) {1}", ShortLabel, FullName)
        End Function
    End Class

    Public Class Xref
        <XmlElement("primaryRef")> Public Property PrimaryReference As Reference
        <XmlElement("secondaryRef")> Public Property SecondaryReference As Reference()

        Public Overrides Function ToString() As String
            Return PrimaryReference.ToString
        End Function
    End Class

    Public Class Reference
        <XmlAttribute("db")> Public Property Db As String
        <XmlAttribute("id")> Public Property Id As String
        <XmlAttribute("dbAc")> Public Property dbAc As String
        <XmlAttribute("refType")> Public Property refType As String
        <XmlAttribute("refTypeAc")> Public Property refTypeAc As String

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", Db, Id)
        End Function
    End Class
End Namespace