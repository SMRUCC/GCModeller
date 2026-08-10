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

    Sub New(Optional settings As Configuration = Nothing)
        Me.settings = settings
    End Sub

    Public Function SetJSONParser(parser As PostReader.JSONParser) As HttpDriver
        Me.jsonParser = parser
        Return Me
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="method">
    ''' get/post/put/delete, the http method name, case-insensitive
    ''' </param>
    ''' <param name="handler"></param>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function HttpMethod(method As String, handler As IAppHandler) As HttpDriver
        Return HttpMethod(method, AddressOf handler.AppHandler)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="method">
    ''' get/post/put/delete, the http method name, case-insensitive
    ''' </param>
    ''' <param name="handler"></param>
    Public Function HttpMethod(method As String, handler As HttpSocket.AppHandler) As HttpDriver
        methods(method.ToUpper) = handler
        Return Me
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function AddResponseHeader(header As String, value As String) As HttpDriver
        Call responseHeader.Add(header, value)
        Return Me
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetSocket(port As Integer) As HttpSocket
        Return New HttpSocket(
            app:=AddressOf AppHandler,
            port:=port,
            configs:=settings,
            jsonParser:=jsonParser
        )
    End Function

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
