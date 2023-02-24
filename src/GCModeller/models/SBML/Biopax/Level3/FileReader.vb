
Imports Microsoft.VisualBasic.MIME.application.xml

Namespace Level3

    Public Class FileReader

        ReadOnly xml As XmlElement

        Sub New(file As String)
            xml = XmlElement.ParseXmlText(file.SolveStream)
        End Sub

        Public Function GetObject() As File

        End Function
    End Class
End Namespace