
Namespace Tamir.SharpSsh.java.net
    ''' <summary>
    ''' Summary description for ServerSocket.
    ''' </summary>
    Public Class ServerSocket
        Inherits System.Net.Sockets.TcpListener

        Public Sub New(ByVal port As Integer, ByVal arg As Integer, ByVal addr As InetAddress)
            MyBase.New(addr.addr, port)
            Me.Start()
        End Sub

        Public Function accept() As Socket
            Return New Socket(Me.AcceptSocket())
        End Function

        Public Sub close()
            Me.Stop()
        End Sub
    End Class
End Namespace
