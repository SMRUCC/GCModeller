
Namespace Tamir.SharpSsh.java.io
    ''' <summary>
    ''' Summary description for FileInputStream.
    ''' </summary>
    Public Class FileOutputStream
        Inherits OutputStream

        Private fs As System.IO.FileStream

        Public Sub New(ByVal file As String)
            Me.New(file, False)
        End Sub

        Public Sub New(ByVal file As File)
            Me.New(file.info.Name, False)
        End Sub

        Public Sub New(ByVal file As String, ByVal append As Boolean)
            If append Then
                fs = New System.IO.FileStream(file, System.IO.FileMode.Append) ' append
            Else
                fs = New System.IO.FileStream(file, System.IO.FileMode.Create)
            End If
        End Sub

        Public Sub New(ByVal file As File, ByVal append As Boolean)
            Me.New(file.info.Name)
        End Sub

        Public Overrides Sub WriteMethod(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer)
            fs.Write(buffer, offset, count)
        End Sub

        Public Overrides Sub FlushMethod()
            fs.Flush()
        End Sub

        Public Overrides Sub CloseMethod()
            fs.Close()
        End Sub

        Public Overrides ReadOnly Property CanSeek As Boolean
            Get
                Return fs.CanSeek
            End Get
        End Property

        Public Overrides Function Seek(ByVal offset As Long, ByVal origin As System.IO.SeekOrigin) As Long
            Return fs.Seek(offset, origin)
        End Function
    End Class
End Namespace
