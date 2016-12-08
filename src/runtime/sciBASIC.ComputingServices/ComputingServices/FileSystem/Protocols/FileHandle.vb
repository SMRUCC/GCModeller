#Region "Microsoft.VisualBasic::29565e0839a1e3ff54d3cf4e55191e31, ..\sciBASIC.ComputingServices\ComputingServices\FileSystem\Protocols\FileHandle.vb"

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

Namespace FileSystem.Protocols

    ''' <summary>
    ''' The file handle object on the remote server machine.
    ''' (在远端服务器上面的文件句柄对象)
    ''' </summary>
    Public Class FileHandle

        ''' <summary>
        ''' The file location on the remote file system.
        ''' </summary>
        ''' <returns></returns>
        Public Property FileName As String
        ''' <summary>
        ''' The hash code value on the remote services program.
        ''' </summary>
        ''' <returns></returns>
        Public Property HashCode As Integer

        Sub New()
        End Sub

        Sub New(handle As FileHandle)
            Me.FileName = handle.FileName
            Me.HashCode = handle.HashCode
        End Sub

        Public Overrides Function ToString() As String
            Return Handle
        End Function

        ''' <summary>
        ''' 远程机器上面唯一标示的文件句柄值
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Handle As String
            Get
                Return $"{HashCode}+{FileName.ToFileURL}"
            End Get
        End Property
    End Class
End Namespace
