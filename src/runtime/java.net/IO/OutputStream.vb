
Namespace Tamir.SharpSsh.java.io
    ''' <summary>
    ''' Summary description for InputStream.
    ''' </summary>
    Public MustInherit Class OutputStream
        Inherits Global.System.IO.Stream

        Public Overridable Function ReadMethod(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer) As Integer
            Return 0
        End Function

        Public Overrides Function ReadByte() As Integer
            Return 0
        End Function

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

        Public Overrides Function Seek(ByVal offset As Long, ByVal origin As Global.System.IO.SeekOrigin) As Long
            Return 0
        End Function
    End Class
End Namespace
