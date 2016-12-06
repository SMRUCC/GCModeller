#Region "Microsoft.VisualBasic::deb21853d9fbaf87d411a66b44ffea7d, ..\ComputingServices\Taskhost.d\Protocols.vb"

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
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.ComputingServices.ComponentModel
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Abstract
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Net.Protocols.Reflection

Namespace TaskHost

    ''' <summary>
    ''' 分布式计算之中的远端调用的堆栈协议
    ''' </summary>
    Public Module Protocols

        ''' <summary>
        ''' Task remotes protocols
        ''' </summary>
        Public Enum TaskProtocols As Long

            ''' <summary>
            ''' Free a object pointer on the remote machine.(释放掉某一个资源)
            ''' </summary>
            Free = -1000L

#Region "Task"
            ''' <summary>
            ''' Invoke a function remotely
            ''' </summary>
            Invoke = 0L
            ''' <summary>
            ''' Invoke a function as a linq data source.
            ''' </summary>
            InvokeLinq
            ''' <summary>
            ''' Gets the portal of the remote FileSystem
            ''' </summary>
            RemoteFileSystem
#End Region

#Region "LINQ supports"
            ''' <summary>
            ''' Linq data source move next
            ''' </summary>
            MoveNext
            ''' <summary>
            ''' Resets the iterator of the remote linq source
            ''' </summary>
            Reset
            ''' <summary>
            ''' The remote linq source reads done! Exit the iterator function.
            ''' </summary>
            ReadsDone = -1000L
#End Region
        End Enum

        Public ReadOnly Property ProtocolEntry As Long =
            New Protocol(GetType(TaskProtocols)).EntryPoint

        ''' <summary>
        ''' Resets remote data source by <see cref="IEnumerator.Reset"/>
        ''' </summary>
        ''' <returns></returns>
        Public Function LinqReset() As RequestStream
            Return New RequestStream(ProtocolEntry, TaskProtocols.Reset)
        End Function

        <Extension> Public Function GetPortal(Of Tsvr As IServicesSocket)(master As IMasterBase(Of Tsvr)) As IPEndPoint
            Dim ip As String = If(EnvironmentLocal, AsynInvoke.LocalIPAddress, GetMyIPAddress())
            Dim port As Integer = master.__host.LocalPort
            Return New IPEndPoint(ip, port)
        End Function

        Public Function Shell(exe As String, args As String) As Integer
            Dim proc As New IORedirect(exe, args)
            Return proc.Run
        End Function
    End Module
End Namespace
