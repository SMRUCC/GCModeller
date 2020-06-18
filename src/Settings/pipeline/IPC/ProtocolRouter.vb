Imports Microsoft.VisualBasic.Net.Protocols.Reflection
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports TcpEndPoint = System.Net.IPEndPoint

<Protocol(GetType(IPCProtocols))>
Public Class ProtocolRouter

    ReadOnly services As IPCHost

    Sub New(services As IPCHost)
        Me.services = services
    End Sub

    Public Function AllocateNewFile(request As RequestStream, remoteDevcie As TcpEndPoint) As RequestStream
        Dim name$
        ' 请注意，这个大小是预分配的大小，数据的实际大小可能小于这个值
        Dim sizeOf As Long
        Dim type As TypeInfo

        Call services.Register(name, sizeOf, type)
    End Function

    Public Function ReleaseFile(request As RequestStream, remoteDevcie As TcpEndPoint) As RequestStream

    End Function

    Public Function Shutdown(request As RequestStream, remoteDevcie As TcpEndPoint) As RequestStream

    End Function
End Class
