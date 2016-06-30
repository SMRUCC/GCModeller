Imports System.Xml.Serialization

Namespace DocumentFormat.XmlOutput

    Public MustInherit Class MEMEXmlBase

        <XmlAttribute("version")> Public Property Version As String
        <XmlAttribute("release")> Public Property Release As String

        Protected Friend Sub New()
        End Sub
    End Class

    Public MustInherit Class ModelBase

        <XmlElement("command_line")>
        Public Property CommandLine As String
        <XmlElement("host")>
        Public Property Host As String

        Protected Friend Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{Host} >>>]   ""{CommandLine}"""
        End Function
    End Class
End Namespace