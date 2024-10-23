#Region "Microsoft.VisualBasic::df72d333ad15a9209ca7a4d85e12a8d5, G:/GCModeller/src/runtime/httpd/src/Flute//HttpDriver.vb"

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

    '   Total Lines: 46
    '    Code Lines: 37
    ' Comment Lines: 0
    '   Blank Lines: 9
    '     File Size: 1.49 KB


    ' Class HttpDriver
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetSocket
    ' 
    '     Sub: AddResponseHeader, AppHandler, HttpMethod
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Flute.Http.Configurations
Imports Flute.Http.Core
Imports Flute.Http.Core.Message

''' <summary>
''' A simple helper for create http service
''' </summary>
Public Class HttpDriver

    Dim responseHeader As New Dictionary(Of String, String)
    Dim methods As New Dictionary(Of String, HttpSocket.AppHandler)
    Dim settings As Configuration

    Sub New(Optional settings As Configuration = Nothing)
        Me.settings = settings
    End Sub

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
            configs:=settings
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

