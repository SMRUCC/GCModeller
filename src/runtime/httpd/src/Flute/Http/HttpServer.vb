#Region "Microsoft.VisualBasic::dac52f95f1e43b14f06983be432eb77b, src\Flute\Http\HttpServer.vb"

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

    '   Total Lines: 314
    '    Code Lines: 141 (44.90%)
    ' Comment Lines: 134 (42.68%)
    '    - Xml Docs: 64.93%
    ' 
    '   Blank Lines: 39 (12.42%)
    '     File Size: 13.25 KB


    '     Class HttpServer
    ' 
    '         Properties: BufferSize, isRunning, localPort, LongPoll, WebSocket
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
Imports Flute.Http.Core.LongPoll
Imports Flute.Http.Core.WebSocket
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
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
        ''' The network data port that this internal http server is listening on.
        ''' </summary>
        ''' <returns>the local tcp port bound by the listener.</returns>
        Public ReadOnly Property localPort As Integer
        ''' <summary>
        ''' the size of the read/write buffer (in bytes) used when streaming
        ''' request and response data.
        ''' </summary>
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
        ''' the long polling connection manager of current http server, a http GET
        ''' request will be treated as a long poll request and blocked for waiting
        ''' a push operation only when an application handler has been registered
        ''' into this connection manager via its route table.
        ''' </summary>
        ''' <returns>
        ''' this property value is always available, an empty routing table just
        ''' means that no long poll endpoint is published on current http server.
        ''' </returns>
        ''' <example>
        ''' Call server.LongPoll.Route("/poll/messages")
        ''' </example>
        Public ReadOnly Property LongPoll As New LongPollManager

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
        ''' create a new http server core listening on the given port, bound to an
        ''' optional configuration and a worker thread pool size.
        ''' </summary>
        ''' <param name="port">The network data port of this internal http server listen.</param>
        ''' <param name="threads%">the size of the connection worker pool; a value &lt;= 0 uses the CPU core count.</param>
        ''' <param name="configs">the optional server wide configuration.</param>
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
        ''' create a new <see cref="HttpProcessor"/> bound to the accepted tcp
        ''' client and this server, with the given read buffer size.
        ''' </summary>
        ''' <param name="client">the accepted tcp client for the incoming connection.</param>
        ''' <param name="bufferSize%">the read buffer size (in bytes) for the processor.</param>
        ''' <returns>a new <see cref="HttpProcessor"/> instance ready to process the request.</returns>
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

            ' wake up all of the pending long poll connections, otherwise those
            ' blocked worker threads will block the shutdown waiting loop below
            ' until the long poll timeout deadline is reached.
            Try
                Call LongPoll.CloseAll()
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
        ''' handle a parsed GET request for the given processor. derived servers
        ''' must implement the route dispatch, write the response through the
        ''' processor, and finally call its <see cref="HttpProcessor.Dispose"/>.
        ''' </summary>
        ''' <param name="p">the http processor that carried the GET request.</param>
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
        ''' <summary>
        ''' handle a parsed POST request for the given processor, with its decoded body.
        ''' </summary>
        ''' <param name="p">the http processor that carried the POST request.</param>
        ''' <param name="inputData$">the decoded POST body string.</param>
        Public MustOverride Sub handlePOSTRequest(p As HttpProcessor, inputData$)
        ''' <summary>
        ''' handle any http method other than GET/POST (e.g. PUT, DELETE, OPTIONS)
        ''' for the given processor.
        ''' </summary>
        ''' <param name="p">the http processor that carried the request.</param>
        Public MustOverride Sub handleOtherMethod(p As HttpProcessor)

        ''' <summary>
        ''' the string representation of this server: its local address and the
        ''' number of currently active http worker threads.
        ''' </summary>
        ''' <returns>a "[http://localhost:port] http_workers: n" description string.</returns>
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
