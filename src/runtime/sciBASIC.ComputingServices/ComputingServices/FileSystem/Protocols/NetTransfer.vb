#Region "Microsoft.VisualBasic::1c9ceb1759fb47880f8050db75993dff, ..\ComputingServices\FileSystem\Protocols\NetTransfer.vb"

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

Imports System.IO
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Net.Protocols.Reflection
Imports Microsoft.VisualBasic.Serialization

Namespace FileSystem.Protocols

    Public Module NetTransfer

        ''' <summary>
        ''' 从本地上传文件
        ''' </summary>
        ''' <param name="local"></param>
        ''' <param name="destination"></param>
        ''' <param name="remote"></param>
        Public Sub Upload(local As String, destination As String, remote As IPEndPoint)
            Using file As New IO.RemoteFileStream(destination, FileMode.OpenOrCreate, remote)
                Dim localFile As New FileStream(local, FileMode.Open)
                Call Transfer(localFile, file, 1024 * 1024)
            End Using
        End Sub

        Public Sub Transfer(source As Stream, target As Stream, bufLen As Integer)
            Dim buffer As Byte() = New Byte(bufLen - 1) {}

            Do While source.Position < source.Length
                Dim d As Integer = (source.Length - source.Position) - buffer.Length

                If d < 0 Then ' 注意：d 是负值的
                    buffer = New Byte(-d - 1) {}
                End If

                Call source.Read(buffer, Scan0, buffer.Length)
                Call target.Write(buffer, Scan0, buffer.Length)
                Call target.Flush()
            Loop
        End Sub

        Public Sub Download(destination As String, local As String, remote As IPEndPoint)
            Using file As New IO.RemoteFileStream(destination, FileMode.OpenOrCreate, remote)
                Dim localFile As New FileStream(local, FileMode.OpenOrCreate)
                Call Transfer(file, localFile, 1024 * 1024)
            End Using
        End Sub
    End Module
End Namespace
