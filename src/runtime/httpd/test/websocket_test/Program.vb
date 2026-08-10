Imports System.Linq
Imports System.Net
Imports System.Net.Sockets
Imports System.Net.WebSockets
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports Flute.Http.Configurations
Imports Flute.Http.Core
Imports Flute.Http.Core.WebSocket

''' <summary>
''' the in-process integration test of the RFC6455 websocket protocol support
''' of the Flute http server module. this test suite hosts a real Flute http
''' server on a random local tcp port, and then verifies the server behaviour
''' with the ``System.Net.WebSockets.ClientWebSocket`` standard client.
''' </summary>
Module Program

    Dim s_results As New List(Of TestResult)
    Dim s_port As Integer = 0
    Dim s_server As HttpSocket = Nothing

    Structure TestResult
        Dim Name As String
        Dim Passed As Boolean
        Dim Detail As String
    End Structure

    Sub Main()
        Console.WriteLine(New String("="c, 70))
        Console.WriteLine("  Flute WebSocket(RFC6455) Integration Test")
        Console.WriteLine(New String("="c, 70))
        Console.WriteLine()

        s_port = FindAvailablePort()

        Console.WriteLine($"  Test port:  {s_port}")
        Console.WriteLine()

        Call StartServer()

        ' give the server a moment to bind the listening port
        Call Thread.Sleep(1200)

        Console.WriteLine(New String("-"c, 70))
        Console.WriteLine("  Running Tests")
        Console.WriteLine(New String("-"c, 70))
        Console.WriteLine()

        Try
            RunTest("Handshake + text echo", AddressOf TestTextEcho).Wait()
            RunTest("Binary message echo", AddressOf TestBinaryEcho).Wait()
            RunTest("Fragmented message re-assembly", AddressOf TestFragmented).Wait()
            RunTest("Large message (256KB) round trip", AddressOf TestLargeMessage).Wait()
            RunTest("Ping -> Pong keep alive", AddressOf TestPingPong).Wait()
            RunTest("Sub-protocol negotiation", AddressOf TestSubProtocol).Wait()
            RunTest("Close handshake", AddressOf TestCloseHandshake).Wait()
            RunTest("Broadcast to multiple clients", AddressOf TestBroadcast).Wait()
            RunTest("Path routing to different handlers", AddressOf TestPathRouting).Wait()
            RunTest("Unknown ws path falls back to http 404", AddressOf TestUnknownPath).Wait()
            RunTest("Regular http request still works", AddressOf TestRegularHttp).Wait()
        Catch ex As Exception
            Console.WriteLine($"[FATAL] Test suite crashed: {ex.Message}")
        End Try

        Call PrintReport()
        Call Cleanup()

        Dim failed As Integer = s_results.Where(Function(r) Not r.Passed).Count

        Call Environment.Exit(If(failed > 0, 1, 0))
    End Sub

