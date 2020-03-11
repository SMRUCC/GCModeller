Imports System.IO

Namespace vcXML

    Public Class Reader

        Dim fs As StreamReader
        ''' <summary>
        ''' [module -> [type -> offset]]
        ''' </summary>
        Dim index As Dictionary(Of String, Dictionary(Of String, offset))

        Sub New(file As String)
            fs = file.OpenReader()

            Call loadOffsets()
        End Sub

        Private Sub loadOffsets()
            fs.BaseStream.Seek(-200, SeekOrigin.End)

            Dim content = fs.ReadToEnd
            Dim i = InStr(content, "<indexOffset>")


        End Sub
    End Class
End Namespace