Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.KEGG.WebServices

    Public Class Map

        <XmlElement> Public Property Areas As Area()

        Public Overrides Function ToString() As String
            Return Areas.GetJson
        End Function
    End Class

    Public Class Area

        <XmlAttribute> Public Property shape As String
        ''' <summary>
        ''' 位置坐标信息
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property coords As String
        <XmlAttribute> Public Property href As String
        <XmlAttribute> Public Property title As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace