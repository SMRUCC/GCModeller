Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.KEGG.WebServices.KGML

    ''' <summary>
    ''' Network edges
    ''' </summary>
    Public Class link

        <XmlAttribute> Public Property type As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class
End Namespace