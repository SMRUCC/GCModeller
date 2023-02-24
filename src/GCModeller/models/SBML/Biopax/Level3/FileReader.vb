
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.xml

Namespace Level3

    Public Class FileReader

        ReadOnly xml As XmlElement

        Sub New(file As String)
            xml = XmlElement.ParseXmlText(file.SolveStream)
        End Sub

        Public Function GetObject() As File
            Dim elements = xml.elements.SafeQuery.GroupBy(Function(tag) tag.name).ToArray

        End Function
    End Class
End Namespace