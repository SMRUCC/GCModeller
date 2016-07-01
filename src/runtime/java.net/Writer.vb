
Public Class Writer : Inherits OutputStream

    Dim _encoding As Global.System.Text.Encoding

    Sub New(outputFile As String, encoding As Global.System.Text.Encoding)
        Call MyBase.New(outputFile)
        _encoding = encoding
        If encoding Is Nothing Then
            _encoding = Global.System.Text.Encoding.UTF8
        End If
    End Sub

End Class
