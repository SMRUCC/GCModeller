
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.xml

Namespace Level3

    Public Class FileReader

        ReadOnly xml As XmlElement

        Sub New(file As String)
            xml = XmlElement.ParseXmlText(file.SolveStream)
        End Sub

        Public Function GetObject() As File
            Dim elements = xml.elements _
                .SafeQuery _
                .GroupBy(Function(tag) tag.name) _
                .ToDictionary(Function(k) k.Key,
                              Function(array)
                                  Return array.ToArray
                              End Function)
            Dim file As New File

            Return file
        End Function
    End Class
End Namespace