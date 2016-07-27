#Region "Microsoft.VisualBasic::62e91f5abc3bea9184ad44449d43a4c0, ..\httpd\HTTPServer\SMRUCC.HTTPInternal\Core\HttpServer.vb"

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

Imports System.Collections
Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports Microsoft.VisualBasic.Parallel

Namespace Core

    ''' <summary>
    ''' Internal http server core.
    ''' </summary>
    Public MustInherit Class HttpServer : Implements System.IDisposable

        Protected Is_active As Boolean = True

        ReadOnly _httpListener As TcpListener
        ReadOnly _homeShowOnStart As Boolean = False

        ''' <summary>
        ''' The network data port of this internal http server listen.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property LocalPort As Integer
        ''' <summary>
        ''' Indicates this http server is running status or not. 
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsRunning As Boolean
            Get
                Return Not _httpListener Is Nothing AndAlso _httpListener.Server.IsBound
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="port">The network data port of this internal http server listen.</param>
        ''' <param name="homeShowOnStart"></param>
        Public Sub New(port As Integer, Optional homeShowOnStart As Boolean = False)
            Me._LocalPort = port
            Me._homeShowOnStart = homeShowOnStart
            Me._httpListener = New TcpListener(IPAddress.Any, _LocalPort)
        End Sub

        ''' <summary>
        ''' Running this http server. 
        ''' NOTE: current thread will be blocked at here until the server core is shutdown. 
        ''' (请注意，在服务器开启之后，当前的线程会被阻塞在这里)
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Function Run() As Integer
            Try
                Call _httpListener.Start()
            Catch ex As Exception
                If ex.IsSocketPortOccupied Then
                    Call $"Could not start http services at {NameOf(_LocalPort)}:={_LocalPort}".__DEBUG_ECHO
                    Call ex.ToString.__DEBUG_ECHO
                    Call Console.WriteLine()
                    Call "Program http server thread was terminated.".__DEBUG_ECHO
                    Call Console.WriteLine()
                    Call Console.WriteLine()
                    Call Console.WriteLine()
                Else
                    ex = New Exception(LocalPort, ex)
                    Call ex.PrintException
                    Call App.LogException(ex)
                End If

                Call Pause()

                Return -1
            End Try

            Call Console.WriteLine("Http Server Start listen at " & _httpListener.LocalEndpoint.ToString)
            Call RunTask(AddressOf Me.OpenAPI_HOME)

            While Is_active
                Dim s As TcpClient = _httpListener.AcceptTcpClient()
                Dim processor As HttpProcessor = __httpProcessor(s)
                Dim proc As New Thread(New ThreadStart(AddressOf processor.Process))

                Call $"Process client from {s.Client.RemoteEndPoint.ToString}".__DEBUG_ECHO
                Call proc.Start()
                Call Thread.Sleep(1)
            End While

            Return 0
        End Function

        ''' <summary>
        ''' New HttpProcessor(Client, Me) with {._404Page = "...."}
        ''' </summary>
        ''' <param name="client"></param>
        ''' <returns></returns>
        Protected MustOverride Function __httpProcessor(client As TcpClient) As HttpProcessor

        Private Sub OpenAPI_HOME()
            Call Thread.Sleep(10 * 1000)

            If Environment.OSVersion.Platform = PlatformID.Win32NT AndAlso
                _homeShowOnStart Then
                Dim uri As String = $"http://127.0.0.1:{_LocalPort}/"
                Call Process.Start(uri)
            End If
        End Sub

        ''' <summary>
        ''' Shutdown this internal http server
        ''' </summary>
        Public Sub Shutdown()
            Is_active = False
            Call _httpListener.Stop()
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
        Public MustOverride Sub handlePOSTRequest(p As HttpProcessor, inputData As MemoryStream)

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Me.Is_active = False
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
