#Region "Microsoft.VisualBasic::8caaf1534422cb25ec80884807cad9e3, WebCloud\SMRUCC.HTTPInternal\test\Module1.vb"

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

' Module Module1
' 
'     Sub: Main, StartWebSocketServer
' 
' /********************************************************************************/

#End Region

Imports SMRUCC.WebCloud.HTTPInternal.Core.WebSocket
Imports WebSocket = SMRUCC.WebCloud.HTTPInternal.Core.WebSocket.WsServer

Module Module1

    Sub Main()  'Program Entry point
        Call StartWebSocketServer()


        Pause()
    End Sub

    Public WebSocketServer As WebSocket
    Public Sub StartWebSocketServer()
        'WebSocketServer = New WebSocket("127.0.0.1", 8000, Function(socket) New WsProcessor(socket))
        'WebSocketServer.StartServer()
    End Sub

End Module
