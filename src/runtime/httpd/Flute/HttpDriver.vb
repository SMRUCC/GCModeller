Imports System.Net.Sockets
Imports System.Runtime.CompilerServices
Imports Flute.Http.Core
Imports Flute.Http.Core.Message

Public Class HttpDriver

    Dim responseHeader As New Dictionary(Of String, String)

    Sub New()
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub AddResponseHeader(header As String, value As String)
        Call responseHeader.Add(header, value)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetSocket(port As Integer) As HttpSocket
        Return New HttpSocket(
            app:=AddressOf AppHandler,
            port:=port
        )
    End Function

    Public Sub AppHandler(request As HttpRequest, response As HttpResponse)

    End Sub

End Class
