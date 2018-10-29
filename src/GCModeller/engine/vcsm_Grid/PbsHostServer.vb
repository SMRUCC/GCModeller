#Region "Microsoft.VisualBasic::2a136bca7b7f0a6dfece5fb44b9f77f6, vcsm_Grid\PbsHostServer.vb"

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

    ' Class PbsHostServer
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: __protocolImplementer, StartServices
    ' 
    '     Sub: (+2 Overloads) Dispose, InternalThreadsDetection, RefreshConsole
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Protocols
Imports LANS.SystemsBiology.GCModeller.Framework.Kernel_Driver.GridPBS

''' <summary>
''' Pbs任务提交和数据处理服务器
''' </summary>
''' <remarks></remarks>
Public Class PbsHostServer : Implements IDisposable

    Dim _ServicesSocket As TcpSynchronizationServicesSocket
    Dim _PbsDriver As PBS = New PBS
    Dim _Running As Boolean = False

    Sub New()
        '_ServicesSocket = New TcpSynchronizationServicesSocket(
        '    DataArrivalEventHandler:=AddressOf __protocolImplementer,
        '    LocalPort:=PbsProtocol.PBS_CLUSTER_SERVICES_PORT)
    End Sub

    ''' <summary>
    ''' 当节点进程以附属节点的形式启动的时候，会根据命令行参数的网络地址主动询问主节点，然后得到一个动态分配的ID编号，注册进入节点网络之中
    ''' 
    ''' </summary>
    ''' <param name="Node"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function __protocolImplementer(uid As Long, request As RequestStream, Node As System.Net.IPEndPoint) As RequestStream

    End Function

    ''' <summary>
    ''' 计算线程的状态检测线程，每隔一段时间刷新一次线程的数据
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InternalThreadsDetection()

        Do While _Running
            Call Threading.Thread.Sleep(1000)
            Call RefreshConsole()
        Loop
    End Sub

    Private Sub RefreshConsole()
        Dim sBuilder As StringBuilder = New StringBuilder(1024)
    End Sub

    Public Function StartServices() As Integer
        _Running = True

        Try
            Call New Threading.Thread(AddressOf InternalThreadsDetection).Start()
            Call _ServicesSocket.Run()
        Catch ex As Exception
            Call Console.WriteLine(ex.ToString)
            Return -1
        End Try

        Return 0
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                _Running = False
                Call _ServicesSocket.Free()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
