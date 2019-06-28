Imports WebSocket = SMRUCC.WebCloud.HTTPInternal.Core.WebSocket.WsServer

Module Module1

    Sub Main()  'Program Entry point
        Call StartWebSocketServer()


        Pause()
    End Sub

    Public WebSocketServer As WebSocket
    Public Sub StartWebSocketServer()
        WebSocketServer = New WebSocket("127.0.0.1", 8000)
        WebSocketServer.StartServer()
    End Sub

End Module
