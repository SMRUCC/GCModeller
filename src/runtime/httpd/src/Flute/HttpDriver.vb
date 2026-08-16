#Region "Microsoft.VisualBasic::8b4107c532de560996b79c76539e2aa7, src\Flute\HttpDriver.vb"

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

    '   Total Lines: 79
    '    Code Lines: 50 (63.29%)
    ' Comment Lines: 18 (22.78%)
    '    - Xml Docs: 83.33%
    ' 
    '   Blank Lines: 11 (13.92%)
    '     File Size: 2.58 KB


    ' Class HttpDriver
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: AddResponseHeader, GetSocket, (+2 Overloads) HttpMethod, SetJSONParser
    ' 
    '     Sub: AppHandler
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Flute.Http.Configurations
Imports Flute.Http.Core
Imports Flute.Http.Core.HttpStream
Imports Flute.Http.Core.Message

''' <summary>
''' A simple helper for create http service
''' </summary>
Public Class HttpDriver

    Dim responseHeader As New Dictionary(Of String, String)
    Dim methods As New Dictionary(Of String, HttpSocket.AppHandler)
    Dim settings As Configuration
    Dim jsonParser As PostReader.JSONParser

    ''' <summary>
    ''' create a new http driver, optionally bound to a server configuration.
    ''' </summary>
    ''' <param name="settings">
    ''' the optional server wide configuration; may be <c>Nothing</c> and
    ''' supplied later when building the socket.
    ''' </param>
    Sub New(Optional settings As Configuration = Nothing)
        Me.settings = settings
    End Sub

    ''' <summary>
    ''' set the custom json body parser that will be used to deserialize
    ''' posted json payloads, then return this driver for chaining.
    ''' </summary>
    ''' <param name="parser">the json parser implementation.</param>
    ''' <returns>this <see cref="HttpDriver"/> instance for method chaining.</returns>
    Public Function SetJSONParser(parser As PostReader.JSONParser) As HttpDriver
        Me.jsonParser = parser
        Return Me
    End Function

    ''' <summary>
    ''' register an <see cref="IAppHandler"/> implementation to handle the given
    ''' http method, then return this driver for chaining.
    ''' </summary>
    ''' <param name="method">
    ''' get/post/put/delete, the http method name, case-insensitive.
    ''' </param>
    ''' <param name="handler">the application handler implementation to invoke.</param>
    ''' <returns>this <see cref="HttpDriver"/> instance for method chaining.</returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function HttpMethod(method As String, handler As IAppHandler) As HttpDriver
        Return HttpMethod(method, AddressOf handler.AppHandler)
    End Function

    ''' <summary>
    ''' register a raw request handler delegate to handle the given http method,
    ''' then return this driver for chaining.
    ''' </summary>
    ''' <param name="method">
    ''' get/post/put/delete, the http method name, case-insensitive.
    ''' </param>
    ''' <param name="handler">the request handler delegate to invoke.</param>
    ''' <returns>this <see cref="HttpDriver"/> instance for method chaining.</returns>
    Public Function HttpMethod(method As String, handler As HttpSocket.AppHandler) As HttpDriver
        methods(method.ToUpper) = handler
        Return Me
    End Function

    ''' <summary>
    ''' add a custom response header that will be written to every response,
    ''' then return this driver for chaining.
    ''' </summary>
    ''' <param name="header">the http header name.</param>
    ''' <param name="value">the http header value.</param>
    ''' <returns>this <see cref="HttpDriver"/> instance for method chaining.</returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function AddResponseHeader(header As String, value As String) As HttpDriver
        Call responseHeader.Add(header, value)
        Return Me
    End Function

    ''' <summary>
    ''' build a configured <see cref="HttpSocket"/> listening on the given port
    ''' that dispatches requests through the handlers registered on this driver.
    ''' </summary>
    ''' <param name="port">the tcp port the http socket will listen on.</param>
    ''' <returns>a ready to start <see cref="HttpSocket"/> instance.</returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetSocket(port As Integer) As HttpSocket
        Return New HttpSocket(
            app:=AddressOf AppHandler,
            port:=port,
            configs:=settings,
            jsonParser:=jsonParser
        )
    End Function

    ''' <summary>
    ''' the internal request dispatcher: it first copies the configured custom
    ''' response headers onto the response, then invokes the handler registered
    ''' for the request's http method, or returns 501 when no handler matches.
    ''' </summary>
    ''' <param name="request">the incoming http request.</param>
    ''' <param name="response">the response to be written back to the client.</param>
    Public Sub AppHandler(request As HttpRequest, response As HttpResponse)
        For Each header In responseHeader
            Call response.AddCustomHttpHeader(header.Key, header.Value)
        Next

        If methods.ContainsKey(request.HTTPMethod) Then
            Call methods(request.HTTPMethod)(request, response)
        Else
            Call response.WriteError(501, "501 Not Implemented")
        End If
    End Sub

End Class