#Region "Test cases"

    ''' <summary>
    ''' the most basic scenario: the handshake should be completed successfully
    ''' and an utf8 text message should be echoed back by the server.
    ''' </summary>
    Async Function TestTextEcho() As Task
        Using client As ClientWebSocket = Await Connect("/ws/echo")
            Assert(client.State = WebSocketState.Open, "The websocket connection should be opened after the handshake.")

            Await SendText(client, "hello flute")

            Dim echo As String = Await ReceiveText(client)

            Assert(echo = "hello flute", $"The server should echo back the text message, but got '{echo}'.")
        End Using
    End Function

    ''' <summary>
    ''' the raw binary payload must be transferred without any modification
    ''' </summary>
    Async Function TestBinaryEcho() As Task
        Using client As ClientWebSocket = Await Connect("/ws/echo")
            Dim payload As Byte() = Enumerable.Range(0, 512).Select(Function(i) CByte(i Mod 256)).ToArray

            Await client.SendAsync(New ArraySegment(Of Byte)(payload), WebSocketMessageType.Binary, True, CancellationToken.None)

            Dim echo As Byte() = Await ReceiveBinary(client)

            Assert(echo.Length = payload.Length, $"The echo payload size should be {payload.Length}, but got {echo.Length}.")
            Assert(echo.SequenceEqual(payload), "The echo binary payload should be identical with the sent payload.")
        End Using
    End Function

    ''' <summary>
    ''' an application message which is split into the multiple data frame
    ''' fragments must be re-assembled by the server before it is dispatched
    ''' to the application message handler.
    ''' </summary>
    Async Function TestFragmented() As Task
        Using client As ClientWebSocket = Await Connect("/ws/echo")
            Dim chunks As String() = {"frag-one|", "frag-two|", "frag-three"}

            ' send an application message via 3 data frame fragments, only the
            ' last fragment carrys the FIN flag.
            For i As Integer = 0 To chunks.Length - 1
                Dim buffer As Byte() = Encoding.UTF8.GetBytes(chunks(i))
                Dim last As Boolean = (i = chunks.Length - 1)

                Await client.SendAsync(New ArraySegment(Of Byte)(buffer), WebSocketMessageType.Text, last, CancellationToken.None)
            Next

            Dim echo As String = Await ReceiveText(client)
            Dim expected As String = String.Join("", chunks)

            Assert(echo = expected, $"The fragmented message should be re-assembled as '{expected}', but got '{echo}'.")
        End Using
    End Function

    ''' <summary>
    ''' verify the extended payload length(the 16bits and the 64bits form) encoding
    ''' </summary>
    Async Function TestLargeMessage() As Task
        Using client As ClientWebSocket = Await Connect("/ws/echo")
            ' 256KB payload requires the 64bits extended payload length field
            Dim text As New String("x"c, 256 * 1024)

            Await SendText(client, text)

            Dim echo As String = Await ReceiveText(client)

            Assert(echo.Length = text.Length, $"The large message size should be {text.Length}, but got {echo.Length}.")
            Assert(echo = text, "The large message content should be transferred without any modification.")
        End Using
    End Function

    ''' <summary>
    ''' the server must reply a pong control frame for a ping probe, the
    ''' ClientWebSocket handles the ping/pong exchange internally, so the
    ''' connection should be still alive after the keep alive interval.
    ''' </summary>
    Async Function TestPingPong() As Task
        Using client As ClientWebSocket = Await Connect("/ws/echo")
            ' the message exchange should still work after a while, which proves
            ' that the control frame handling never breaks the data frame loop.
            Await SendText(client, "before")
            Assert(Await ReceiveText(client) = "before", "The message before the keep alive probe should be echoed.")

            Await Task.Delay(1500)

            Await SendText(client, "after")
            Assert(Await ReceiveText(client) = "after", "The connection should be still alive after the keep alive probe.")
            Assert(client.State = WebSocketState.Open, "The websocket connection should still be opened.")
        End Using
    End Function

    ''' <summary>
    ''' the server should pick the first client offered sub-protocol which is
    ''' also supported by the server configuration.
    ''' </summary>
    Async Function TestSubProtocol() As Task
        Dim client As New ClientWebSocket()

        ' ``chat`` is configured as a supported sub-protocol of the test server,
        ' and the ``unknown-protocol`` should be ignored by the server.
        Call client.Options.AddSubProtocol("unknown-protocol")
        Call client.Options.AddSubProtocol("chat")

        Using client
            Await client.ConnectAsync(New Uri($"ws://localhost:{s_port}/ws/echo"), CancellationToken.None)

            Assert(client.State = WebSocketState.Open, "The handshake with the sub-protocol offer should be completed.")
            Assert(client.SubProtocol = "chat", $"The negotiated sub-protocol should be 'chat', but got '{client.SubProtocol}'.")
        End Using
    End Function

    ''' <summary>
    ''' the RFC6455 close handshake should be completed gracefully
    ''' </summary>
    Async Function TestCloseHandshake() As Task
        Using client As ClientWebSocket = Await Connect("/ws/echo")
            Await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", CancellationToken.None)

            Assert(client.State = WebSocketState.Closed, $"The connection state should be closed, but got {client.State}.")
            Assert(client.CloseStatus = WebSocketCloseStatus.NormalClosure,
                   $"The close status should be a normal closure, but got {client.CloseStatus}.")
        End Using
    End Function

    ''' <summary>
    ''' a message which is pushed via the connection manager should be delivered
    ''' to all of the active connections on the target url path.
    ''' </summary>
    Async Function TestBroadcast() As Task
        Dim clients As ClientWebSocket() = {
            Await Connect("/ws/chat"),
            Await Connect("/ws/chat"),
            Await Connect("/ws/chat")
        }

        Try
            ' wait for all of the connections to be registered into the manager
            Await Task.Delay(500)

            Assert(s_server.WebSocket.GetConnections("/ws/chat").Length = 3,
                   $"The connection manager should keep tracking of 3 connections, but got {s_server.WebSocket.GetConnections("/ws/chat").Length}.")

            Dim delivered As Integer = s_server.WebSocket.Broadcast("/ws/chat", "broadcast-message")

            Assert(delivered = 3, $"The broadcast message should be delivered to 3 clients, but got {delivered}.")

            For Each client As ClientWebSocket In clients
                Dim message As String = Await ReceiveText(client)
                Assert(message = "broadcast-message", $"Each client should receive the broadcast message, but got '{message}'.")
            Next
        Finally
            For Each client As ClientWebSocket In clients
                Try
                    Call client.Dispose()
                Catch
                End Try
            Next
        End Try
    End Function

    ''' <summary>
    ''' the websocket connection should be routed to its corresponding
    ''' application message handler via the requested url path.
    ''' </summary>
    Async Function TestPathRouting() As Task
        Using client As ClientWebSocket = Await Connect("/ws/upper")
            Await SendText(client, "make me loud")

            Dim reply As String = Await ReceiveText(client)

            Assert(reply = "MAKE ME LOUD", $"The '/ws/upper' endpoint should reply an upper case text, but got '{reply}'.")
        End Using
    End Function

    ''' <summary>
    ''' a websocket handshake request on an unpublished url path should be
    ''' rejected by the regular http request handler instead of being upgraded.
    ''' </summary>
    Async Function TestUnknownPath() As Task
        Dim client As New ClientWebSocket()
        Dim rejected As Boolean = False

        Using client
            Try
                Await client.ConnectAsync(New Uri($"ws://localhost:{s_port}/ws/not-exists"), CancellationToken.None)
            Catch ex As WebSocketException
                rejected = True
            End Try
        End Using

        Assert(rejected, "The handshake on an unpublished url path should be rejected by the server.")
    End Function

    ''' <summary>
    ''' the websocket support should never break the regular http request
    ''' handling of the existed http server.
    ''' </summary>
    Async Function TestRegularHttp() As Task
        Using http As New Net.Http.HttpClient()
            http.Timeout = TimeSpan.FromSeconds(15)

            Dim body As String = Await http.GetStringAsync($"http://localhost:{s_port}/hello")

            Assert(body.Contains("http-ok"), $"The regular http request should still be served, but got '{body}'.")
        End Using
    End Function

#End Region

#Region "Test server"

    ''' <summary>
    ''' start a Flute http server which publishes several websocket endpoints
    ''' on a background thread.
    ''' </summary>
    Sub StartServer()
        Dim settings As Configuration = Configuration.Default

        settings.silent = True
        settings.websocket_enabled = True
        settings.websocket_subprotocols = "chat, superchat"

        s_server = New HttpSocket(AddressOf HttpHandler, s_port, configs:=settings)

        ' the echo endpoint for the protocol level verification
        Call s_server.WebSocket.Route("/ws/echo", WebSocketHandler.Echo)
        ' the chat endpoint for the broadcast verification, the received message
        ' is not echoed back so that the broadcast message could be asserted
        ' without any interference.
        Call s_server.WebSocket.Route("/ws/chat", New WebSocketHandler(message:=Sub(connection, message)
                                                                                    ' no echo at here on purpose
                                                                                End Sub))
        ' an endpoint which transforms the received message, for verify that the
        ' connection is routed to the correct application message handler.
        Call s_server.WebSocket.Route("/ws/upper", New WebSocketHandler(
            message:=Sub(connection, message)
                         Call connection.SendText(message.Text.ToUpper)
                     End Sub))

        Call Task.Run(Sub()
                          Try
                              Call s_server.Run()
                          Catch ex As Exception
                              Console.WriteLine($"[server] {ex.Message}")
                          End Try
                      End Sub)
    End Sub

    ''' <summary>
    ''' the regular http request handler of the test server
    ''' </summary>
    Sub HttpHandler(request As Message.HttpRequest, response As Message.HttpResponse)
        Call response.WriteHTML("<html><body>http-ok</body></html>")
    End Sub

