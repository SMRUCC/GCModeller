Imports System.Net.Sockets
Imports Flute.Http.Core.Message

Namespace Core

    ''' <summary>
    ''' A simple http server module with no file system access.
    ''' </summary>
    Public Class HttpSocket : Inherits HttpServer

        Public Delegate Sub AppHandler(request As HttpRequest, response As HttpResponse)

        ''' <summary>
        ''' handle http request
        ''' </summary>
        ReadOnly app As AppHandler
        ReadOnly silent As Boolean = False

        Public Sub New(app As AppHandler, port As Integer, Optional threads As Integer = -1, Optional silent As Boolean = False)
            MyBase.New(port, threads, silent:=silent)

            ' handle http request
            Me.app = app
            Me.silent = silent
        End Sub

        Public Overrides Sub handleGETRequest(p As HttpProcessor)
            Call app(New HttpRequest(p), New HttpResponse(p.outputStream, AddressOf p.writeFailure))
        End Sub

        Public Overrides Sub handlePOSTRequest(p As HttpProcessor, inputData As String)
            Call app(New HttpPOSTRequest(p, inputData), New HttpResponse(p.outputStream, AddressOf p.writeFailure))
        End Sub

        Public Overrides Sub handleOtherMethod(p As HttpProcessor)
            Dim req As New HttpRequest(p)
            Dim response As New HttpResponse(p.outputStream, AddressOf p.writeFailure)

            If req.HTTPMethod = "OPTIONS" AndAlso req.URL.path.Trim("/"c) = "ctrl/kill" Then
                Call response.WriteHTML("OK!")
                Call Me.Shutdown()
            Else
                Call app(req, response)
            End If
        End Sub

        Protected Overrides Function getHttpProcessor(client As TcpClient, bufferSize As Integer) As HttpProcessor
            Return New HttpProcessor(client, Me, MAX_POST_SIZE:=bufferSize * 4, silent:=silent)
        End Function
    End Class
End Namespace