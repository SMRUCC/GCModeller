#Region "Microsoft.VisualBasic::a4d62b7a5888bc8d783e3505fadf1427, WebCloud\SMRUCC.HTTPInternal\Core\WebSocket\Events.vb"

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

    '     Delegate Sub
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Net.Sockets

Namespace Core.WebSocket

    Public Delegate Sub OnClientConnectDelegate(sender As Object, ByRef client As WsProcessor)
    Public Delegate Sub OnClientDisconnectDelegateHandler(sender As Object)

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="data"></param>
    ''' <param name="responseStream">请注意，使用完了不可以关闭这个流对象</param>
    Public Delegate Sub OnClientTextMessage(sender As WsProcessor, data As String, responseStream As NetworkStream)
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="data"></param>
    ''' <param name="responseStream">请注意，使用完了不可以关闭这个流对象</param>
    Public Delegate Sub OnClinetBinaryMessage(sender As WsProcessor, data As MemoryStream, responseStream As NetworkStream)

End Namespace
