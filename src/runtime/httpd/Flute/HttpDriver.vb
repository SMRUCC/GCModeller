Imports System.Net.Sockets
Imports Flute.Http.Core

Public Class HttpDriver : Inherits HttpServer

    Public Sub New(port As Integer, Optional threads As Integer = -1)
        MyBase.New(port, threads)
    End Sub

    Public Overrides Sub handleGETRequest(p As HttpProcessor)
        Throw New NotImplementedException()
    End Sub

    Public Overrides Sub handlePOSTRequest(p As HttpProcessor, inputData As String)
        Throw New NotImplementedException()
    End Sub

    Public Overrides Sub handleOtherMethod(p As HttpProcessor)
        Throw New NotImplementedException()
    End Sub

    Protected Overrides Function getHttpProcessor(client As TcpClient, bufferSize As Integer) As HttpProcessor
        Return New HttpProcessor(client, Me, bufferSize)
    End Function
End Class
