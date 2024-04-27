#Region "Microsoft.VisualBasic::b27ebceb8395cd1ef5b854052afc4694, G:/GCModeller/src/runtime/httpd/src/Flute//Http/HttpSocket.vb"

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

    '   Total Lines: 50
    '    Code Lines: 32
    ' Comment Lines: 7
    '   Blank Lines: 11
    '     File Size: 1.93 KB


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

        Public Sub New(app As AppHandler, port As Integer, Optional threads As Integer = -1, Optional configs As Configuration = Nothing)
            MyBase.New(port, threads, configs)

            ' handle http request
            Me.app = app
        End Sub

        Public Overrides Sub handleGETRequest(p As HttpProcessor)
            Call app(New HttpRequest(p), New HttpResponse(p.outputStream, AddressOf p.writeFailure, _settings))
        End Sub

        Public Overrides Sub handlePOSTRequest(p As HttpProcessor, inputData As String)
            Call app(New HttpPOSTRequest(p, inputData), New HttpResponse(p.outputStream, AddressOf p.writeFailure, _settings))
        End Sub

        Public Overrides Sub handleOtherMethod(p As HttpProcessor)
            Dim req As New HttpRequest(p)
            Dim response As New HttpResponse(p.outputStream, AddressOf p.writeFailure, _settings)

            If req.HTTPMethod = "OPTIONS" AndAlso req.URL.path.Trim("/"c) = "ctrl/kill" Then
                Call response.WriteHTML("OK!")
                Call Me.Shutdown()
            Else
                Call app(req, response)
            End If
        End Sub

        Protected Overrides Function getHttpProcessor(client As TcpClient, bufferSize As Integer) As HttpProcessor
            Return New HttpProcessor(client, Me, MAX_POST_SIZE:=bufferSize * 4, _settings)
        End Function
    End Class
End Namespace
