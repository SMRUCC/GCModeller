#Region "Microsoft.VisualBasic::e327406db88c3348c745a234981e453e, pipeline\IPC\IPCHost.vb"

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

    ' Class IPCHost
    ' 
    '     Sub: Delete, Register
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports CliPipeline = Microsoft.VisualBasic.CommandLine

Public Class IPCHost

    ReadOnly host As TcpServicesSocket
    ReadOnly resources As Dictionary(Of String, Resource)

    Sub New(port As Integer)
        host = New TcpServicesSocket(port)
    End Sub

    Public Function Run() As Integer
        Return host.Run
    End Function

    Public Sub Register(name$, size&, type As TypeInfo)
        Call CliPipeline.OpenForWrite($"memory:/{name}", size)
    End Sub

    Public Sub Delete(name As String)

    End Sub

End Class

