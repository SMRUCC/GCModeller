﻿#Region "Microsoft.VisualBasic::319e955aa8f25d9aadbf64f96dca23bb, WebCloud\SMRUCC.HTTPInternal\Core\HttpServer.vb"

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

'     Class HttpServer
' 
'         Properties: BufferSize, IsRunning, localhost, LocalPort
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: getProcessor, Run
' 
'         Sub: __accept, (+2 Overloads) Dispose, OpenAPI_HOME, RunTask, Shutdown
' 
' 
' /********************************************************************************/

#End Region

Imports System.Net
Imports System.Net.Sockets
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Parallel.Linq

Namespace Core

    ''' <summary>
    ''' Internal http server core.
    ''' </summary>
    Public MustInherit Class HttpServer
        Implements IDisposable
        Implements ITaskDriver

        Protected Is_active As Boolean = True

        ''' <summary>
        ''' 处理连接的线程池
        ''' </summary>
        Protected Friend ReadOnly _threadPool As Threads.ThreadPool
        Protected Friend ReadOnly _httpListener As TcpListener
        Protected Friend ReadOnly _silent As Boolean = False

        ''' <summary>
        ''' The network data port of this internal http server listen.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property localPort As Integer
        Public Property BufferSize As Integer = 4096

        ''' <summary>
        ''' Indicates this http server is running status or not. 
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property isRunning As Boolean
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Not _httpListener Is Nothing AndAlso _httpListener.Server.IsBound
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="port">The network data port of this internal http server listen.</param>
        Public Sub New(port%, Optional threads% = -1, Optional silent As Boolean = False)
            Static defaultThreads As [Default](Of Integer) = (LQuerySchedule.Recommended_NUM_THREADS * 8).AsDefault(Function(n) CInt(n) <= 0)

            Me._localPort = port
            Me._httpListener = New TcpListener(IPAddress.Any, _localPort)
            Me._threadPool = New Threads.ThreadPool(threads Or defaultThreads)
            Me._BufferSize = Val(App.GetVariable("httpserver.buffer_size"))
            Me._BufferSize = If(BufferSize <= 0, 4096, BufferSize)
            Me._silent = silent

            Call $"Web server threads_pool_size={_threadPool.NumOfThreads}, buffer_size={BufferSize}bytes".__INFO_ECHO(silent)
        End Sub

        ''' <summary>
        ''' Running this http server. 
        ''' NOTE: current thread will be blocked at here until the server core is shutdown. 
        ''' (请注意，在服务器开启之后，当前的线程会被阻塞在这里)
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Function Run() As Integer Implements ITaskDriver.Run
            Is_active = False

            Try
                _httpListener.Start(10240)
                _threadPool.Start()

                Is_active = True
            Catch ex As Exception When ex.IsSocketPortOccupied
                Call $"Could not start http services at {NameOf(_localPort)}:={_localPort}".__DEBUG_ECHO
                Call ex.ToString.__DEBUG_ECHO
                Call App.LogException(ex)
                Call Console.WriteLine()
                Call "Program http server thread was terminated.".__DEBUG_ECHO
                Call Console.WriteLine()
                Call Console.WriteLine()
                Call Console.WriteLine()

                Return 500
            Catch ex As Exception
                ex = New Exception(CStr(localPort), ex)

                Call ex.PrintException
                Call App.LogException(ex)

                Return 500
            Finally
                Call $"Http Server Start listen at {_httpListener.LocalEndpoint.ToString}".__INFO_ECHO(silent:=_silent)
            End Try

            While Is_active
                If Not _threadPool.FullCapacity Then
                    Call _threadPool.RunTask(AddressOf accept)
                Else
                    Thread.Sleep(1)
                End If
            End While

            Return 0
        End Function

        ''' <summary>
        ''' 向网页服务器内部的线程池之中添加执行任务
        ''' </summary>
        ''' <param name="task"></param>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub RunTask(task As Action)
            Call _threadPool.RunTask(task)
        End Sub

        Private Sub accept()
            Try
                Dim s As TcpClient = _httpListener.AcceptTcpClient
                Dim processor As HttpProcessor = getHttpProcessor(s, BufferSize)

                Call $"Process client from {s.Client.RemoteEndPoint.ToString}".__DEBUG_ECHO(mute:=_silent)
                Call Time(AddressOf processor.Process)
            Catch ex As Exception
                Call App.LogException(ex)
            End Try
        End Sub

        ''' <summary>
        ''' New HttpProcessor(Client, Me) with {._404Page = "...."}
        ''' </summary>
        ''' <param name="client"></param>
        ''' <returns></returns>
        Protected MustOverride Function getHttpProcessor(client As TcpClient, bufferSize%) As HttpProcessor

        ''' <summary>
        ''' Shutdown this internal http server
        ''' </summary>
        Public Sub Shutdown()
            Is_active = False
            _httpListener.Stop()
            _threadPool.Dispose()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="p"></param>
        ''' <example>
        ''' 
        ''' If p.http_url.Equals("/Test.png") Then
        '''     Dim fs As Stream = File.Open("../../Test.png", FileMode.Open)
        '''
        '''     p.writeSuccess("image/png")
        '''     fs.CopyTo(p.outputStream.BaseStream)
        '''     p.outputStream.BaseStream.Flush()
        ''' End If
        '''
        '''  Console.WriteLine("request: {0}", p.http_url)
        ''' 
        '''  p.writeSuccess()
        '''  p.outputStream.WriteLine("&lt;html>&lt;body>&lt;h1>Shoal SystemsBiology Shell Language&lt;/h1>")
        '''  p.outputStream.WriteLine("Current Time: " &amp; DateTime.Now.ToString())
        '''  p.outputStream.WriteLine("url : {0}", p.http_url)
        '''
        '''  p.outputStream.WriteLine("&lt;form method=post action=/local_wiki>")
        '''  p.outputStream.WriteLine("&lt;input type=text name=SearchValue value=Keyword>")
        '''  p.outputStream.WriteLine("&lt;input type=submit name=Invoker value=""Search"">")
        '''  p.outputStream.WriteLine("&lt;/form>")
        ''' 
        ''' </example>
        Public MustOverride Sub handleGETRequest(p As HttpProcessor)
        Public MustOverride Sub handlePOSTRequest(p As HttpProcessor, inputData$)
        Public MustOverride Sub handleOtherMethod(p As HttpProcessor)

        Public Overrides Function ToString() As String
            Return $"http://localhost:{localPort}"
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call Shutdown()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