#End Region

#Region "Helpers"

    ''' <summary>
    ''' create a websocket client connection to the test server
    ''' </summary>
    Async Function Connect(path As String) As Task(Of ClientWebSocket)
        Dim client As New ClientWebSocket()

        Await client.ConnectAsync(New Uri($"ws://localhost:{s_port}{path}"), CancellationToken.None)

        Return client
    End Function

    Async Function SendText(client As ClientWebSocket, text As String) As Task
        Dim buffer As Byte() = Encoding.UTF8.GetBytes(text)
        Await client.SendAsync(New ArraySegment(Of Byte)(buffer), WebSocketMessageType.Text, True, CancellationToken.None)
    End Function

    ''' <summary>
    ''' receive a complete application message from the server as an utf8 text
    ''' </summary>
    Async Function ReceiveText(client As ClientWebSocket) As Task(Of String)
        Return Encoding.UTF8.GetString(Await ReceiveBinary(client))
    End Function

    ''' <summary>
    ''' receive a complete application message from the server, the message which
    ''' is transferred via the multiple data frame fragments will be re-assembled
    ''' at here.
    ''' </summary>
    Async Function ReceiveBinary(client As ClientWebSocket) As Task(Of Byte())
        Dim buffer As Byte() = New Byte(8191) {}

        Using memory As New IO.MemoryStream()
            Dim result As WebSocketReceiveResult

            Do
                Dim timeout As New CancellationTokenSource(TimeSpan.FromSeconds(30))

                result = Await client.ReceiveAsync(New ArraySegment(Of Byte)(buffer), timeout.Token)

                If result.MessageType = WebSocketMessageType.Close Then
                    Exit Do
                End If

                Call memory.Write(buffer, 0, result.Count)
            Loop While Not result.EndOfMessage

            Return memory.ToArray
        End Using
    End Function

    Function FindAvailablePort() As Integer
        Dim listener As New TcpListener(IPAddress.Loopback, 0)

        Try
            Call listener.Start()
            Return DirectCast(listener.LocalEndpoint, IPEndPoint).Port
        Finally
            Call listener.Stop()
        End Try
    End Function

    Async Function RunTest(name As String, testCase As Func(Of Task)) As Task
        Console.Write($"  {name.PadRight(48)}")

        Try
            Await testCase()

            Call s_results.Add(New TestResult With {.Name = name, .Passed = True})
            Console.WriteLine("[PASS]")
        Catch ex As Exception
            Dim detail As String = If(TypeOf ex Is AggregateException, ex.InnerException?.Message, ex.Message)

            Call s_results.Add(New TestResult With {.Name = name, .Passed = False, .Detail = detail})
            Console.WriteLine("[FAIL]")
            Console.WriteLine($"         -> {detail}")
        End Try
    End Function

    Sub Assert(condition As Boolean, message As String)
        If Not condition Then
            Throw New Exception(message)
        End If
    End Sub

    Sub PrintReport()
        Dim passed As Integer = s_results.Where(Function(r) r.Passed).Count
        Dim failed As Integer = s_results.Count - passed

        Console.WriteLine()
        Console.WriteLine(New String("="c, 70))
        Console.WriteLine($"  Total: {s_results.Count}    Passed: {passed}    Failed: {failed}")
        Console.WriteLine(New String("="c, 70))

        If failed > 0 Then
            Console.WriteLine()
            Console.WriteLine("  Failed cases:")

            For Each result As TestResult In s_results.Where(Function(r) Not r.Passed)
                Console.WriteLine($"    - {result.Name}: {result.Detail}")
            Next
        End If

        Console.WriteLine()
    End Sub

    Sub Cleanup()
        Try
            Call s_server?.Shutdown()
        Catch ex As Exception
            ' the server may have already been stopped
        End Try
    End Sub

#End Region

End Module
