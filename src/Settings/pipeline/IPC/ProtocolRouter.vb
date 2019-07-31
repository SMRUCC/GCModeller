Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Net.Protocols.Reflection
Imports TcpEndPoint = System.Net.IPEndPoint

<Protocol(GetType(IPCProtocols))>
Public Class ProtocolRouter

    ReadOnly services As IPCHost

    Sub New(services As IPCHost)
        Me.services = services
    End Sub

    Public Function AllocateNewFile(request As RequestStream, remoteDevcie As TcpEndPoint) As RequestStream

    End Function

    Public Function ReleaseFile(request As RequestStream, remoteDevcie As TcpEndPoint) As RequestStream

    End Function

    Public Function Shutdown(request As RequestStream, remoteDevcie As TcpEndPoint) As RequestStream

    End Function
End Class
