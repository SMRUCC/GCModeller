#Region "Microsoft.VisualBasic::4ba6d7f49c17ca6c0b26ea3b2eeb7dc0, src\Flute\Http\HttpSocket.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 63
    '    Code Lines: 41 (65.08%)
    ' Comment Lines: 7 (11.11%)
    '    - Xml Docs: 85.71%
    ' 
    '   Blank Lines: 15 (23.81%)
    '     File Size: 2.33 KB


    '     Interface IAppHandler
    ' 
    '         Sub: AppHandler
    ' 
    '     Class HttpSocket
    ' 
    ' 
    '         Delegate Sub
    ' 
    '             Constructor: (+1 Overloads) Sub New
    ' 
    '             Function: getHttpProcessor
    ' 
    '             Sub: handleGETRequest, handleOtherMethod, handlePOSTRequest
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Net.Sockets
Imports Flute.Http.Configurations
Imports Flute.Http.Core.HttpStream
Imports Flute.Http.Core.Message

Namespace Core

    Public Interface IAppHandler

        Sub AppHandler(request As HttpRequest, response As HttpResponse)

    End Interface

    ''' <summary>
    ''' A simple http server module with no file system access.
    ''' </summary>
    Public Class HttpSocket : Inherits HttpServer

        Public Delegate Sub AppHandler(request As HttpRequest, response As HttpResponse)

        ''' <summary>
        ''' handle http request
        ''' </summary>
        ReadOnly app As AppHandler
        ReadOnly parseJSON As PostReader.JSONParser

        Public Sub New(app As AppHandler, port As Integer,
                       Optional threads As Integer = -1,
                       Optional configs As Configuration = Nothing,
                       Optional jsonParser As PostReader.JSONParser = Nothing)

            MyBase.New(port, threads, configs)

            ' handle http request
            Me.app = app
            Me.parseJSON = jsonParser
        End Sub

        Public Overrides Sub handleGETRequest(p As HttpProcessor)
            Dim response As New HttpResponse(p.outputStream, AddressOf p.writeFailure, _settings)
            response.m_requestHeaders = p.httpHeaders
            Call app(New HttpRequest(p), response)
        End Sub

        Public Overrides Sub handlePOSTRequest(p As HttpProcessor, inputData As String)
            Dim response As New HttpResponse(p.outputStream, AddressOf p.writeFailure, _settings)
            response.m_requestHeaders = p.httpHeaders
            Call app(New HttpPOSTRequest(p, inputData, parseJSON), response)
        End Sub

        Public Overrides Sub handleOtherMethod(p As HttpProcessor)
            Dim req As New HttpRequest(p)
            Dim response As New HttpResponse(p.outputStream, AddressOf p.writeFailure, _settings)
            response.m_requestHeaders = p.httpHeaders

            If req.HTTPMethod = "OPTIONS" AndAlso req.URL.path.Trim("/"c) = "ctrl/kill" Then
                ' remote shutdown requires a configured token to be present
                ' in the X-Shutdown-Token header, otherwise it is rejected.
                Dim token As String = _settings.shutdown_token

                If token.StringEmpty Then
                    Call response.WriteHTML("Remote shutdown is disabled.")
                ElseIf Not String.Equals(If(req.HttpHeaders.TryGetValue("X-Shutdown-Token"), ""), token, StringComparison.Ordinal) Then
                    Call response.WriteHTML("Invalid shutdown token.")
                Else
                    Call response.WriteHTML("OK!")
                    Call Me.Shutdown()
                End If
            Else
                Call app(req, response)
            End If
        End Sub

        Protected Overrides Function getHttpProcessor(client As TcpClient, bufferSize As Integer) As HttpProcessor
            ' use a generous default POST body limit (16 MB) instead of
            ' bufferSize*4 which is only ~16 KB for the default 4 KB buffer.
            Return New HttpProcessor(client, Me, MAX_POST_SIZE:=16 * 1024 * 1024, _settings)
        End Function
    End Class
End Namespace
