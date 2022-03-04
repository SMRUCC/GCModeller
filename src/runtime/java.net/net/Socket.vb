
Namespace Tamir.SharpSsh.java.net
    ''' <summary>
    ''' Summary description for Socket.
    ''' </summary>
    Public Class Socket
        Friend sock As System.Net.Sockets.Socket

        Protected Sub SetSocketOption(ByVal level As System.Net.Sockets.SocketOptionLevel, ByVal name As System.Net.Sockets.SocketOptionName, ByVal val As Integer)
            Try
                sock.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.NoDelay, val)
            Catch
            End Try
        End Sub

        '		public Socket(AddressFamily af, SocketType st, ProtocolType pt)
        '		{
        '			this.sock = new Sock(af, st, pt);
        '			this.sock.Connect();
        '		}

        Public Sub New(ByVal host As String, ByVal port As Integer)
            Dim ep As System.Net.IPEndPoint = New System.Net.IPEndPoint(System.Net.Dns.GetHostByName(host).AddressList(0), port)
            sock = New System.Net.Sockets.Socket(ep.AddressFamily, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp)
            sock.Connect(ep)
        End Sub

        Public Sub New(ByVal sock As System.Net.Sockets.Socket)
            Me.sock = sock
        End Sub

        Public Function getInputStream() As System.IO.Stream
            Return New System.Net.Sockets.NetworkStream(sock)
        End Function

        Public Function getOutputStream() As System.IO.Stream
            Return New System.Net.Sockets.NetworkStream(sock)
        End Function

        Public Function isConnected() As Boolean
            Return sock.Connected
        End Function

        Public Sub setTcpNoDelay(ByVal b As Boolean)
            If b Then
                Me.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.NoDelay, 1)
            Else
                Me.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.NoDelay, 0)
            End If
        End Sub

        Public Sub setSoTimeout(ByVal t As Integer)
            Me.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.ReceiveTimeout, t)
            Me.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.SendTimeout, t)
        End Sub

        Public Sub close()
            sock.Close()
        End Sub

        Public Function getInetAddress() As InetAddress
            Return New InetAddress(CType(sock.RemoteEndPoint, System.Net.IPEndPoint).Address)
        End Function

        Public Function getPort() As Integer
            Return CType(sock.RemoteEndPoint, System.Net.IPEndPoint).Port
        End Function
    End Class
End Namespace
