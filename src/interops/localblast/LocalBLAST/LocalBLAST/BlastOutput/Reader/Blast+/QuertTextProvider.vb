Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Text

Namespace LocalBLAST.BLASTOutput.BlastPlus

    Public Class QuertTextProvider : Inherits BufferedStream

        Sub New(file As String, Optional encoding As Encodings = Encodings.Default, Optional bufSize As Integer = 64 * 1024 * 1024)
            Call MyBase.New(file, encoding.CodePage, bufSize)
        End Sub

        ''' <summary>
        ''' 這個函數所返回的是已經解析好的query文本部分
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function QueryDatas() As IEnumerable(Of String)
            Do While Not EndRead
                Dim lines$() = MyBase.BufferProvider
            Loop
        End Function
    End Class
End Namespace