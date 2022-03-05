
Namespace Tamir.SharpSsh.java.io
    ''' <summary>
    ''' Summary description for FileInputStream.
    ''' </summary>
    Public Class FileInputStream
        Inherits InputStream

        Private fs As Global.System.IO.FileStream

        Public Sub New(ByVal file As String)
            fs = Global.System.IO.File.OpenRead(file)
        End Sub

        Public Sub New(ByVal file As File)
            Me.New(file.info.Name)
        End Sub

        Public Overrides Sub CloseMethod()
            fs.Close()
        End Sub

        Public Overrides Function ReadMethod(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer) As Integer
            Return fs.Read(buffer, offset, count)
        End Function

        Public Overrides ReadOnly Property CanSeek As Boolean
            Get
                Return fs.CanSeek
            End Get
        End Property

        Public Overrides Function Seek(ByVal offset As Long, ByVal origin As Global.System.IO.SeekOrigin) As Long
            Return fs.Seek(offset, origin)
        End Function
    End Class
End Namespace
