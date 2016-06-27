Imports System.Xml.Serialization

Namespace Analysis

    Public Class LogsPair

        <XmlAttribute> Public LogsDir As String
        <XmlArray> Public Logs As Pair()()

        Public Shared Function GetFileName(LogsDir As String) As String
            Return String.Format("{0}/blast_venn.xml", LogsDir)
        End Function

        Public Shared Function GetXmlFileName(XmlLogsDir As String) As String
            Return String.Format("{0}/blast________xml_logs.xml", XmlLogsDir)
        End Function

        Public Overrides Function ToString() As String
            Return Logs.ToString
        End Function
    End Class

    Public Class Pair
        <XmlAttribute> Public File1, File2 As String

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]  &&  [{1}]", File1, File2)
        End Function
    End Class
End Namespace