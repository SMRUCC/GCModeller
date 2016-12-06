#Region "Microsoft.VisualBasic::8e9f5db49afa62bc11f1c2a4c9ad4309, ..\ComputingServices\FileSystem\FileStream\BaseStream.vb"

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

Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ComputingServices.FileSystem.Protocols
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Protocols

Namespace FileSystem.IO

    ''' <summary>
    ''' <see cref="System.IO.FileStream"/><see cref="System.IO.StreamWriter"/>
    ''' </summary>
    Public MustInherit Class BaseStream : Inherits Stream
        Implements IDisposable

        Dim _FileSystem As IPEndPoint

        ''' <summary>
        ''' 远端机器上面的文件系统的访问入口点
        ''' </summary>
        ''' <returns></returns>
        Public Property FileSystem As IPEndPoint
            Get
                Return _FileSystem
            End Get
            Protected Set(value As IPEndPoint)
                _FileSystem = value
            End Set
        End Property

        Sub New()
        End Sub

        Sub New(remote As FileSystem)
            FileSystem = remote.Portal
        End Sub

        Sub New(remote As IPEndPoint)
            FileSystem = remote
        End Sub

        Sub New(remote As String, port As Integer)
            Call Me.New(New IPEndPoint(remote, port))
        End Sub

        Sub New(remote As System.Net.IPEndPoint)
            Call Me.New(New IPEndPoint(remote))
        End Sub

        Public Overrides Function ToString() As String
            Return FileSystem.ToString
        End Function
    End Class
End Namespace
