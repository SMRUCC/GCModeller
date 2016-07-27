#Region "Microsoft.VisualBasic::4ece90af983b253d0a89b0323032cc11, ..\ComputingServices\SharedMemory\Protocols.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComputingServices.TaskHost
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Net.Protocols.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace SharedMemory

    Module Protocols

        Public Enum MemoryProtocols
            Read
            Write
            [TypeOf]
        End Enum

        Public ReadOnly Property ProtocolEntry As Long = New Protocol(GetType(MemoryProtocols)).EntryPoint

        <Extension>
        Public Function [TypeOf](remote As IPEndPoint, name As String) As Type
            Dim req As New RequestStream(ProtocolEntry, MemoryProtocols.TypeOf, name)
            Dim ref As TypeInfo = req.LoadObject(Of TypeInfo)
            Return ref.GetType(True)
        End Function

        Public Function ReadValue(name As String) As RequestStream
            Return New RequestStream(ProtocolEntry, MemoryProtocols.Read, name)
        End Function

        Public Function WriteValue(name As String, value As Object) As RequestStream
            Dim json As New Argv(name, value)
            Dim req As New RequestStream(ProtocolEntry, MemoryProtocols.Write, json.GetJson)
            Return req
        End Function

        <Extension>
        Public Function ReadValue(remote As IPEndPoint, name As String, type As Type) As Object
            Dim req As RequestStream = ReadValue(name)
            Dim rep As RequestStream = New AsynInvoke(remote).SendMessage(req)
            Return JsonContract.LoadObject(rep.GetUTF8String, type)
        End Function

        <Extension>
        Public Function ReadValue(Of T)(remote As IPEndPoint, name As String) As T
            Return DirectCast(remote.ReadValue(name, GetType(T)), T)
        End Function

        <Extension>
        Public Function WriteValue(remote As IPEndPoint, name As String, value As Object) As Boolean
            Dim req As RequestStream = WriteValue(name, value)
            Dim rep As RequestStream = New AsynInvoke(remote).SendMessage(req)
            Return rep.Protocol = HTTP_RFC.RFC_OK
        End Function
    End Module
End Namespace
