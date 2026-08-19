#Region "Microsoft.VisualBasic::5713192e7d5c0ffacb2e06c4e71779b6, src\Flute\Http\HttpSocket.vb"

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

    '   Total Lines: 80
    '    Code Lines: 53 (66.25%)
    ' Comment Lines: 11 (13.75%)
    '    - Xml Docs: 54.55%
    ' 
    '   Blank Lines: 16 (20.00%)
    '     File Size: 3.33 KB


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
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit

Namespace Core

    ''' <summary>
    ''' the application level request handler contract that is invoked for every
    ''' incoming http request handled by a <see cref="HttpSocket"/>.
    ''' </summary>
    Public Interface IAppHandler

        ''' <summary>
        ''' handle a single http request and write the response back to the client.
        ''' </summary>
        ''' <param name="request">the parsed incoming http request.</param>
        ''' <param name="response">the response object to be written to the client.</param>
        Sub AppHandler(request As HttpRequest, response As HttpResponse)

    End Interface

    ''' <summary>
    ''' A simple http server module with no file system access.
    ''' </summary>
    Public Class HttpSocket : Inherits HttpServer

        ''' <summary>
        ''' the delegate signature for an application request handler, receiving
        ''' the parsed request and the response to write back.
        ''' </summary>
        ''' <param name="request">the parsed incoming http request.</param>
        ''' <param name="response">the response object to be written to the client.</param>
        Public Delegate Sub AppHandler(request As HttpRequest, response As HttpResponse)

        ''' <summary>
        ''' the application callback that handles every incoming http request.
        ''' </summary>
        ReadOnly app As AppHandler
        ''' <summary>
        ''' the optional custom json parser used to deserialize POST json payloads.
        ''' </summary>
        ReadOnly parseJSON As PostReader.JSONParser

        ''' <summary>
        ''' create a new in-memory http socket that dispatches every request to the
        ''' given application handler, listening on the given port.
        ''' </summary>
        ''' <param name="app">the application request handler callback.</param>
        ''' <param name="port">the tcp port to listen on.</param>
        ''' <param name="threads">the worker thread pool size; a value &lt;= 0 uses the CPU core count.</param>
        ''' <param name="configs">the optional server wide configuration.</param>
        ''' <param name="jsonParser">the optional custom json body parser.</param>
        Public Sub New(app As AppHandler, port As Integer,
                       Optional threads As Integer = -1,
                       Optional configs As Configuration = Nothing,
                       Optional jsonParser As PostReader.JSONParser = Nothing)

            MyBase.New(port, threads, configs)

            ' handle http request
            Me.app = app
            Me.parseJSON = jsonParser
        End Sub

        ''' <summary>
        ''' create a new in-memory http socket that dispatches every request to the
        ''' given application handler, listening on the given port.
        ''' </summary>
        ''' <param name="router">the application request handler callback.</param>
        ''' <param name="port">the tcp port to listen on.</param>
        ''' <param name="threads">the worker thread pool size; a value &lt;= 0 uses the CPU core count.</param>
        ''' <param name="configs">the optional server wide configuration.</param>
        ''' <param name="jsonParser">the optional custom json body parser.</param>
        Public Sub New(router As IAppHandler, port As Integer,
                       Optional threads As Integer = -1,
                       Optional configs As Configuration = Nothing,
                       Optional jsonParser As PostReader.JSONParser = Nothing)

            MyBase.New(port, threads, configs)

            ' handle http request
            Me.app = AddressOf router.AppHandler
            Me.parseJSON = jsonParser
        End Sub

        ''' <summary>
        ''' build an <see cref="HttpResponse"/> and dispatch the GET request to
        ''' the application handler as a plain <see cref="HttpRequest"/>.
        ''' </summary>
        ''' <param name="p">the http processor that carried the GET request.</param>
        Public Overrides Sub handleGETRequest(p As HttpProcessor)
            Dim response As New HttpResponse(p.outputStream, AddressOf p.writeFailure, _settings)
            response.m_requestHeaders = p.httpHeaders
            Call app(New HttpRequest(p), response)
        End Sub

        ''' <summary>
        ''' build an <see cref="HttpResponse"/> and dispatch the POST request to
        ''' the application handler as an <see cref="HttpPOSTRequest"/> that can
        ''' parse the posted json/form body.
        ''' </summary>
        ''' <param name="p">the http processor that carried the POST request.</param>
        ''' <param name="inputData">the decoded POST body string.</param>
        Public Overrides Sub handlePOSTRequest(p As HttpProcessor, inputData As String)
            Dim response As New HttpResponse(p.outputStream, AddressOf p.writeFailure, _settings)
            response.m_requestHeaders = p.httpHeaders
            Call app(New HttpPOSTRequest(p, inputData, parseJSON), response)
        End Sub

        ''' <summary>
        ''' handle a non-GET/POST method. besides forwarding to the application
        ''' handler, this also implements the remote shutdown endpoint
        ''' <c>OPTIONS /ctrl/kill</c>, which is gated by a configured
        ''' <c>X-Shutdown-Token</c> header.
        ''' </summary>
        ''' <param name="p">the http processor that carried the request.</param>
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

        ''' <summary>
        ''' create an <see cref="HttpProcessor"/> for the accepted connection,
        ''' using a generous 16 MB POST body limit independent of the read buffer size.
        ''' </summary>
        ''' <param name="client">the accepted tcp client.</param>
        ''' <param name="bufferSize">the read buffer size (unused, kept for signature compatibility).</param>
        ''' <returns>a new <see cref="HttpProcessor"/> bound to this server.</returns>
        Protected Overrides Function getHttpProcessor(client As TcpClient, bufferSize As Integer) As HttpProcessor
            ' use a generous default POST body limit (16 MB) instead of
            ' bufferSize*4 which is only ~16 KB for the default 4 KB buffer.
            Return New HttpProcessor(client, Me, MAX_POST_SIZE:=16 * ByteSize.MB, _settings)
        End Function
    End Class
End Namespace
