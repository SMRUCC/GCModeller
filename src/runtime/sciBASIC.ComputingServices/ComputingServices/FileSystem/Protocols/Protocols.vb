#Region "Microsoft.VisualBasic::77f30e80f19b76467dd9d67b8b2122ee, ..\sciBASIC.ComputingServices\ComputingServices\FileSystem\Protocols\Protocols.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports System.IO
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Net.Protocols.Reflection
Imports Microsoft.VisualBasic.Serialization

Namespace FileSystem.Protocols

    ''' <summary>
    ''' 
    ''' </summary>
    Public Module API

        Public ReadOnly Property ProtocolEntry As Long =
            New Protocol(GetType(FileSystemAPI)).EntryPoint

        Public Function OpenHandle(file As String, mode As FileMode, access As FileAccess) As RequestStream
            Dim params As New FileOpen With {
                .FileName = file,
                .Mode = mode,
                .Access = access
            }
            Return RequestStream.CreateProtocol(ProtocolEntry, FileSystemAPI.OpenHandle, params)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pos">-100表示set</param>
        ''' <param name="handle"></param>
        ''' <returns></returns>
        Public Function GetSetReadPosition(pos As Long, handle As FileHandle) As RequestStream
            Dim args As New FileStreamPosition(handle) With {.Position = pos}
            Return RequestStream.CreateProtocol(ProtocolEntry, FileSystemAPI.FilePosition, args)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="handle"></param>
        ''' <returns></returns>
        Public Function GetSetLength(length As Long, handle As FileHandle) As RequestStream
            Dim args As New FileStreamPosition(handle) With {.Position = length}
            Return RequestStream.CreateProtocol(ProtocolEntry, FileSystemAPI.FileStreamLength, args)
        End Function
    End Module
End Namespace
