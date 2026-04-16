
Namespace Tamir.SharpSsh.java.io
    ''' <summary>
    ''' Summary description for InputStream.
    ''' </summary>
    Public MustInherit Class InputStream
        Inherits Global.System.IO.Stream

        Public Overridable Function read(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer) As Integer
            Return Read(buffer, offset, count)
        End Function

        Public Overridable Function read(ByVal buffer As Byte()) As Integer
            Return Read(buffer, 0, buffer.Length)
        End Function

        Public Overridable Function read() As Integer
            Return Me.ReadByte()
        End Function

        Public Overridable Sub close()
            MyBase.Close()
        End Sub

        Public Overrides Sub WriteByte(ByVal value As Byte)
        End Sub

        Public Overrides Sub WriteMethod(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer)
        End Sub

        Public Overrides ReadOnly Property CanRead As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides ReadOnly Property CanWrite As Boolean
            Get
                Return False
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

        Public Overrides Function Seek(ByVal offset As Long, ByVal origin As Global.System.IO.SeekOrigin) As Long
            Return 0
        End Function

        Public Function skip(ByVal len As Long) As Long
            'Seek doesn't work
            'return Seek(offset, IO.SeekOrigin.Current);
            Dim i = 0
            Dim count = 0
            Dim buf = New Byte(len - 1) {}

            While len > 0
                i = MyBase.Read(buf, count, len) 'tamir: possible lost of pressision

                If i <= 0 Then
                    Throw New Exception("inputstream is closed")
                    'return (s-foo)==0 ? i : s-foo;
                End If

                count += i
                len -= i
            End While

            Return count
        End Function
    End Class
End Namespace
