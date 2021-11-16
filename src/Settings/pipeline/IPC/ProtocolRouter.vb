#Region "Microsoft.VisualBasic::8047c1c684704a21dcf94839e000bcae, pipeline\IPC\ProtocolRouter.vb"

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

    ' Class ProtocolRouter
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: AllocateNewFile, ReleaseFile, Shutdown
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Net.Protocols.Reflection
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports TcpEndPoint = System.Net.IPEndPoint

<Protocol(GetType(IPCProtocols))>
Public Class ProtocolRouter

    ReadOnly services As IPCHost

    Sub New(services As IPCHost)
        Me.services = services
    End Sub

    Public Function AllocateNewFile(request As RequestStream, remoteDevcie As TcpEndPoint) As RequestStream
        Dim name$ = Nothing
        ' 请注意，这个大小是预分配的大小，数据的实际大小可能小于这个值
        Dim sizeOf As Long
        Dim type As TypeInfo = Nothing

        Call services.Register(name, sizeOf, type)

        Throw New NotImplementedException
    End Function

    Public Function ReleaseFile(request As RequestStream, remoteDevcie As TcpEndPoint) As RequestStream
        Throw New NotImplementedException
    End Function

    Public Function Shutdown(request As RequestStream, remoteDevcie As TcpEndPoint) As RequestStream
        Throw New NotImplementedException
    End Function
End Class

