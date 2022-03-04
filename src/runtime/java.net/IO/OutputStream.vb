
Namespace Tamir.SharpSsh.java.io
    ''' <summary>
    ''' Summary description for InputStream.
    ''' </summary>
    Public MustInherit Class OutputStream
        Inherits System.IO.Stream

        Public Overrides Function ReadMethod(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer) As Integer
            Return 0
        End Function

        Public Overrides Function ReadByte() As Integer
            Return 0
        End Function

        Public Overridable Sub write(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer)
            MyBase.Write(buffer, offset, count)
        End Sub

        Public Overridable Sub close()
            MyBase.Close()
        End Sub

        Public Overridable Sub flush()
            FlushMethod()
        End Sub

        Public Overrides ReadOnly Property CanRead As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides ReadOnly Property CanWrite As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides ReadOnly Property CanSeek As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides Sub FlushMethod()
        End Sub

        Public Overrides ReadOnly Property Length As Long
            Get
                Return 0
            End Get
        End Property

        Public Overrides Property Position As Long
            Get
                Return 0
            End Get
            Set(ByVal value As Long)
            End Set
        End Property

        Public Overrides Sub SetLength(ByVal value As Long)
        End Sub

        Public Overrides Function Seek(ByVal offset As Long, ByVal origin As System.IO.SeekOrigin) As Long
            Return 0
        End Function
    End Class
End Namespace
