#Region "Microsoft.VisualBasic::972387fdd3296832e8c2ce0ad5d79fe5, ..\ComputingServices\FileSystem\Protocols\FileStreamInfo.vb"

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
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Net.Protocols.Reflection
Imports Microsoft.VisualBasic.Serialization

Namespace FileSystem.Protocols

    Public Class FileStreamInfo : Inherits FileHandle

        Public Property CanRead As Boolean
        Public Property CanSeek As Boolean
        Public Property CanWrite As Boolean
        Public Property FileHandle As IntPtr
        Public Property IsAsync As Boolean

        Sub New()

        End Sub

        Public Shared Function GetInfo(file As FileStream) As FileStreamInfo
#Disable Warning
            Dim handle As New FileStreamInfo With {
                .HashCode = file.GetHashCode,
                .CanRead = file.CanRead,
                .CanSeek = file.CanSeek,
                .CanWrite = file.CanWrite,
                .FileHandle = file.Handle,
                .IsAsync = file.IsAsync
            }  ' 可能会出现重复的文件名，所以使用这个句柄对象来进行唯一标示
#Enable Warning
            Return handle
        End Function
    End Class

    Public Class SeekArgs : Inherits FileHandle

        Public Property offset As Long
        Public Property origin As Integer

        Sub New(handle As FileHandle)
            Call MyBase.New(handle)
        End Sub

        Sub New()

        End Sub

        Public Function Seek(stream As System.IO.FileStream) As Long
            Dim ori As SeekOrigin = CType(origin, SeekOrigin)
            Return stream.Seek(offset, ori)
        End Function
    End Class

    Public Class LockArgs : Inherits FileHandle

        Public Property Lock As Boolean
        Public Property position As Long
        Public Property length As Long

        Sub New(handle As FileHandle)
            Call MyBase.New(handle)
        End Sub

        Sub New()

        End Sub

        Public Sub LockOrNot(stream As System.IO.FileStream)
            If Lock Then
                Call stream.Lock(position, length)
            Else
                Call stream.Unlock(position, length)
            End If
        End Sub
    End Class
End Namespace
