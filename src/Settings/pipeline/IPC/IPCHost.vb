#Region "Microsoft.VisualBasic::85d6b911439c07e7b7c4a1ca933a9be0, pipeline\IPC\IPCHost.vb"

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
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: Run
    ' 
    '     Sub: Delete, (+2 Overloads) Dispose, Register
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Net.Protocols.Reflection
Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports CliPipeline = Microsoft.VisualBasic.CommandLine

Public Class IPCHost : Implements IDisposable

    ReadOnly host As TcpServicesSocket
    ReadOnly resources As New Dictionary(Of String, Resource)
    ReadOnly protocol As ProtocolHandler

    Sub New(port As Integer)
        protocol = New ProtocolHandler(New ProtocolRouter(Me))
        host = New TcpServicesSocket(AddressOf protocol.HandleRequest, port)
    End Sub

    Public Function Run() As Integer
        Return host.Run
    End Function

    ''' <summary>
    ''' 主要是利用这个函数在内存中打开一个内存映射文件
    ''' 写数据操作是由上游程序来完成的
    ''' </summary>
    ''' <param name="name"></param>
    ''' <param name="size"></param>
    ''' <param name="type"></param>
    Public Sub Register(name$, size&, type As TypeInfo)
        Dim resource As New Resource With {
            .contentType = type,
            .fullName = name,
            .size = 0
        }

        Using buffer = CliPipeline.OpenForWrite($"memory:/{name}", size)
        End Using

        resource.Write()
        resources(name) = resource
    End Sub

    Public Sub Delete(name As String)
        If resources.ContainsKey(name) Then
            Call resources.Remove(name)
        End If
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' 要检测冗余调用

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)。
                Call host.Dispose()
            End If

            ' TODO: 释放未托管资源(未托管对象)并在以下内容中替代 Finalize()。
            ' TODO: 将大型字段设置为 null。
        End If
        disposedValue = True
    End Sub

    ' TODO: 仅当以上 Dispose(disposing As Boolean)拥有用于释放未托管资源的代码时才替代 Finalize()。
    'Protected Overrides Sub Finalize()
    '    ' 请勿更改此代码。将清理代码放入以上 Dispose(disposing As Boolean)中。
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Visual Basic 添加此代码以正确实现可释放模式。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 请勿更改此代码。将清理代码放入以上 Dispose(disposing As Boolean)中。
        Dispose(True)
        ' TODO: 如果在以上内容中替代了 Finalize()，则取消注释以下行。
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
