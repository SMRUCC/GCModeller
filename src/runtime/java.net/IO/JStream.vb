
Namespace Tamir.SharpSsh.java.io
    ''' <summary>
    ''' Summary description for Stream.
    ''' </summary>
    Public Class JStream
        Inherits Global.System.IO.Stream

        Friend s As Global.System.IO.Stream

        Public Sub New(ByVal s As Global.System.IO.Stream)
            Me.s = s
        End Sub

        Public Overrides Function ReadMethod(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer) As Integer
            Return s.Read(buffer, offset, count)
        End Function

        Public Overrides Function ReadByte() As Integer
            Return s.ReadByte()
        End Function

        Public Function readMethod(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer) As Integer
            Return Me.ReadMethod(buffer, offset, count)
        End Function

        Public Function read(ByVal buffer As Byte()) As Integer
            Return Me.ReadMethod(buffer, 0, buffer.Length)
        End Function

        Public Function read() As Integer
            Return ReadByte()
        End Function

        Public Sub close()
            CloseMethod()
        End Sub

        Public Overrides Sub CloseMethod()
            s.Close()
        End Sub

        Public Overrides Sub WriteByte(ByVal value As Byte)
            s.WriteByte(value)
        End Sub

        Public Overrides Sub WriteMethod(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer)
            s.Write(buffer, offset, count)
        End Sub

        Public Sub writeMethod(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer)
            Me.WriteMethod(buffer, offset, count)
        End Sub

        Public Sub write(ByVal buffer As Byte())
            Me.WriteMethod(buffer, 0, buffer.Length)
        End Sub

        Public Overrides ReadOnly Property CanRead As Boolean
            Get
                Return s.CanRead
            End Get
        End Property

        Public Overrides ReadOnly Property CanWrite As Boolean
            Get
                Return s.CanWrite
            End Get
        End Property

        Public Overrides ReadOnly Property CanSeek As Boolean
            Get
                Return s.CanSeek
            End Get
        End Property

        Public Overrides Sub FlushMethod()
            s.Flush()
        End Sub

        Public Overrides ReadOnly Property Length As Long
            Get
                Return s.Length
            End Get
        End Property

        Public Overrides Property Position As Long
            Get
                Return s.Position
            End Get
            Set(ByVal value As Long)
                s.Position = value
            End Set
        End Property

        Public Overrides Sub SetLength(ByVal value As Long)
            s.SetLength(value)
        End Sub

        Public Overrides Function Seek(ByVal offset As Long, ByVal origin As Global.System.IO.SeekOrigin) As Long
            Return s.Seek(offset, origin)
        End Function

        Public Function skip(ByVal len As Long) As Long
            'Seek doesn't work
            'return Seek(offset, IO.SeekOrigin.Current);
            Dim i = 0
            Dim count = 0
            Dim buf = New Byte(len - 1) {}

            While len > 0
                i = Me.ReadMethod(buf, count, CInt(len)) 'tamir: possible lost of pressision

                If i <= 0 Then
                    Throw New Exception("inputstream is closed")
                    'return (s-foo)==0 ? i : s-foo;
                End If

                count += i
                len -= i
            End While

            Return count
        End Function

        Public Function available() As Integer
            If TypeOf s Is Streams.PipedInputStream Then
                Return CType(s, Streams.PipedInputStream).available()
            End If

            Throw New Exception("JStream.available() -- Method not implemented")
        End Function

        Public Sub flushMethod()
            s.Flush()
        End Sub
    End Class
End Namespace
