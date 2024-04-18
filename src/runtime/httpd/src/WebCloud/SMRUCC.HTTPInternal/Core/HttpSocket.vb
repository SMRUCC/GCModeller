Imports System.Net.Sockets

Namespace Core

    ''' <summary>
    ''' A simple http server module with no file system access.
    ''' </summary>
    Public Class HttpSocket : Inherits HttpServer

        Public Delegate Sub AppHandler(request As HttpRequest, response As HttpResponse)

        ReadOnly app As AppHandler

        Public Sub New(app As AppHandler, port As Integer, Optional threads As Integer = -1)
            MyBase.New(port, threads)

            Me.app = app
        End Sub

        Public Overrides Sub handleGETRequest(p As HttpProcessor)
            Call app(New HttpRequest(p), New HttpResponse(p.outputStream, Nothing))
        End Sub

        Public Overrides Sub handlePOSTRequest(p As HttpProcessor, inputData As String)
            Call app(New HttpPOSTRequest(p, inputData), New HttpResponse(p.outputStream, Nothing))
        End Sub

        Public Overrides Sub handlePUTMethod(p As HttpProcessor, inputData As String)
            Call app(New HttpRequest(p), New HttpResponse(p.outputStream, Nothing))
        End Sub

        Public Overrides Sub handleOtherMethod(p As HttpProcessor)
            Call app(New HttpRequest(p), New HttpResponse(p.outputStream, Nothing))
        End Sub

        Protected Overrides Function __httpProcessor(client As TcpClient) As HttpProcessor
            Return New HttpProcessor(client, Me, MAX_POST_SIZE:=BufferSize * 4)
        End Function
    End Class
End Namespace