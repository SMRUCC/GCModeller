#Region "Microsoft.VisualBasic::b2f3db263e449569daa720562bc6d13a, src\Flute\Http\HttpServer.vb"

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


    ' Code Statistics:

    '   Total Lines: 227
    '    Code Lines: 114 (50.22%)
    ' Comment Lines: 81 (35.68%)
    '    - Xml Docs: 70.37%
    ' 
    '   Blank Lines: 32 (14.10%)
    '     File Size: 8.90 KB


    '     Class HttpServer
    ' 
    '         Properties: BufferSize, isRunning, localPort
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Run, ToString
    ' 
    '         Sub: accept, (+2 Overloads) Dispose, RunTask, Shutdown
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Net
Imports System.Net.Sockets
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Flute.Http.Configurations
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Flute.Http.Core.WebSocket
Imports Microsoft.VisualBasic.Language.Default
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
        Dim _threadPool As Integer
        Dim _accept_workers As Integer = 0

        ''' <summary>
        ''' semaphore used to limit the number of concurrent connection handling tasks
        ''' instead of mutating the global ThreadPool size.
        ''' </summary>
        Dim _connectionSemaphore As SemaphoreSlim

        Protected Friend ReadOnly _settings As Configuration
        Protected Friend ReadOnly _httpListener As TcpListener

        ''' <summary>
        ''' The network data port of this internal http server listen.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property localPort As Integer
        Public Property BufferSize As Integer = 4096

        ''' <summary>
        ''' the websocket connection manager of current http server, the RFC6455
        ''' websocket upgrade handshake request will be accepted by this http
        ''' server only when an application message handler has been registered
        ''' into this connection manager via its route table.
        ''' </summary>
        ''' <returns>
        ''' this property value is always available, an empty routing table just
        ''' means that no websocket endpoint is published on current http server.
        ''' </returns>
        ''' <example>
        ''' Call server.WebSocket.Route("/ws/echo", WebSocketHandler.Echo)
        ''' </example>
        Public ReadOnly Property WebSocket As New WebSocketManager

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
        Public Sub New(port%, Optional threads% = -1, Optional configs As Configuration = Nothing)
            Static defaultThreads As [Default](Of Integer) = (LQuerySchedule.CPU_NUMBER).AsDefault(Function(n) CInt(n) <= 0)

            Me._settings = If(configs, New Configuration)
            Me._localPort = port
            Me._httpListener = New TcpListener(IPAddress.Any, _localPort)
            Me._threadPool = threads Or defaultThreads
            Me._BufferSize = Val(App.GetVariable("httpserver.buffer_size"))
            Me._BufferSize = If(BufferSize <= 0, 4096, BufferSize)
            Me._connectionSemaphore = New SemaphoreSlim(_threadPool, _threadPool)

            Call $"Web server threads_pool_size={_threadPool}, buffer_size={BufferSize}bytes".info(_settings.silent)
        End Sub

        ''' <summary>
        ''' Running this http server. 
        ''' NOTE: current thread will be blocked at here until the server core is shutdown. 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' (请注意，在服务器开启之后，当前的线程会被阻塞在这里)
        ''' </remarks>
        Public Overridable Function Run() As Integer Implements ITaskDriver.Run
            Is_active = False

            Try
                _httpListener.Start()
                Is_active = True
                Call $"Http Server Start listen at {_httpListener.LocalEndpoint.ToString}".info(_settings.silent)
            Catch ex As Exception When ex.IsSocketPortOccupied
                Call $"Could not start http services at port {_localPort}: socket port is occupied.".debug
                Call App.LogException(ex)
                Return 500
            Catch ex As Exception
                ex = New Exception(CStr(localPort), ex)

                Call ex.PrintException
                Call App.LogException(ex)
                Return 500
            End Try

            ' if the listener failed to start, Is_active stays False and we exit here
            If Not Is_active Then
                Return 500
            End If

            While Is_active
                ' accept() blocks on the connection semaphore internally, so the
                ' number of concurrently handled connections never exceeds the pool size.
                Call accept()
            End While

            Return 0
        End Function

        ''' <summary>
        ''' 向网页服务器内部的线程池之中添加执行任务
        ''' </summary>
        ''' <param name="task"></param>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Sub RunTask(task As WaitCallback)
            Interlocked.Increment(_accept_workers)
            ThreadPool.QueueUserWorkItem(
                Sub(o)
                    Try
                        Call task(o)
                    Finally
                        Call _connectionSemaphore.Release()
                        Interlocked.Decrement(_accept_workers)
                    End Try
                End Sub)
        End Sub

        Private Sub accept()
            Try
                ' 20250517 do not put the tcp client accept into the thread pool,
                ' or the worker will be stucked laterly
                Dim s As TcpClient = _httpListener.AcceptTcpClient
                Dim processor As HttpProcessor = getHttpProcessor(s, BufferSize)

                Call $"Process client from {s.Client.RemoteEndPoint.ToString}".debug(mute:=_settings.silent)
                ' acquire a semaphore slot before scheduling the handler; the slot
                ' will be released by RunTask once processing completes.
                Call _connectionSemaphore.Wait()

                Try
                    Call RunTask(Sub(o) Call processor.Process())
                Catch ex As Exception
                    ' if RunTask itself throws before the work item is queued,
                    ' release the slot we just acquired to avoid a leak.
                    Call _connectionSemaphore.Release()
                    Throw
                End Try
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

            Try
                _httpListener.Stop()
            Catch ex As Exception
                Call App.LogException(ex)
            End Try

            ' notify all of the active websocket clients that this server is going
            ' away, otherwise those long-live connections will block the shutdown
            ' waiting loop below until the timeout deadline is reached.
            Try
                Call WebSocket.CloseAll()
            Catch ex As Exception
                Call App.LogException(ex)
            End Try

            ' wait for active workers to finish (with a reasonable timeout)
            ' so in-flight requests are not abruptly terminated.
            ' note: if Shutdown is called from within a worker thread (e.g.
            ' the /ctrl/kill handler), that thread itself holds one
            ' _accept_workers count which it cannot release until this method
            ' returns. So we wait for the count to drop to at most 1 (the
            ' calling worker itself) rather than 0.
            Dim deadline As DateTime = DateTime.UtcNow.AddSeconds(10)

            Do While _accept_workers > 1 AndAlso DateTime.UtcNow < deadline
                Call Thread.Sleep(50)
            Loop
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
            Return $"[http://localhost:{localPort}] http_workers: {_accept_workers}"
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call Shutdown()

                    ' release the connection semaphore to free its unmanaged
                    ' wait handle. CurrentVersion (net6+) SemaphoreSlim.Dispose()
                    ' is safe to call after Shutdown.
                    _connectionSemaphore?.Dispose()
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
